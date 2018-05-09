//////////////////////////////////////////////
// Apache 2.0  - 2018
// Author : Janus Tida
// Modified by : Derek Tremblay
//////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;

namespace WpfHexaEditor
{
    public static class ToolTipExtension
    {
        private static readonly Dictionary<FrameworkElement, Popup> _toolTipDics =
            new Dictionary<FrameworkElement, Popup>();

        public static UIElement GetOperatableToolTip(DependencyObject obj) => 
            (UIElement) obj.GetValue(OperatableToolTipProperty);

        public static void SetOperatableToolTip(DependencyObject obj, FrameworkElement value) => 
            obj.SetValue(OperatableToolTipProperty, value);

        public static void SetToolTipOpen(this FrameworkElement elem, bool open, Point? point = null)
        {
            if (!_toolTipDics.ContainsKey(elem))
                throw new InvalidOperationException($"{nameof(_toolTipDics)} doesn't contain the {nameof(elem)}.");

            if (point != null)
            {
                _toolTipDics[elem].VerticalOffset = point.Value.Y;
                _toolTipDics[elem].HorizontalOffset = point.Value.X;
            }

            _toolTipDics[elem].IsOpen = open;
        }

        // Using a DependencyProperty as the backing store for OperatableTool.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OperatableToolTipProperty =
            DependencyProperty.RegisterAttached("OperatableToolTip", typeof(FrameworkElement),
                typeof(ToolTipExtension), new PropertyMetadata(null, OperatableToolProperty_Changed));

        private static void OperatableToolProperty_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is FrameworkElement elem)) return;

            if (_toolTipDics.ContainsKey(elem))
                _toolTipDics.Remove(elem);

            if (!(e.NewValue is UIElement newElem)) return;

            var toolPop = new Popup
            {
                Child = newElem,
                PopupAnimation = PopupAnimation.Fade,
                PlacementTarget = elem,
                Placement = PlacementMode.Relative
            };

            toolPop.MouseLeave += Popup_MouseLeave;

            _toolTipDics.Add(elem, toolPop);

            elem.MouseDown += FrameworkElem_MouseDown;
            elem.MouseUp += FrameworkElem_MouseUp;
            elem.MouseEnter += FrameworkElem_MouseEnter;
            elem.MouseLeave += FrameworkElem_MouseLeave;
            elem.Unloaded += FrameworkElem_Unload;

            toolPop.SetBinding(FrameworkElement.DataContextProperty,
                new Binding(nameof(FrameworkElement.DataContext))
                {
                    Source = elem
                }
            );
        }

        private static void FrameworkElem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is FrameworkElement elem)) return;

            if (!_toolTipDics.ContainsKey(elem)) return;

            if (GetAutoHide(elem))
                SetToolTipOpen(elem, false);
        }

        private static void FrameworkElem_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //if (!(sender is FrameworkElement elem)) return;

            //if (!_toolTipDics.ContainsKey(elem)) return;

            //var pop = _toolTipDics[elem];
            //_toolTipDics[elem] = pop;
        }

        private static void FrameworkElem_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!(sender is FrameworkElement elem)) return;

            if (!_toolTipDics.ContainsKey(elem)) return;

            var pop = _toolTipDics[elem];

            if (pop.IsMouseOver)
                return;

            if (GetAutoHide(elem))
                SetToolTipOpen(elem, false);

            _toolTipDics[elem] = pop;
        }

        private static void FrameworkElem_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!(sender is FrameworkElement elem)) return;

            if (!_toolTipDics.ContainsKey(elem)) return;

            if (Mouse.LeftButton == MouseButtonState.Pressed)
                return;

            try
            {
                var position = Mouse.GetPosition(elem);
                _toolTipDics[elem].VerticalOffset = position.Y;
                _toolTipDics[elem].HorizontalOffset = position.X;

                if (GetAutoShow(elem))
                    SetToolTipOpen(elem, true);
            }
            catch
            {
                // ignored
            }
        }

        private static void FrameworkElem_Unload(object sender, RoutedEventArgs e)
        {
            if (!(sender is FrameworkElement elem)) return;

            elem.MouseDown -= FrameworkElem_MouseDown;
            elem.MouseUp -= FrameworkElem_MouseUp;
            elem.MouseEnter -= FrameworkElem_MouseEnter;
            elem.MouseLeave -= FrameworkElem_MouseLeave;
            elem.Unloaded -= FrameworkElem_Unload;

            if (_toolTipDics.ContainsKey(elem))
            {
                _toolTipDics[elem].MouseLeave -= Popup_MouseLeave;
                _toolTipDics.Remove(elem);
            }
        }

        private static void Popup_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!(sender is Popup pop)) return;

            foreach (var dic in _toolTipDics)
                if (Equals(dic.Value, pop))
                {
                    if ((pop.Child as FrameworkElement)?.ContextMenu?.IsOpen ?? false)
                        return;

                    SetToolTipOpen(dic.Key, false);
                    break;
                }
        }

        //This dp show the popup while the mouse entering the targetElem if set to true;
        public static bool GetAutoShow(DependencyObject obj) => 
            (bool) obj.GetValue(AutoShowProperty);

        public static void SetAutoShow(DependencyObject obj, bool value) => obj.SetValue(AutoShowProperty, value);

        // Using a DependencyProperty as the backing store for AutoShow.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AutoShowProperty =
            DependencyProperty.RegisterAttached("AutoShow", typeof(bool), typeof(ToolTipExtension),
                new PropertyMetadata(true));

        //This dp hide the popup while the mouse leaving the targetElem if set to true;
        public static bool GetAutoHide(DependencyObject obj) => 
            (bool) obj.GetValue(AutoHideProperty);

        public static void SetAutoHide(DependencyObject obj, bool value) => 
            obj.SetValue(AutoHideProperty, value);

        // Using a DependencyProperty as the backing store for AutoHide.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AutoHideProperty =
            DependencyProperty.RegisterAttached("AutoHide", typeof(bool), typeof(ToolTipExtension),
                new PropertyMetadata(true));

    }
}
