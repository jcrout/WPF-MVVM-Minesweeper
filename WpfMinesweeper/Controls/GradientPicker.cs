namespace WpfMinesweeper.Controls
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    /// <summary>
    ///     Interaction logic for GradientPicker.xaml
    /// </summary>
    public partial class GradientPicker : UserControl
    {
        private static readonly GradientBrush defaultGradientBrush = new LinearGradientBrush(Colors.Red, Colors.Black, new Point(0, 0), new Point(1, 1));

        public static readonly DependencyProperty SelectedBrushProperty =
            DependencyProperty.Register("SelectedBrush", typeof(GradientBrush), typeof(GradientPicker), new PropertyMetadata(GradientPicker.defaultGradientBrush));

        // Using a DependencyProperty as the backing store for SelectedBrush.  This enables animation, styling, binding, etc...
        public GradientPicker()
        {
            this.InitializeComponent();
            this.Width = 400;
            this.Height = 300;
        }

        public GradientBrush SelectedBrush
        {
            get
            {
                return (GradientBrush)this.GetValue(GradientPicker.SelectedBrushProperty);
            }
            set
            {
                this.SetValue(GradientPicker.SelectedBrushProperty, value);
            }
        }
    }
}
