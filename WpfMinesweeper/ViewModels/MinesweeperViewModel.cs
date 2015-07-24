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

    class MinesweeperViewModel : MinesweeperComponentViewModel
    {
        private MinesweeperComponentViewModel displayViewModel;
        private MinesweeperComponentViewModel tileBoardViewModel;
        private ViewModelBase menuViewModel;
        private Timer gameTimer;
        private bool gameStarted;

        public MinesweeperViewModel()
        {
            Mediator.Instance.Register(ViewModelMessages.CreateNewBoard, this.OnCreateNewBoard);
            Mediator.Instance.Register(ViewModelMessages.GameStarted, this.OnGameStarted);
            Mediator.Instance.Register(ViewModelMessages.GameOver, this.OnGameOver);
            Mediator.Instance.Register(ViewModelMessages.Victory, this.OnVictory);

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

            var minesweeper = this.Minesweeper;
            Settings.LastBoardWidth = minesweeper.Tiles.Width;
            Settings.LastBoardHeight = minesweeper.Tiles.Height;
            Settings.LastBoardMineCount = minesweeper.MineCount;

            this.DisplayViewModel.Minesweeper = minesweeper;
            this.TileBoardViewModel.Minesweeper = minesweeper;          
        }

        private void OnGameStarted(object paramter)
        {
            this.gameStarted = true;
            this.gameTimer = new Timer(TimerProcThread, null, 1000, 1000);
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
            this.DisposeGameTimer();
            Mediator.Instance.Notify(ViewModelMessages.UpdateSmileyIndex, SmileyState.GameOver);
        }

        private void OnVictory(object paramter)
        {
            this.DisposeGameTimer();
            Mediator.Instance.Notify(ViewModelMessages.UpdateSmileyIndex, SmileyState.Victory);
        }

        private void ResetGame()
        {
            this.DisposeGameTimer();
            this.gameStarted = false;
            //this.minesweeper = null;
        }

        private void DisposeGameTimer()
        {
            if (this.gameTimer != null)
            {
                this.gameTimer.Dispose();
                this.gameTimer = null;
            }
        }

        private void TimerProcThread(object state)
        {
            this.Minesweeper.TimeElapsed++;
        }
    }
}
