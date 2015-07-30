namespace WpfMinesweeper.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using WpfMinesweeper.Models;

    public class StatisticsViewModel : ViewModelBase
    {
        private ObservableCollection<BoardSize> boardSizeItems;
        private BoardSize selectedBoardSize;

        public StatisticsViewModel()
        {
            this.boardSizeItems = new ObservableCollection<BoardSize>();
            this.boardSizeItems.Add(BoardSize.Beginner);
            this.boardSizeItems.Add(BoardSize.Intermediate);
            this.boardSizeItems.Add(BoardSize.Expert);

            this.SelectedBoardSize = this.boardSizeItems[0];           
        }

        public ObservableCollection<BoardSize> BoardSizeItems
        {
            get
            {
                return this.boardSizeItems;
            }
            set
            {
                if (this.boardSizeItems != value)
                {
                    this.boardSizeItems = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public BoardSize SelectedBoardSize
        {
            get
            {
                return this.selectedBoardSize;
            }
            set
            {
                if (this.selectedBoardSize != value)
                {
                    this.selectedBoardSize = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private void OnBoardSizeSelectionChanged()
        {

        }
    }

}
