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
            Mediator.Register(ViewModelMessages.TileBoardSizeChanged, o => this.OnTileBoardInitialized());

            this.minWidth = this.Settings.LastWindowMinSize.Width;
            this.width = this.minWidth;
            this.ViewModel = new MinesweeperViewModel();

            this.PositionWindow();
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
            if (!this.initialized)
            {
                this.initialized = true;
                this.MinWidth = this.Settings.LastWindowMinSize.Width;
                this.MinHeight = this.Settings.LastWindowMinSize.Height;
                this.SizeToContentMode = SizeToContent.Manual;
                return;
            }

            this.MinWidth = 0;
            this.MinHeight = 0;
            this.SizeToContentMode = SizeToContent.WidthAndHeight;
            this.MinWidth = this.width;
            this.MinHeight = this.height;
            this.SizeToContentMode = SizeToContent.Manual;

            this.Settings.LastWindowMinSize = new Size(this.width, this.height);
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
