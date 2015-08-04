﻿namespace WpfMinesweeper.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    public class NumberBox : FrameworkElement
    {
        private static readonly ImageSource[] bitmaps;

        public static readonly DependencyProperty NumberProperty =
            DependencyProperty.Register(
                "Number",
                typeof(int),
                typeof(NumberBox),
                new FrameworkPropertyMetadata(
                    0,
                    FrameworkPropertyMetadataOptions.AffectsRender,
                    NumberBox.OnNumberChanged,
                    NumberBox.CoerceNumberCallback));

        public static readonly DependencyProperty DigitsProperty =
            DependencyProperty.Register(
                "Digits",
                typeof(int),
                typeof(NumberBox),
                new FrameworkPropertyMetadata(
                    0,
                    FrameworkPropertyMetadataOptions.AffectsRender,
                    NumberBox.OnDigitsChanged,
                    NumberBox.CoerceDigitsCallback));

        private int maxNumber = 999;
        private int minNumber = -99;

        static NumberBox()
        {
            NumberBox.bitmaps = new ImageSource[11];
            for (var i = 0; i < 10; i++)
            {
                NumberBox.bitmaps[i] = new BitmapImage(new Uri("pack://application:,,,/WpfMinesweeper;component/Resources/Images/N" + i + ".png",
                    UriKind.Absolute));
            }

            NumberBox.bitmaps[10] = new BitmapImage(new Uri("pack://application:,,,/WpfMinesweeper;component/Resources/Images/NMinus.png",
                UriKind.Absolute));
        }

        public NumberBox()
        {
            this.Digits = 3;
        }

        public int Digits
        {
            get
            {
                return (int)this.GetValue(
                    NumberBox.DigitsProperty);
            }
            set
            {
                this.SetValue(
                    NumberBox.DigitsProperty,
                    value);
            }
        }

        public int Number
        {
            get
            {
                return (int)this.GetValue(
                    NumberBox.NumberProperty);
            }
            set
            {
                this.SetValue(
                    NumberBox.NumberProperty,
                    value);
            }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(
                drawingContext);

            var digits = this.Digits;
            var number = this.Number;
            var imgWidth = NumberBox.bitmaps[0].Width;
            var imgHeight = NumberBox.bitmaps[0].Height;

            var numberText = number < 0 ? number.ToString(
                "0".PadRight(
                    digits - 1,
                    '0')) : number.ToString(
                        "0".PadRight(
                            digits,
                            '0'));

            for (var i = 0; i < digits; i++)
            {
                var numIndex = numberText[i] == '-' ? 10 : numberText[i] - 48;

                drawingContext.DrawImage(
                    NumberBox.bitmaps[numIndex],
                    new Rect((imgWidth * i),
                        0d,
                        imgWidth,
                        imgHeight));
            }
        }

        private static object CoerceDigitsCallback(DependencyObject d, object value)
        {
            var current = (int)value;
            if (current < 1)
            {
                return 1;
            }

            if (current > 6)
            {
                return 6;
            }

            return current;
        }

        private static object CoerceNumberCallback(DependencyObject d, object value)
        {
            var numberBox = (NumberBox)d;
            var current = (int)value;

            if (current > numberBox.maxNumber)
            {
                return numberBox.maxNumber;
            }

            if (current < numberBox.minNumber)
            {
                return numberBox.minNumber;
            }

            return current;
        }

        private static void OnDigitsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var numberBox = (NumberBox)d;
            var newDigits = (int)e.NewValue;
            numberBox.InitializeNumberBox();
        }

        private static void OnNumberChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        private void InitializeNumberBox()
        {
            var digits = (int)this.GetValue(
                NumberBox.DigitsProperty);
            this.Width = digits * NumberBox.bitmaps[0].Width;
            this.Height = NumberBox.bitmaps[0].Height;
            this.maxNumber = (int)Math.Pow(
                10d,
                digits) - 1;
            this.minNumber = -1 * (int)Math.Pow(
                10d,
                digits - 1) + 1;
        }
    }
}
