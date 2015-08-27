namespace WpfMinesweeper.ViewModels
{
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media;
    using JonUtility.WPF;
    using Models;

    public class MenuViewModel : ViewModelBase
    {
        private ICommand boardSizeCommand;
        private ViewModelBase customBoardViewModel;
        private ViewModelBase gradientViewModel;
        private Color selectedColor;
        private ICommand statisticsPromptCommand;
        private ViewModelBase statisticsViewModel;
        private ICommand tileColorCommand;

        public MenuViewModel()
        {
            this.BoardSizeCommand = new Command(this.OnBoardSizeSelected);
            this.StatisticsPromptCommand = new Command<string>(this.OnStatisticsPromptCommand);
            this.selectedColor = this.Settings.TileBrush.GetType() == typeof(SolidColorBrush) ?
                ((SolidColorBrush)this.Settings.TileBrush).Color
                : Colors.Maroon;
            this.CustomBoardViewModel = new CustomBoardViewModel();
            this.StatisticsViewModel = new StatisticsViewModel();
            this.GradientViewModel = new GradientViewModel();
        }

        public ICommand BoardSizeCommand
        {
            get
            {
                return this.boardSizeCommand;
            }
            set
            {
                if (this.boardSizeCommand != value)
                {
                    this.boardSizeCommand = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public ViewModelBase CustomBoardViewModel
        {
            get
            {
                return this.customBoardViewModel;
            }
            set
            {
                if (this.customBoardViewModel != value)
                {
                    this.customBoardViewModel = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public ViewModelBase GradientViewModel
        {
            get
            {
                return this.gradientViewModel;
            }
            set
            {
                if (this.gradientViewModel != value)
                {
                    this.gradientViewModel = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public Color SelectedTileColor
        {
            get
            {
                return this.selectedColor;
            }
            set
            {
                if (this.selectedColor != value)
                {
                    this.selectedColor = value;
                    var brush = new SolidColorBrush(value);
                    this.Mediator.Notify(ViewModelMessages.TileColorsChanged, brush);
                    this.OnPropertyChanged();
                }
            }
        }

        public ICommand StatisticsPromptCommand
        {
            get
            {
                return this.statisticsPromptCommand;
            }
            set
            {
                if (this.statisticsPromptCommand != value)
                {
                    this.statisticsPromptCommand = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public ViewModelBase StatisticsViewModel
        {
            get
            {
                return this.statisticsViewModel;
            }
            set
            {
                if (this.statisticsViewModel != value)
                {
                    this.statisticsViewModel = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public ICommand TileColorCommand
        {
            get
            {
                return this.tileColorCommand;
            }
            set
            {
                if (this.tileColorCommand != value)
                {
                    this.tileColorCommand = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private void OnBoardSizeSelected(object parameter)
        {
            this.Mediator.Notify(ViewModelMessages.CreateNewBoard, parameter);
        }

        private void OnStatisticsPromptCommand(string result)
        {
            if (!string.Equals(result, "Clear"))
            {
                return;
            }

            var decision = MessageBox.Show(
                "Clear all recorded statistics to this point?",
                "Clear Statistics",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question,
                MessageBoxResult.No);

            if (decision == MessageBoxResult.No)
            {
                return;
            }

            this.Settings.Statistics.Clear();
            this.Settings.Save();
            this.Mediator.Notify(ViewModelMessages.StatisticsLoaded);
        }
    }
}
