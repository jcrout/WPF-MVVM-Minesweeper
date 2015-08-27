namespace WpfMinesweeper.Controls
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Media;
    using JonUtility.WPF;

    /// <summary>
    ///     Represents a bindable Window that can be created in and modified in
    ///     XAML.
    /// </summary>
    /// <remarks>
    ///     This class is useful in XAML coding because it can be bound to the
    ///     content of a Control or to a content container like a Grid. To
    ///     display the window, call the <see cref="Show" /> or
    ///     <see cref="ShowDialog" /> method from a trigger action, such as an
    ///     <see cref="EventTrigger" /> that uses a
    ///     <see cref="Microsoft.Expression.Interactivity.Core.CallMethodAction" />
    ///     .
    /// </remarks>
    [ContentProperty("Content"), DefaultProperty("Content"),
     Localizability(LocalizationCategory.None, Readability = Readability.Unreadable)]
    public class PromptBox : FrameworkElement
    {
        /// <summary>
        ///     Identifies the <see cref="ButtonBorderBackground" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty ButtonBorderBackgroundProperty =
            DependencyProperty.RegisterAttached(
                "ButtonBorderBackground",
                typeof(Brush),
                typeof(PromptBox),
                new PropertyMetadata(null, PromptBox.ButtonBorderBackgroundChanged));

        /// <summary>
        ///     Identifies the <see cref="Content" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(
            "Content",
            typeof(object),
            typeof(PromptBox),
            new PropertyMetadata(null, PromptBox.ContentChanged));

        /// <summary>
        ///     Identifies the <see cref="OKButton" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty OKButtonProperty = DependencyProperty.Register(
            "OKButton",
            typeof(MessageButton),
            typeof(PromptBox),
            new PropertyMetadata(null, PromptBox.OKButtonChanged));

        /// <summary>
        ///     Identifies the <see cref="CancelButton" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty CancelButtonProperty = DependencyProperty.Register(
            "CancelButton",
            typeof(MessageButton),
            typeof(PromptBox),
            new PropertyMetadata(null, PromptBox.CancelButtonChanged));

        /// <summary>
        ///     Identifies the <see cref="OtherButtons" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty OtherButtonsProperty = DependencyProperty.Register(
            "OtherButtons",
            typeof(ObservableCollection<MessageButton>),
            typeof(PromptBox),
            new PropertyMetadata(null, PromptBox.OtherButtonsChanged));

        /// <summary>
        ///     Identifies the <see cref="IsModal" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsModalProperty = DependencyProperty.Register(
            "IsModal",
            typeof(bool),
            typeof(PromptBox));

        /// <summary>
        ///     Identifies the <see cref="ResultCancelCommand" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty ResultCancelCommandProperty =
            DependencyProperty.Register("ResultCancelCommand", typeof(ICommand), typeof(PromptBox));

        /// <summary>
        ///     Identifies the <see cref="ResultCommand" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty ResultCommandProperty = DependencyProperty.Register(
            "ResultCommand",
            typeof(ICommand),
            typeof(PromptBox));

        /// <summary>
        ///     Identifies the <see cref="ResultOKCommand" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty ResultOKCommandProperty =
            DependencyProperty.Register("ResultOKCommand", typeof(ICommand), typeof(PromptBox));

        /// <summary>
        ///     Identifies the <see cref="ResultOtherCommand" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty ResultOtherCommandProperty =
            DependencyProperty.Register("ResultOtherCommand", typeof(ICommand), typeof(PromptBox));

        /// <summary>
        ///     Identifies the <see cref="Title" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title",
            typeof(string),
            typeof(PromptBox),
            new PropertyMetadata(string.Empty, PromptBox.TitleChanged));

        /// <summary>
        ///     Identifies the <see cref="WindowStyle" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty WindowStyleProperty = DependencyProperty.Register(
            "WindowStyle",
            typeof(WindowStyle),
            typeof(PromptBox),
            new PropertyMetadata(WindowStyle.SingleBorderWindow, PromptBox.WindowStyleChanged));

        /// <summary>
        ///     The buttons
        /// </summary>
        private readonly List<MessageButton> buttons = new List<MessageButton>();

        /// <summary>
        ///     True if the <see cref="MessageResult" /> was obtained from a button click on the <see cref="MessagePanel" />;
        ///     otherwise, if the <see cref="MessageResult" /> was obtained from clicking the X button on the <see cref="Window" />
        ///     , false.
        /// </summary>
        private bool obtainedResultBeforeClosing;

        /// <summary>
        ///     The <see cref="MessagePanel" /> containing the specified <see cref="MessageButton" /> instances.
        /// </summary>
        private MessagePanel panel;

        /// <summary>
        ///     The <see cref="MessageResult" /> to return from the static <see cref="ShowPrompt(PromptBox)" /> method.
        /// </summary>
        private MessageResult result;

        /// <summary>
        ///     The <see cref="Window" /> displayed to the user.
        /// </summary>
        private Window window;

        /// <summary>
        ///     Initializes a new instance of the <see cref="PromptBox" /> class.
        /// </summary>
        public PromptBox()
        {
            this.OtherButtons = new ObservableCollection<MessageButton>();
            this.OKButton = this.GetDefaultOKButton();
            this.CancelButton = this.GetDefaultCancelButton();
            this.OtherButtons.CollectionChanged += this.OtherButtons_CollectionChanged;
        }

        /// <summary>
        ///     Gets or sets the background brush used for the panel containing the <see cref="MessageButton.Button" /> instances.
        /// </summary>
        /// <value>The background bush used for the <see cref="MessagePanel" />.</value>
        [Bindable(false)]
        public Brush ButtonBorderBackground
        {
            get
            {
                return (Brush)this.GetValue(PromptBox.ButtonBorderBackgroundProperty);
            }
            set
            {
                this.SetValue(PromptBox.ButtonBorderBackgroundProperty, value);
            }
        }

        /// <summary>
        ///     Gets or sets the <see cref="MessageButton" /> to use as the Cancel button.
        /// </summary>
        /// <value>
        ///     The Cancel <see cref="MessageButton" />. This <see cref="MessageButton" /> indicates the default negative user
        ///     response. The default value is a <see cref="MessageButton" /> containing a <see cref="Button" /> with the text
        ///     "Cancel" and a result of <see cref="MessageResult.Cancel" />.
        /// </value>
        [Bindable(true)]
        public MessageButton CancelButton
        {
            get
            {
                return (MessageButton)this.GetValue(PromptBox.CancelButtonProperty);
            }
            set
            {
                this.SetValue(PromptBox.CancelButtonProperty, value);
            }
        }

        /// <summary>
        ///     Gets or sets the content.
        /// </summary>
        /// <value>The content.</value>
        [Bindable(true)]
        public object Content
        {
            get
            {
                return this.GetValue(PromptBox.ContentProperty);
            }
            set
            {
                this.SetValue(PromptBox.ContentProperty, value);
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the prompt <see cref="Window" /> should be displayed as a modal
        ///     <see cref="Window" />.
        /// </summary>
        /// <value><c>true</c> if the <see cref="Window" /> shown is modal; otherwise, <c>false</c>.</value>
        [Bindable(true)]
        public bool IsModal
        {
            get
            {
                return (bool)this.GetValue(PromptBox.IsModalProperty);
            }
            set
            {
                this.SetValue(PromptBox.IsModalProperty, value);
            }
        }

        /// <summary>
        ///     Gets or sets the <see cref="MessageButton" /> to use as the OK button.
        /// </summary>
        /// <value>
        ///     The OK <see cref="MessageButton" />. This <see cref="MessageButton" /> indicates the default positive user
        ///     response. The default value is a <see cref="MessageButton" /> containing a <see cref="Button" /> with the text "OK"
        ///     and a result of <see cref="MessageResult.OK" />
        /// </value>
        [Bindable(true)]
        public MessageButton OKButton
        {
            get
            {
                return (MessageButton)this.GetValue(PromptBox.OKButtonProperty);
            }
            set
            {
                this.SetValue(PromptBox.OKButtonProperty, value);
            }
        }

        /// <summary>
        ///     Gets or sets the collection of <see cref="MessageButton" /> instances to use in place of or in addition to the
        ///     <see cref="OKButton" /> and/or the <see cref="CancelButton" />.
        /// </summary>
        /// <value>The other collection of other <see cref="MessageButton" /> instances.</value>
        [Bindable(true)]
        public ObservableCollection<MessageButton> OtherButtons
        {
            get
            {
                return (ObservableCollection<MessageButton>)this.GetValue(PromptBox.OtherButtonsProperty);
            }
            set
            {
                this.SetValue(PromptBox.OtherButtonsProperty, value);
            }
        }

        /// <summary>
        ///     Gets or sets the <see cref="ICommand" /> to invoke when the <see cref="CancelButton" /> or <see cref="Window" />
        ///     close button is pressed
        /// </summary>
        /// <value>
        ///     A command to invoke when the <see cref="CancelButton" /> or <see cref="Window" /> close button is pressed. The
        ///     default value is null.
        /// </value>
        [Bindable(true)]
        public ICommand ResultCancelCommand
        {
            get
            {
                return (ICommand)this.GetValue(PromptBox.ResultCancelCommandProperty);
            }
            set
            {
                this.SetValue(PromptBox.ResultCancelCommandProperty, value);
            }
        }

        /// <summary>
        ///     Gets or sets the <see cref="ICommand" /> to invoke when any <see cref="MessageButton" /> or the
        ///     <see cref="Window" /> close button is pressed.
        /// </summary>
        /// <value>A command to invoke when any <see cref="MessageButton" /> is pressed. The default value is null.</value>
        [Bindable(true)]
        public ICommand ResultCommand
        {
            get
            {
                return (ICommand)this.GetValue(PromptBox.ResultCommandProperty);
            }
            set
            {
                this.SetValue(PromptBox.ResultCommandProperty, value);
            }
        }

        /// <summary>
        ///     Gets or sets the <see cref="ICommand" /> to invoke when the <see cref="OKButton" /> is pressed.
        /// </summary>
        /// <value>A command to invoke when the <see cref="OKButton" /> is pressed. The default value is null.</value>
        [Bindable(true)]
        public ICommand ResultOKCommand
        {
            get
            {
                return (ICommand)this.GetValue(PromptBox.ResultOKCommandProperty);
            }
            set
            {
                this.SetValue(PromptBox.ResultOKCommandProperty, value);
            }
        }

        /// <summary>
        ///     Gets or sets the <see cref="ICommand" /> to invoke when a button is pressed that either has
        ///     <see cref="MessageResult.Other" /> as the result or does not have <see cref="MessageResult.OK" /> or
        ///     <see cref="MessageResult.Cancel" /> as the result.
        /// </summary>
        /// <value>
        ///     A command to invoke when an <see cref="OtherButtons" /> <see cref="MessageButton" /> is pressed. The default
        ///     value is null.
        /// </value>
        [Bindable(true)]
        public ICommand ResultOtherCommand
        {
            get
            {
                return (ICommand)this.GetValue(PromptBox.ResultOtherCommandProperty);
            }
            set
            {
                this.SetValue(PromptBox.ResultOtherCommandProperty, value);
            }
        }

        /// <summary>
        ///     Gets or sets the prompt <see cref="Window.Title" /> property.
        /// </summary>
        /// <value>
        ///     The title string to display in the caption bar at the top of the prompt window. The default value is
        ///     <see cref="System.String.Empty" />
        /// </value>
        [Bindable(true)]
        public string Title
        {
            get
            {
                return (string)this.GetValue(PromptBox.TitleProperty);
            }
            set
            {
                this.SetValue(PromptBox.TitleProperty, value);
            }
        }

        /// <summary>
        ///     Gets or sets the prompt <see cref="Window.WindowStyle" /> property.
        /// </summary>
        /// <value>The window style of the prompt.</value>
        [Bindable(true)]
        public WindowStyle WindowStyle
        {
            get
            {
                return (WindowStyle)this.GetValue(PromptBox.WindowStyleProperty);
            }
            set
            {
                this.SetValue(PromptBox.WindowStyleProperty, value);
            }
        }

        /// <summary>
        ///     Overrides the <see cref="VisualChildrenCount" /> method to always return zero.
        /// </summary>
        /// <returns>
        ///     The number of visual child elements for this element. This value is always zero.
        /// </returns>
        protected override int VisualChildrenCount => 0;

        /// <summary>
        ///     Displays the <see cref="PromptBox" /> window to the user and returns the <see cref="MessageResult" /> from the
        ///     button clicked.
        /// </summary>
        /// <param name="messageText">The prompt message text.</param>
        /// <param name="windowTitle">The prompt window title.</param>
        /// <returns>MessageResult.</returns>
        public static MessageResult ShowPrompt(string messageText, string windowTitle)
        {
            var prompt = new PromptBox();

            if (!string.IsNullOrEmpty(messageText))
            {
                var grid = new Grid {Margin = new Thickness(15)};
                var text = new TextBlock {Text = messageText};
                grid.Children.Add(text);
                prompt.Content = grid;
            }

            prompt.Title = windowTitle ?? string.Empty;

            return PromptBox.ShowPrompt(prompt);
        }

        /// <summary>
        ///     Displays the <see cref="PromptBox" /> window to the user and returns the <see cref="MessageResult" /> from the
        ///     button clicked.
        /// </summary>
        /// <param name="prompt">The prompt.</param>
        /// <returns>MessageResult.</returns>
        public static MessageResult ShowPrompt(PromptBox prompt)
        {
            prompt.ShowDialog();
            return prompt.result;
        }

        /// <summary>
        ///     Displays the <see cref="PromptBox" /> window to the user.
        /// </summary>
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

        /// <summary>
        ///     Displays the <see cref="PromptBox" /> window to the user as a modal window.
        /// </summary>
        public void ShowDialog()
        {
            this.window = this.GetNewWindow();
            this.window.ShowDialog();
        }

        /// <summary>
        ///     Overrides the <see cref="GetVisualChild(int)" /> method to return null.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>Visual.</returns>
        protected override Visual GetVisualChild(int index)
        {
            return null;
        }

        private static void ButtonBorderBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pb = (PromptBox)d;
            if (pb.panel != null)
            {
                pb.panel.ButtonPanelBorder.Background = (Brush)e.NewValue;
            }
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

        private static void TitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pb = (PromptBox)d;

            if (pb.window != null)
            {
                pb.window.Title = e.NewValue.ToString();
            }
        }

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

        /// <summary>
        ///     Clears the <see cref="panel" /> by removing event handlers and setting the <see cref="panel" /> field to null.
        /// </summary>
        private void ClearPanel()
        {
            if (this.panel != null)
            {
                this.panel.ButtonClicked -= this.panel_ButtonClicked;
                this.panel = null;
            }
        }

        /// <summary>
        ///     Returns a new <see cref="ObservableCollection{MessageButton}" /> from the <see cref="buttons" /> field.
        /// </summary>
        /// <returns>ObservableCollection&lt;MessageButton&gt;.</returns>
        private ObservableCollection<MessageButton> GetButtons()
        {
            var buttonList = new ObservableCollection<MessageButton>(this.buttons);
            return buttonList;
        }

        /// <summary>
        ///     Gets the default Cancel <see cref="MessageButton" />.
        /// </summary>
        /// <returns>MessageButton.</returns>
        private MessageButton GetDefaultCancelButton()
        {
            var button = new Button {Width = 80, Height = 22, Content = "Cancel", IsCancel = true};

            var messageButton = new MessageButton {Button = button};
            messageButton.RightToLeftIndex = 1;
            messageButton.Result = MessageResult.Cancel;

            return messageButton;
        }

        /// <summary>
        ///     Gets the default OK <see cref="MessageButton" />.
        /// </summary>
        /// <returns>MessageButton.</returns>
        private MessageButton GetDefaultOKButton()
        {
            var button = new Button {Width = 80, Height = 22, Content = "OK", IsDefault = true};

            var messageButton = new MessageButton
            {
                Button = button,
                RightToLeftIndex = 0,
                Result = MessageResult.OK
            };

            return messageButton;
        }

        /// <summary>
        ///     Creates and returns a new Window to use as the prompt window.
        /// </summary>
        /// <returns>Window.</returns>
        private Window GetNewWindow()
        {
            var newWindow = new Window();
            object contentContext = null;
            newWindow.WindowStyle = this.WindowStyle;
            newWindow.SizeToContent = SizeToContent.WidthAndHeight;
            newWindow.ResizeMode = ResizeMode.NoResize;
            newWindow.ShowInTaskbar = false;
            newWindow.Title = this.Title ?? string.Empty;

            this.panel = new MessagePanel();
            if (this.Content != null)
            {
                var content = this.Content;
                contentContext = (content is FrameworkElement) ? ((FrameworkElement)content).DataContext : null;
                this.RemoveLogicalChild(content);

                this.panel.InnerContent = content;
            }

            this.panel.Buttons = this.GetButtons();
            this.panel.ButtonClicked += this.panel_ButtonClicked;

            if (this.ButtonBorderBackground != null)
            {
                this.panel.ButtonPanelBorder.Background = this.ButtonBorderBackground;
            }

            newWindow.Content = this.panel;
            newWindow.DataContext = contentContext;
            newWindow.Closing += this.window_Closing;

            if (this.WindowStyle == WindowStyle.None)
            {
                newWindow.AllowsTransparency = true;
            }

            if (this.Parent != null)
            {
                newWindow.Owner = Window.GetWindow(this.Parent);
                newWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }
            else
            {
                newWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }

            return newWindow;
        }

        /// <summary>
        ///     Invokes the commands.
        /// </summary>
        /// <param name="buttonClicked">The <see cref="MessageButton" /> containing the button that was clicked.</param>
        /// <param name="resultOfButtonClick">The <see cref="MessageResult" /> associated with the button clicked.</param>
        private void InvokeCommands(MessageButton buttonClicked, MessageResult resultOfButtonClick)
        {
            var buttonResult = (buttonClicked != null) ? buttonClicked.Result ?? resultOfButtonClick : resultOfButtonClick;

            switch (resultOfButtonClick)
            {
                case MessageResult.OK:
                    this.ResultOKCommand.ExecuteIfAbleTo(buttonResult);
                    break;
                case MessageResult.Cancel:
                    this.ResultCancelCommand.ExecuteIfAbleTo(buttonResult);
                    break;
                case MessageResult.Other:
                    this.ResultOtherCommand.ExecuteIfAbleTo(buttonResult);
                    break;
            }

            this.ResultCommand.ExecuteIfAbleTo(buttonResult);
            this.result = resultOfButtonClick;
        }

        /// <summary>
        ///     Handles the CollectionChanged event of the <see cref="OtherButtons" /> property.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs" /> instance containing the event data.</param>
        private void OtherButtons_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (MessageButton button in e.NewItems)
                {
                    if (button != null)
                    {
                        this.buttons.Add(button);
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (MessageButton button in e.OldItems)
                {
                    if (button != null)
                    {
                        this.buttons.Remove(button);
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                var newItemEnumerator = e.NewItems.GetEnumerator();
                foreach (MessageButton button in e.OldItems)
                {
                    newItemEnumerator.MoveNext();
                    var newButton = (MessageButton)newItemEnumerator.Current;
                    if (button != null)
                    {
                        var index = this.buttons.IndexOf(button);
                        this.buttons[index] = newButton;
                    }
                }
            }
            else
            {
                this.buttons.Clear();
            }
        }

        /// <summary>
        ///     Handles a buttonClicked click within the MessagePanel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MessageResultEventArgs" /> instance containing the event data.</param>
        private void panel_ButtonClicked(object sender, MessageResultEventArgs e)
        {
            this.InvokeCommands(e.Button, e.Result);

            if (!e.Button.CloseOnClick)
            {
                return;
            }

            this.ClearPanel();
            if (this.window != null)
            {
                this.obtainedResultBeforeClosing = true;
                this.window.Close();
                this.window = null;
                this.obtainedResultBeforeClosing = false;
            }
        }

        /// <summary>
        ///     Handles the Closing event of the window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="CancelEventArgs" /> instance containing the event data.</param>
        private void window_Closing(object sender, CancelEventArgs e)
        {
            this.window.Closing -= this.window_Closing;

            if (!this.obtainedResultBeforeClosing)
            {
                this.InvokeCommands(this.CancelButton, MessageResult.Cancel);
                this.ClearPanel();
                this.window = null;
            }
        }
    }
}
