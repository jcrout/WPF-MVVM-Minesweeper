namespace WpfMinesweeper.ViewModels
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Data;
    using System.Windows.Input;
    using WpfMinesweeper.Models;

    public class StatisticsViewModel : MinesweeperComponentViewModel
    {
        private const string defaultStatText = "-None-";
        
        private static StatValueViewModel emptyPage;

        private IEnumerable<object> dropDownStatisticsList;
        private IEnumerable<StatDisplay> statisticList;
        private IList<object> statisticNameSelectedItems;
        private List<Statistic> sortByList;
        private List<StatValueViewModel> pages;
        private StatValueViewModel selectedPage;
        private string valueHeader;
        private double columnWidth = double.NaN;

        static StatisticsViewModel()
        {
            emptyPage = new StatValueViewModel(defaultStatText);
            emptyPage.StatisticValues = new List<object>(1) { "No values to display." };
        }

        public StatisticsViewModel()
        {
            Mediator.Instance.Register(ViewModelMessages.StatisticsLoaded, o => this.StatisticsLoaded());

            var statList = StatisticHelper.GetGameStatistics().Select(s => StatisticHelper.GetDisplayText(s)).OrderBy(s => s).ToList();
            statList.Insert(0, defaultStatText);

            this.Pages = new List<StatValueViewModel>();
            this.sortByList = new List<Statistic>();
            this.StatisticNameList = statList;
            this.StatisticNameSelectedItems = new List<object>() { statList[0] };   
        }

        public IEnumerable<object> StatisticNameList
        {
            get
            {
                return this.dropDownStatisticsList;
            }
            set
            {
                if (this.dropDownStatisticsList != value)
                {
                    this.dropDownStatisticsList = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public IList<object> StatisticNameSelectedItems
        {
            get
            {
                return this.statisticNameSelectedItems;
            }
            set
            {
                if (this.statisticNameSelectedItems != value)
                {
                    this.statisticNameSelectedItems = value;
                }

                this.StatSelectionChanged();
                this.OnPropertyChanged();
            }
        }

        public string ValueHeader
        {
            get
            {
                return this.valueHeader;
            }
            set
            {
                if (this.valueHeader != value)
                {
                    this.valueHeader = value;
                    this.OnPropertyChanged();
                }  
            }
        }


        public IEnumerable<StatValueViewModel> Pages
        {
            get
            {
                return this.pages;
            }
            set
            {
                if (this.pages != value)
                {
                    this.pages = (List<StatValueViewModel>)value;
                    this.OnPropertyChanged();
                }
            }
        }

        public StatValueViewModel SelectedPage
        {
            get
            {
                return this.selectedPage;
            }
            set
            {
                if (this.selectedPage != value)
                {
                    this.selectedPage = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public IEnumerable<StatDisplay> StatisticList
        {
            get
            {
                return this.statisticList;
            }
            set
            {
                if (this.statisticList != value)
                {
                    this.statisticList = value; 
                    this.ColumnWidth = double.NaN;
                    this.OnPropertyChanged();
                }
            }
        }

        public double ColumnWidth
        {
            get
            {
                return this.columnWidth;
            }
            set
            {
                if (double.IsNaN(this.columnWidth) && double.IsNaN(value))
                {
                    this.columnWidth = 30.53d;
                    this.OnPropertyChanged();
                }
                if (this.columnWidth != value)
                {
                    this.columnWidth = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private void StatisticsLoaded()
        {
            this.LoadDefaultList();
        }

        private void StatSelectionChanged()
        {
            if (this.statisticNameSelectedItems.Count > 1 && this.statisticNameSelectedItems.Contains(defaultStatText))
            {
                this.StatisticNameSelectedItems = new List<object>() { defaultStatText };
            }
            else
            {
                this.OnStatisticListSelectionChanged();
            }
        }

        private void SelectFirstDropDownStat()
        {
            var firstElement = this.dropDownStatisticsList.ElementAt(0);
        }

        private List<StatDisplay> GetDefaultList()
        {            
            var defaultList = new List<StatDisplay>();
            int wins = 0;
            int losses = 0;
            foreach (var module in Settings.Statistics)
            {
                var finalState = (GameState)module[Statistic.GameState];
                if (finalState == GameState.GameOver)
                {
                    losses++;
                }
                else if (finalState == GameState.Victory)
                {
                    wins++;
                }
            }

            defaultList.Add(
                new StatDisplay(
                    "Wins",
                    wins.ToString(),
                    "Total number of wins."));

            defaultList.Add(
                new StatDisplay(
                    "Losses",
                    losses.ToString(),
                    "Total number of losses."));

            //defaultList.Sort((o1, o2) => o1.Label.CompareTo(o2.Label));
            return defaultList;
        }

        private void OnStatisticListSelectionChanged()
        {
            if (this.statisticNameSelectedItems == null || this.statisticNameSelectedItems.Count == 0)
            {
                return;
            }         

            if (this.statisticNameSelectedItems.Count == 0 || this.statisticNameSelectedItems.Contains(defaultStatText))
            {
                this.LoadDefaultList();
            }
            else
            {
                this.sortByList = this.statisticNameSelectedItems.Select(o => StatisticHelper.FromDisplayText(o.ToString())).ToList();
                var newPages = new List<StatValueViewModel>(sortByList.Count);

                foreach (var stat in this.sortByList)
                {                    
                    var newPage = new StatValueViewModel(StatisticHelper.GetDisplayText(stat));
                    var statList = Settings.Statistics.Select(st => st[stat]).Distinct().ToList();
                    statList.Sort((o1, o2) => StatisticComparer.Default.Compare(o1, o2));
                    newPage.StatisticValues = statList;
                    newPage.PropertyChanged += newPage_PropertyChanged;
                    newPages.Add(newPage);
                }

                this.Pages = newPages;
                this.SelectedPage = newPages[0];
            }
        }

        private bool HasAllValues(IStatisticsModule module, IEnumerable<KeyValuePair<Statistic, object>> values)
        {
            foreach (var kvp in values)
            {
                var value = module[kvp.Key];
                if (!value.Equals(kvp.Value))
                {
                    return false;
                }
            }

            return true;
        }

        private void newPage_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "SelectedValue")
            {
                return;
            }

            var model = (StatValueViewModel)sender;
            int index = this.pages.IndexOf(model);
            var values = this.pages[0].StatisticValues;
            var stats = new List<KeyValuePair<Statistic, object>>(index + 1);

            for (int i = 0; i < this.pages.Count; i++ )
            {
                var selectedValue = this.pages[i].SelectedValue;
                if (selectedValue != null)
                {
                    stats.Add(new KeyValuePair<Statistic, object>(this.sortByList[i], selectedValue));
                }
            }

            if (stats.Count == 0)
            {
                this.ValueHeader = "Value";
                this.StatisticList = GetDefaultList();
                return;
            }

            var moduleList = Settings.Statistics.Where(st => this.HasAllValues(st, stats));
            var statList = new List<StatDisplay>();
            if (moduleList.Count() == 0)
            {
                var statDisplay = new StatDisplay(
                    StatisticHelper.GetDisplayText(Statistic.MatchCount),
                    "0",
                    StatisticHelper.GetDescription(Statistic.MatchCount));
                statList.Add(statDisplay);
            }
            else
            {
                foreach (var stat in this.GetStatDisplayList())
                {
                    var statDisplay = new StatDisplay(
                        StatisticHelper.GetDisplayText(stat),
                        this.GetAverageValue(moduleList, stat),
                        StatisticHelper.GetDescription(stat));
                    statList.Add(statDisplay);
                }
            }

            this.ValueHeader = "Average Value";
            this.StatisticList = statList;
        }

        private void LoadDefaultList()
        {
            this.ValueHeader = "Value";
            this.sortByList.Clear();
            this.StatisticList = GetDefaultList();
            this.Pages = new List<StatValueViewModel>(1) { emptyPage };
            this.SelectedPage = this.pages[0];
        }

        private IEnumerable<Statistic> GetStatDisplayList()
        {
            var displayStats = StatisticHelper.GetGlobalStatistics()
                .OrderBy(stat => StatisticHelper.GetDisplayText(stat))
                .Concat(StatisticHelper.GetGameStatistics()
                    .Where(stat => !this.IsSelected(stat))
                    .OrderBy(stat => StatisticHelper.GetDisplayText(stat)));

            return displayStats;
        }

        private bool IsSelected(Statistic stat)
        {
            int index = this.sortByList.IndexOf(stat);
            if (index < 0)
            {
                return false;
            }

            return (this.pages[index].SelectedValue != null);
        }

        private string GetAverageValue(IEnumerable<IStatisticsModule> modules, Statistic stat)
        {
            if (stat == Statistic.MatchCount)
            {
                return modules.Count().ToString();
            }

            var statType = StatisticHelper.GetType(stat);
            if (statType.IsPrimitive)
            {
                var average = modules.Average(module => (double)Convert.ChangeType(module[stat], typeof(double)));
                return average.ToString("0.00");
            }
            else if (statType == typeof(DateTime))
            {
                var average = (int)modules.Select(module => (((DateTime)module[stat]).Hour * 60) + ((DateTime)module[stat]).Minute).Average();
                var time = new DateTime(1, 1, 1, (int)Math.Floor(average / 60d), average % 60, 0, DateTimeKind.Local);
                return time.ToString("h:mm tt", System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat);
            }
            else
            {
                var average = modules.GroupBy(module => module[stat]).OrderByDescending(g => g.Count());
                return average.ElementAt(0).Key.ToString();
            }
        }

    }

    public class StatValueViewModel : ViewModelBase
    {
        private IEnumerable<object> statisticValues;
        private string name;
        private object selectedValue;
        private bool hasContent;

        public StatValueViewModel(string name)
        {       
            this.Name = name;
        }

        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                if (this.name != value)
                {
                    this.name = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public IEnumerable<object> StatisticValues
        {
            get
            {
                return this.statisticValues;
            }
            set
            {
                if (this.statisticValues != value)
                {
                    this.statisticValues = value;
                    this.HasContent = (value != null);
                    this.OnPropertyChanged();
                }
            }
        }

        public object SelectedValue
        {
            get
            {
                return this.selectedValue;
            }
            set
            {
                if (this.selectedValue != value)
                {
                    this.selectedValue = value;

                    this.OnPropertyChanged();
                }
            }
        }

        public bool HasContent
        {
            get
            {
                return this.hasContent;
            }
            set
            {
                if (this.hasContent != value)
                {
                    this.hasContent = value;
                    this.OnPropertyChanged();
                }
            }
        }
    }
}
