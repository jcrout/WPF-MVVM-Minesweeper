namespace WpfMinesweeper.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using WpfMinesweeper.Models;

    class DisplayPanelViewModel : ViewModelBase
    {
        private IMinesweeper minesweeper;
        private ImageSource smileyImage;
        private ImageSource smileyBackground;

        public DisplayPanelViewModel(IMinesweeper minesweeper)
        {
            this.minesweeper = minesweeper;
            this.minesweeper.PropertyChanged += minesweeper_PropertyChanged;
            this.smileyImage = new BitmapImage(new Uri("pack://application:,,,/WpfMinesweeper;component/Resources/Animations/SmileyDefault.gif", UriKind.Absolute));

            // temp
            var target = new RenderTargetBitmap(23, 23, 96, 96, PixelFormats.Pbgra32);
            var newImage = new DrawingVisual();
            using (var dc = newImage.RenderOpen())
            {
                dc.DrawRectangle(Brushes.LightBlue, null, new System.Windows.Rect(0, 0, 23, 23));
            }

            target.Render(newImage);
            this.smileyBackground = target;
       
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
                    if (this.minesweeper != null)
                    {
                        this.minesweeper.PropertyChanged -= minesweeper_PropertyChanged;
                    }
                    this.minesweeper = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public int MinesRemaining
        {
            get
            {
                return this.minesweeper != null ? this.minesweeper.MinesRemaining : 0;
            }
        }

        public int TimeElapsed
        {
            get
            {
                return this.minesweeper != null ? this.minesweeper.TimeElapsed : 0;
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
    }
}
