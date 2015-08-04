namespace WpfMinesweeper.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Controls;

    public class ViewBoxAutosize : Viewbox
    {
        private static readonly DependencyProperty KeepExpandedRatioProperty = DependencyProperty.Register(
            "KeepExpandedRatio",
            typeof(bool),
            typeof(ViewBoxAutosize),
            new PropertyMetadata(
                false));

        private FrameworkElement child;
        private FrameworkElement parent;
        private bool updatedSize;

        public override UIElement Child
        {
            get
            {
                return base.Child;
            }
            set
            {
                base.Child = value;

                if (this.child != null)
                {
                    this.child.SizeChanged -= this.child_SizeChanged;
                    this.child = null;
                }

                if (typeof(FrameworkElement).IsAssignableFrom(
                    value.GetType()))
                {
                    this.child = value as FrameworkElement;
                    this.child.SizeChanged += this.child_SizeChanged;
                }
            }
        }

        public bool KeepExpandedRatio
        {
            get
            {
                return (bool)this.GetValue(
                    ViewBoxAutosize.KeepExpandedRatioProperty);
            }
            set
            {
                this.SetValue(
                    ViewBoxAutosize.KeepExpandedRatioProperty,
                    value);
            }
        }

        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(
                oldParent);

            if (this.parent != null)
            {
                this.parent.SizeChanged -= this.parent_SizeChanged;
                this.parent = null;
            }

            if (this.Parent == null)
            {
                return;
            }

            var currentParentType = this.Parent.GetType();
            if (typeof(FrameworkElement).IsAssignableFrom(
                currentParentType))
            {
                this.parent = (FrameworkElement)this.Parent;
                this.parent.SizeChanged += this.parent_SizeChanged;
            }
        }

        private void child_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.updatedSize = true;
            if (this.KeepExpandedRatio && !double.IsNaN(
                this.Width) && !double.IsNaN(
                    this.Height))
            {
                var widthRatio = Math.Max(
                    1d,
                    this.Width / e.PreviousSize.Width);
                var heightRatio = Math.Max(
                    1d,
                    this.Height / e.PreviousSize.Height);
                this.Width = e.NewSize.Width * widthRatio;
                this.Height = e.NewSize.Height * heightRatio;
            }
            else
            {
                this.Width = e.NewSize.Width;
                this.Height = e.NewSize.Height;
            }
        }

        private void parent_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.updatedSize)
            {
                this.updatedSize = false;
                return;
            }

            if (this.Width == e.NewSize.Width && this.Height == e.NewSize.Height)
            {
                return;
            }

            this.Width = Math.Max(
                0,
                this.Width + e.NewSize.Width - e.PreviousSize.Width);
            this.Height = Math.Max(
                0,
                this.Height + e.NewSize.Height - e.PreviousSize.Height);
        }
    }
}
