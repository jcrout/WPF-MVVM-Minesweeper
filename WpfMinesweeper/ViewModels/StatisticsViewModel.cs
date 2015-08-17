namespace WpfMinesweeper.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using Models;

    public class StatisticsViewModel : MinesweeperComponentViewModel
    {
        private const string DefaultStatText = "-None-";
        private const string NoValuesToDisplay = "No values to display.";
        private static readonly StatValueViewModel emptyPage;
        private double columnWidth = double.NaN;
        private IEnumerable<object> dropDownStatisticsList;
        private List<StatValueViewModel> pages;
        private StatValueViewModel selectedPage;
        private List<Statistic> sortByList;
        private IEnumerable<StatDisplay> statisticList;
        private IList<object> statisticNameSelectedItems;
        private string valueHeader;

        static StatisticsViewModel()
        {
            StatisticsViewModel.emptyPage = new StatValueViewModel(StatisticsViewModel.DefaultStatText)
            {
                StatisticValues = new List<object>(1) {StatisticsViewModel.NoValuesToDisplay}
            };
        }

        public StatisticsViewModel()
        {
            var statList = StatisticHelper.GetGameStatistics().Select(StatisticHelper.GetDisplayText).OrderBy(s => s).ToList();
            statList.Insert(0, StatisticsViewModel.DefaultStatText);

            this.Pages = new List<StatValueViewModel>();
            this.sortByList = new List<Statistic>();
            this.StatisticNameList = statList;
            this.StatisticNameSelectedItems = new List<object> {statList[0]};
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

        protected override void OnMediatorChanged()
        {
            var mediator = this.Mediator;
            if (mediator == null)
            {
                return;
            }

            this.Mediator.Register(ViewModelMessages.StatisticsLoaded, o => this.StatisticsLoaded());
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
            if (statType == typeof(DateTime))
            {
                var average = (int)modules.Select(module => (((DateTime)module[stat]).Hour * 60) + ((DateTime)module[stat]).Minute).Average();
                var time = new DateTime(1, 1, 1, (int)Math.Floor(average / 60d), average % 60, 0, DateTimeKind.Local);
                return time.ToString("h:mm tt", CultureInfo.CurrentCulture.DateTimeFormat);
            }
            else
            {
                var average = modules.GroupBy(module => module[stat]).OrderByDescending(g => g.Count());
                return average.ElementAt(0).Key.ToString();
            }
        }

        private List<StatDisplay> GetDefaultList()
        {
            var defaultList = new List<StatDisplay>();
            var wins = 0;
            var losses = 0;
            foreach (var module in this.Settings.Statistics)
            {
                var finalState = (GameResult)module[Statistic.GameState];
                if (finalState == GameResult.GameOver)
                {
                    losses++;
                }
                else if (finalState == GameResult.Victory)
                {
                    wins++;
                }
            }

            defaultList.Add(new StatDisplay("Wins", wins.ToString(), "Total number of wins."));

            defaultList.Add(new StatDisplay("Losses", losses.ToString(), "Total number of losses."));

            //defaultList.Sort((o1, o2) => o1.Label.CompareTo(o2.Label));
            return defaultList;
        }

        private IEnumerable<Statistic> GetStatDisplayList()
        {
            var displayStats = StatisticHelper.GetGlobalStatistics().OrderBy(StatisticHelper.GetDisplayText).Concat(StatisticHelper.GetGameStatistics().Where(stat => !this.IsSelected(stat)).OrderBy(StatisticHelper.GetDisplayText));

            return displayStats;
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

        private bool IsSelected(Statistic stat)
        {
            var index = this.sortByList.IndexOf(stat);
            if (index < 0)
            {
                return false;
            }

            return (this.pages[index].SelectedValue != null);
        }

        private void LoadDefaultList()
        {
            this.ValueHeader = "Value";
            this.sortByList.Clear();
            this.StatisticList = this.GetDefaultList();
            this.Pages = new List<StatValueViewModel>(1) {StatisticsViewModel.emptyPage};
            this.SelectedPage = this.pages[0];
        }

        private void newPage_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "SelectedValue")
            {
                return;
            }

            var model = (StatValueViewModel)sender;
            var index = this.pages.IndexOf(model);
            var stats = new List<KeyValuePair<Statistic, object>>(index + 1);

            for (var i = 0; i < this.pages.Count; i++)
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
                this.StatisticList = this.GetDefaultList();
                return;
            }

            var moduleList = this.Settings.Statistics.Where(st => this.HasAllValues(st, stats)).ToArray();
            var statList = new List<StatDisplay>();

            if (moduleList.Length == 0)
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

        private void OnStatisticListSelectionChanged()
        {
            if (this.statisticNameSelectedItems == null || this.statisticNameSelectedItems.Count == 0)
            {
                return;
            }

            if (this.statisticNameSelectedItems.Count == 0 || this.statisticNameSelectedItems.Contains(StatisticsViewModel.DefaultStatText))
            {
                this.LoadDefaultList();
            }
            else
            {
                this.sortByList = this.statisticNameSelectedItems.Select(o => StatisticHelper.FromDisplayText(o.ToString())).ToList();
                var newPages = new List<StatValueViewModel>(this.sortByList.Count);

                foreach (var stat in this.sortByList)
                {
                    var newPage = new StatValueViewModel(StatisticHelper.GetDisplayText(stat));
                    var statList = this.Settings.Statistics.Select(st => st[stat]).Distinct().ToList();
                    statList.Sort((o1, o2) => StatisticComparer.Default.Compare(o1, o2));
                    newPage.StatisticValues = statList;
                    newPage.PropertyChanged += this.newPage_PropertyChanged;
                    newPages.Add(newPage);
                }

                this.Pages = newPages;
                this.SelectedPage = newPages[0];
            }
        }

        private void StatisticsLoaded()
        {
            this.LoadDefaultList();
        }

        private void StatSelectionChanged()
        {
            if (this.statisticNameSelectedItems.Count > 1 && this.statisticNameSelectedItems.Contains(StatisticsViewModel.DefaultStatText))
            {
                this.StatisticNameSelectedItems = new List<object> {StatisticsViewModel.DefaultStatText};
            }
            else
            {
                this.OnStatisticListSelectionChanged();
            }
        }
    }
}
