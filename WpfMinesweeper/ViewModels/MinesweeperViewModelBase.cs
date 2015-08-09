namespace WpfMinesweeper.ViewModels
{
    public abstract class MinesweeperViewModelBase : MinesweeperComponentViewModel
    {
        private MinesweeperComponentViewModel displayViewModel;
        private ViewModelBase menuViewModel;
        private MinesweeperComponentViewModel tileBoardViewModel;

        public MinesweeperComponentViewModel DisplayViewModel
        {
            get
            {
                return this.displayViewModel;
            }
            set
            {
                if (this.displayViewModel != value)
                {
                    this.displayViewModel = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public ViewModelBase MenuViewModel
        {
            get
            {
                return this.menuViewModel;
            }
            set
            {
                if (this.menuViewModel != value)
                {
                    this.menuViewModel = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public MinesweeperComponentViewModel TileBoardViewModel
        {
            get
            {
                return this.tileBoardViewModel;
            }
            set
            {
                if (this.tileBoardViewModel != value)
                {
                    this.tileBoardViewModel = value;
                    this.OnPropertyChanged();
                }
            }
        }

    }
}