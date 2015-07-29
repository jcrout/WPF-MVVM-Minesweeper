namespace WpfMinesweeper.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Windows.Shapes;
    using JonUtility;

    /// <summary>
    /// Interaction logic for MessagePanel.xaml
    /// </summary>
    public partial class MessagePanel : UserControl
    {
        public event EventHandler<MessageResultArgs> ButtonClicked;

        private static Thickness nullButtonMargins = new Thickness(0.125, 0.521, 0.127, 0.527);

        static MessagePanel()
        {
        }

        public static DependencyProperty InnerContentProperty = DependencyProperty.Register(
            "InnerContent",
            typeof(object),
            typeof(MessagePanel),
            new PropertyMetadata(
                null,
                InnerContentChanged));

        public static DependencyProperty ButtonsProperty = DependencyProperty.Register(
            "Buttons",
            typeof(ObservableCollection<MessageButton>),
            typeof(MessagePanel),
            new PropertyMetadata(
                new ObservableCollection<MessageButton>(),
                ButtonsChanged));

        public static DependencyProperty UseCustomButtonMarginsProperty = DependencyProperty.Register(
            "UseCustomButtonMargins",
            typeof(bool),
            typeof(MessagePanel),
            new PropertyMetadata(
                false));

        void button_Click(object sender, RoutedEventArgs e)
        {
            var buttonClicked = this.ButtonClicked;
            if (buttonClicked != null)
            {
                var button = sender as Button;
                var messageButton = this.Buttons.FirstOrDefault(mb => mb.Button == button);

                buttonClicked(this, new MessageResultArgs(messageButton));
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

        public MessagePanel()
        {
            InitializeComponent();
            this.ButtonPanelBorder.Background = new SolidColorBrush(Color.FromArgb(255, 240, 240, 240));
        }

        [Bindable(true)]
        public object InnerContent
        {
            get
            {
                return this.GetValue(InnerContentProperty);
            }
            set
            {
                this.SetValue(InnerContentProperty, value);
            }
        }

        [Bindable(true)]
        public bool UseCustomButtonMargins
        {
            get
            {
                return (bool)this.GetValue(UseCustomButtonMarginsProperty);
            }
            set
            {
                this.SetValue(UseCustomButtonMarginsProperty, value);
            }
        }

        [Bindable(true)]
        public ObservableCollection<MessageButton> Buttons
        {
            get
            {
                return (ObservableCollection<MessageButton>)this.GetValue(ButtonsProperty);
            }
            set
            {
                this.SetValue(ButtonsProperty, value);
            }
        }

        private void UpdateButtons(object paramter)
        {
            if (!(paramter is ObservableCollection<MessageButton>))
            {
                return;
            }

            var buttonList = (ObservableCollection<MessageButton>)paramter;
            if (buttonList.Count == 0)
            {
                return;
            }            

            bool useCustomMargins = this.UseCustomButtonMargins; 
            var buttons = buttonList.ToList();
            buttons.Sort((b1, b2) => b1.RightToLeftIndex > b2.RightToLeftIndex ? 1 : b1.RightToLeftIndex < b2.RightToLeftIndex ? -1 : 0);

            int highestIndex = buttons[buttons.Count - 1].RightToLeftIndex;
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

                button.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                button.Click -= button_Click;
                button.Click += button_Click;

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

    public class MessageResultArgs : EventArgs
    {
        public MessageButton Button { get; private set; }

        public MessageResult Result { get; private set; }

        public MessageResultArgs(MessageButton button)
        {
            this.Button = button;

            if (button.Result is MessageResult)
            {
                this.Result = (MessageResult)button.Result;
            }
            else
            {
                this.Result = MessageResult.Other;
            }
        }
    }

    public enum MessageResult
    {
        OK,
        Cancel,
        Other
    }

    public class MessageButtonConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(Button))
            {
                return true;
            }
            else if (sourceType == typeof(string))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(Button))
            {
                return true;
            }
            else if (destinationType == typeof(string))
            {
                return true;
            }

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            var valueType = value.GetType();
            if (valueType == typeof(Button))
            {
                var button = (Button)value;
                return new MessageButton() { Button = button };
            }
            else if (valueType == typeof(string))
            {
                var textFragments = value.ToString().Split(new char[] { ';' });
                var button = new Button { Content = textFragments[0] };
                var mbutton = new MessageButton() { Button = button };

                button.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                button.Width = Math.Max(80, button.DesiredSize.Width);
                button.Height = Math.Max(22, button.DesiredSize.Height);

                if (textFragments.Length > 1)
                {
                    int rightToLeftIndex = 0;
                    if (int.TryParse(textFragments[1], out rightToLeftIndex))
                    {
                        mbutton.RightToLeftIndex = rightToLeftIndex;
                    }
                }

                if (textFragments.Length > 2)
                {
                    var resultFragment = textFragments[2].ToUpper();
                    if (string.Equals(resultFragment, "OK"))
                    {
                        mbutton.Result = MessageResult.OK;
                    }
                    else if (string.Equals(resultFragment, "CANCEL"))
                    {
                        mbutton.Result = MessageResult.Cancel;
                    }
                    else if (string.Equals(resultFragment, "OTHER"))
                    {
                        mbutton.Result = MessageResult.Other;
                    }
                    else if (string.Equals(resultFragment, "NULL"))
                    {
                        mbutton.Result = null;
                    }
                    else if (!string.Equals(resultFragment, string.Empty))
                    {
                        mbutton.Result = textFragments[2];
                    }
                }

                return mbutton;
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return false;
        }
    }

    [TypeConverter(typeof(MessageButtonConverter)), ContentProperty("Button")]
    public class MessageButton
    {
        /// <summary>
        /// Backing field for the Button property.
        /// </summary>
        private Button button;

        /// <summary>
        /// Backing field for the Result property.
        /// </summary>
        private object result;

        /// <summary>
        /// Backing field for the RightToLeftIndex property.
        /// </summary>
        private int rightToLeftIndex = -1;

        public MessageButton()
        {
        }

        /// <summary>
        /// Gets or sets Button.
        /// </summary>
        public Button Button
        {
            get
            {
                return this.button;
            }
            set
            {
                this.button = value;
            }
        }

        /// <summary>
        /// Gets or sets Result.
        /// </summary>
        public object Result
        {
            get
            {
                return this.result;
            }
            set
            {
                this.result = value;
            }
        }

        /// <summary>
        /// <para>Gets or sets the RightToLeftIndex, which determines the order in which buttons appear.</para>
        /// <para>The higher the number, the further to the right this button will be placed.</para>
        /// <para>The default value is -1. All values below 0 will result in the button's order being determined before the button container is rendered.</para>
        /// </summary>
        public int RightToLeftIndex
        {
            get
            {
                return this.rightToLeftIndex;
            }
            set
            {
                this.rightToLeftIndex = value;
            }
        }

        public override string ToString()
        {
            var resultText = this.result != null ? this.result.ToString() : "null";
            if (this.button != null && this.button.Content != null)
            {
                return this.button.Content.ToString() + ";" + resultText;
            }

            return resultText;
        }
    }
}
