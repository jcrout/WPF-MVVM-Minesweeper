namespace WpfMinesweeper.Models
{
    using System.Collections.Generic;

    /// <summary>
    ///     Represents a 2-dimensional array with a default property taking in
    ///     two index values.
    /// </summary>
    public interface IEnumerable2D<T> : IEnumerable<T>
    {
        /// <summary>
        ///     The default indexer property of the IEnumerable.
        /// </summary>
        /// <param name="x">The first dimensional index value.</param>
        /// <param name="y">The second dimensional index value.</param>
        /// <returns></returns>
        T this[int x, int y] { get; set; }
    }
}
