namespace WpfMinesweeper.Controls
{
    using System.ComponentModel;
    using System.Windows.Controls;
    using System.Windows.Markup;

    [TypeConverter(typeof(MessageButtonConverter)), ContentProperty("Button")]
    public class MessageButton
    {
        /// <summary>
        ///     Gets or sets the Button.
        /// </summary>
        public Button Button { get; set; }

        public bool CloseOnClick { get; set; } = true;

        /// <summary>
        ///     Gets or sets the object Result that is returned.
        /// </summary>
        public object Result { get; set; }

        /// <summary>
        ///     <para>Gets or sets the RightToLeftIndex, which determines the order in which buttons appear.</para>
        ///     <para>
        ///         The higher the number, the further to the right this
        ///         <see cref="WpfMinesweeper.Controls.MessageButton.Button" /> will be placed.
        ///     </para>
        ///     <para>
        ///         <para>
        ///             The default value is -1. All values below 0 will
        ///             <see cref="WpfMinesweeper.Controls.MessageButton.Result" />
        ///         </para>
        ///         <para>
        ///             in the button's order being determined before the
        ///             <see cref="WpfMinesweeper.Controls.MessageButton.Button" />
        ///         </para>
        ///         <para>container is rendered.</para>
        ///     </para>
        /// </summary>
        public int RightToLeftIndex { get; set; } = -1;

        public override string ToString()
        {
            var resultText = this.Result != null ? this.Result.ToString() : "null";
            if (this.Button != null && this.Button.Content != null)
            {
                return this.Button.Content + ";" + resultText;
            }

            return resultText;
        }
    }
}
