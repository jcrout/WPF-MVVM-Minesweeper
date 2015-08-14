namespace WpfMinesweeper.ViewModels
{
    using System;
    using System.Windows;
    using JonUtility;
    using Models;

    public class MinesweeperViewModel : MinesweeperViewModelBase
    {
        private readonly WinTimer gameTimer;
        private bool gameStarted;
        private bool minimized;

        public MinesweeperViewModel()
        {
            this.gameTimer = new WinTimer(this.TimerProc, 1000);

            this.MenuViewModel = new MenuViewModel();
            this.DisplayViewModel = new DisplayPanelViewModel();
            this.TileBoardViewModel = new TileBoardViewModel();

            this.Minesweeper = MinesweeperFactory.GetFromSettings();
        }

        protected override void OnMediatorChanged()
        {
            var mediator = this.Mediator;
            if (mediator == null)
            {
                return;
            }

            mediator.Register(ViewModelMessages.GameWindowStateChanged, o => this.OnWindowStateChanged((WindowState)o));
            mediator.Register(ViewModelMessages.CreateNewBoard, this.OnCreateNewBoard);
            mediator.Register(ViewModelMessages.GameStarted, this.OnGameStarted);
            mediator.Register(ViewModelMessages.GameOver, this.OnGameOver);
            mediator.Register(ViewModelMessages.Victory, this.OnVictory);
        }

        protected override void OnMinesweeperChanged()
        {
            if (this.gameStarted)
            {
                this.ResetGame();
            }

            var gameStatistics = StatisticsModule.Create();
            var minesweeper = this.Minesweeper;

            this.Settings.LastBoardSize = new BoardSize(minesweeper.Tiles.Width, minesweeper.Tiles.Height, minesweeper.MineCount);

            gameStatistics[Statistic.BoardSize] = new BoardSize(minesweeper.Tiles.Width, minesweeper.Tiles.Height, minesweeper.MineCount);

            this.DisplayViewModel.Minesweeper = minesweeper;
            this.TileBoardViewModel.Minesweeper = minesweeper;

            this.GameStatistics = gameStatistics;
            this.DisplayViewModel.GameStatistics = gameStatistics;
            this.TileBoardViewModel.GameStatistics = gameStatistics;
        }

        private void EndGame(GameResult finalResult)
        {
            this.gameTimer.Stop();

            this.GameStatistics[Statistic.GameEndTime] = DateTime.Now;
            this.GameStatistics[Statistic.GameState] = finalResult;
            this.GameStatistics[Statistic.MinesRemaining] = this.Minesweeper.MinesRemaining;
            this.GameStatistics[Statistic.TimeElapsed] = this.Minesweeper.TimeElapsed;

            this.Settings.Statistics.Add(this.GameStatistics);
            this.Settings.Save();
        }

        private void OnCreateNewBoard(object parameter)
        {
            if (parameter == null)
            {
                this.Minesweeper = MinesweeperFactory.Create(this.Minesweeper);
                return;
            }

            if (parameter is BoardSize)
            {
                this.Minesweeper = MinesweeperFactory.Create((BoardSize)parameter);
            }
            else if (parameter is string)
            {
                var boardSize = BoardSize.Parse(parameter.ToString());
                this.Minesweeper = MinesweeperFactory.Create(boardSize);
            }
            else
            {
                throw new ArgumentException("parameter must either be of type String or BoardSize, or be set to null.");
            }
        }

        private void OnGameOver(object parameter)
        {
            this.EndGame(GameResult.GameOver);
            this.Mediator.Notify(ViewModelMessages.UpdateSmileyIndex, SmileyState.GameOver);
        }

        private void OnGameStarted(object parameter)
        {
            this.gameStarted = true;
            this.gameTimer.Start();
        }

        private void OnVictory(object parameter)
        {
            this.EndGame(GameResult.Victory);
            this.Mediator.Notify(ViewModelMessages.UpdateSmileyIndex, SmileyState.Victory);
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

        private void ResetGame()
        {
            this.gameTimer.Stop();
            this.gameStarted = false;
        }

        private void TimerProc()
        {
            //Console.WriteLine("Thread: " + System.Threading.Thread.CurrentThread.ManagedThreadId);
            this.Minesweeper.TimeElapsed++;
            //if (this.Minesweeper.TimeElapsed % 3 == 0)
            //{

            //    Mediator.Notify(
            //        ViewModelMessages.TileColorsChanged,
            //        new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.DarkBlue));
            //    //    this.TileShadingMode = Controls.TileShadingMode.AllTiles;
            //    //this.TileBrush = new LinearGradientBrush(Color.FromArgb(100, 255, 0, 0), Color.FromArgb(100, 0, 0, 255), new Point(0, 0), new Point(.75, .75));

            //}
        }
    }
}
