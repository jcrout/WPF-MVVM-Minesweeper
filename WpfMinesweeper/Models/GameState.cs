namespace WpfMinesweeper.Models
{
    using System;
    using System.Runtime.Serialization;

    [Serializable, DataContract(Name = "GameState")]
    public enum GameState
    {
        Unfinished,
        Victory,
        GameOver
    }
}
