//////////////////////////////////////////////
// Apache 2.0  - 2018
// Author : Janus Tida
// Modified by : Derek Tremblay
//////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using WpfHexaEditor.Core.Interfaces;

namespace WpfHexaEditor
{
    public abstract class DataLayerBase : FontControlBase, IDataLayer, ICellsLayer
    {

        public event EventHandler<(int cellIndex, MouseButtonEventArgs e)> MouseLeftDownOnCell;
        public event EventHandler<(int cellIndex, MouseButtonEventArgs e)> MouseLeftUpOnCell;
        public event EventHandler<(int cellIndex, MouseEventArgs e)> MouseMoveOnCell;
        public event EventHandler<(int cellIndex, MouseButtonEventArgs e)> MouseRightDownOnCell;

        public byte[] Data
        {
            get => (byte[]) GetValue(DataProperty);
            set => SetValue(DataProperty, value);
        }

        // Using a DependencyProperty as the backing store for DataProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register(
                nameof(Data),
                typeof(byte[]),
                typeof(DataLayerBase),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.AffectsRender,
                    DataProperty_Changed
                )
            );

        private static void DataProperty_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is DataLayerBase ctrl))
                return;

            ctrl.InitializeMouseState();
        }

        private void RefreshRender(object sender, NotifyCollectionChangedEventArgs e) =>
            InvalidateVisual();

        public IEnumerable<(int index, int length, Brush foreground)> ForegroundBlocks
        {
            get => (IEnumerable<(int index, int length, Brush background)>) GetValue(ForegroundBlocksProperty);
            set => SetValue(ForegroundBlocksProperty, value);
        }

        // Using a DependencyProperty as the backing store for ForegroundBlocks.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ForegroundBlocksProperty =
            DependencyProperty.Register(nameof(ForegroundBlocks),
                typeof(IEnumerable<(int index, int length, Brush foreground)>),
                typeof(DataLayerBase),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.AffectsRender
                ));

        public IEnumerable<(int index, int length, Brush background)> BackgroundBlocks
        {
            get => (IEnumerable<(int index, int length, Brush foreground)>) GetValue(BackgroundBlocksProperty);
            set => SetValue(BackgroundBlocksProperty, value);
        }

        // Using a DependencyProperty as the backing store for BackgroundBlocks.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BackgroundBlocksProperty =
            DependencyProperty.Register(nameof(BackgroundBlocks),
                typeof(IEnumerable<(int index, int length, Brush background)>),
                typeof(DataLayerBase),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.AffectsRender
                ));


        public int BytePerLine
        {
            get => (int) GetValue(BytePerLineProperty);
            set => SetValue(BytePerLineProperty, value);
        }

        // Using a DependencyProperty as the backing store for BytePerLine.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BytePerLineProperty =
            DependencyProperty.Register(nameof(BytePerLine), typeof(int), typeof(DataLayerBase),
                new FrameworkPropertyMetadata(
                    16,
                    FrameworkPropertyMetadataOptions.AffectsRender
                ));

        public Thickness CellPadding { get; set; } = new Thickness(2);
        public Thickness CellMargin { get; set; } = new Thickness(2);

        public int AvailableRowsCount =>
            (int) (ActualHeight / (CellSize.Height + CellMargin.Top + CellMargin.Bottom));

        public abstract Size CellSize { get; }

        private (int index, Brush background)[] _drawedRects;

        private (int index, Brush background)[] DrawedRects
        {
            get
            {
                if (Data == null)
                    return null;

                if (_drawedRects == null || _drawedRects.Length < Data.Length)
                    _drawedRects = new(int index, Brush background)[Data.Length];

                return _drawedRects;
            }
        }
        

        public Brush Background
        {
            get => (Brush) GetValue(BackgroundProperty);
            set => SetValue(BackgroundProperty, value);
        }

        // Using a DependencyProperty as the backing store for Background.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register(nameof(Background), typeof(Brush), typeof(DataLayerBase),
                new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.AffectsRender));


        protected virtual void DrawBackgrounds(DrawingContext drawingContext)
        {
            if (BackgroundBlocks == null)
                return;

            if (DrawedRects == null)
                return;

            if (Data == null)
                return;

            for (var i = 0; i < Data.Length; i++)
                DrawedRects[i].background = Brushes.Transparent;

#if DEBUG
            //double lastY = 0;
#endif
            foreach (var (index, length, background) in BackgroundBlocks)
                for (var i = 0; i < length; i++)
                {
                    DrawedRects[index + i].background = background;
#if DEBUG
                    //if(this is HexDataLayer && lastY != rect.Y) {
                    //    lastY = rect.Y;
                    //    System.Diagnostics.Debug.WriteLine(rect.Y);
                    //}
#endif
                }

            drawingContext.DrawRectangle(Background, null, new Rect
            {
                Width = ActualWidth,
                Height = ActualHeight
            });

            for (var i = 0; i < Data.Length; i++)
            {
                var col = i % BytePerLine;
                var row = i / BytePerLine;
                if (Equals(DrawedRects[i].background, Background))
                    continue;

                drawingContext.DrawRectangle(
                    DrawedRects[i].background,
                    null,
                    new Rect
                    {
                        X = col * (CellMargin.Right + CellMargin.Left + CellSize.Width) + CellMargin.Left,
                        Y = row * (CellMargin.Top + CellMargin.Bottom + CellSize.Height) + CellMargin.Top,
                        Height = CellSize.Height,
                        Width = CellSize.Width
                    }
                );
            }

        }

        protected virtual void DrawText(DrawingContext drawingContext)
        {
            if (Data == null)
                return;

            var index = 0;
            foreach (var bt in Data)
            {
                var col = index % BytePerLine;
                var row = index / BytePerLine;

                var foreground = Foreground;
                if (ForegroundBlocks != null)
                    foreach (var tuple in ForegroundBlocks)
                    {
                        if (tuple.index > index || tuple.index + tuple.length < index) continue;

                        foreground = tuple.foreground;
                        break;
                    }

                DrawByte(drawingContext, bt, foreground,
                    new Point
                    (
                        (CellMargin.Right + CellMargin.Left + CellSize.Width) * col + CellPadding.Left + CellMargin.Left,
                        (CellMargin.Top + CellMargin.Bottom + CellSize.Height) * row + CellPadding.Top + CellMargin.Top
                    )
                );

                index++;
            }

        }

        protected abstract void DrawByte(DrawingContext drawingContext, byte bt, Brush foreground, Point startPoint);

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            DrawBackgrounds(drawingContext);
            DrawText(drawingContext);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            //availableSize = base.MeasureOverride(availableSize);
            availableSize.Width = (CellSize.Width + CellMargin.Left + CellMargin.Right) * BytePerLine;

            if (double.IsInfinity(availableSize.Height))
                availableSize.Height = 0;

            return availableSize;
        }

        private int? GetIndexFromLocation(Point location)
        {
            if (Data == null)
                return null;

            var col = (int) (location.X / (CellMargin.Left + CellMargin.Right + CellSize.Width));
            var row = (int) (location.Y / (CellMargin.Top + CellMargin.Bottom + CellSize.Height));

            if (row * BytePerLine + col < Data.Length)
                return row * BytePerLine + col;

            return null;
        }

        private int? GetIndexFromMouse(MouseEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            return GetIndexFromLocation(e.GetPosition(this));
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            if (e.Handled)
                return;

            if (Data == null)
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
            {
                if (index == lastMouseMoveIndex)
                    return;

                lastMouseMoveIndex = index;
                MouseMoveOnCell?.Invoke(this, (index.Value, e));
            }
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

        private int? lastMouseMoveIndex;


        private void InitializeMouseState() => 
            lastMouseMoveIndex = null;

        public Point? GetCellLocation(int index)
        {
            if (Data == null)
                return null;

            if (index > Data.Length)
                throw new IndexOutOfRangeException($"{nameof(index)} is larger than elements.");

            var col = index % BytePerLine;
            var row = index / BytePerLine;

            return new Point((CellSize.Width + CellMargin.Left + CellMargin.Right) * col,
                            (CellSize.Height + CellMargin.Top + CellMargin.Bottom) * row);
        }
    }

}
