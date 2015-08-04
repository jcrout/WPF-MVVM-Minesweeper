namespace WpfMinesweeper.Models
{
    /// <summary>
    /// Represents all of the data necessary to display a statistic in a ListView.
    /// </summary>
    public class StatDisplay
    {
        /// <summary>
        /// Backing field for the Description property.
        /// </summary>
        private string description;

        /// <summary>
        /// Backing field for the Label property.
        /// </summary>
        private string label;

        /// <summary>
        /// Backing field for the Value property.
        /// </summary>
        private string value;

        /// <summary>
        /// Initializes a new instance of the StatDisplay class.
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
        /// Gets the stat description.
        /// </summary>
        public string Description
        {
            get
            {
                return this.description;
            }
            private set
            {
                this.description = value;
            }
        }

        /// <summary>
        /// Gets the stat display text.
        /// </summary>
        public string Label
        {
            get
            {
                return this.label;
            }
            private set
            {
                this.label = value;
            }
        }

        /// <summary>
        /// Gets the value for the specific stat.
        /// </summary>
        public string Value
        {
            get
            {
                return this.value;
            }
            private set
            {
                this.value = value;
            }
        }
    }
}