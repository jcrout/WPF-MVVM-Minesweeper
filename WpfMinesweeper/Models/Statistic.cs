namespace WpfMinesweeper.Models
{
    using System;
    using System.Runtime.Serialization;

    [Serializable, DataContract]
    public enum Statistic
    {
        [Statistics(typeof(BoardSize), Description = "The width, height, and mine count of the board.", DisplayLabel = "Board Size")]
        BoardSize = 0,

        [Statistics(typeof(int), Description = "The number of seconds that have elapsed since the game began.", DisplayLabel = "Time Elapsed")]
        TimeElapsed = 1,

        [Statistics(typeof(int), Description = "The total number of unflagged mines on the board. A negative number indicates that more flags have been placed than there are mines.", DisplayLabel = "Mines Remaining")]
        MinesRemaining = 2,

        [Statistics(typeof(GameResult), Description = "The final state of the game, which is either Incomplete, Victory, or Gameover.", DisplayLabel = "Result")]
        GameState = 3,

        [Statistics(typeof(int), Description = "Total number of flages placed at the end of the game.", DisplayLabel = "Flags Placed")]
        FlagsPlaced = 4,

        [Statistics(typeof(int), Description = "Total number of question marks placed on the board, including question marks later removed.", DisplayLabel = "Question Marks Placed")]
        QuestionMarksPlacedTotal = 5,

        [Statistics(typeof(int), Description = "Total number of moves resulting in tiles being revealed.", DisplayLabel = "Moves")]
        Moves = 6,

        [Statistics(typeof(DateTime), Description = "The time that the game began.", DisplayLabel = "Game Start Time")]
        GameStartTime = 7,

        [Statistics(typeof(DateTime), Description = "The time that the game ended.", DisplayLabel = "Game End Time")]
        GameEndTime = 8,

        [Statistics(typeof(int), Description = "The total number of matches.", DisplayLabel = "Count", IsAggregate = true)]
        MatchCount = 9
    }
}
