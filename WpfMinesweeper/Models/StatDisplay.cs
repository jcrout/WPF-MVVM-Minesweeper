namespace WpfMinesweeper.Models
{
    /// <summary>
    ///     Represents all of the data necessary to display a statistic in a
    ///     ListView.
    /// </summary>
    public class StatDisplay
    {
        /// <summary>
        ///     Backing field for the <see cref="Description" /> property.
        /// </summary>
        private readonly string description;

        /// <summary>
        ///     Backing field for the <see cref="Label" /> property.
        /// </summary>
        private readonly string label;

        /// <summary>
        ///     Backing field for the <see cref="Value" /> property.
        /// </summary>
        private readonly string value;

        /// <summary>
        ///     Initializes a new instance of the StatDisplay class.
        /// </summary>
        /// <param name="label">The display label of the stat.</param>
        /// <param name="value">The value of the stat.</param>
        /// <param name="description">Optional. A description of the stat.</param>
        public StatDisplay(string label, string value, string description = "")
        {
            this.label = label;
            this.value = value;
            this.description = description;
        }

        /// <summary>
        ///     Gets the stat description.
        /// </summary>
        public string Description
        {
            get
            {
                return this.description;
            }
        }

        /// <summary>
        ///     Gets the stat display text.
        /// </summary>
        public string Label
        {
            get
            {
                return this.label;
            }
        }

        /// <summary>
        ///     Gets the <see cref="value" /> for the specific stat.
        /// </summary>
        public string Value
        {
            get
            {
                return this.value;
            }
        }
    }
}
