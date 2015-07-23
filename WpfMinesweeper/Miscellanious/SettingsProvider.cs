namespace WpfMinesweeper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Media;
    using WpfMinesweeper.Properties;

    public interface ISettingsProvider
    {
        int LastBoardWidth { get; set; }
        int LastBoardHeight { get; set; }
        int LastBoardMineCount { get; set; }

        double LastWindowMinWidth { get; set; }
        double LastWindowMinHeight { get; set; }

        Color TileColor { get; set; }
        Brush TileBrush { get; set; }
        void Save();
    }

    public class SettingsProvider : ISettingsProvider
    {
        private static ISettingsProvider instance = new SettingsProvider();

        private Settings userSettings = new Settings();

        public static ISettingsProvider Instance
        {
            get
            {
                return instance;
            }
        }

        public int LastBoardWidth
        {
            get
            {
                return userSettings.LastBoardWidth;
            }
            set
            {
                this.userSettings.LastBoardWidth = value;
            }
        }

        public int LastBoardHeight
        {
            get
            {
                return userSettings.LastBoardHeight;
            }
            set
            {
                this.userSettings.LastBoardHeight = value;
            }
        }

        public int LastBoardMineCount
        {
            get
            {
                return userSettings.LastBoardMineCount;
            }
            set
            {
                this.userSettings.LastBoardMineCount = value;
            }
        }

        public double LastWindowMinWidth
        {
            get
            {
                return userSettings.LastWindowMinWidth;
            }
            set
            {
                this.userSettings.LastWindowMinWidth = value;
            }
        }

        public double LastWindowMinHeight
        {
            get
            {
                return userSettings.LastWindowMinHeight;
            }
            set
            {
                this.userSettings.LastWindowMinHeight = value;
            }
        }

        public Color TileColor
        {
            get
            {
                return userSettings.TileColor;
            }
            set
            {
                userSettings.TileColor = value;
            }
        }

        // valid property name/type, but implements a temporary method; later, use serialization and store it myself.
        public Brush TileBrush
        {
            get
            {
                if (userSettings.TileBrushSolid != null)
                {
                    return userSettings.TileBrushSolid;
                }
                else
                {
                    return userSettings.TileBrushGradient; 
                }
            }
            set
            {
                var brushType = value.GetType();
                if (brushType.IsAssignableFrom(typeof(SolidColorBrush)))
                {
                    userSettings.TileBrushSolid = value as SolidColorBrush;
                }
                else if (brushType.IsAssignableFrom(typeof(LinearGradientBrush)))
                {
                    userSettings.TileBrushGradient = value as LinearGradientBrush;
                }
            }
        }

        public void Save()
        {
            userSettings.Save();
        }

        private SettingsProvider()
        {
        }
    }
}
