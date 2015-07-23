using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfMinesweeper.ViewModels
{
    class MainWindowViewModel : ViewModelBase
    {
        private SizeToContent sizeToContentMode = SizeToContent.WidthAndHeight;
        private ViewModelBase viewModel;
        private double minWidth = 144;
        private double minHeight = 200;
        private double height = 144;
        private double width = 200;

        public MainWindowViewModel()
        {
            Mediator.Instance.Register(ViewModelMessages.TileBoardSizeChanged, o => this.OnTileBoardInitialized(o));
            this.viewModel = new WpfMinesweeper.ViewModels.MinesweeperViewModel();            
        }

        private async void OnTileBoardInitialized(object paramter)
        {    
            await Task.Delay(1);
            this.MinWidth = 0;
            this.MinHeight = 0;
            this.SizeToContentMode = SizeToContent.WidthAndHeight;
            this.MinWidth = this.width;
            this.MinHeight = this.height;
            this.SizeToContentMode = SizeToContent.Manual;
        }

        public ViewModelBase ViewModel
        {
            get
            {
                return this.viewModel;
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
    }
}
