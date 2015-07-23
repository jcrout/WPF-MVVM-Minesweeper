namespace WpfMinesweeper.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media;
    using Xceed.Wpf.Toolkit;

    public class MenuViewModel : ViewModelBase
    {
        private static Color defaultSelectedColor = Settings.TileColor;

        private Color selectedColor;
        private ICommand exitCommand;
        private ICommand boardSizeCommand;
        private ICommand tileColorCommand;
        private double tileSize = 1.00d;

        public MenuViewModel()
        {    
            this.exitCommand = new Command(o => this.OnExit((IClosable)o));
            this.boardSizeCommand = new Command(o => this.OnBoardSizeSelected(o));
            this.tileColorCommand = new Command(o => this.OnTileColorChange(o));
            this.selectedColor = defaultSelectedColor;
        }

        public ICommand ExitCommand
        {
            get
            {
                return this.exitCommand;
            }
            set
            {
                if (this.exitCommand != value)
                {
                    this.exitCommand = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public ICommand BoardSizeCommand
        {
            get
            {
                return this.boardSizeCommand;
            }
            set
            {
                if (this.boardSizeCommand != value)
                {
                    this.boardSizeCommand = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public ICommand TileColorCommand
        {
            get
            {
                return this.tileColorCommand;
            }
            set
            {
                if (this.tileColorCommand != value)
                {
                    this.tileColorCommand = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public Color SelectedTileColor
        {
            get
            {
                return this.selectedColor;
            }
            set
            {
                if (this.selectedColor != value)
                {
                    this.selectedColor = value;
                    Settings.TileColor = value;
                    var brush = new SolidColorBrush(value);
                    Mediator.Instance.Notify(ViewModelMessages.TileColorsChanged, brush);
                    this.OnPropertyChanged();
                }
            }
        }

        public double TileSize
        {
            get
            {
                return this.tileSize;
            }
            set
            {
                if (this.tileSize != value)
                {
                    this.tileSize = value;
                    Mediator.Instance.Notify(ViewModelMessages.TileSizeChanged, value);
                    this.OnPropertyChanged();
                }
            }
        }

        private void OnTileColorChange(object paramter)
        {
           // var windowz = new WpfMinesweeper.Views.ColorPickerView();
           // windowz.ShowDialog();
          
           //// this.ShowTileColorPicker = true;
        }

        private void OnBoardSizeSelected(object paramter)
        {
            if (string.Equals(paramter.ToString(), "custom"))
            {
                var windowz = new WpfMinesweeper.Controls.ModalDialog();
                windowz.Content = new WpfMinesweeper.Views.CustomBoardView();
                windowz.DataContext = new WpfMinesweeper.ViewModels.CustomBoardViewModel();
                windowz.ShowDialog();
                return;
            }
            Mediator.Instance.Notify(ViewModelMessages.CreateNewBoard, paramter);
        }

        private void OnExit(IClosable paramter)
        {
            paramter.Close();
        }
    }
}
