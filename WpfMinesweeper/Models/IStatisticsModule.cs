namespace WpfMinesweeper.Models
{
    using System.Collections.Generic;

    public interface IStatisticsModule : IEnumerable<KeyValuePair<Statistic, object>>
    {
        /// <summary>
        ///     Gets or sets the value of the <paramref name="statistic" /> for
        ///     this module.
        /// </summary>
        /// <param name="statistic">
        ///     The statistic to use as an indexer to retrieve an associated
        ///     value.
        /// </param>
        /// <returns>Returns the value associated with the statistic.</returns>
        object this[Statistic statistic] { get; set; }
    }
}
