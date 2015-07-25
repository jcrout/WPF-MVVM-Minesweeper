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
    using System.Windows.Input;
    using System.Windows.Interop;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using JonUtility;

    [ContentProperty("Content"), DefaultProperty("Content"), Localizability(LocalizationCategory.None, Readability = Readability.Unreadable)]
    public class PromptBox : FrameworkElement
    {
        private Window window;
        private MessagePanel panel;
        private List<MessageButton> buttons = new List<MessageButton>();

        public static DependencyProperty ButtonBorderBackgroundProperty = DependencyProperty.RegisterAttached(
            "ButtonBorderBackground",
            typeof(Brush),
            typeof(PromptBox),
            new PropertyMetadata(
                null,
                ButtonBorderBackgroundChanged));

        public static DependencyProperty ContentProperty = DependencyProperty.Register(
            "Content",
            typeof(object),
            typeof(PromptBox),
            new PropertyMetadata(
                null,
                ContentChanged));

        public static DependencyProperty OKButtonProperty = DependencyProperty.Register(
            "OKButton",
            typeof(MessageButton),
            typeof(PromptBox),
            new PropertyMetadata(
                null,
                OKButtonChanged));

        public static DependencyProperty CancelButtonProperty = DependencyProperty.Register(
            "CancelButton",
            typeof(MessageButton),
            typeof(PromptBox),
            new PropertyMetadata(
                null,
                CancelButtonChanged));

        public static DependencyProperty OtherButtonsProperty = DependencyProperty.Register(
            "OtherButtons",
            typeof(ObservableCollection<MessageButton>),
            typeof(PromptBox),
            new PropertyMetadata(
                new ObservableCollection<MessageButton>(),
                OtherButtonsChanged));

        public static DependencyProperty ResultCancelCommandProperty = DependencyProperty.Register(
            "ResultCancelCommand",
            typeof(ICommand),
            typeof(PromptBox));

        public static DependencyProperty ResultOKCommandProperty = DependencyProperty.Register(
            "ResultOKCommand",
            typeof(ICommand),
            typeof(PromptBox));

        public static DependencyProperty ResultOtherCommandProperty = DependencyProperty.Register(
            "ResultOtherCommand",
            typeof(ICommand),
            typeof(PromptBox));

        public static DependencyProperty ResultCommandProperty = DependencyProperty.Register(
            "ResultCommand",
            typeof(ICommand),
            typeof(PromptBox));

        public static DependencyProperty IsModalProperty = DependencyProperty.Register(
            "IsModal",
            typeof(bool),
            typeof(PromptBox));

        public static DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title",
            typeof(string),
            typeof(PromptBox),
            new PropertyMetadata(
                string.Empty,
                TitleChanged));

        public static DependencyProperty WindowStyleProperty = DependencyProperty.Register(
            "WindowStyle",
            typeof(WindowStyle),
            typeof(PromptBox),
            new PropertyMetadata(
                WindowStyle.SingleBorderWindow,
                WindowStyleChanged));

        private static void WindowStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pb = (PromptBox)d;

            var newStyle = (WindowStyle)e.NewValue;
            if (pb.window != null && pb.window.WindowStyle != WindowStyle.None && newStyle != WindowStyle.None)
            {
                pb.window.WindowStyle = newStyle;
                pb.window.SizeToContent = SizeToContent.Manual;
                pb.window.SizeToContent = SizeToContent.WidthAndHeight;
                pb.window.Icon = null;
            }        
        }

        private static void OKButtonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pb = (PromptBox)d;

            if (e.OldValue != null)
            {
                pb.buttons.Remove((MessageButton)e.OldValue);
            }

            if (e.NewValue == null)
            {
                return;
            }

            var mb = (MessageButton)e.NewValue;
            mb.Result = MessageResult.OK;
            mb.Button.IsCancel = false;
            mb.Button.IsDefault = true;
            pb.buttons.Add(mb);
        }

        private static void CancelButtonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pb = (PromptBox)d;            

            if (e.OldValue != null)
            {
                pb.buttons.Remove((MessageButton)e.OldValue);
            }

            if (e.NewValue == null)
            {
                return;
            }

            var mb = (MessageButton)e.NewValue;
            mb.Result = MessageResult.Cancel;
            mb.Button.IsCancel = true;
            mb.Button.IsDefault = false;
            pb.buttons.Add(mb);
        }

        private static void OtherButtonsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pb = (PromptBox)d;

            if (e.OldValue != null)
            {
                var oldButtons = (ObservableCollection<MessageButton>)e.OldValue;
                foreach (var button in oldButtons)
                {
                    pb.buttons.Remove(button);
                }
            }

            if (e.NewValue != null)
            {
                var newButtons = (ObservableCollection<MessageButton>)e.NewValue;
                foreach (var button in newButtons)
                {
                    pb.buttons.Add(button);
                }
            }
        }

        private static void ContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pb = (PromptBox)d;

            if (e.OldValue != null)
            {
                pb.RemoveLogicalChild(e.OldValue);
            }

            if (e.NewValue != null)
            {
                pb.AddLogicalChild(e.NewValue);
            }
        }

        private static void TitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pb = (PromptBox)d;

            if (pb.window != null)
            {
                pb.window.Title = e.NewValue.ToString();
            }
        }

        private static void ButtonBorderBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pb = (PromptBox)d;
            if (pb.panel != null)
            {
                pb.panel.ButtonPanelBorder.Background = (Brush)e.NewValue;
            }
        }

        [Bindable(true)]
        public object Content
        {
            get
            {
                return this.GetValue(ContentProperty);
            }
            set
            {
                this.SetValue(ContentProperty, value);
            }
        }

        [Bindable(true)]
        public string Title
        {
            get
            {
                return (string)this.GetValue(TitleProperty);
            }
            set
            {
                this.SetValue(TitleProperty, value);
            }
        }

        [Bindable(true)]
        public bool IsModal
        {
            get
            {
                return (bool)this.GetValue(IsModalProperty);
            }
            set
            {
                this.SetValue(IsModalProperty, value);
            }
        }

        [Bindable(true)]
        public WindowStyle WindowStyle
        {
            get
            {
                return (WindowStyle)this.GetValue(WindowStyleProperty);
            }
            set
            {
                this.SetValue(WindowStyleProperty, value);
            }
        }

        [Bindable(true)]
        public MessageButton OKButton
        {
            get
            {
                return (MessageButton)this.GetValue(OKButtonProperty);
            }
            set
            {
                this.SetValue(OKButtonProperty, value);
            }
        }

        [Bindable(true)]
        public MessageButton CancelButton
        {
            get
            {
                return (MessageButton)this.GetValue(CancelButtonProperty);
            }
            set
            {
                this.SetValue(CancelButtonProperty, value);
            }
        }

        [Bindable(false)]
        public Brush ButtonBorderBackground
        {
            get
            {
                return (Brush)this.GetValue(ButtonBorderBackgroundProperty);
            }
            set
            {
                this.SetValue(ButtonBorderBackgroundProperty, value);
            }
        }

        [Bindable(true)]
        public ObservableCollection<MessageButton> OtherButtons
        {
            get
            {
                return (ObservableCollection<MessageButton>)this.GetValue(OtherButtonsProperty);
            }
            set
            {
                this.SetValue(OtherButtonsProperty, value);
            }
        }

        [Bindable(true)]
        public ICommand ResultCancelCommand
        {
            get
            {
                return (ICommand)this.GetValue(ResultCancelCommandProperty);
            }
            set
            {
                this.SetValue(ResultCancelCommandProperty, value);
            }
        }

        [Bindable(true)]
        public ICommand ResultOKCommand
        {
            get
            {
                return (ICommand)this.GetValue(ResultOKCommandProperty);
            }
            set
            {
                this.SetValue(ResultOKCommandProperty, value);
            }
        }

        [Bindable(true)]
        public ICommand ResultOtherCommand
        {
            get
            {
                return (ICommand)this.GetValue(ResultOtherCommandProperty);
            }
            set
            {
                this.SetValue(ResultOtherCommandProperty, value);
            }
        }

        [Bindable(true)]
        public ICommand ResultCommand
        {
            get
            {
                return (ICommand)this.GetValue(ResultCommandProperty);
            }
            set
            {
                this.SetValue(ResultCommandProperty, value);
            }
        }

        public PromptBox()
        {
            this.OKButton = this.GetDefaultOKButton();
            this.CancelButton = this.GetDefaultCancelButton();
            this.OtherButtons.CollectionChanged += OtherButtons_CollectionChanged;
        }

        void OtherButtons_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                foreach (MessageButton button in e.NewItems)
                {
                    if (button != null)
                    {
                        this.buttons.Add(button);
                    }
                }
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                foreach (MessageButton button in e.OldItems)
                {
                    if (button != null)
                    {
                        this.buttons.Remove(button);
                    }
                }
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
            {
                var newItemEnumerator = e.NewItems.GetEnumerator();
                foreach (MessageButton button in e.OldItems)
                {
                    newItemEnumerator.MoveNext();
                    var newButton = (MessageButton)newItemEnumerator.Current;
                    if (button != null)
                    {
                        int index = this.buttons.IndexOf(button);
                        this.buttons[index] = newButton;
                    }
                }
            }
            else
            {
                this.buttons.Clear();
            }
        }

        public void ShowDialog()
        {
            this.window = this.GetNewWindow();
            this.window.ShowDialog();
        }

        public void Show()
        {
            if (this.window != null)
            {
                return;
            }

            this.window = this.GetNewWindow();

            if (this.IsModal)
            {
                this.window.ShowDialog();
            }
            else
            {
                this.window.Show();
            }
        }

        private Window GetNewWindow()
        { 
            var window = new Window();
            object contentContext = null;
            window.WindowStyle = this.WindowStyle;
            window.SizeToContent = SizeToContent.WidthAndHeight;
            window.ResizeMode = ResizeMode.NoResize;
            window.ShowInTaskbar = false;
            window.Title = this.Title ?? string.Empty;

            this.panel = new MessagePanel();
            if (this.Content != null)
            {
                var content = this.Content;
                contentContext = (content as FrameworkElement != null) ? (content as FrameworkElement).DataContext : null;
                this.RemoveLogicalChild(content);

                this.panel.InnerContent = content;
            }
        
            this.panel.Buttons = this.GetButtons();
            this.panel.ButtonClicked += panel_ButtonClicked;
            
            if (this.ButtonBorderBackground != null)
            {
                this.panel.ButtonPanelBorder.Background = this.ButtonBorderBackground;
            }
 
            window.Content = panel;
            window.DataContext = contentContext;
            window.Closing += window_Closing;

            if (this.WindowStyle == System.Windows.WindowStyle.None)
            {
                window.AllowsTransparency = true;
            }

            if (this.Parent != null)
            {
                window.Owner = Window.GetWindow(this.Parent);
                window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }
            else
            {
                window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }

            return window;
        }

        private bool obtainedResultBeforeClosing = false;

        private void InvokeCommands(MessageButton button, MessageResult result)
        {
            switch (result)
            {
                case MessageResult.OK:
                    this.ResultOKCommand.ExecuteIfAbleTo(button.Result ?? result);
                    break;
                case MessageResult.Cancel:
                    this.ResultCancelCommand.ExecuteIfAbleTo(button.Result ?? result);
                    break;
                case MessageResult.Other:
                    this.ResultOtherCommand.ExecuteIfAbleTo(button.Result ?? result);
                    break;
                default:
                    break;
            }

            this.ResultCommand.ExecuteIfAbleTo(button.Result);
        }

        private void window_Closing(object sender, CancelEventArgs e)
        {
            this.window.Closing -= window_Closing;

            if (!this.obtainedResultBeforeClosing)
            {
                this.InvokeCommands(this.CancelButton, MessageResult.Cancel);
                this.ClearPanel();
                this.window = null;
            }
        }

        private void ClearPanel()
        {
            if (this.panel != null)
            {
                this.panel.ButtonClicked -= panel_ButtonClicked;
                this.panel = null;
            }
        }
        
        private void panel_ButtonClicked(object sender, MessageResultArgs e)
        {
            this.InvokeCommands(e.Button, e.Result);
            this.ClearPanel();

            if (this.window != null)
            {
                this.obtainedResultBeforeClosing = true;
                this.window.Close();
                this.window = null;
                this.obtainedResultBeforeClosing = false;
            }
        }

        private MessageButton GetDefaultCancelButton()
        {
            var button = new Button
            {
                Width = 80,
                Height = 22,
                Content = "Cancel",
                IsCancel = true
            };

            var messageButton = new MessageButton();
            messageButton.Button = button;
            messageButton.RightToLeftIndex = 1;
            messageButton.Result = MessageResult.Cancel;

            return messageButton;
        }

        private MessageButton GetDefaultOKButton()
        {
            var button = new Button
            {
                Width = 80,
                Height = 22,
                Content = "OK",
                IsDefault = true
            };

            var messageButton = new MessageButton();
            messageButton.Button = button;
            messageButton.RightToLeftIndex = 0;
            messageButton.Result = MessageResult.OK;

            return messageButton;
        }

        private ObservableCollection<MessageButton> GetButtons()
        {
            var buttonList = new ObservableCollection<MessageButton>(this.buttons);
            return buttonList;
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

    }
}