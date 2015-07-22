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
        private IMinesweeper minesweeper;

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
    }
}
