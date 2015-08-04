namespace WpfMinesweeper.Models
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    ///     Specifies the end result of the game.
    /// </summary>
    [Serializable, DataContract(Name = "GameResult")]
    public enum GameResult
    {
        /// <summary>
        ///     The game ended prior to victory or game over.
        /// </summary>
        Unfinished = 0,

        /// <summary>
        ///     The game ended in a win.
        /// </summary>
        Victory = 1,

        /// <summary>
        ///     The game ended in a loss..
        /// </summary>
        GameOver = 2
    }
}
