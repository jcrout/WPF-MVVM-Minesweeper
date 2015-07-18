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
        private ViewModelBase viewModel;
        private int minWidth = 550;
        private int minHeight = 550;
        private int height = 600;
        private int width = 700;

        public MainWindowViewModel()
        {
            this.viewModel = new WpfMinesweeper.ViewModels.MinesweeperViewModel();            
        }

        public ViewModelBase ViewModel
        {
            get
            {
                return this.viewModel;
            }
        }

        public double MinWidth
        {
            get
            {
                return this.minWidth;
            }
        }

        public double MinHeight
        {
            get
            {
                return this.minHeight;
            }
        }

        public double Width
        {
            get
            {
                return this.width;
            }
        }

        public double Height
        {
            get
            {
                return this.height;
            }
        }
    }
}
