using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace WpfHexaEditor.Core.Interfaces
{
    public interface ICellsLayer
    {
        Thickness CellMargin { get; set; }
        Thickness CellPadding { get; set; }
        Size CellSize { get; }

        event EventHandler<(int cellIndex, MouseButtonEventArgs e)> MouseLeftDownOnCell;
        event EventHandler<(int cellIndex, MouseButtonEventArgs e)> MouseLeftUpOnCell;
        event EventHandler<(int cellIndex, MouseEventArgs e)> MouseMoveOnCell;
        event EventHandler<(int cellIndex, MouseButtonEventArgs e)> MouseRightDownOnCell;

        Point? GetCellLocation(int index);
    }

    public interface IFontControl
    {
        double FontSize { get; set; }
        FontFamily FontFamily { get; set; }
        FontWeight FontWeight { get; set; }

        Brush Foreground { get; set; }

        //How big a char text will be,this value will be caculated internally.
        Size CharSize { get; }
    }

    public interface IDataLayer
    {
        byte[] Data { get; set; }
        IEnumerable<(int index, int length, Brush background)> BackgroundBlocks { get; set; }
        IEnumerable<(int index, int length, Brush foreground)> ForegroundBlocks { get; set; }

        //int ColumnGroupCount { get; set; }
        //double GroupMargin { get; set; }

        Brush Foreground { get; }

        int BytePerLine { get; set; }
        int AvailableRowsCount { get; }
    }
}
