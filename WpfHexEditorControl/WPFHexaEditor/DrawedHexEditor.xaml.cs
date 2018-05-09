//////////////////////////////////////////////
// Apache 2.0  - 2018
// Author : Janus Tida
// Modified by : Derek Tremblay
//////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
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
    /// Interaction logic for DrawedHexEditor.xaml
    /// </summary>
    public partial class DrawedHexEditor
    {
        #region DevBranch

        public DrawedHexEditor()
        {
            InitializeComponent();
            FontSize = 8;
            FontFamily = new FontFamily("Courier New");
            DataVisualType = DataVisualType.Decimal;
            InitilizeEvents();
        }

        //Cuz xaml designer's didn't support valuetuple,events subscribing will be executed in code-behind.
        private void InitilizeEvents()
        {
            SizeChanged += delegate { UpdateContent(); };

            void initialCellsLayer(ICellsLayer layer)
            {
                layer.MouseLeftDownOnCell += DataLayer_MouseLeftDownOnCell;
                layer.MouseLeftUpOnCell += DataLayer_MouseLeftUpOnCell;
                layer.MouseMoveOnCell += DataLayer_MouseMoveOnCell;
                layer.MouseRightDownOnCell += DataLayer_MouseRightDownOnCell;
            }

            initialCellsLayer(HexDataLayer);
            initialCellsLayer(StringDataLayer);

            InitializeTooltipEvents();
        }

        /// <summary>
        /// Save the view byte buffer as a field. 
        /// To save the time when Scolling i do not building them every time when scolling.
        /// </summary>
        private byte[] _viewBuffer;

        private byte[] _viewBuffer2;

        //To avoid resigning buffer everytime and to notify the UI to rerender,
        //we're gonna switch from one to another while refreshing.
        private byte[] _realViewBuffer;

        //To avoid wrong mousemove event;
        private bool _contextMenuShowing;

        private int MaxVisibleLength
        {
            get
            {
                if (Stream == null)
                    return 0;

                return (int) Math.Min(HexDataLayer.AvailableRowsCount * BytePerLine,
                    Stream.Length - Position / BytePerLine * BytePerLine);
            }
        }

        protected override void OnContextMenuOpening(ContextMenuEventArgs e)
        {
            base.OnContextMenuOpening(e);
            _contextMenuShowing = true;
        }

        protected override void OnContextMenuClosing(ContextMenuEventArgs e)
        {
            base.OnContextMenuClosing(e);
            _contextMenuShowing = false;
#if DEBUG
            //ss++;
#endif
        }

#if DEBUG
        //private long ss = 0;
#endif

        /// <summary>
        /// Obtain the max line for verticalscrollbar
        /// </summary>
        private long MaxLine => Stream.Length / BytePerLine;

#if DEBUG
        private readonly Stopwatch watch = new Stopwatch();
#endif

        private readonly List<(int index, int length, Brush background)> dataBackgroundBlocks =
            new List<(int index, int length, Brush background)>();

        //To avoid endless looping of ScrollBar_ValueChanged and Position_PropertyChanged.
        private bool _scrollBarValueUpdating;

        //Remember the position in which the mouse last clicked.
        private long? _lastMouseDownPosition;

        #region EventSubscriber handlers;

        private void Control_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Stream == null) return;

            if (e.Delta > 0) //UP
                VerticalScrollBar.Value -= e.Delta / 120 * (int) MouseWheelSpeed;

            if (e.Delta < 0) //Down
                VerticalScrollBar.Value += e.Delta / 120 * -(int) MouseWheelSpeed;
        }

        private void DataLayer_MouseLeftDownOnCell(object sender, (int cellIndex, MouseButtonEventArgs e) arg)
        {
            if (arg.cellIndex >= MaxVisibleLength)
                return;

            var clickPosition = Position / BytePerLine * BytePerLine + arg.cellIndex;
            if (Keyboard.Modifiers == ModifierKeys.Shift)
            {
                long oldStart = -1;

                if (SelectionStart != -1)
                    oldStart = SelectionStart;

                if (FocusPosition != -1)
                    oldStart = FocusPosition;

                if (oldStart != -1)
                {
                    SelectionStart = Math.Min(oldStart, clickPosition);
                    SelectionLength = Math.Abs(oldStart - clickPosition) + 1;
                }
            }

            _lastMouseDownPosition = clickPosition;

            FocusPosition = _lastMouseDownPosition.Value;
        }

        private void DataLayer_MouseRightDownOnCell(object sender, (int cellIndex, MouseButtonEventArgs e) arg) => 
            DataLayer_MouseLeftDownOnCell(sender, arg);

        private void DataLayer_MouseMoveOnCell(object sender, (int cellIndex, MouseEventArgs e) arg)
        {
            if (arg.e.LeftButton != MouseButtonState.Pressed)
                return;

            if (_contextMenuShowing)
                return;

#if DEBUG
            //arg.cellIndex = 15;
            //_lastMouseDownPosition = 0;
#endif
            //Operate Selection;
            if (_lastMouseDownPosition == null)
                return;

            var cellPosition = Position / BytePerLine * BytePerLine + arg.cellIndex;
            if (_lastMouseDownPosition.Value == cellPosition)
                return;

            var length = Math.Abs(cellPosition - _lastMouseDownPosition.Value) + 1;
            SelectionStart = Math.Min(cellPosition, _lastMouseDownPosition.Value);
            SelectionLength = length;
        }

        private void DataLayer_MouseLeftUpOnCell(object sender, (int cellIndex, MouseButtonEventArgs e) arg) => 
            _lastMouseDownPosition = null;

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (Stream == null)
                return;

            if (FocusPosition == -1)
                return;

            if (KeyValidator.IsArrowKey(e.Key))
            {
                OnArrowKeyDown(e);
                e.Handled = true;
            }

        }

        //Deal with operation while arrow key is pressed.
        private void OnArrowKeyDown(KeyEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (!KeyValidator.IsArrowKey(e.Key))
                throw new ArgumentException($"The key '{e.Key}' is not a arrow key.");

            if (Stream == null)
                return;

            if (FocusPosition == -1)
                return;

            //Update Selection if shift key is pressed;
            if (Keyboard.Modifiers == ModifierKeys.Shift)
            {
                long vectorEnd = -1;
                switch (e.Key)
                {
                    case Key.Left:
                        if (FocusPosition > 0)
                        {
                            vectorEnd = FocusPosition - 1;
                        }

                        break;
                    case Key.Up:
                        if (FocusPosition >= BytePerLine)
                        {
                            vectorEnd = FocusPosition - BytePerLine;
                        }

                        break;
                    case Key.Right:
                        if (FocusPosition + 1 < Stream.Length)
                        {
                            vectorEnd = FocusPosition + 1;
                        }

                        break;
                    case Key.Down:
                        if (FocusPosition + BytePerLine < Stream.Length)
                        {
                            vectorEnd = FocusPosition + BytePerLine;
                        }

                        break;
                }

                if (vectorEnd != -1)
                {
                    //BackWard;
                    if (vectorEnd < FocusPosition)
                    {
                        if (FocusPosition == SelectionStart)
                        {
                            SelectionLength += SelectionStart - vectorEnd;
                            SelectionStart = vectorEnd;
                        }
                        else if (FocusPosition == SelectionStart + SelectionLength - 1 && 
                                 SelectionLength >= FocusPosition - vectorEnd + 1)
                        {
                            SelectionLength -= FocusPosition - vectorEnd;
                        }
                        else
                        {
                            SelectionStart = vectorEnd;
                            SelectionLength = FocusPosition - vectorEnd + 1;
                        }
                    }
                    //Forward;
                    else if (vectorEnd > FocusPosition)
                    {
                        if (FocusPosition == SelectionStart + SelectionLength - 1)
                        {
                            SelectionLength += vectorEnd - FocusPosition;
                        }
                        else if (FocusPosition == SelectionStart && 
                                 SelectionLength >= vectorEnd - FocusPosition + 1)
                        {
                            SelectionLength -= vectorEnd - SelectionStart;
                            SelectionStart = vectorEnd;
                        }
                        else
                        {
                            SelectionStart = FocusPosition;
                            SelectionLength = vectorEnd - FocusPosition + 1;
                        }
                    }
                }

            }

            //Updte FocusSelection;
            switch (e.Key)
            {
                case Key.Left:
                    if (FocusPosition > 0)
                        FocusPosition--;

                    break;
                case Key.Up:
                    if (FocusPosition >= BytePerLine)
                        FocusPosition -= BytePerLine;

                    break;
                case Key.Right:
                    if (FocusPosition + 1 < Stream.Length)
                        FocusPosition++;

                    break;
                case Key.Down:
                    if (FocusPosition + BytePerLine < Stream.Length)
                        FocusPosition += BytePerLine;

                    break;
                default:
                    return;
            }

            //Update scrolling(if needed);
            var firstVisiblePosition = Position / BytePerLine * BytePerLine;
            var lastVisiblePosition = firstVisiblePosition + MaxVisibleLength - 1;
            if (FocusPosition < firstVisiblePosition)
            {
                Position -= BytePerLine;
            }
            else if (FocusPosition > lastVisiblePosition)
            {
                Position += BytePerLine;
            }

        }

        #endregion


        /// <summary>
        /// This method won't be while scrolling,but only when stream is opened or closed,byteperline changed(UpdateInfo);
        /// </summary>
        private void UpdateInfoes()
        {
            UpdateScrollBarInfo();
            UpdateColumnHeaderInfo();
            UpdateOffsetLinesInfo();

            //Position PropertyChangedCallBack will update the content;
            Position = 0;

            //Restore/Update Focus Position;
            if (FocusPosition >= (Stream?.Length ?? 0))
                FocusPosition = -1;

            //RestoreSelection;
            SelectionStart = -1;
            SelectionLength = 0;
        }

        #region These methods won't be invoked everytime scrolling.but only when stream is opened or closed,byteperline changed(UpdateInfo).

        /// <summary>
        /// Update vertical scrollbar with file info
        /// </summary>
        private void UpdateScrollBarInfo()
        {
            VerticalScrollBar.Visibility = Visibility.Collapsed;

            if (Stream == null) return;

            VerticalScrollBar.Visibility = Visibility.Visible;
            VerticalScrollBar.SmallChange = 1;
            //VerticalScrollBar.LargeChange = ScrollLargeChange;
            VerticalScrollBar.Maximum = MaxLine - 1;
        }

        /// <summary>
        /// Update the position info panel at top of the control
        /// </summary>
        private void UpdateColumnHeaderInfo()
        {
            ColumnsOffsetInfoLayer.StartStepIndex = 0;
            ColumnsOffsetInfoLayer.StepsCount = 16;
        }

        /// <summary>
        /// Update the position info panel at left of the control,see this won't change the content of the OffsetLines;
        /// </summary>
        private void UpdateOffsetLinesInfo()
        {
            if (Stream == null)
                return;

            LinesOffsetInfoLayer.DataVisualType = DataVisualType;
            LinesOffsetInfoLayer.StepLength = BytePerLine;

            LinesOffsetInfoLayer.SavedBits = DataVisualType == DataVisualType.Hexadecimal
                ? ByteConverters.GetHexBits(Stream.Length)
                : ByteConverters.GetDecimalBits(Stream.Length);
        }

        //This will affect how a linesinfo and columnsinfo index change.
        public DataVisualType DataVisualType
        {
            get => (DataVisualType) GetValue(DataVisualTypeProperty);
            set => SetValue(DataVisualTypeProperty, value);
        }

        // Using a DependencyProperty as the backing store for DataVisualType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataVisualTypeProperty =
            DependencyProperty.Register(nameof(DataVisualType),
                typeof(DataVisualType), typeof(DrawedHexEditor),
                new PropertyMetadata(DataVisualType.Hexadecimal, DataVisualTypeProperty_Changed));

        private static void DataVisualTypeProperty_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is DrawedHexEditor ctrl))
            {
                return;
            }

            ctrl.LinesOffsetInfoLayer.DataVisualType = (DataVisualType) e.NewValue;
            ctrl.ColumnsOffsetInfoLayer.DataVisualType = (DataVisualType) e.NewValue;
            ctrl.UpdateContent();
        }

        #endregion


        public long Position
        {
            get { return (long) GetValue(PositionProperty); }
            set
            {
                SetValue(PositionProperty, value);
#if DEBUG
                watch.Restart();
#endif
                UpdateContent();
#if DEBUG
                watch.Stop();
                Debug.Print($"REFRESH TIME: {watch.ElapsedMilliseconds} ms");
#endif
            }

        }

        // Using a DependencyProperty as the backing store for Position.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register(nameof(Position), typeof(long), typeof(DrawedHexEditor),
                new FrameworkPropertyMetadata(-1L, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));


        public IEnumerable<(long index, long length, Brush background)> CustomBackgroundBlocks
        {
            get => (IEnumerable<(long index, long length, Brush background)>) GetValue(
                CustomBackgroundBlocksProperty);
            set => SetValue(CustomBackgroundBlocksProperty, value);
        }

        // Using a DependencyProperty as the backing store for CustomBackgroundBlocks.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CustomBackgroundBlocksProperty =
            DependencyProperty.Register(nameof(CustomBackgroundBlocks),
                typeof(IEnumerable<(long index, long length, Brush background)>),
                typeof(DrawedHexEditor),
                new PropertyMetadata(null));


        public Thickness CellMargin
        {
            get => (Thickness) GetValue(CellMarginProperty);
            set => SetValue(CellMarginProperty, value);
        }

        // Using a DependencyProperty as the backing store for CellMargion.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CellMarginProperty =
            DependencyProperty.Register(nameof(CellMargin), typeof(Thickness), typeof(DrawedHexEditor),
                new PropertyMetadata(new Thickness(0), CellMargionProperty_Changed));

        private static void CellMargionProperty_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is DrawedHexEditor ctrl)) return;

            var newVal = (Thickness) e.NewValue;
            ctrl.HexDataLayer.CellMargin = newVal;
            ctrl.StringDataLayer.CellMargin = newVal;
            ctrl.LinesOffsetInfoLayer.CellMargin = new Thickness(0, newVal.Top, 0, newVal.Bottom);
            ctrl.ColumnsOffsetInfoLayer.CellMargin = new Thickness(newVal.Left, 0, newVal.Right, 0);
        }

        public Thickness CellPadding
        {
            get => (Thickness) GetValue(CellPaddingProperty);
            set => SetValue(CellPaddingProperty, value);
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CellPaddingProperty =
            DependencyProperty.Register(nameof(CellPadding), typeof(Thickness), typeof(DrawedHexEditor),
                new PropertyMetadata(new Thickness(0), CellPaddingProperty_Changed));

        private static void CellPaddingProperty_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is DrawedHexEditor ctrl)) return;

            var newVal = (Thickness) e.NewValue;
            ctrl.HexDataLayer.CellPadding = newVal;
            ctrl.StringDataLayer.CellPadding = newVal;
            ctrl.LinesOffsetInfoLayer.CellPadding = new Thickness(0, newVal.Top, 0, newVal.Bottom);
            ctrl.ColumnsOffsetInfoLayer.CellPadding = new Thickness(newVal.Left, 0, newVal.Right, 0);
        }

        /**/

        /// <summary>
        /// Refresh currentview of hexeditor
        /// </summary>
        public void UpdateContent()
        {
            UpdateOffsetLinesContent();
            UpdateScrollBarContent();
            //Update visual of byte control
            //UpdateByteModified();

            //UpdateHighLight();
            //UpdateStatusBar();
            //UpdateVisual();
            //UpdateFocus();

            //CheckProviderIsOnProgress();

            //if (controlResize) {
            //    UpdateScrollMarkerPosition();
            //    UpdateHeader(true);
            //}

            UpdateBackgroundBlocks();

            UpdateDataContent();
        }



        #region  These methods will be invoked every time scrolling the content(scroll or position changed)(Refreshview calling);

        ///<see cref="UpdateContent"/>
        /// <summary>
        /// Update the hex and string layer you current view;
        /// </summary>
        private void UpdateDataContent()
        {
            if (!(Stream?.CanRead ?? false))
            {
                HexDataLayer.Data = null;
                StringDataLayer.Data = null;
                return;
            }

            Stream.Position = Position / BytePerLine * BytePerLine;

            if (_viewBuffer == null || _viewBuffer.Length != MaxVisibleLength)
                _viewBuffer = new byte[MaxVisibleLength];

            if (_viewBuffer2 == null || _viewBuffer2.Length != MaxVisibleLength)
                _viewBuffer2 = new byte[MaxVisibleLength];

            _realViewBuffer = _realViewBuffer == _viewBuffer ? _viewBuffer2 : _viewBuffer;

            Stream.Read(_realViewBuffer, 0, MaxVisibleLength);

            HexDataLayer.Data = _realViewBuffer;
            StringDataLayer.Data = _realViewBuffer;
        }

        private void UpdateOffsetLinesContent()
        {
            if (Stream == null)
            {
                LinesOffsetInfoLayer.StartStepIndex = 0;
                LinesOffsetInfoLayer.StepsCount = 0;
                return;
            }

            LinesOffsetInfoLayer.StartStepIndex = Position / BytePerLine * BytePerLine;
            LinesOffsetInfoLayer.StepsCount =
                Math.Min(HexDataLayer.AvailableRowsCount,
                    MaxVisibleLength / BytePerLine + (MaxVisibleLength % BytePerLine != 0 ? 1 : 0));
        }

        private void UpdateScrollBarContent()
        {
            if (_scrollBarValueUpdating) return;

            _scrollBarValueUpdating = true;
            VerticalScrollBar.Value = Position / BytePerLine;
            _scrollBarValueUpdating = false;
        }

        #region Data Backgrounds

        private void UpdateBackgroundBlocks()
        {
            //ClearBackgroundBlocks;
            HexDataLayer.BackgroundBlocks = null;
            StringDataLayer.BackgroundBlocks = null;

            dataBackgroundBlocks.Clear();

            AddCustomBackgroundBlocks();
            AddSelectionBackgroundBlocks();
            AddFocusPositionBlock();

            HexDataLayer.BackgroundBlocks = dataBackgroundBlocks;
            StringDataLayer.BackgroundBlocks = dataBackgroundBlocks;
        }

        private void AddBackgroundBlock(long index, long length, Brush background)
        {
            if (Stream == null)
                return;

            //Check whether Selection is in sight;
            if (!(index + length >= Position && index < Position + MaxVisibleLength))
                return;

            var maxIndex = Math.Max(index, Position);
            var minEnd = Math.Min(index + length, Position + MaxVisibleLength);

            dataBackgroundBlocks.Add(((int) (maxIndex - Position), (int) (minEnd - maxIndex), background));
        }

        private void AddSelectionBackgroundBlocks() =>
            AddBackgroundBlock(SelectionStart, SelectionLength, SelectionBrush);

        private void AddCustomBackgroundBlocks()
        {
            if (CustomBackgroundBlocks == null) return;

            foreach (var (index, length, background) in CustomBackgroundBlocks)
                AddBackgroundBlock(index, length, background);
        }

        private void AddFocusPositionBlock()
        {
            if (FocusPosition >= 0)
                AddBackgroundBlock(FocusPosition, 1, FocusBrush);
        }

        #endregion

        private void UpdateForegroundBlocks()
        {

        }

        #endregion



        private void VerticalScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_scrollBarValueUpdating)
                return;

            _scrollBarValueUpdating = true;
            Position = (long) e.NewValue * BytePerLine;
            _scrollBarValueUpdating = false;
        }

        private void BottomRectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void BottomRectangle_MouseEnter(object sender, MouseEventArgs e)
        {

        }

        private void BottomRectangle_MouseLeave(object sender, MouseEventArgs e)
        {

        }

        private void TopRectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void TopRectangle_MouseEnter(object sender, MouseEventArgs e)
        {

        }

        private void TopRectangle_MouseLeave(object sender, MouseEventArgs e)
        {

        }

        #endregion

        #region DependencyPorperties

        #region BytePerLine property/methods

        /// <summary>
        /// Get or set the number of byte are show in control
        /// </summary>
        public int BytePerLine
        {
            get => (int) GetValue(BytePerLineProperty);
            set => SetValue(BytePerLineProperty, value);
        }

        public static readonly DependencyProperty BytePerLineProperty =
            DependencyProperty.Register("BytePerLine", typeof(int), typeof(HexEditor),
                new PropertyMetadata(16, BytePerLine_PropertyChanged));

        private static void BytePerLine_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is DrawedHexEditor ctrl) || e.NewValue == e.OldValue) return;
            ctrl.HexDataLayer.BytePerLine = (int) e.NewValue;
            ctrl.StringDataLayer.BytePerLine = (int) e.NewValue;

            ctrl.UpdateInfoes();
            ctrl.UpdateContent();
        }

        #endregion

        public long SelectionStart
        {
            get => (long) GetValue(SelectionStartProperty);
            set => SetValue(SelectionStartProperty, value);
        }

        // Using a DependencyProperty as the backing store for SelectionStart.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectionStartProperty =
            DependencyProperty.Register(nameof(SelectionStart), typeof(long), typeof(DrawedHexEditor),
                new FrameworkPropertyMetadata(-1L, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    SelectionStart_PropertyChanged));

        private static void SelectionStart_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is DrawedHexEditor ctrl))
                return;

            ctrl.UpdateBackgroundBlocks();
        }

        public long SelectionLength
        {
            get => (long) GetValue(SelectionLengthProperty);
            set => SetValue(SelectionLengthProperty, value);
        }


        // Using a DependencyProperty as the backing store for SelectionLength.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectionLengthProperty =
            DependencyProperty.Register(nameof(SelectionLength), typeof(long), typeof(DrawedHexEditor),
                new FrameworkPropertyMetadata(0L, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    SelectionLengthProperty_Changed));

        private static void SelectionLengthProperty_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is DrawedHexEditor ctrl))
                return;

            ctrl.UpdateBackgroundBlocks();
        }

        public long FocusPosition
        {
            get => (long) GetValue(FocusPositionProperty);
            set => SetValue(FocusPositionProperty, value);
        }

        // Using a DependencyProperty as the backing store for FocusPosition.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FocusPositionProperty =
            DependencyProperty.Register(nameof(FocusPosition), typeof(long), typeof(DrawedHexEditor),
                new FrameworkPropertyMetadata(-1L, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    FocusPositionProperty_Changed));

        private static void FocusPositionProperty_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is DrawedHexEditor ctrl))
                return;

            ctrl.UpdateBackgroundBlocks();

            if ((long) e.NewValue == -1) return;

            ctrl.Focusable = true;
            ctrl.Focus();
        }



        public Brush FocusBrush
        {
            get => (Brush) GetValue(FocusBrushProperty);
            set => SetValue(FocusBrushProperty, value);
        }

        // Using a DependencyProperty as the backing store for FocusBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FocusBrushProperty =
            DependencyProperty.Register(nameof(FocusBrush), typeof(Brush), typeof(DrawedHexEditor),
                new PropertyMetadata(Brushes.Blue));



        public Brush SelectionBrush
        {
            get => (Brush) GetValue(SelectionBrushProperty);
            set => SetValue(SelectionBrushProperty, value);
        }

        // Using a DependencyProperty as the backing store for SelectionBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectionBrushProperty =
            DependencyProperty.Register(nameof(SelectionBrush), typeof(Brush), typeof(DrawedHexEditor),
                new PropertyMetadata(new SolidColorBrush(Color.FromRgb(0xe0, 0xe0, 0xff))));


        public MouseWheelSpeed MouseWheelSpeed
        {
            get => (MouseWheelSpeed) GetValue(MouseWheelSpeedProperty);
            set => SetValue(MouseWheelSpeedProperty, value);
        }

        // Using a DependencyProperty as the backing store for MouseWheelSpeed.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MouseWheelSpeedProperty =
            DependencyProperty.Register(nameof(MouseWheelSpeed), typeof(MouseWheelSpeed), typeof(DrawedHexEditor),
                new PropertyMetadata(MouseWheelSpeed.Normal));


        /// <summary>
        /// Set the Stream are used by ByteProvider
        /// </summary>
        public Stream Stream
        {
            get => (Stream) GetValue(StreamProperty);
            set => SetValue(StreamProperty, value);
        }

        // Using a DependencyProperty as the backing store for Stream.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StreamProperty =
            DependencyProperty.Register(nameof(Stream), typeof(Stream), typeof(DrawedHexEditor),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits,
                    Stream_PropertyChanged));

        private static void Stream_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is DrawedHexEditor ctrl)) return;
            //These methods won't be invoked everytime scrolling.but only when stream is opened or closed.
            ctrl.UpdateInfoes();
        }

        #endregion
    }

    //Tooltip being worked;...
    public partial class DrawedHexEditor
    {
        private void InitializeTooltipEvents()
        {
            HexDataLayer.MouseMoveOnCell += Datalayer_MouseMoveOnCell;
            StringDataLayer.MouseMoveOnCell += Datalayer_MouseMoveOnCell;
        }

        private long mouseOverLevel;

        private void Datalayer_MouseMoveOnCell(object sender, (int cellIndex, MouseEventArgs e) arg)
        {
            var index = arg.cellIndex;
            if (!(sender is DataLayerBase dataLayer))
                return;

            if (_contextMenuShowing)
                return;

            var popPoint = dataLayer.GetCellLocation(index);
            if (popPoint == null)
                return;

            var pointValue = popPoint.Value;
            if (Mouse.LeftButton == MouseButtonState.Pressed)
                return;

            HoverPosition = Position / BytePerLine * BytePerLine + arg.cellIndex;

            if (ToolTipExtension.GetOperatableToolTip(dataLayer) == null)
                return;

            dataLayer.SetToolTipOpen(false);
            var thisLevel = mouseOverLevel++;

            //Delay is designed to improve the experience;
            ThreadPool.QueueUserWorkItem(cb =>
            {
                Thread.Sleep(500);
                if (mouseOverLevel > thisLevel + 1)
                    return;

                Dispatcher.Invoke(() =>
                {
                    if (Mouse.LeftButton == MouseButtonState.Pressed)
                        return;

                    dataLayer.SetToolTipOpen(true, new Point
                    {
                        X = pointValue.X + dataLayer.CellMargin.Left + dataLayer.CharSize.Width +
                            dataLayer.CellPadding.Left,
                        Y = pointValue.Y + dataLayer.CharSize.Height + dataLayer.CellPadding.Top +
                            dataLayer.CellMargin.Top
                    });
                });
            });
        }



        public FrameworkElement HexDataToolTip
        {
            get => (FrameworkElement) GetValue(HexDataToolTipProperty);
            set => SetValue(HexDataToolTipProperty, value);
        }

        // Using a DependencyProperty as the backing store for HexDataToolTip.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HexDataToolTipProperty =
            DependencyProperty.Register(nameof(HexDataToolTip), typeof(FrameworkElement), typeof(DrawedHexEditor),
                new PropertyMetadata(null,
                    HexDataToolTip_PropertyChanged));

        private static void HexDataToolTip_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is DrawedHexEditor ctrl))
                return;

            if (e.NewValue is FrameworkElement newElem)
                ToolTipExtension.SetOperatableToolTip(ctrl.HexDataLayer, newElem);
        }

        public FrameworkElement StringDataToolTip
        {
            get => (FrameworkElement) GetValue(HexDataToolTipProperty);
            set => SetValue(HexDataToolTipProperty, value);
        }

        // Using a DependencyProperty as the backing store for HexDataToolTip.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StringDataToolTipProperty =
            DependencyProperty.Register(nameof(StringDataToolTip), typeof(FrameworkElement), typeof(DrawedHexEditor),
                new PropertyMetadata(null,
                    StringDataToolTip_PropertyChanged));

        private static void StringDataToolTip_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is DrawedHexEditor ctrl))
                return;

            if (e.NewValue is FrameworkElement newElem)
                ToolTipExtension.SetOperatableToolTip(ctrl.StringDataLayer, newElem);
        }

        public long HoverPosition
        {
            get => (long) GetValue(HoverPositionProperty);
            set => SetValue(HoverPositionProperty, value);
        }

        // Using a DependencyProperty as the backing store for HoverPosition.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HoverPositionProperty =
            DependencyProperty.Register(nameof(HoverPosition), typeof(long), typeof(DrawedHexEditor),
                new FrameworkPropertyMetadata(-1L, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
    }
}
