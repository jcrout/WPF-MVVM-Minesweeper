namespace WpfMinesweeper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Threading;

    public class NumberBox : FrameworkElement
    {
        private static ImageSource[] bitmaps;
        private int minNumber = -99;
        private int maxNumber = 999;

        static NumberBox()
        {
            bitmaps = new ImageSource[11];
            for (int i = 0; i < 10; i++)
            {
                bitmaps[i] = new BitmapImage(new Uri("pack://application:,,,/WpfMinesweeper;component/Resources/Images/N" + i.ToString() + ".png", UriKind.Absolute));
            }

            bitmaps[10] = new BitmapImage(new Uri("pack://application:,,,/WpfMinesweeper;component/Resources/Images/NMinus.png", UriKind.Absolute));
        }

        public static readonly DependencyProperty NumberProperty =
            DependencyProperty.Register(
            "Number",
            typeof(int),
            typeof(NumberBox),
            new FrameworkPropertyMetadata(
                0,
                FrameworkPropertyMetadataOptions.AffectsRender,
                OnNumberChanged,
                CoerceNumberCallback));

        public static readonly DependencyProperty DigitsProperty =
            DependencyProperty.Register(
            "Digits",
            typeof(int),
            typeof(NumberBox),
            new FrameworkPropertyMetadata(
                0,
                FrameworkPropertyMetadataOptions.AffectsRender,
                OnDigitsChanged,
                CoerceDigitsCallback));

        private static object CoerceNumberCallback(DependencyObject d, object value)
        {
            var numberBox = (NumberBox)d;
            int current = (int)value;
     
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

        private static object CoerceDigitsCallback(DependencyObject d, object value)
        {
            int current = (int)value;
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

        private static void OnNumberChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {                    
        }

        private static void OnDigitsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var numberBox = (NumberBox)d;
            int newDigits = (int)e.NewValue;
            numberBox.InitializeNumberBox();
        }

        public NumberBox()
        {
            this.Digits = 3;      
        }

        private void InitializeNumberBox()
        {
            int digits = (int)this.GetValue(DigitsProperty);
            this.Width = digits * bitmaps[0].Width;
            this.Height = bitmaps[0].Height;
            this.maxNumber = (int)Math.Pow(10d, digits) - 1;
            this.minNumber = -1 * (int)Math.Pow(10d, digits - 1) + 1;
        }

        public int Number
        {
            get
            {
                return (int)this.GetValue(NumberProperty);
            }
            set
            {
                this.SetValue(NumberProperty, value);
            }
        }

        public int Digits
        {
            get
            {
                return (int)this.GetValue(DigitsProperty);
            }
            set
            {
                this.SetValue(DigitsProperty, value);
            }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            int digits = this.Digits;
            int number = this.Number;
            double imgWidth = bitmaps[0].Width;
            double imgHeight = bitmaps[0].Height;

            var numberText = number < 0 ? number.ToString("0".PadRight(digits - 1, '0')) : number.ToString("0".PadRight(digits, '0'));
            int numIndex = 0;

            for (int i = 0; i < digits; i++)
            {
                if (numberText[i] == '-')
                {
                    numIndex = 10;
                }
                else
                {
                    numIndex = ((int)numberText[i]) - 48;
                }
                drawingContext.DrawImage(bitmaps[numIndex], new Rect((imgWidth * (double)i), 0d, imgWidth, imgHeight));
            }
        }

    }
}
