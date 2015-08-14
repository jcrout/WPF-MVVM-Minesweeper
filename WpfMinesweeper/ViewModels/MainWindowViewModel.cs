namespace WpfMinesweeper.ViewModels
{
    using System.Windows;
    using Models;

    public class MainWindowViewModel : ViewModelBase
    {
        private double height;
        private bool initialized;
        private double left;
        private double minHeight;
        private double minWidth;
        private SizeToContent sizeToContentMode = SizeToContent.WidthAndHeight;
        private double top;
        private ViewModelBase viewModel;
        private double width;
        private WindowState windowState = WindowState.Normal;

        public MainWindowViewModel()
        {
            this.ViewModel = new MinesweeperViewModel();
            this.PositionWindow();
        }

        protected override void OnMediatorChanged()
        {
            base.OnMediatorChanged();
            Mediator.Register(ViewModelMessages.TileBoardSizeChanged, o => this.OnTileBoardInitialized());
        }

        public double Height
        {
            get
            {
                return this.height;
            }
            set
            {
                if (this.height != value)
                {
                    this.height = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public double Left
        {
            get
            {
                return this.left;
            }
            set
            {
                if (this.left != value)
                {
                    this.left = value;
                    this.Settings.LastLocation = new Point(this.left, this.top);
                    this.OnPropertyChanged();
                }
            }
        }

        public double MinHeight
        {
            get
            {
                return this.minHeight;
            }
            set
            {
                if (this.minHeight != value)
                {
                    this.minHeight = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public double MinWidth
        {
            get
            {
                return this.minWidth;
            }
            set
            {
                if (this.minWidth != value)
                {
                    this.minWidth = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public SizeToContent SizeToContentMode
        {
            get
            {
                return this.sizeToContentMode;
            }
            set
            {
                if (this.sizeToContentMode != value)
                {
                    this.sizeToContentMode = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public double Top
        {
            get
            {
                return this.top;
            }
            set
            {
                if (this.top != value)
                {
                    this.top = value;
                    this.Settings.LastLocation = new Point(this.left, this.top);
                    this.OnPropertyChanged();
                }
            }
        }

        public ViewModelBase ViewModel
        {
            get
            {
                return this.viewModel;
            }
            private set
            {
                if (this.viewModel != value)
                {
                    this.viewModel = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public double Width
        {
            get
            {
                return this.width;
            }
            set
            {
                if (this.width != value)
                {
                    this.width = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public WindowState WindowState
        {
            get
            {
                return this.windowState;
            }
            set
            {
                if (this.windowState != value)
                {
                    this.windowState = value;
                    Mediator.Notify(ViewModelMessages.GameWindowStateChanged, value);
                    this.OnPropertyChanged();
                }
            }
        }

        private void CenterWindow()
        {
            this.Left = (SystemParameters.FullPrimaryScreenWidth - this.width) / 2;
            this.Top = (SystemParameters.FullPrimaryScreenHeight - this.height) / 2;
        }

        private void OnTileBoardInitialized()
        {
            // GUI code in the ViewModel; consider replacing if a better solution involving binding can be used, one that does not involve sizing to content and back to manual;
            var window = App.Current.MainWindow;
            window.Measure(new Size(SystemParameters.WorkArea.Width, SystemParameters.WorkArea.Height));
            window.Arrange(new Rect(0, 0, window.DesiredSize.Width, window.DesiredSize.Height));
            window.UpdateLayout();

            var desiredSize = window.DesiredSize;
            this.MinWidth = desiredSize.Width;
            this.MinHeight = desiredSize.Height;
            this.Width = this.minWidth;
            this.Height = this.minHeight;
        }

        private void PositionWindow()
        {
            if (this.Settings.LastLocation.X < 0 || this.Settings.LastLocation.Y < 0)
            {
                this.CenterWindow();
            }
            else
            {
                var point = this.Settings.LastLocation;
                this.Left = point.X;
                this.Top = point.Y;
            }
        }
    }
}
