namespace WpfMinesweeper.Miscellanious
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.IO;
    using System.IO.IsolatedStorage;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Media;
    using JonUtility;
    using Models;
    using Newtonsoft.Json;
    using Properties;

    public interface ISettingsProvider
    {
        /// <summary>
        ///     Gets or sets the most recent board size used.
        /// </summary>
        BoardSize LastBoardSize { get; set; }

        /// <summary>
        ///     Gets or sets the last main window location.
        /// </summary>
        Point LastLocation { get; set; }

        /// <summary>
        ///     Gets or sets the last main window size.
        /// </summary>
        Size LastWindowMinSize { get; set; }

        /// <summary>
        ///     Gets or sets the list of individual game statistics. Additionally, the user can directly modify the list itself
        ///     using standard operations such as Add and Remove.
        /// </summary>
        ObservableCollection<IStatisticsModule> Statistics { get; set; }

        /// <summary>
        ///     Gets or sets the brush used to paint the tiles on a Minesweeper tile board.
        /// </summary>
        Brush TileBrush { get; set; }

        /// <summary>
        ///     Gets or sets the color of the tiles on a Minesweeper tile board.
        /// </summary>
        Color TileColor { get; set; }

        /// <summary>
        ///     Saves all settings.
        /// </summary>
        void Save();
    }

    public class SettingsProvider : ISettingsProvider
    {
        private const char settingsDelimiter = (char)20;
        private static readonly ISettingsProvider instance = new SettingsProvider();
        private static string statisticsFileName = "statistics.txt";
        private readonly List<IStatisticsModule> newModules = new List<IStatisticsModule>();
        private readonly object statisticsSyncLock = new object();
        private readonly Settings userSettings = new Settings();
        private bool saveAllModules;
        private ObservableCollection<IStatisticsModule> statistics = new ObservableCollection<IStatisticsModule>();

        private SettingsProvider()
        {
            this.statistics.CollectionChanged += this.statistics_CollectionChanged;
            this.LoadStatistics();
        }

        public static ISettingsProvider Instance
        {
            get
            {
                return SettingsProvider.instance;
            }
        }

        public BoardSize LastBoardSize
        {
            get
            {
                return this.userSettings.LastBoardSize;
            }
            set
            {
                this.userSettings.LastBoardSize = value;
            }
        }

        public Point LastLocation
        {
            get
            {
                return this.userSettings.LastLocation;
            }
            set
            {
                this.userSettings.LastLocation = value;
            }
        }

        public Size LastWindowMinSize
        {
            get
            {
                return this.userSettings.LastWindowMinSize;
            }
            set
            {
                this.userSettings.LastWindowMinSize = value;
            }
        }

        public ObservableCollection<IStatisticsModule> Statistics
        {
            get
            {
                lock (this.statisticsSyncLock)
                {
                    return this.statistics;
                }
            }
            set
            {
                if (value == null)
                {
                    return;
                }

                lock (this.statisticsSyncLock)
                {
                    this.statistics = value;
                }
            }
        }

        // valid property name/type, but implements a temporary method; later, use serialization and store it myself.
        public Brush TileBrush
        {
            get
            {
                if (this.userSettings.TileBrushSolid != null)
                {
                    return this.userSettings.TileBrushSolid;
                }
                return this.userSettings.TileBrushGradient;
            }
            set
            {
                var brushType = value.GetType();
                if (brushType.IsAssignableFrom(
                    typeof(SolidColorBrush)))
                {
                    this.userSettings.TileBrushSolid = value as SolidColorBrush;
                }
                else if (brushType.IsAssignableFrom(
                    typeof(LinearGradientBrush)))
                {
                    this.userSettings.TileBrushGradient = value as LinearGradientBrush;
                }
            }
        }

        public Color TileColor
        {
            get
            {
                return this.userSettings.TileColor;
            }
            set
            {
                this.userSettings.TileColor = value;
            }
        }

        public async void Save()
        {
            this.userSettings.Save();
            await Task.Run(
                () => this.SaveStatistics()).ConfigureAwait(
                    false);
        }

        private IEnumerable<IStatisticsModule> GetStatModulesToSave()
        {
            if (this.saveAllModules)
            {
                return this.statistics;
            }
            return this.newModules;
        }

        private async void LoadStatistics()
        {
            if (SettingsProvider.statisticsFileName == null)
            {
                SettingsProvider.statisticsFileName = "statistics.txt";
            }

            long time1 = 0, time2 = 0;
            Diagnostics.QueryPerformanceCounter(
                ref time1);
            var statLines = await this.LoadStatText();

            if (statLines.Length == 0)
            {
                Mediator.Instance.Notify(
                    ViewModelMessages.StatisticsLoaded);
                return;
            }

            var loadedStatModules = await Task.Factory.StartNew(
                st => this.LoadStatLines(
                    (string[])st),
                statLines).ConfigureAwait(
                    true);
            var statisticsList = new ObservableCollection<IStatisticsModule>(loadedStatModules);
            var oldStatList = this.statistics;

            if (loadedStatModules.Count > 0)
            {
                lock (this.statisticsSyncLock)
                {
                    this.statistics.CollectionChanged -= this.statistics_CollectionChanged;
                    if (this.statistics.Count > 0)
                    {
                        foreach (var statModule in this.statistics)
                        {
                            statisticsList.Add(
                                statModule);
                        }
                    }

                    this.statistics = statisticsList;
                    this.statistics.CollectionChanged += this.statistics_CollectionChanged;
                }
            }

            Mediator.Instance.Notify(
                ViewModelMessages.StatisticsLoaded);
            Diagnostics.QueryPerformanceCounter(
                ref time2);
        }

        private List<IStatisticsModule> LoadStatLines(string[] statLines)
        {
            var list = new List<IStatisticsModule>(statLines.Length);
            var serializer = new JsonSerializer();

            foreach (var statLine in statLines)
            {
                var module = StatisticsModule.Create();
                var parts = statLine.Split(
                    new[] {SettingsProvider.settingsDelimiter},
                    StringSplitOptions.RemoveEmptyEntries);
                foreach (var part in parts)
                {
                    var index = part.IndexOf(
                        ';');
                    var key = (Statistic)int.Parse(
                        part.Substring(
                            0,
                            index));
                    var valueString = part.Substring(
                        index + 1);
                    var conversionType = StatisticHelper.GetType(
                        key);

                    using (var reader = new StringReader(part.Substring(
                        index + 1)))
                    {
                        var value = serializer.Deserialize(
                            reader,
                            conversionType);
                        module[key] = value;
                    }
                }

                list.Add(
                    module);
            }

            return list;
        }

        private async Task<string[]> LoadStatText()
        {
            using (var isoStore = IsolatedStorageFile.GetStore(
                IsolatedStorageScope.User | IsolatedStorageScope.Assembly,
                null,
                null))
            {
                if (!isoStore.FileExists(
                    SettingsProvider.statisticsFileName))
                {
                    return new string[0];
                }

                using (var isoStream = new IsolatedStorageFileStream(SettingsProvider.statisticsFileName,
                    FileMode.Open,
                    isoStore))
                {
                    using (var reader = new StreamReader(isoStream))
                    {
                        var statText = await reader.ReadToEndAsync().ConfigureAwait(
                            true);
                        var statLines = statText.Split(
                            new[] {Environment.NewLine},
                            StringSplitOptions.RemoveEmptyEntries);
                        return statLines;
                    }
                }
            }
        }

        private void SaveStatistics()
        {
            lock (this.statisticsSyncLock)
            {
                if (this.newModules.Count == 0 && !this.saveAllModules)
                {
                    return;
                }

                using (var isoStore = IsolatedStorageFile.GetStore(
                    IsolatedStorageScope.User | IsolatedStorageScope.Assembly,
                    null,
                    null))
                {
                    using (var isoStream = new IsolatedStorageFileStream(SettingsProvider.statisticsFileName,
                        this.saveAllModules ? FileMode.Create : FileMode.Append,
                        isoStore))
                    {
                        using (var writer = new StreamWriter(isoStream))
                        {
                            lock (this.statisticsSyncLock)
                            {
                                var serializer = new JsonSerializer();
                                foreach (var module in this.GetStatModulesToSave())
                                {
                                    foreach (var pair in module)
                                    {
                                        using (var sw = new StringWriter())
                                        {
                                            serializer.Serialize(
                                                sw,
                                                pair.Value);
                                            writer.Write(
                                                ((int)pair.Key).ToString() + ';' + sw + SettingsProvider.settingsDelimiter);
                                        }
                                    }

                                    writer.Write(
                                        Environment.NewLine);
                                }
                            }
                        }
                    }
                }

                this.newModules.Clear();
            }
        }

        private void statistics_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                lock (this.statisticsSyncLock)
                {
                    foreach (var newModule in e.NewItems)
                    {
                        if (newModule != null)
                        {
                            this.newModules.Add(
                                (IStatisticsModule)newModule);
                        }
                    }
                }
            }
            else
            {
                this.saveAllModules = true;
            }
        }
    }
}
