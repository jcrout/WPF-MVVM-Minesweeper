namespace WpfMinesweeper.Controls
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Markup;
    using System.Windows.Media;

    [ContentProperty("Content"), DefaultProperty("Content"), Localizability(LocalizationCategory.None, Readability = Readability.Unreadable)]
    public class ModalWindowCreator : FrameworkElement
    {
        public static DependencyProperty ContentProperty = DependencyProperty.Register(
            "Content",
            typeof (object),
            typeof (ModalWindowCreator),
            new PropertyMetadata(
                null,
                ModalWindowCreator.ContentChanged));

        private static void ContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var mwc = (ModalWindowCreator) d;

            if (e.OldValue != null)
            {
                mwc.RemoveLogicalChild(e.OldValue);
            }

            if (e.NewValue != null)
            {
                mwc.AddLogicalChild(e.NewValue);
            }
        }

        protected override int VisualChildrenCount
        {
            get
            {
                return 0;
            }
        }

        protected override Visual GetVisualChild(int index)
        {
            return null;
        }

        [Bindable(true)]
        public object Content
        {
            get
            {
                return this.GetValue(ModalWindowCreator.ContentProperty);
            }
            set
            {
                this.SetValue(ModalWindowCreator.ContentProperty,
                    value);
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
                window.Owner = Window.GetWindow(this.Parent);
                window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }
            else
            {
                window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }

            window.ShowDialog();
        }
    }
}