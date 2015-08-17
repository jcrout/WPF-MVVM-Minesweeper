namespace WpfMinesweeper.Controls
{
    using System.Windows.Media;

    public class AnimationFrame
    {
        public AnimationFrame(ImageSource image, long interval)
        {
            this.Interval = interval;
            this.Image = image;
        }

        public ImageSource Image { get; set; }

        public long Interval { get; set; } // milliseconds
    }
}
