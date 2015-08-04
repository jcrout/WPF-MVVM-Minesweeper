﻿namespace WpfMinesweeper.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;

    public class MainWindowViewModel : ViewModelBase
    {
        private WindowState windowState = WindowState.Normal;
        private SizeToContent sizeToContentMode = SizeToContent.WidthAndHeight;
        private ViewModelBase viewModel;
        private double minWidth = 0;
        private double minHeight = 0;
        private double height = 0;
        private double width = 0;
        private double left = 0;
        private double top = 0;
        private bool initialized = false;

        public MainWindowViewModel()
        {
            Mediator.Instance.Register(ViewModelMessages.TileBoardSizeChanged,
                o => this.OnTileBoardInitialized(o));

            this.minWidth = ViewModelBase.Settings.LastWindowMinSize.Width;
            this.width = this.minWidth;
            this.ViewModel = new MinesweeperViewModel();

            this.PositionWindow();
        }

        private void OnTileBoardInitialized(object paramter)
        {
            if (!this.initialized)
            {
                this.initialized = true;
                this.MinWidth = ViewModelBase.Settings.LastWindowMinSize.Width;
                this.MinHeight = ViewModelBase.Settings.LastWindowMinSize.Height;
                this.SizeToContentMode = SizeToContent.Manual;
                return;
            }

            this.MinWidth = 0;
            this.MinHeight = 0;
            this.SizeToContentMode = SizeToContent.WidthAndHeight;
            this.MinWidth = this.width;
            this.MinHeight = this.height;
            this.SizeToContentMode = SizeToContent.Manual;

            ViewModelBase.Settings.LastWindowMinSize = new Size(this.width,
                this.height);
        }

        private void PositionWindow()
        {
            if (ViewModelBase.Settings.LastLocation.X < 0 || ViewModelBase.Settings.LastLocation.Y < 0)
            {
                this.CenterWindow();
            }
            else
            {
                var point = ViewModelBase.Settings.LastLocation;
                this.Left = point.X;
                this.Top = point.Y;
            }
        }

        private void CenterWindow()
        {
            this.Left = (SystemParameters.FullPrimaryScreenWidth - this.width)/2;
            this.Top = (SystemParameters.FullPrimaryScreenHeight - this.height)/2;
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
                    Mediator.Instance.Notify(ViewModelMessages.GameWindowStateChanged,
                        value);
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
                    ViewModelBase.Settings.LastLocation = new Point(this.left,
                        this.top);
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
                    ViewModelBase.Settings.LastLocation = new Point(this.left,
                        this.top);
                    this.OnPropertyChanged();
                }
            }
        }
    }
}