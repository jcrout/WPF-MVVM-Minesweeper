namespace WpfMinesweeper.Controls
{
    using System.Windows;

    /// <summary>
    ///     Interaction logic for ModalDialog.xaml
    /// </summary>
    public partial class ModalDialog : Window
    {
        public ModalDialog()
        {
            this.ShowInTaskbar = false;
            this.SizeToContent = SizeToContent.WidthAndHeight;
            this.InitializeComponent();
        }

        public object ReturnValue
        {
            get
            {
                return this.GetValue(
                    ModalDialog.ReturnValueProperty);
            }
            set
            {
                this.SetValue(
                    ModalDialog.ReturnValueProperty,
                    value);
            }
        }

        public void ShowAsModal()
        {
            this.ShowDialog();
        }

        public static DependencyProperty ReturnValueProperty = DependencyProperty.Register(
            "ReturnValue",
            typeof(object),
            typeof(ModalDialog));
    }
}
