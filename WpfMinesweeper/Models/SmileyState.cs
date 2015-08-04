namespace WpfMinesweeper.Models
{
    /// <summary>
    ///     Specifies the state of the Smiley animation.
    /// </summary>
    public enum SmileyState
    {
        /// <summary>
        ///     The normal state used during inaction.
        /// </summary>
        Default,

        /// <summary>
        ///     The smiley state while the user input device is pressed down.
        /// </summary>
        TapDown,

        /// <summary>
        ///     The smiley state in use after the game ends in a victory.
        /// </summary>
        Victory,

        /// <summary>
        ///     The smiley state in use after the game ends in a game over.
        /// </summary>
        GameOver
    }
}
