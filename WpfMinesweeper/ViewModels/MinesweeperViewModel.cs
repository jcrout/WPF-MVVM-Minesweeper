namespace WpfMinesweeper.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Threading;
    using WpfMinesweeper.Models;
    using JonUtility;

    public class MinesweeperViewModel : MinesweeperComponentViewModel
    {
        private MinesweeperComponentViewModel displayViewModel;
        private MinesweeperComponentViewModel tileBoardViewModel;
        private ViewModelBase menuViewModel;
        private WinTimer gameTimer;
        private bool minimized;
        private bool gameStarted;

        public MinesweeperViewModel()
        {
            Mediator.Instance.Register(ViewModelMessages.GameWindowStateChanged, o => this.OnWindowStateChanged((WindowState)o));
            Mediator.Instance.Register(ViewModelMessages.CreateNewBoard, this.OnCreateNewBoard);
            Mediator.Instance.Register(ViewModelMessages.GameStarted, this.OnGameStarted);
            Mediator.Instance.Register(ViewModelMessages.GameOver, this.OnGameOver);
            Mediator.Instance.Register(ViewModelMessages.Victory, this.OnVictory);

            this.gameTimer = new WinTimer(TimerProc, 1000);

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

            Settings.LastBoardSize = new BoardSize(minesweeper.Tiles.Width, minesweeper.Tiles.Height, minesweeper.MineCount);

            gameStatistics[Statistic.BoardSize] = new BoardSize(minesweeper.Tiles.Width, minesweeper.Tiles.Height, minesweeper.MineCount);
      
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
            if (paramter == null || !(paramter is string))
            {
                this.Minesweeper = MinesweeperFactory.Create(this.Minesweeper);
                return;
            }

            var paramters = paramter.ToString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (paramters.Length != 3)
            {
                throw new ArgumentException("paramter must contain 3 integer values.");
            }

            int width, height, mineCount;
            try
            {
                width = (paramters[0] == "*") ? this.Minesweeper.Tiles.Width : int.Parse(paramters[0]);
                height = (paramters[1] == "*") ? this.Minesweeper.Tiles.Height : int.Parse(paramters[1]);
                mineCount = (paramters[2] == "*") ? this.Minesweeper.MineCount : int.Parse(paramters[2]);
            }
            catch (FormatException)
            {
                throw new ArgumentException("paramter must contain 3 integer values.");
            }

            var newBoard = MinesweeperFactory.Create(width, height, mineCount);
            this.Minesweeper = newBoard;
        }

        private void OnGameOver(object paramter)
        {
            this.EndGame(GameState.GameOver);
            Mediator.Instance.Notify(ViewModelMessages.UpdateSmileyIndex, SmileyState.GameOver);
        }

        private void OnVictory(object paramter)
        {
            this.EndGame(GameState.Victory);
            Mediator.Instance.Notify(ViewModelMessages.UpdateSmileyIndex, SmileyState.Victory);
        }

        private void EndGame(GameState finalState)
        {
            this.gameTimer.Stop();
            this.GameStatistics[Statistic.GameState] = finalState;
            this.GameStatistics[Statistic.MinesRemaining] = Minesweeper.MinesRemaining;
            this.GameStatistics[Statistic.TimeElapsed] = Minesweeper.TimeElapsed;
            this.GameStatistics[Statistic.GameEndTime] = DateTime.Now;

            Settings.Statistics.Add(this.GameStatistics);
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
