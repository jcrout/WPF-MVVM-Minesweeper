namespace WpfMinesweeper.Controls
{
    using System;

    public class MessageResultEventArgs : EventArgs
    {
        public MessageResultEventArgs(MessageButton button)
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

        public MessageButton Button { get; private set; }

        public MessageResult Result { get; private set; }
    }
}
