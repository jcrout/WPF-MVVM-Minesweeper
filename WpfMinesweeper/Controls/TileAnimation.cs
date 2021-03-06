namespace WpfMinesweeper.Controls
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    public class TileAnimation : IEnumerable<AnimationFrame>
    {
        private static readonly TileAnimation fadeInRectanglesAnimation;
        private readonly IEnumerable<AnimationFrame> frames;

        static TileAnimation()
        {
            var frameCount = 3;
            double tileWidth = 16;
            double tileHeight = 16;
            var tileWidthHalf = (tileWidth / 2);
            var tileHeightHalf = (tileHeight / 2);
            var tileWidthIncrement = tileWidthHalf / frameCount;
            var tileHeightIncrement = tileHeightHalf / frameCount;

            var images = new ImageSource[frameCount];
            var frames = new AnimationFrame[frameCount];
            for (var i = 0; i < frameCount; i++)
            {
                var rectWidth = tileWidthHalf + (tileWidthIncrement * i);
                var rectHeight = tileHeightHalf + (tileHeightIncrement * i);

                var rectTarget = new RenderTargetBitmap((int)rectWidth, (int)rectHeight, 96, 96, PixelFormats.Pbgra32);
                var rectVisual = new DrawingVisual();

                using (var drawingContext = rectVisual.RenderOpen())
                {
                    drawingContext.DrawRectangle(
                        new SolidColorBrush(Colors.DarkGray),
                        null,
                        new Rect(0, 0, rectWidth, rectHeight));
                }

                rectTarget.Render(rectVisual);
                images[i] = rectTarget;
                frames[i] = new AnimationFrame(images[i], 50);
            }

            TileAnimation.fadeInRectanglesAnimation = new TileAnimation(frames);
        }

        public TileAnimation(IEnumerable<AnimationFrame> frames)
        {
            this.frames = frames;
        }

        public static TileAnimation FadeInRectangles
        {
            get
            {
                return TileAnimation.fadeInRectanglesAnimation;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.frames.GetEnumerator();
        }

        public IEnumerator<AnimationFrame> GetEnumerator()
        {
            return this.frames.GetEnumerator();
        }
    }
}
