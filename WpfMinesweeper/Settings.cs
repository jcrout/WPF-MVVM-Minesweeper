using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("WpfMInesweeperUnitTests")]

namespace WpfMinesweeper.Properties
{
    using System.Configuration;
    using Models;

    internal sealed partial class Settings
    {
        [UserScopedSetting, DefaultSettingValue("9,9,10")]
        public BoardSize LastBoardSize
        {
            get
            {
                return ((BoardSize)(this["LastBoardSize"]));
            }
            set
            {
                this["LastBoardSize"] = value;
            }
        }
    }
}
