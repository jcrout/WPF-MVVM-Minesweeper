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

namespace WpfMinesweeper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IClosable
    {
        public MainWindow()
        {
            Mediator.Instance.Register(ViewModelMessages.UpdateContainerSize, o => this.OnUpdateWindowSize(o));
            InitializeComponent();
        }

        private void OnUpdateWindowSize(object paramter)
        {
            this.MaxHeight = SystemParameters.FullPrimaryScreenHeight;
            this.MaxWidth = SystemParameters.FullPrimaryScreenWidth;
            this.MinWidth = 0;
            this.MinHeight = 0;
            this.SizeToContent = System.Windows.SizeToContent.WidthAndHeight;
            double width = this.Width;
            double height = this.Height;
            this.SizeToContent = System.Windows.SizeToContent.Manual;
            this.Width = width;
            this.Height = height;
            this.MinWidth = width;
            this.MinHeight = height;
        }

        void IClosable.Close()
        {
            this.Close();
        }
    }
}
