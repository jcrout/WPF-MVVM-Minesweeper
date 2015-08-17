namespace WpfMinesweeper.Controls
{
    using System.Collections.Generic;
    using JonUtility;

    public class AnimatedTilesCollection
    {
        private AnimatedTilesCollection(List<Point<int>> tilesToUpdate, TileAnimation frames = null)
        {
            this.Tiles = tilesToUpdate;
            this.Frames = frames;
        }

        public TileAnimation Frames { get; }

        public List<Point<int>> Tiles { get; }

        public static AnimatedTilesCollection Create(List<Point<int>> tilesToUpdate, TileAnimation frames = null)
        {
            return new AnimatedTilesCollection(tilesToUpdate, frames);
        }
    }
}
