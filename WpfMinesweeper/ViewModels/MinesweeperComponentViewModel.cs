namespace WpfMinesweeper.ViewModels
{
    using Models;

    public abstract class MinesweeperComponentViewModel : ViewModelBase
    {
        private IStatisticsModule gameStatistics;
        private IMinesweeper minesweeper;

        public IStatisticsModule GameStatistics
        {
            get
            {
                return this.gameStatistics;
            }
            set
            {
                if (this.gameStatistics != value)
                {
                    this.gameStatistics = value;
                    this.OnGameStatisticsChanged();
                    this.OnPropertyChanged();
                }
            }
        }

        public IMinesweeper Minesweeper
        {
            get
            {
                return this.minesweeper;
            }
            set
            {
                if (this.minesweeper != value)
                {
                    this.minesweeper = value;
                    this.OnMinesweeperChanged();
                    this.OnPropertyChanged();
                }
            }
        }

        protected virtual void OnMinesweeperChanged()
        {
        }

        protected virtual void OnGameStatisticsChanged()
        {
        }
    }
}
