namespace WpfMinesweeper.Models
{
    /// <summary>
    ///     Specifies the extra data associated with a Tile, including whether
    ///     or not the <see cref="Tile" /> is flagged or contains a question
    ///     mark.
    /// </summary>
    public enum ExtraTileData
    {
        None,
        Flag,
        QuestionMark
    }
}
