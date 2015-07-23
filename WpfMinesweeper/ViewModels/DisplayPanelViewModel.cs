namespace WpfMinesweeper.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using WpfMinesweeper.Models;

    class DisplayPanelViewModel : MinesweeperComponentViewModel
    {
        private static Dictionary<SmileyState, ImageSource> smileyImages;
        private static Brush defaultSmileyBorderBrush = Brushes.White;

        private ICommand borderSizeCommand;
        private ImageSource smileyImage;
        private ImageSource smileyBackground;
        private Brush smileyBorderBrush;
        private string repeatAnimation;

        static DisplayPanelViewModel()
        {
            smileyImages = new Dictionary<SmileyState, ImageSource>
            {
                {SmileyState.Default, new BitmapImage(new Uri("pack://application:,,,/WpfMinesweeper;component/Resources/Animations/SmileyDefault.gif", UriKind.Absolute))},
                {SmileyState.TapDown, new BitmapImage(new Uri("pack://application:,,,/WpfMinesweeper;component/Resources/Animations/SmileyTapDown.gif", UriKind.Absolute))},
                {SmileyState.GameOver, new BitmapImage(new Uri("pack://application:,,,/WpfMinesweeper;component/Resources/Animations/SmileyGameOver.gif", UriKind.Absolute))},
                {SmileyState.Victory, new BitmapImage(new Uri("pack://application:,,,/WpfMinesweeper;component/Resources/Animations/SmileyVictory.gif", UriKind.Absolute))}
            };
        }

        public DisplayPanelViewModel()
        {
            Mediator.Instance.Register(ViewModelMessages.UpdateSmileyIndex, o => this.OnUpdateSmileyIndex((SmileyState)o));
            Mediator.Instance.Register(ViewModelMessages.TileColorsChanged, o => this.UpdateSmileyBackgroundImage((Brush)o));

            this.borderSizeCommand = new Command(OnBorderSizeCommand);
        }

        private void UpdateSmileyBackgroundImage(Brush brush)
        {
            var target = new RenderTargetBitmap(23, 23, 96, 96, PixelFormats.Pbgra32);
            var newImage = new DrawingVisual();
            using (var dc = newImage.RenderOpen())
            {
                dc.DrawRectangle(brush, null, new System.Windows.Rect(0, 0, 23, 23));
            }

            target.Render(newImage);
            this.SmileyBorderBrush = brush;
            this.SmileyBackground = target;
        }

        protected override void OnMinesweeperChanged()
        {       
            this.Minesweeper.PropertyChanged += minesweeper_PropertyChanged;
            this.OnPropertyChanged("TimeElapsed");
            this.OnPropertyChanged("MinesRemaining");
            this.OnUpdateSmileyIndex(SmileyState.Default);
        }

        public ICommand BoardSizeCommand
        {
            get
            {
                return this.borderSizeCommand;
            }
            set
            {
                if (this.borderSizeCommand != value)
                {
                    this.borderSizeCommand = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public int MinesRemaining
        {
            get
            {
                return this.Minesweeper != null ? this.Minesweeper.MinesRemaining : 0;
            }            
        }

        public int TimeElapsed
        {
            get
            {
                return this.Minesweeper != null ? this.Minesweeper.TimeElapsed : 0;
            }
        }

        public Brush SmileyBorderBrush
        {
            get
            {
                return this.smileyBorderBrush;
            }
            set
            {
                if (this.smileyBorderBrush != value)
                {
                    this.smileyBorderBrush = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public ImageSource SmileyImage
        {
            get
            {
                return this.smileyImage;
            }
            set
            {
                if (this.smileyImage != value)
                {
                    this.smileyImage = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public string RepeatAnimation
        {
            get
            {
                return this.repeatAnimation;
            }
            set
            {
                if (this.repeatAnimation != value)
                {
                    this.repeatAnimation = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public ImageSource SmileyBackground
        {
            get
            {
                return this.smileyBackground;
            }
            set
            {
                if (this.smileyBackground != value)
                {
                    this.smileyBackground = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private void minesweeper_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "TimeElapsed")
            {
                this.OnPropertyChanged("TimeElapsed");
            }
            else if (e.PropertyName == "MinesRemaining")
            {
                this.OnPropertyChanged("MinesRemaining");
            }
        }

        private void OnUpdateSmileyIndex(SmileyState newState)
        {
            this.SmileyImage = smileyImages[newState];
            this.RepeatAnimation = (newState == SmileyState.GameOver) ? "1x" : "Forever";
        }

        private void OnBorderSizeCommand()
        {
            Mediator.Instance.Notify(ViewModelMessages.CreateNewBoard);
        }
    }
}
