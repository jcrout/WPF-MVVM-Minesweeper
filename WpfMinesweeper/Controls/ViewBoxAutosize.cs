namespace WpfMinesweeper.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    public class ViewBoxAutosize : Viewbox
    {
        private FrameworkElement parent;
        private FrameworkElement child;
        private bool updatedSize = false;
 
        public ViewBoxAutosize()
        {
        }

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
                    this.child.SizeChanged -= child_SizeChanged;
                    this.child = null;
                }

                if (typeof(FrameworkElement).IsAssignableFrom(value.GetType()))
                {
                    this.child = value as FrameworkElement;
                    this.child.SizeChanged += child_SizeChanged;
                }
            }
        }

        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);

            if (this.parent != null)
            {
                this.parent.SizeChanged -= parent_SizeChanged;
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
                this.parent.SizeChanged += parent_SizeChanged;
            }
        }
        
        private void child_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.updatedSize = true;
            this.Width = e.NewSize.Width;
            this.Height = e.NewSize.Height;  
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

            this.Width = Math.Max(0, this.Width + e.NewSize.Width - e.PreviousSize.Width);
            this.Height = Math.Max(0, this.Height + e.NewSize.Height - e.PreviousSize.Height);
        }
    }
}