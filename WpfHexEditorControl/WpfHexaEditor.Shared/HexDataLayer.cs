//////////////////////////////////////////////
// Apache 2.0  - 2018
// Author : Janus Tida
// Modified by : Derek Tremblay
//////////////////////////////////////////////

using System.Globalization;
using System.Windows;
using System.Windows.Media;
using WpfHexaEditor.Core.Bytes;

namespace WpfHexaEditor
{
    public class HexDataLayer : DataLayerBase
    {
        public override Size CellSize => new Size
        (
            2 * CharSize.Width + CellPadding.Left + CellPadding.Right,
            CellPadding.Top + CellPadding.Bottom + CharSize.Height
        );

        protected override void DrawByte(DrawingContext drawingContext, byte bt, Brush foreground, Point startPoint)
        {
            var chs = ByteConverters.ByteToHexCharArray(bt);

            for (var chIndex = 0; chIndex < 2; chIndex++)
            {
#if NET451
                var text = new FormattedText
                (
                    chs[chIndex].ToString(), CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight, TypeFace, FontSize,
                    foreground
                );

                startPoint.X += CharSize.Width * chIndex;

                drawingContext.DrawText
                (
                    text,
                    startPoint
                );

#endif
#if NET47
                var text = new FormattedText
                (
                    chs[chIndex].ToString(), CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight, TypeFace, FontSize,
                    foreground, PixelPerDip
                );

                startPoint.X += CharSize.Width * chIndex;

                drawingContext.DrawText
                (
                    text,
                    startPoint
                );
#endif
            }
        }
    }
}
