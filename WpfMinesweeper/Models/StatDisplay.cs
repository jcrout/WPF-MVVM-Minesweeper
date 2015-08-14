namespace WpfMinesweeper.Models
{
    /// <summary>
    ///     Represents all of the data necessary to display a statistic in a
    ///     ListView.
    /// </summary>
    public class StatDisplay
    {
        /// <summary>
        ///     Initializes a new instance of the StatDisplay class.
        /// </summary>
        /// <param name="label">The display label of the stat.</param>
        /// <param name="value">The value of the stat.</param>
        /// <param name="description">Optional. A description of the stat.</param>
        public StatDisplay(string label, string value, string description = "")
        {
            this.Label = label;
            this.Value = value;
            this.Description = description;
        }

        /// <summary>
        ///     Gets the stat description.
        /// </summary>
        public string Description { get; }

        /// <summary>
        ///     Gets the stat display text.
        /// </summary>
        public string Label { get; }

        /// <summary>
        ///     Gets the <see cref="Value" /> for the specific stat.
        /// </summary>
        public string Value { get; }
    }
}
