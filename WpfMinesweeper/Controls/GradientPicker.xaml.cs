namespace WpfMinesweeper.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Windows.Shapes;

    /// <summary>
    ///     Interaction logic for GradientPicker.xaml
    /// </summary>
    public partial class GradientPicker : UserControl
    {
        private static GradientBrush defaultGradientBrush = new LinearGradientBrush(Colors.Red, Colors.Black, new Point(0, 0), new Point(1, 1));

        public GradientBrush SelectedBrush
        {
            get { return (GradientBrush)GetValue(SelectedBrushProperty); }
            set { SetValue(SelectedBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedBrushProperty =
            DependencyProperty.Register("SelectedBrush", typeof(GradientBrush), typeof(GradientPicker), new PropertyMetadata(defaultGradientBrush));



        public GradientPicker()
        {
            InitializeComponent();
            this.Width = 400;
            this.Height = 300;
        }
    }
}
