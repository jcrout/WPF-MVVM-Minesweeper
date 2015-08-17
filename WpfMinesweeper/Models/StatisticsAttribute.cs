namespace WpfMinesweeper.Models
{
    using System;

    /// <summary>
    ///     Specifies meta-data for a <see cref="Statistic" /> enumeration
    ///     field.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class StatisticsAttribute : Attribute
    {
        private string description = string.Empty;
        private string displayLabel = string.Empty;

        public StatisticsAttribute(Type statType)
        {
            if (statType == null)
            {
                throw new ArgumentNullException(nameof(statType));
            }

            this.Type = statType;
        }

        /// <summary>
        ///     Gets or sets a description of the stat.
        /// </summary>
        /// <value>The description string. This value defaults to an empty string on <see langword="null" />.</value>
        public string Description
        {
            get
            {
                return this.description;
            }
            set
            {
                this.description = value ?? string.Empty;
            }
        }

        /// <summary>
        ///     Gets or sets the display label of the stat.
        /// </summary>
        /// <value>The display label string. This value defaults to an empty string on <see langword="null" />.</value>
        public string DisplayLabel
        {
            get
            {
                return this.displayLabel;
            }
            set
            {
                this.displayLabel = value ?? string.Empty;
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="Statistic" /> is recorded during individual games.
        /// </summary>
        /// <value><c>true</c> if this instance is single game statistic; otherwise, <c>false</c>.</value>
        public bool IsAggregate { get; set; }

        public Type Type { get; }
    }
}
