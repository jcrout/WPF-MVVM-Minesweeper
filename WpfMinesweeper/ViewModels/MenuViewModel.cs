namespace WpfMinesweeper.ViewModels
{
    using System.Windows.Input;
    using System.Windows.Media;

    public class MenuViewModel : ViewModelBase
    {
        private static readonly Color defaultSelectedColor = ViewModelBase.Settings.TileColor;
        private ICommand boardSizeCommand;
        private ViewModelBase customBoardViewModel;
        private Color selectedColor;
        private ViewModelBase statisticsViewModel;
        private ICommand tileColorCommand;

        public MenuViewModel()
        {
            this.boardSizeCommand = new Command(o => this.OnBoardSizeSelected(
                o));
            this.selectedColor = MenuViewModel.defaultSelectedColor;
            this.CustomBoardViewModel = new CustomBoardViewModel();
            this.StatisticsViewModel = new StatisticsViewModel();
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
                    ViewModelBase.Settings.TileColor = value;
                    var brush = new SolidColorBrush(value);
                    Mediator.Instance.Notify(
                        ViewModelMessages.TileColorsChanged,
                        brush);
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

        private void OnBoardSizeSelected(object paramter)
        {
            Mediator.Instance.Notify(
                ViewModelMessages.CreateNewBoard,
                paramter);
        }
    }
}
