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
    public class StringDataLayer : DataLayerBase
    {
        public override Size CellSize =>
            new Size(CellPadding.Right + CellPadding.Left + CharSize.Width,
                CharSize.Height + CellPadding.Top + CellPadding.Bottom);

        protected override void DrawByte(DrawingContext drawingContext, byte bt, Brush foreground, Point startPoint)
        {
            var ch = ByteConverters.ByteToChar(bt);
#if NET451
            var text = new FormattedText(ch.ToString(), CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight, TypeFace, FontSize,
                foreground);
            drawingContext.DrawText(text, startPoint);
#endif

#if NET47
            var text = new FormattedText
            (
                ch.ToString(), CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight, TypeFace, FontSize,
                foreground, PixelPerDip
            );

            drawingContext.DrawText(text, startPoint);
#endif

        }
    }
}
