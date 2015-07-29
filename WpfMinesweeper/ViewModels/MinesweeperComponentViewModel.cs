using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfMinesweeper.Models;

namespace WpfMinesweeper.ViewModels
{
    public abstract class MinesweeperComponentViewModel : ViewModelBase
    {
        private static IStatisticsModule globalStatistics = StatisticsModule.Create();

        private IStatisticsModule gameStatistics;
        private IMinesweeper minesweeper;
        
        public static IStatisticsModule GlobalStatistics
        {
            get
            {
                return globalStatistics;
            }
            set
            {
                if (globalStatistics != value)
                {
                    globalStatistics = value; 
                }
            }
        }

        public IStatisticsModule GameStatistics
        {
            get
            {
                return this.gameStatistics;
            }
            set
            {
                if (this.gameStatistics != value)
                {
                    this.gameStatistics = value;
                    this.OnGameStatisticsChanged();
                    this.OnPropertyChanged();
                }
            }
        }

        public IMinesweeper Minesweeper
        {
            get
            {
                return this.minesweeper;
            }
            set
            {
                if (this.minesweeper != value)
                {
                    this.minesweeper = value;
                    this.OnMinesweeperChanged();
                    this.OnPropertyChanged();
                }
            }
        }

        protected virtual void OnMinesweeperChanged()
        {

        }

        protected virtual void OnGameStatisticsChanged()
        {

        }
    }
}
