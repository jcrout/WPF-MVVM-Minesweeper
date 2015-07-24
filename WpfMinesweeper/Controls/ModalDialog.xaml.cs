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
using System.Windows.Shapes;

namespace WpfMinesweeper.Controls
{
    /// <summary>
    /// Interaction logic for ModalDialog.xaml
    /// </summary>
    public partial class ModalDialog : Window
    {
        public static DependencyProperty ReturnValueProperty = DependencyProperty.Register(
            "ReturnValue",
            typeof(object),
            typeof(ModalDialog));

        public ModalDialog()
        {
            this.ShowInTaskbar = false;          
            this.SizeToContent = System.Windows.SizeToContent.WidthAndHeight;
            InitializeComponent();
        }

        public object ReturnValue
        {
            get
            {
                return this.GetValue(ReturnValueProperty);
            }
            set
            {
                this.SetValue(ReturnValueProperty, value);
            }
        }

        public void ShowAsModal()
        {            
            this.ShowDialog();
        }
    }
}
