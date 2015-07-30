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
        double LastWindowMinWidth { get; set; }
        double LastWindowMinHeight { get; set; }

        BoardSize LastBoardSize { get; set; }
        Point LastLocation { get; set; }
        Color TileColor { get; set; }
        Brush TileBrush { get; set; }

        ObservableCollection<IStatisticsModule> Statistics { get; set; }

        void Save();
    }

    public class SettingsProvider : ISettingsProvider
    {
        private static ISettingsProvider instance = new SettingsProvider();
        private static string statisticsFileName = "statistics.txt";

        private Settings userSettings = new Settings();
        private ObservableCollection<IStatisticsModule> statistics = new ObservableCollection<IStatisticsModule>();
        private object syncLock = new object();

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

        public double LastWindowMinWidth
        {
            get
            {
                return userSettings.LastWindowMinWidth;
            }
            set
            {
                this.userSettings.LastWindowMinWidth = value;
            }
        }

        public double LastWindowMinHeight
        {
            get
            {
                return userSettings.LastWindowMinHeight;
            }
            set
            {
                this.userSettings.LastWindowMinHeight = value;
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
                lock (this.syncLock)
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

                lock (this.syncLock)
                {
                    this.statistics = value;
                }
            }
        }

        public void Save()
        {
            userSettings.Save();
            this.SaveStatistics();
        }

        private const char settingsDelimiter = (char)20;
        private void SaveStatistics()
        {
            long time1 = 0, time2 = 0, time3 = 0, time4 = 0;
            JonUtility.Diagnostics.QueryPerformanceCounter(ref time1);

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
                        lock(this.syncLock)
                        {
                            var serializer = new JsonSerializer();
                            JonUtility.Diagnostics.QueryPerformanceCounter(ref time2);
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
                        JonUtility.Diagnostics.QueryPerformanceCounter(ref time3);
                    }             
                }
            }
         
            JonUtility.Diagnostics.QueryPerformanceCounter(ref time4);
            Console.WriteLine(JonUtility.StringFunctions.TicksToMS(time2 - time1, 4));
            Console.WriteLine(JonUtility.StringFunctions.TicksToMS(time3 - time2, 4));
            Console.WriteLine(JonUtility.StringFunctions.TicksToMS(time4 - time3, 4));
            Console.WriteLine();
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

        private async Task<string[]> GetStatText()
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

        private List<IStatisticsModule> newModules = new List<IStatisticsModule>();
        private bool saveAllModules = false;

        private async void LoadStatistics()
        {
            if (statisticsFileName == null)
            {
                statisticsFileName = "statistics.txt";
            }
            long time1 = 0, time2 = 0;
            JonUtility.Diagnostics.QueryPerformanceCounter(ref time1);
            string[] statLines = await this.GetStatText();

            if (statLines.Length == 0)
            {
                return;
            }

            var loadedStatModules = await Task.Factory.StartNew(st => this.LoadStatLines((string[])st), statLines).ConfigureAwait(true);
            var statisticsList = new ObservableCollection<IStatisticsModule>(loadedStatModules);
            var oldStatList = this.statistics;

            if (loadedStatModules.Count == 0)
            {
                return;
            }

            lock(this.syncLock)
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

            JonUtility.Diagnostics.QueryPerformanceCounter(ref time2);
            //App.Current.Dispatcher.Invoke(new Action(() => App.Current.MainWindow.Title = JonUtility.StringFunctions.TicksToMS(time2 - time1, 2) + " (" + loadedStatModules.Count.ToString() + ")"));
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
                    var conversionType = StatisticHelper.GetStatType(key);

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

        private SettingsProvider()
        {
            this.statistics.CollectionChanged += statistics_CollectionChanged;
            this.LoadStatistics();
        }

        void statistics_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {        
                foreach (var newModule in e.NewItems)
                {
                   if (newModule != null)
                   {
                       newModules.Add((IStatisticsModule)newModule);
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
