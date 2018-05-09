//////////////////////////////////////////////
// Apache 2.0  - 2018
// Author : Janus Tida
// Modified by : Derek Tremblay
//////////////////////////////////////////////

using System.Globalization;
using System.Windows;
using System.Windows.Media;
using WpfHexaEditor.Core.Interfaces;

namespace WpfHexaEditor
{
    public abstract class FontControlBase : FrameworkElement, IFontControl
    {
        public double FontSize
        {
            get => (double) GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        // Using a DependencyProperty as the backing store for FontSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FontSizeProperty =
            DependencyProperty.Register(nameof(FontSize), typeof(double), typeof(FontControlBase),
                new FrameworkPropertyMetadata(
                    12.0D,
                    FrameworkPropertyMetadataOptions.AffectsRender,
                    FontSize_PropertyChanged
                ));

        //Cuz font size may affectrender,the CellSize and squareSize should be updated.
        private static void FontSize_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is FontControlBase dataLB))
                return;

            dataLB.OnFontSizeChanged((double) e.OldValue, (double) e.NewValue);
        }

        protected virtual void OnFontSizeChanged(double oldFontSize, double newFontSize) => 
            OnUpdateSizes();

        /// <summary>
        /// Update CharSize...
        /// </summary>
        protected virtual void OnUpdateSizes() => 
            _charSize = null;


        public FontFamily FontFamily
        {
            get => (FontFamily) GetValue(FontFamilyProperty);
            set => SetValue(FontFamilyProperty, value);
        }

        // Using a DependencyProperty as the backing store for FontFamily.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FontFamilyProperty =
            DependencyProperty.Register(nameof(FontFamily), typeof(FontFamily), typeof(FontControlBase),
                new FrameworkPropertyMetadata(
                    new FontFamily("Microsoft YaHei"),
                    FrameworkPropertyMetadataOptions.AffectsRender,
                    FontFamily_PropertyChanged
                ));

        private static void FontFamily_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is FontControlBase dataLB))
                return;

            dataLB.OnFontFamilyChanged((FontFamily) e.OldValue, (FontFamily) e.NewValue);
        }

        protected virtual void OnFontFamilyChanged(FontFamily oldFontSize, FontFamily newFontSize) => OnUpdateSizes();

        public FontWeight FontWeight
        {
            get => (FontWeight) GetValue(FontWeightProperty);
            set => SetValue(FontWeightProperty, value);
        }

        public Brush Foreground
        {
            get => (Brush) GetValue(ForegroundProperty);
            set => SetValue(ForegroundProperty, value);
        }

        // Using a DependencyProperty as the backing store for DefaultForeground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ForegroundProperty =
            DependencyProperty.Register(nameof(Foreground), typeof(Brush),
                typeof(DataLayerBase),
                new FrameworkPropertyMetadata(
                    Brushes.Black,
                    FrameworkPropertyMetadataOptions.AffectsRender
                ));


        // Using a DependencyProperty as the backing store for FontWeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FontWeightProperty =
            DependencyProperty.Register(nameof(FontWeight), typeof(FontWeight), typeof(FontControlBase),
                new FrameworkPropertyMetadata(
                    new FontWeight(),
                    FrameworkPropertyMetadataOptions.AffectsRender,
                    FontWeight_PropertyChanged
                ));

        private static void FontWeight_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is FontControlBase fontLB))
                return;

            fontLB.OnFontWeightChanged((FontWeight) e.OldValue, (FontWeight) e.NewValue);
        }

        protected virtual void OnFontWeightChanged(FontWeight oldFontWeight, FontWeight newFontWeight) =>
            OnUpdateSizes();

#if NET47
        protected double PixelPerDip =>
            (_pixelPerDip ?? (_pixelPerDip = VisualTreeHelper.GetDpi(this).PixelsPerDip)).Value;

        private double? _pixelPerDip;
#endif
        protected Typeface TypeFace => _typeface ??
                                       (_typeface = new Typeface(FontFamily, new FontStyle(), FontWeight,
                                           new FontStretch()));

        private Typeface _typeface;

        private Size? _charSize;

        //Get the size of every char text;
        public Size CharSize
        {
            get
            {
                if (_charSize == null)
                {
                    //Cuz "D" may hold the "widest" size,we got the char width when the char is 'D';
                    var typeface = new Typeface(FontFamily, new FontStyle(), new FontWeight(), new FontStretch());
#if NET451
                    var measureText = new FormattedText(
                                    "D", CultureInfo.CurrentCulture,
                                    FlowDirection.LeftToRight, typeface, FontSize, Brushes.Black);
#endif
#if NET47
                    var measureText = new FormattedText(
                        "D", CultureInfo.CurrentCulture,
                        FlowDirection.LeftToRight, typeface, FontSize, Brushes.Black,
                        VisualTreeHelper.GetDpi(this).PixelsPerDip);
#endif
                    _charSize = new Size(measureText.Width, measureText.Height);
                }

                return _charSize.Value;
            }
        }
    }
}
