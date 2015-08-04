namespace WpfMinesweeper.Controls
{
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Markup;
    using System.Windows.Media;

    [ContentProperty("Content"), DefaultProperty("Content"), Localizability(LocalizationCategory.None, Readability = Readability.Unreadable)]
    public class ModalWindowCreator : FrameworkElement
    {
        [Bindable(true)]
        public object Content
        {
            get
            {
                return this.GetValue(
                    ModalWindowCreator.ContentProperty);
            }
            set
            {
                this.SetValue(
                    ModalWindowCreator.ContentProperty,
                    value);
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
            var window = new Window();
            window.SizeToContent = SizeToContent.WidthAndHeight;
            window.ResizeMode = ResizeMode.NoResize;
            window.ShowInTaskbar = false;
            window.Content = this.Content;

            if (this.Parent != null)
            {
                window.Owner = Window.GetWindow(
                    this.Parent);
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
            var mwc = (ModalWindowCreator)d;

            if (e.OldValue != null)
            {
                mwc.RemoveLogicalChild(
                    e.OldValue);
            }

            if (e.NewValue != null)
            {
                mwc.AddLogicalChild(
                    e.NewValue);
            }
        }

        public static DependencyProperty ContentProperty = DependencyProperty.Register(
            "Content",
            typeof(object),
            typeof(ModalWindowCreator),
            new PropertyMetadata(
                null,
                ModalWindowCreator.ContentChanged));
    }
}
