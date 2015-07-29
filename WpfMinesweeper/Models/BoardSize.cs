using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfMinesweeper.Models
{
    public struct BoardSize
    {
        private static BoardSize beginner = new BoardSize(9, 9, 10);
        private static BoardSize intermediate = new BoardSize(16, 16, 40);
        private static BoardSize expert = new BoardSize(30, 16, 99);
        private int height;
        private int mineCount;
        private int width;

        public BoardSize(int width, int height, int mineCount)
        {
            this.width = width;
            this.height = height;
            this.mineCount = mineCount;
        }

        /// <summary>
        /// Gets Height.
        /// </summary>
        public int Height
        {
            get
            {
                return this.height;
            }
        }

        /// <summary>
        /// Gets MineCount.
        /// </summary>
        public int MineCount
        {
            get
            {
                return this.mineCount;
            }
        }

        /// <summary>
        /// Gets Width.
        /// </summary>
        public int Width
        {
            get
            {
                return this.width;
            }
        }

        public override string ToString()
        {
            if (this == beginner)
            {
                return "Beginner";
            }
            else if (this == intermediate)
            {
                return "Intermediate";
            }
            else if (this == expert)
            {
                return "Expert";
            }
            else
            {
                return this.Description;
            }
        }

        public string Description
        {
            get
            {
                return string.Format(@"{0}x{1}, {2} mines", this.width, this.height, this.mineCount);
            }
        }

        public static BoardSize Beginner
        {
            get
            {
                return beginner;
            }
        }

        public static BoardSize Intermediate
        {
            get
            {
                return intermediate;
            }
        }

        public static BoardSize Expert
        {
            get
            {
                return expert;
            }
        }

        public static bool operator ==(BoardSize bs1, BoardSize bs2)
        {
            return (bs1.width == bs2.width &&
                    bs1.height == bs2.height &&
                    bs1.mineCount == bs2.mineCount);
        }

        public static bool operator !=(BoardSize bs1, BoardSize bs2)
        {
            return (bs1.width != bs2.width ||
                    bs1.height != bs2.height ||
                    bs1.mineCount != bs2.mineCount);
        }
    }
}
