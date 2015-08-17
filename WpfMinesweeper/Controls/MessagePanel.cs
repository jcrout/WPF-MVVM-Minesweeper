namespace WpfMinesweeper.Controls
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using JonUtility.WPF;

    /// <summary>
    ///     Interaction logic for MessagePanel.xaml
    /// </summary>
    public partial class MessagePanel : UserControl
    {
        private static Thickness nullButtonMargins = new Thickness(0.125, 0.521, 0.127, 0.527);

        public static readonly DependencyProperty InnerContentProperty = DependencyProperty.Register(
            "InnerContent",
            typeof(object),
            typeof(MessagePanel),
            new PropertyMetadata(null, MessagePanel.InnerContentChanged));

        public static readonly DependencyProperty ButtonsProperty = DependencyProperty.Register(
            "Buttons",
            typeof(ObservableCollection<MessageButton>),
            typeof(MessagePanel),
            new PropertyMetadata(new ObservableCollection<MessageButton>(), MessagePanel.ButtonsChanged));

        public static readonly DependencyProperty UseCustomButtonMarginsProperty =
            DependencyProperty.Register(
                "UseCustomButtonMargins",
                typeof(bool),
                typeof(MessagePanel),
                new PropertyMetadata(false));

        static MessagePanel()
        {
        }

        public MessagePanel()
        {
            this.InitializeComponent();
            this.ButtonPanelBorder.Background = new SolidColorBrush(Color.FromArgb(255, 240, 240, 240));
        }

        public event EventHandler<MessageResultEventArgs> ButtonClicked;

        [Bindable(true)]
        public ObservableCollection<MessageButton> Buttons
        {
            get
            {
                return (ObservableCollection<MessageButton>)this.GetValue(MessagePanel.ButtonsProperty);
            }
            set
            {
                this.SetValue(MessagePanel.ButtonsProperty, value);
            }
        }

        [Bindable(true)]
        public object InnerContent
        {
            get
            {
                return this.GetValue(MessagePanel.InnerContentProperty);
            }
            set
            {
                this.SetValue(MessagePanel.InnerContentProperty, value);
            }
        }

        [Bindable(true)]
        public bool UseCustomButtonMargins
        {
            get
            {
                return (bool)this.GetValue(MessagePanel.UseCustomButtonMarginsProperty);
            }
            set
            {
                this.SetValue(MessagePanel.UseCustomButtonMarginsProperty, value);
            }
        }

        private static void ButtonsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var panel = (MessagePanel)d;
            panel.UpdateButtons(e.NewValue);
        }

        private static void InnerContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var panel = (MessagePanel)d;
            panel.UpdateInnerContent(e.NewValue);
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            var buttonClicked = this.ButtonClicked;
            if (buttonClicked != null)
            {
                var button = sender as Button;
                var messageButton = this.Buttons.FirstOrDefault(mb => mb.Button == button);

                buttonClicked(this, new MessageResultEventArgs(messageButton));
            }
        }

        private void UpdateButtons(object parameter)
        {
            if (!(parameter is ObservableCollection<MessageButton>))
            {
                return;
            }

            var buttonList = (ObservableCollection<MessageButton>)parameter;
            if (buttonList.Count == 0)
            {
                return;
            }

            var useCustomMargins = this.UseCustomButtonMargins;
            var buttons = buttonList.ToList();
            buttons.Sort(
                (b1, b2) =>
                    b1.RightToLeftIndex > b2.RightToLeftIndex ? 1 : b1.RightToLeftIndex < b2.RightToLeftIndex ? -1 : 0);

            var highestIndex = buttons[buttons.Count - 1].RightToLeftIndex;
            foreach (var mb in buttons)
            {
                if (mb.RightToLeftIndex < 0)
                {
                    highestIndex++;
                    mb.RightToLeftIndex = highestIndex;
                }
                else
                {
                    break;
                }
            }

            foreach (var mb in buttons)
            {
                if (mb == null || mb.Button == null)
                {
                    return;
                }

                var button = mb.Button;

                if (button.Parent != null && button.Parent != this)
                {
                    var frameworkParent = button.Parent as FrameworkElement;
                    if (frameworkParent != null && !frameworkParent.ForceRemoveChild(button))
                    {
                        continue;
                    }
                }

                if (!useCustomMargins)
                {
                    button.Margin = new Thickness(10, 10, 10, 10);
                }

                button.HorizontalAlignment = HorizontalAlignment.Right;
                button.Click -= this.button_Click;
                button.Click += this.button_Click;

                if (button.Parent == null)
                {
                    this.ButtonPanel.Children.Add(button);
                }
            }
        }

        private void UpdateInnerContent(object innerContent)
        {
            var element = innerContent as UIElement;
            if (element == null)
            {
                return;
            }

            var parent = LogicalTreeHelper.GetParent(element);
            if (parent != null)
            {
                var frameworkParent = parent as FrameworkElement;
                if (frameworkParent != null)
                {
                    if (!frameworkParent.ForceRemoveChild(innerContent))
                    {
                        return;
                    }
                }
                else if (parent == this)
                {
                    this.RemoveLogicalChild(innerContent);
                }
                else
                {
                    return;
                }
            }

            this.ContentGrid.Children.Add(element);
        }
    }
}
