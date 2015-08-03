namespace WpfMinesweeper
{
    using System.Xml.Serialization;
    using System.Runtime.Serialization;
    using System.Windows;
    using System.Windows.Media;
    using WpfMinesweeper.Models;
    using WpfMinesweeper.Properties;
    using System.IO;
    using System.Xml;
    using System.Collections.Generic;
    using System.IO.IsolatedStorage;
    using System;
    using Newtonsoft.Json;
    using System.Threading.Tasks;
    using System.Collections.ObjectModel;

    public interface ISettingsProvider
    {
        /// <summary>
        /// Gets or sets the last main window size.
        /// </summary>
        Size LastWindowMinSize { get; set; }

        /// <summary>
        /// Gets or sets the last main window location.
        /// </summary>
        Point LastLocation { get; set; }

        /// <summary>
        /// Gets or sets the most recent board size used.
        /// </summary>
        BoardSize LastBoardSize { get; set; }

        /// <summary>
        /// Gets or sets the color of the tiles on a Minesweeper tile board.
        /// </summary>
        Color TileColor { get; set; }

        /// <summary>
        /// Gets or sets the brush used to paint the tiles on a Minesweeper tile board.
        /// </summary>
        Brush TileBrush { get; set; }

        /// <summary>
        /// Gets or sets the list of individual game statistics. Additionally, the user can directly modify the list itself using standard operations such as Add and Remove.
        /// </summary>
        ObservableCollection<IStatisticsModule> Statistics { get; set; }

        /// <summary>
        /// Saves all settings.
        /// </summary>
        void Save();
    }

    public class SettingsProvider : ISettingsProvider
    {
        private const char settingsDelimiter = (char)20;

        private static ISettingsProvider instance = new SettingsProvider();
        private static string statisticsFileName = "statistics.txt";

        private Settings userSettings = new Settings();
        private ObservableCollection<IStatisticsModule> statistics = new ObservableCollection<IStatisticsModule>();
        private List<IStatisticsModule> newModules = new List<IStatisticsModule>();
        private object statisticsSyncLock = new object();
        private bool saveAllModules = false;

        static SettingsProvider()
        {

        }

        public static ISettingsProvider Instance
        {
            get
            {
                return instance;
            }
        }

        public BoardSize LastBoardSize
        {
            get
            {
                return userSettings.LastBoardSize;
            }
            set
            {
                this.userSettings.LastBoardSize = value;
            }
        }

        public Size LastWindowMinSize 
        {
            get
            {
                return userSettings.LastWindowMinSize;
            }
            set
            {
                this.userSettings.LastWindowMinSize = value;
            }
        }

        public Color TileColor
        {
            get
            {
                return userSettings.TileColor;
            }
            set
            {
                userSettings.TileColor = value;
            }
        }

        public Point LastLocation
        {
            get
            {
                return userSettings.LastLocation;
            }
            set
            {
                userSettings.LastLocation = value;
            }
        }

        // valid property name/type, but implements a temporary method; later, use serialization and store it myself.
        public Brush TileBrush
        {
            get
            {
                if (userSettings.TileBrushSolid != null)
                {
                    return userSettings.TileBrushSolid;
                }
                else
                {
                    return userSettings.TileBrushGradient;
                }
            }
            set
            {
                var brushType = value.GetType();
                if (brushType.IsAssignableFrom(typeof(SolidColorBrush)))
                {
                    userSettings.TileBrushSolid = value as SolidColorBrush;
                }
                else if (brushType.IsAssignableFrom(typeof(LinearGradientBrush)))
                {
                    userSettings.TileBrushGradient = value as LinearGradientBrush;
                }
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

        public async void Save()
        {
            this.userSettings.Save(); 
            await Task.Run(() => this.SaveStatistics()).ConfigureAwait(false);
        }

        private SettingsProvider()
        {
            this.statistics.CollectionChanged += statistics_CollectionChanged;
            this.LoadStatistics();
        }

        private void SaveStatistics()
        {
            lock (this.statisticsSyncLock)
            {
                if (this.newModules.Count == 0 && !this.saveAllModules)
                {
                    return;
                }

                using (var isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null))
                {
                    using (IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream(statisticsFileName, this.saveAllModules ? FileMode.Create : FileMode.Append, isoStore))
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
                                            serializer.Serialize(sw, pair.Value);
                                            writer.Write(((int)pair.Key).ToString() + ';' + sw.ToString() + settingsDelimiter);
                                        }
                                    }

                                    writer.Write(Environment.NewLine);
                                }
                            }
                        }
                    }
                }

                this.newModules.Clear();
            }
        }

        private IEnumerable<IStatisticsModule> GetStatModulesToSave()
        {
            if (this.saveAllModules)
            {
                return this.statistics;
            }
            else
            {
                return this.newModules;
            }
        }

        private async void LoadStatistics()
        {
            if (statisticsFileName == null)
            {
                statisticsFileName = "statistics.txt";
            }

            long time1 = 0, time2 = 0;
            JonUtility.Diagnostics.QueryPerformanceCounter(ref time1);
            string[] statLines = await this.LoadStatText();

            if (statLines.Length == 0)
            {
                Mediator.Instance.Notify(ViewModelMessages.StatisticsLoaded);
                return;
            }

            var loadedStatModules = await Task.Factory.StartNew(st => this.LoadStatLines((string[])st), statLines).ConfigureAwait(true);
            var statisticsList = new ObservableCollection<IStatisticsModule>(loadedStatModules);
            var oldStatList = this.statistics;

            if (loadedStatModules.Count > 0)
            {
                lock (this.statisticsSyncLock)
                {

                    this.statistics.CollectionChanged -= statistics_CollectionChanged;
                    if (this.statistics.Count > 0)
                    {

                        foreach (var statModule in this.statistics)
                        {
                            statisticsList.Add(statModule);
                        }
                    }

                    this.statistics = statisticsList;
                    this.statistics.CollectionChanged += statistics_CollectionChanged;
                }
            }

            Mediator.Instance.Notify(ViewModelMessages.StatisticsLoaded);
            JonUtility.Diagnostics.QueryPerformanceCounter(ref time2);
        }

        private async Task<string[]> LoadStatText()
        {
            using (var isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null))
            {
                if (!isoStore.FileExists(statisticsFileName))
                {
                    return new string[0];
                }

                using (var isoStream = new IsolatedStorageFileStream(statisticsFileName, FileMode.Open, isoStore))
                {
                    using (var reader = new StreamReader(isoStream))
                    {
                        var statText = await reader.ReadToEndAsync().ConfigureAwait(true);
                        var statLines = statText.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                        return statLines;
                    }
                }
            }
        }

        private List<IStatisticsModule> LoadStatLines(string[] statLines)
        {
            var list = new List<IStatisticsModule>(statLines.Length);
            var serializer = new JsonSerializer();

            foreach (var statLine in statLines)
            {
                var module = StatisticsModule.Create();
                var parts = statLine.Split(new char[] { settingsDelimiter }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var part in parts)
                {
                    int index = part.IndexOf(';');
                    var key = (Statistic)int.Parse(part.Substring(0, index));
                    var valueString = part.Substring(index + 1);
                    var conversionType = StatisticHelper.GetType(key);

                    using (var reader = new StringReader(part.Substring(index + 1)))
                    {
                        object value = serializer.Deserialize(reader, conversionType);
                        module[key] = value;
                        continue;
                    }
                }

                list.Add(module);
            }

            return list;
        }

        private void statistics_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                lock (this.statisticsSyncLock)
                {
                    foreach (var newModule in e.NewItems)
                    {
                        if (newModule != null)
                        {
                            newModules.Add((IStatisticsModule)newModule);
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
