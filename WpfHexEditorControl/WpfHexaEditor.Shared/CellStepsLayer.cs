//////////////////////////////////////////////
// Apache 2.0  - 2018
// Author : Janus Tida
// Modified by : Derek Tremblay
//////////////////////////////////////////////

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WpfHexaEditor.Core;
using WpfHexaEditor.Core.Bytes;
using WpfHexaEditor.Core.Interfaces;

namespace WpfHexaEditor
{
    /// <summary>
    /// To show Stream Offsets(left of HexEditor) and Column Index(top of HexEditor);
    /// </summary>
    public class CellStepsLayer : FontControlBase, ICellsLayer, IOffsetsInfoLayer
    {

        public event EventHandler<(int cellIndex, MouseButtonEventArgs e)> MouseLeftDownOnCell;
        public event EventHandler<(int cellIndex, MouseButtonEventArgs e)> MouseLeftUpOnCell;
        public event EventHandler<(int cellIndex, MouseEventArgs e)> MouseMoveOnCell;
        public event EventHandler<(int cellIndex, MouseButtonEventArgs e)> MouseRightDownOnCell;

        public Thickness CellMargin { get; set; } = new Thickness(2);
        public Thickness CellPadding { get; set; } = new Thickness(2);

        //If datavisualtype is Hex,"ox" should be calculated.
        public virtual Size CellSize => new Size(
            ((DataVisualType == DataVisualType.Hexadecimal ? 2 : 0) + SavedBits) * 
            CharSize.Width + CellPadding.Left + CellPadding.Right,
            CharSize.Height + CellPadding.Top + CellPadding.Bottom);

        public int SavedBits { get; set; } = 2;

        public Orientation Orientation
        {
            get => (Orientation) GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        // Using a DependencyProperty as the backing store for Orientation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(CellStepsLayer),
                new FrameworkPropertyMetadata(Orientation.Vertical, FrameworkPropertyMetadataOptions.AffectsRender));

        public long StartStepIndex
        {
            get => (long) GetValue(StartStepIndexProperty);
            set => SetValue(StartStepIndexProperty, value);
        }

        // Using a DependencyProperty as the backing store for StartOffset.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StartStepIndexProperty =
            DependencyProperty.Register(nameof(StartStepIndex), typeof(long), typeof(CellStepsLayer),
                new FrameworkPropertyMetadata(-1L, FrameworkPropertyMetadataOptions.AffectsRender));

        public int StepsCount
        {
            get => (int) GetValue(StepsProperty);
            set => SetValue(StepsProperty, value);
        }

        public DataVisualType DataVisualType { get; set; }

        // Using a DependencyProperty as the backing store for EndOffset.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StepsProperty =
            DependencyProperty.Register(nameof(StepsCount), typeof(int), typeof(CellStepsLayer),
                new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsRender));
        

        public int StepLength
        {
            get => (int) GetValue(StepLengthProperty);
            set => SetValue(StepLengthProperty, value);
        }

        // Using a DependencyProperty as the backing store for StepLength.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StepLengthProperty =
            DependencyProperty.Register(nameof(StepLength), typeof(int), typeof(CellStepsLayer),
                new PropertyMetadata(1));

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            void DrawOneStep(long offSet, Point startPoint)
            {
                var str = string.Empty;
                switch (DataVisualType)
                {
                    case DataVisualType.Hexadecimal:
                        str = $"0x{ByteConverters.LongToHex(offSet, SavedBits)}";
                        break;
                    case DataVisualType.Decimal:
                        str = ByteConverters.LongToString(offSet, SavedBits);
                        break;
                }
#if NET451
                var text = new FormattedText(str, CultureInfo.CurrentCulture, 
                    FlowDirection.LeftToRight, TypeFace, FontSize, Foreground);
#endif
#if NET47
                var text = new FormattedText(str, CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight, TypeFace, FontSize, Foreground, PixelPerDip);
#endif
                drawingContext.DrawText(text, startPoint);
            }

            void DrawSteps(Func<int, Point> getOffsetLocation)
            {
                for (var i = 0; i < StepsCount; i++)
                    DrawOneStep(
                        i * StepLength + StartStepIndex,
                        getOffsetLocation(i)
                    );
            }

            if (Orientation == Orientation.Horizontal)
            {
                DrawSteps(step =>
                    new Point
                    (
                        (CellMargin.Left + CellMargin.Right + CellSize.Width) *
                        step + CellMargin.Left + CellPadding.Left, CellMargin.Top + CellPadding.Top
                    )
                );

            }
            else
            {
#if DEBUG
                //double lastY = 0;
#endif
                DrawSteps(step => new Point(
                    CellMargin.Left + CellPadding.Left,
                    (CellMargin.Top + CellMargin.Bottom + CellSize.Height) *
                    step + CellMargin.Top + CellPadding.Top));

                {


#if DEBUG
                    //if(lastY != pot.Y) {
                    //    lastY = pot.Y;
                    //    System.Diagnostics.Debug.WriteLine(lastY);
                    //}
#endif
                    //return pot;
                }

            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            availableSize = base.MeasureOverride(availableSize);

            if (Orientation == Orientation.Horizontal)
            {
                availableSize.Height = CellMargin.Top + CellMargin.Bottom + CellSize.Height;

                if (double.IsInfinity(availableSize.Width))
                    availableSize.Width = 0;
            }
            else
            {
                availableSize.Width = CellMargin.Left + CellMargin.Right + CellSize.Width;

                if (double.IsInfinity(availableSize.Height))
                    availableSize.Height = 0;
            }

            return availableSize;
        }

        private int? GetIndexFromLocation(Point location)
        {
            if (StartStepIndex == -1)
                return null;

            if (Orientation == Orientation.Horizontal)
            {
                if (!(location.Y > 0 && location.Y < CellMargin.Bottom + CellMargin.Top + CellSize.Height))
                    return null;

                var col = (int) (location.X / (CellSize.Width + CellMargin.Left + CellMargin.Right));
                if (col >= StepsCount)
                    return null;

                return col;
            }

            if (!(location.X > 0 && location.X < CellMargin.Left + CellMargin.Right + CellSize.Width))
                return null;

            var row = (int) (location.Y / (CellSize.Width + CellMargin.Top + CellMargin.Bottom));
            if (row >= StepsCount)
                return null;

            return row;
        }

        private int? GetIndexFromMouse(MouseEventArgs e) => 
            e == null ? null : GetIndexFromLocation(e.GetPosition(this));

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (e.Handled)
                return;

            var index = GetIndexFromMouse(e);
            if (index != null)
                MouseLeftDownOnCell?.Invoke(this, (index.Value, e));
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            if (e.Handled)
                return;

            var index = GetIndexFromMouse(e);
            if (index != null)
                MouseLeftUpOnCell?.Invoke(this, (index.Value, e));
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.Handled)
                return;

            var index = GetIndexFromMouse(e);
            if (index != null)
                MouseMoveOnCell?.Invoke(this, (index.Value, e));
        }

        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonDown(e);
            if (e.Handled)
                return;

            var index = GetIndexFromMouse(e);
            if (index != null)
                MouseRightDownOnCell?.Invoke(this, (index.Value, e));
        }

        public Point? GetCellLocation(int index) => 
            throw new NotImplementedException();
    }

}
