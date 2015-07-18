namespace WpfMinesweeper.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;

    public class MenuViewModel : ViewModelBase
    {
        private ICommand exitCommand;
        private ICommand boardSizeCommand;

        public MenuViewModel()
        {
            this.exitCommand = new Command(o => this.OnExit((IClosable)o));
            this.boardSizeCommand = new Command(o => this.OnBoardSizeSelected(o));
        }

        public ICommand ExitCommand
        {
            get
            {
                return this.exitCommand;
            }
            set
            {
                if (this.exitCommand != value)
                {
                    this.exitCommand = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public ICommand BoardSizeCommand
        {
            get
            {
                return this.boardSizeCommand;
            }
            set
            {
                if (this.boardSizeCommand != value)
                {
                    this.boardSizeCommand = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private void OnBoardSizeSelected(object paramter)
        {
            Mediator.Instance.Notify(ViewModelMessages.CreateNewBoard, paramter);
        }

        private void OnExit(IClosable paramter)
        {
            paramter.Close();
        }
    }
}
