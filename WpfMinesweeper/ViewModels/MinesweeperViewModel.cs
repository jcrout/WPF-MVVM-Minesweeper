namespace WpfMinesweeper.ViewModels
{
    using System;
    using System.Windows;
    using JonUtility;
    using Models;

    public class MinesweeperViewModel : MinesweeperComponentViewModel
    {
        private readonly WinTimer gameTimer;
        private MinesweeperComponentViewModel displayViewModel;
        private bool gameStarted;
        private ViewModelBase menuViewModel;
        private bool minimized;
        private MinesweeperComponentViewModel tileBoardViewModel;

        public MinesweeperViewModel()
        {
            Mediator.Instance.Register(
                ViewModelMessages.GameWindowStateChanged,
                o => this.OnWindowStateChanged(
                    (WindowState)o));
            Mediator.Instance.Register(
                ViewModelMessages.CreateNewBoard,
                this.OnCreateNewBoard);
            Mediator.Instance.Register(
                ViewModelMessages.GameStarted,
                this.OnGameStarted);
            Mediator.Instance.Register(
                ViewModelMessages.GameOver,
                this.OnGameOver);
            Mediator.Instance.Register(
                ViewModelMessages.Victory,
                this.OnVictory);

            this.gameTimer = new WinTimer(this.TimerProc,
                1000);

            this.MenuViewModel = new MenuViewModel();
            this.DisplayViewModel = new DisplayPanelViewModel();
            this.TileBoardViewModel = new TileBoardViewModel();

            this.Minesweeper = MinesweeperFactory.GetFromSettings();
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

        protected override void OnMinesweeperChanged()
        {
            if (this.gameStarted)
            {
                this.ResetGame();
            }

            var gameStatistics = StatisticsModule.Create();
            var minesweeper = this.Minesweeper;

            ViewModelBase.Settings.LastBoardSize = new BoardSize(minesweeper.Tiles.Width,
                minesweeper.Tiles.Height,
                minesweeper.MineCount);

            gameStatistics[Statistic.BoardSize] = new BoardSize(minesweeper.Tiles.Width,
                minesweeper.Tiles.Height,
                minesweeper.MineCount);

            this.DisplayViewModel.Minesweeper = minesweeper;
            this.TileBoardViewModel.Minesweeper = minesweeper;

            this.GameStatistics = gameStatistics;
            this.DisplayViewModel.GameStatistics = gameStatistics;
            this.TileBoardViewModel.GameStatistics = gameStatistics;
        }

        private void OnWindowStateChanged(WindowState state)
        {
            if (!this.gameStarted)
            {
                return;
            }

            if (state == WindowState.Minimized)
            {
                this.gameTimer.Stop();
                this.minimized = true;
            }
            else if (this.minimized)
            {
                this.gameTimer.Start();
                this.minimized = false;
            }
        }

        private void OnGameStarted(object paramter)
        {
            this.gameStarted = true;
            this.gameTimer.Start();
        }

        private void OnCreateNewBoard(object paramter)
        {
            if (paramter == null)
            {
                this.Minesweeper = MinesweeperFactory.Create(
                    this.Minesweeper);
                return;
            }

            if (paramter.GetType() == typeof (BoardSize))
            {
                this.Minesweeper = MinesweeperFactory.Create(
                    (BoardSize)paramter);
                return;
            }
            else if (paramter is string)
            {
                var boardSize = BoardSize.Parse(
                    paramter.ToString());
                this.Minesweeper = MinesweeperFactory.Create(
                    boardSize);
                return;
            }
            else
            {
                throw new ArgumentException("paramter must either be of type String or BoardSize.");
            }
        }

        private void OnGameOver(object paramter)
        {
            this.EndGame(
                GameState.GameOver);
            Mediator.Instance.Notify(
                ViewModelMessages.UpdateSmileyIndex,
                SmileyState.GameOver);
        }

        private void OnVictory(object paramter)
        {
            this.EndGame(
                GameState.Victory);
            Mediator.Instance.Notify(
                ViewModelMessages.UpdateSmileyIndex,
                SmileyState.Victory);
        }

        private void EndGame(GameState finalState)
        {
            this.gameTimer.Stop();

            this.GameStatistics[Statistic.GameEndTime] = DateTime.Now;
            this.GameStatistics[Statistic.GameState] = finalState;
            this.GameStatistics[Statistic.MinesRemaining] = this.Minesweeper.MinesRemaining;
            this.GameStatistics[Statistic.TimeElapsed] = this.Minesweeper.TimeElapsed;

            ViewModelBase.Settings.Statistics.Add(
                this.GameStatistics);
            ViewModelBase.Settings.Save();
        }

        private void ResetGame()
        {
            this.gameTimer.Stop();
            this.gameStarted = false;
        }

        private void TimerProc()
        {
            this.Minesweeper.TimeElapsed++;
        }
    }
}
