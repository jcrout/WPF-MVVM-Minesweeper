namespace WpfMinesweeper.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Threading;
    using WpfMinesweeper.Models;

    class MinesweeperViewModel : ViewModelBase
    {
        private IMinesweeper minesweeper;
        private ViewModelBase displayViewModel;
        private ViewModelBase tileBoardViewModel;
        private ViewModelBase menuViewModel;
        private Timer gameTimer;
        private bool gameStarted;

        public MinesweeperViewModel()
        {
            Mediator.Instance.Register(ViewModelMessages.CreateNewBoard, this.OnCreateNewBoard);
            Mediator.Instance.Register(ViewModelMessages.GameStarted, this.OnGameStarted);
            Mediator.Instance.Register(ViewModelMessages.GameOver, this.OnGameOver);

            this.CreateNewBoard(MinesweeperFactory.Create(9, 9, 10));
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
                    this.OnPropertyChanged();
                }
            }
        }

        public ViewModelBase DisplayViewModel
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

        public ViewModelBase TileBoardViewModel
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

        private void CreateNewBoard(IMinesweeper minesweeper)
        {
            if (this.gameStarted)
            {
                this.ResetGame();
            }

            this.Minesweeper = minesweeper;
            this.DisplayViewModel = new DisplayPanelViewModel(this.minesweeper);
            this.TileBoardViewModel = new TileBoardViewModel(this.minesweeper);
            this.MenuViewModel = new MenuViewModel();
        }

        private void OnGameStarted(object paramter)
        {
            this.gameStarted = true;
            this.gameTimer = new Timer(TimerProcThread, null, 1000, 1000);
        }

        private void OnCreateNewBoard(object paramter)
        {
            if (!(paramter is string))
            {
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
                width = (paramters[0] == "*") ? this.minesweeper.Tiles.Width : int.Parse(paramters[0]);
                height = (paramters[1] == "*") ? this.minesweeper.Tiles.Height : int.Parse(paramters[1]);
                mineCount = (paramters[2] == "*") ? this.minesweeper.MineCount : int.Parse(paramters[2]);
            }
            catch (FormatException ex)
            {
                throw new ArgumentException("paramter must contain 3 integer values.");
            }

            var newBoard = MinesweeperFactory.Create(width, height, mineCount);
            this.CreateNewBoard(newBoard);
        }

        private void OnGameOver(object paramter)
        {
            this.DisposeGameTimer();         
        }

        private void ResetGame()
        {
            this.DisposeGameTimer();
            this.gameStarted = false;
            this.minesweeper = null;
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
            this.minesweeper.TimeElapsed++;
        }
    }
}
