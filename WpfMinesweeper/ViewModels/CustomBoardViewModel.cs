namespace WpfMinesweeper.ViewModels
{
    using System.ComponentModel;
    using System.Windows.Input;
    using Models;

    public class CustomBoardViewModel : ViewModelBase, IDataErrorInfo
    {
        private readonly IMinesweeperBoardValidator validator;
        private int height;
        private int mines;
        private ICommand saveCustomBoardCommand;
        private int width;

        public CustomBoardViewModel(int width = 9, int height = 9, int mines = 10)
        {
            this.width = width;
            this.height = height;
            this.mines = mines;
            this.validator = MinesweeperBoardValidator.Create();
            this.saveCustomBoardCommand = new Command(this.OnSaveCustomBoard, () => this.IsValid);
        }

        public string Error
        {
            get
            {
                return null;
            }
        }

        public int Height
        {
            get
            {
                return this.height;
            }
            set
            {
                if (this.height != value)
                {
                    this.height = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public bool IsValid
        {
            get
            {
                var isValid = this.validator.ValidateBoard(this.width, this.height, this.mines) == null;
                return isValid;
            }
        }

        public int Mines
        {
            get
            {
                return this.mines;
            }
            set
            {
                if (this.mines != value)
                {
                    this.mines = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public ICommand SaveCustomBoardCommand
        {
            get
            {
                return this.saveCustomBoardCommand;
            }
            set
            {
                if (this.saveCustomBoardCommand != value)
                {
                    this.saveCustomBoardCommand = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public int Width
        {
            get
            {
                return this.width;
            }
            set
            {
                if (this.width != value)
                {
                    this.width = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public string this[string columnName]
        {
            get
            {
                if (columnName == "Width")
                {
                    return this.validator.ValidateWidth(this.width);
                }

                if (columnName == "Height")
                {
                    return this.validator.ValidateHeight(this.height);
                }

                if (columnName == "Mines")
                {
                    return this.validator.ValidateMineCount(this.mines, this.width, this.height);
                }

                return null;
            }
        }

        private void OnSaveCustomBoard()
        {
            var customBoard = this.width.ToString() + ',' + this.height + ',' + this.mines;
            this.Mediator.Notify(ViewModelMessages.CreateNewBoard, customBoard);
        }
    }
}
