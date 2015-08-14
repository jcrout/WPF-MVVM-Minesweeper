namespace WpfMinesweeper.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Controls;

    public class ViewBoxAutosize : Viewbox
    {
        private static readonly DependencyProperty KeepExpandedRatioProperty =
            DependencyProperty.Register(
                "KeepExpandedRatio",
                typeof(bool),
                typeof(ViewBoxAutosize),
                new PropertyMetadata(false));

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

                if (value is FrameworkElement)
                {
                    this.child = (FrameworkElement)value;
                    this.child.SizeChanged += this.child_SizeChanged;
                }
            }
        }

        public bool KeepExpandedRatio
        {
            get
            {
                return (bool)this.GetValue(ViewBoxAutosize.KeepExpandedRatioProperty);
            }
            set
            {
                this.SetValue(ViewBoxAutosize.KeepExpandedRatioProperty, value);
            }
        }

        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);

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
            if (typeof(FrameworkElement).IsAssignableFrom(currentParentType))
            {
                this.parent = (FrameworkElement)this.Parent;
                this.parent.SizeChanged += this.parent_SizeChanged;
            }
        }

        private Size oldParentSize = new Size(123456, 123456);

        private async void child_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var newWidth = this.Width != e.NewSize.Width;
            var newHeight = this.Height != e.NewSize.Height;
            if (this.parent != null && (double.IsNaN(this.parent.Width) || double.IsNaN(this.parent.Height)) && (newWidth || newHeight))
            {
                this.updatedSize = true;
            }

            if (this.KeepExpandedRatio && !double.IsNaN(this.Width) && !double.IsNaN(this.Height))
            {
                var widthRatio = Math.Max(1d, this.Width / e.PreviousSize.Width);
                var heightRatio = Math.Max(1d, this.Height / e.PreviousSize.Height);
                this.Width = e.NewSize.Width * widthRatio;
                this.Height = e.NewSize.Height * heightRatio;
            }
            else
            {
                this.oldParentSize = new Size(this.parent.ActualWidth, this.parent.ActualHeight);
                if (newWidth)
                {
                    this.Width = e.NewSize.Width;
                }

                if (newHeight)
                {
                    this.Height = e.NewSize.Height;
                }

                if (this.updatedSize)
                {
                    // allow time for the parent to update size in accordance to this control changing
                    this.parent.SizeChanged -= this.parent_SizeChanged;
                    await System.Threading.Tasks.Task.Delay(5);
                    this.parent.SizeChanged += this.parent_SizeChanged;

                    this.updatedSize = false;
                }
            }
        }

        private void parent_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.Width = Math.Max(0, this.Width + e.NewSize.Width - e.PreviousSize.Width);
            this.Height = Math.Max(0, this.Height + e.NewSize.Height - e.PreviousSize.Height);
        }
    }
}
