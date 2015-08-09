namespace WpfMinesweeper.Controls
{
    /// <summary>
    ///     Provides data for user input events centered on a Tile.
    /// </summary>
    public class TileTapEventArgs
    {
        public TileTapEventArgs(TileEventArgs tileEventArgs, InputButtons button, bool doubleClicked, bool pressedDown,
            InputButtons allButtons = InputButtons.None)
        {
            this.TileEventArgs = tileEventArgs;
            this.Button = button;
            this.DoubleClicked = doubleClicked;
            this.PressedDown = pressedDown;
            this.AllButtonStates = allButtons;
        }

        /// <summary>
        ///     Gets the state of all mouse buttons. Inclusion means that the
        ///     <see cref="WpfMinesweeper.Controls.TileTapEventArgs.Button" /> is currently pressed down.
        /// </summary>
        public InputButtons AllButtonStates { get; private set; }

        /// <summary>
        ///     Gets the input tap button.
        /// </summary>
        public InputButtons Button { get; private set; }

        /// <summary>
        ///     Gets a value indicating whether input was double-tapped.
        /// </summary>
        public bool DoubleClicked { get; private set; }

        /// <summary>
        ///     Gets a value indicating whether input was pressed down or released.
        /// </summary>
        public bool PressedDown { get; private set; }

        /// <summary>
        ///     Gets the associated <see cref="TileTapEventArgs" /> .
        /// </summary>
        public TileEventArgs TileEventArgs { get; private set; }
    }
}
