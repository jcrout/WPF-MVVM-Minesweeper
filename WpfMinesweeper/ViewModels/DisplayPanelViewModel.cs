﻿namespace WpfMinesweeper.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using JonUtility.WPF;
    using Models;

    public class DisplayPanelViewModel : MinesweeperComponentViewModel
    {
        private static readonly Dictionary<SmileyState, ImageSource> smileyImages;
        private static Brush defaultSmileyBorderBrush = Brushes.White;
        private ICommand boardSizeCommand;
        private string repeatAnimation;
        private ImageSource smileyBackground;
        private Brush smileyBorderBrush;
        private ImageSource smileyImage;

        static DisplayPanelViewModel()
        {
            DisplayPanelViewModel.smileyImages = new Dictionary<SmileyState, ImageSource> {{SmileyState.Default, new BitmapImage(new Uri("pack://application:,,,/WpfMinesweeper;component/Resources/Animations/SmileyDefault.gif", UriKind.Absolute))}, {SmileyState.TapDown, new BitmapImage(new Uri("pack://application:,,,/WpfMinesweeper;component/Resources/Animations/SmileyTapDown.gif", UriKind.Absolute))}, {SmileyState.GameOver, new BitmapImage(new Uri("pack://application:,,,/WpfMinesweeper;component/Resources/Animations/SmileyGameOver.gif", UriKind.Absolute))}, {SmileyState.Victory, new BitmapImage(new Uri("pack://application:,,,/WpfMinesweeper;component/Resources/Animations/SmileyVictory.gif", UriKind.Absolute))}};
        }

        public DisplayPanelViewModel()
        {
            this.BoardSizeCommand = new Command(this.OnBorderSizeCommand);
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

        public int MinesRemaining
        {
            get
            {
                return this.Minesweeper != null ? this.Minesweeper.MinesRemaining : 0;
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

        public int TimeElapsed
        {
            get
            {
                return this.Minesweeper != null ? this.Minesweeper.TimeElapsed : 0;
            }
        }

        protected override void OnMediatorChanged()
        {
            var mediator = this.Mediator;
            if (mediator == null)
            {
                return;
            }

            this.Mediator.Register(ViewModelMessages.UpdateSmileyIndex, o => this.OnUpdateSmileyIndex((SmileyState)o));
            this.Mediator.Register(ViewModelMessages.TileColorsChanged, o => this.UpdateSmileyBackgroundImage((Brush)o));
        }

        protected override void OnMinesweeperChanged()
        {
            this.Minesweeper.PropertyChanged += this.minesweeper_PropertyChanged;
            this.OnPropertyChanged("TimeElapsed");
            this.OnPropertyChanged("MinesRemaining");
            this.OnUpdateSmileyIndex(SmileyState.Default);
        }

        private void minesweeper_PropertyChanged(object sender, PropertyChangedEventArgs e)
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

        private void OnBorderSizeCommand()
        {
            this.Mediator.Notify(ViewModelMessages.CreateNewBoard);
        }

        private void OnUpdateSmileyIndex(SmileyState newState)
        {
            this.SmileyImage = DisplayPanelViewModel.smileyImages[newState];
            this.RepeatAnimation = (newState == SmileyState.GameOver) ? "1x" : "Forever";
        }

        private void UpdateSmileyBackgroundImage(Brush brush)
        {
            var target = new RenderTargetBitmap(23, 23, 96, 96, PixelFormats.Pbgra32);
            var newImage = new DrawingVisual();
            using (var dc = newImage.RenderOpen())
            {
                dc.DrawRectangle(brush, null, new Rect(0, 0, 23, 23));
            }

            target.Render(newImage);
            this.SmileyBorderBrush = brush;
            this.SmileyBackground = target;
        }
    }
}
