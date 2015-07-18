using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfMinesweeper.Views
{
    /// <summary>
    /// Interaction logic for MinesweeperView.xaml
    /// </summary>
    public partial class MinesweeperView : UserControl
    {
        public MinesweeperView()
        {
            Mediator.Instance.Register(ViewModelMessages.SizeChanged, o => this.OnSizeChanged(o));
            InitializeComponent();
        }

        private void OnSizeChanged(object parameter)
        {
            Mediator.Instance.Notify(ViewModelMessages.UpdateContainerSize, parameter);
        }

        private void Board_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.BoardViewBox.Width = this.Board.ActualWidth;
            this.BoardViewBox.Height = this.Board.ActualHeight;
        }

        private void BorderBoard_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!double.IsNaN(this.BorderBoard.ActualWidth))
            {
                this.BoardViewBox.Width = this.BorderBoard.ActualWidth - (this.BorderBoard.BorderThickness.Right * 2);
                this.BoardViewBox.Height = this.BorderBoard.ActualHeight - this.BorderBoard.BorderThickness.Bottom;
            }
        }
    }
}
