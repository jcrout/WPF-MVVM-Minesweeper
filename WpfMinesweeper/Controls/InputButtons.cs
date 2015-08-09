namespace WpfMinesweeper.Controls
{
    using System;

    /// <summary>
    ///     Defines flags that specify buttons on an input device.
    /// </summary>
    [Flags]
    public enum InputButtons
    {
        /// <summary>
        ///     No input buttons.
        /// </summary>
        None = 0,

        /// <summary>
        ///     The left mouse button.
        /// </summary>
        Left = 1,

        /// <summary>
        ///     The right mouse button.
        /// </summary>
        Right = 2,

        /// <summary>
        ///     The middle mouse button (wheel).
        /// </summary>
        Middle = 4,

        /// <summary>
        ///     The first extended mouse button.
        /// </summary>
        XButton1 = 8,

        /// <summary>
        ///     The second extended mouse button.
        /// </summary>
        XButton2 = 16
    }
}
