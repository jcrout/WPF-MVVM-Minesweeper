namespace WpfMinesweeper.Controls
{
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Markup;
    using System.Windows.Media;

    [ContentProperty("Content"), DefaultProperty("Content"),
     Localizability(LocalizationCategory.None, Readability = Readability.Unreadable)]
    public class WindowCreator : FrameworkElement
    {
        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(
            "Content",
            typeof(object),
            typeof(WindowCreator),
            new PropertyMetadata(null, WindowCreator.ContentChanged));

        [Bindable(true)]
        public object Content
        {
            get
            {
                return this.GetValue(WindowCreator.ContentProperty);
            }
            set
            {
                this.SetValue(WindowCreator.ContentProperty, value);
            }
        }

        protected override int VisualChildrenCount
        {
            get
            {
                return 0;
            }
        }

        public void Show()
        {
            var window = new Window
            {
                SizeToContent = SizeToContent.WidthAndHeight,
                ResizeMode = ResizeMode.NoResize,
                ShowInTaskbar = false,
                Content = this.Content
            };

            if (this.Parent != null)
            {
                window.Owner = Window.GetWindow(this.Parent);
                window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }
            else
            {
                window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }

            window.ShowDialog();
        }

        protected override Visual GetVisualChild(int index)
        {
            return null;
        }

        private static void ContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var mwc = (WindowCreator)d;

            if (e.OldValue != null)
            {
                mwc.RemoveLogicalChild(e.OldValue);
            }

            if (e.NewValue != null)
            {
                mwc.AddLogicalChild(e.NewValue);
            }
        }
    }
}
