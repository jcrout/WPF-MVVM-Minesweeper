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
        private int lastTilesWidth = 55;
        public MinesweeperView()
        {
            Mediator.Instance.Register(ViewModelMessages.TileBoardInitialized, o => this.OnSizeChanged(o));
            InitializeComponent();
        }

        private void OnSizeChanged(object parameter)
        {
            if ((this.BoardViewBox.Width == this.BoardView.Board.Width) && (this.BoardViewBox.Height == this.BoardView.Board.Height))
            {
                return;
            }

            this.BoardViewBox.Width = this.BoardView.Board.Width;
            this.BoardViewBox.Height = this.BoardView.Board.Height;
            Mediator.Instance.Notify(ViewModelMessages.UpdateContainerSize, parameter);
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
