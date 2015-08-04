// Obtained from: http://stackoverflow.com/questions/8096852/brush-to-brush-animation
// Author: Koopakiller http://stackoverflow.com/users/1623754/koopakiller

namespace WpfMinesweeper.Miscellanious
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Animation;

    public class BrushAnimation : AnimationTimeline
    {
        public static readonly DependencyProperty FromProperty =
            DependencyProperty.Register(
                "From",
                typeof (Brush),
                typeof (BrushAnimation));

        public static readonly DependencyProperty ToProperty =
            DependencyProperty.Register(
                "To",
                typeof (Brush),
                typeof (BrushAnimation));

        public override Type TargetPropertyType
        {
            get
            {
                return typeof (Brush);
            }
        }

        //we must define From and To, AnimationTimeline does not have this properties
        public Brush From
        {
            get
            {
                return (Brush)this.GetValue(
                    BrushAnimation.FromProperty);
            }
            set
            {
                this.SetValue(
                    BrushAnimation.FromProperty,
                    value);
            }
        }

        public Brush To
        {
            get
            {
                return (Brush)this.GetValue(
                    BrushAnimation.ToProperty);
            }
            set
            {
                this.SetValue(
                    BrushAnimation.ToProperty,
                    value);
            }
        }

        public override object GetCurrentValue(object defaultOriginValue,
            object defaultDestinationValue,
            AnimationClock animationClock)
        {
            return this.GetCurrentValue(
                defaultOriginValue as Brush,
                defaultDestinationValue as Brush,
                animationClock);
        }

        public object GetCurrentValue(Brush defaultOriginValue,
            Brush defaultDestinationValue,
            AnimationClock animationClock)
        {
            if (!animationClock.CurrentProgress.HasValue)
            {
                return Brushes.Transparent;
            }

            //use the standard values if From and To are not set 
            //(it is the value of the given property)
            defaultOriginValue = this.From ?? defaultOriginValue;
            defaultDestinationValue = this.To ?? defaultDestinationValue;

            if (animationClock.CurrentProgress.Value == 0)
            {
                return defaultOriginValue;
            }
            if (animationClock.CurrentProgress.Value == 1)
            {
                return defaultDestinationValue;
            }

            return new VisualBrush(new Border()
            {
                Width = 1,
                Height = 1,
                Background = defaultOriginValue,
                Child = new Border()
                {
                    Background = defaultDestinationValue,
                    Opacity = animationClock.CurrentProgress.Value,
                }
            });
        }

        protected override Freezable CreateInstanceCore()
        {
            return new BrushAnimation();
        }
    }
}
