namespace WpfMinesweeper.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading.Tasks;

    [Serializable, DataContract(Name = "GameState")]
    public enum GameState
    {
        Unfinished,
        Victory,
        GameOver
    }
}