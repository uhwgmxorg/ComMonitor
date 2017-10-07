//////////////////////////////////////////////
// Apache 2.0  - 2017
// Author : Derek Tremblay (derektremblay666@gmail.com)
//////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.IO;
using WpfHexaEditor.Core.Bytes;

namespace WpfHexaEditor.Core.Interfaces
{
    public interface IByteProvider
    {
        //Properties
        bool CanRead { get; }

        bool CanSeek { get; }
        bool CanUndo { get; }
        bool CanWrite { get; }
        bool Eof { get; }
        string FileName { get; set; }
        bool IsEmpty { get; }
        bool IsOnLongProcess { get; }
        bool IsOpen { get; }
        bool IsUndoEnabled { get; set; }
        long Length { get; }
        double LongProcessProgress { get; }
        long Position { get; set; }
        bool ReadOnlyMode { get; set; }
        MemoryStream Stream { get; set; }
        ByteProviderStreamType StreamType { get; }
        int UndoCount { get; }
        Stack<ByteModified> UndoStack { get; }

        //Events
        event EventHandler ChangesSubmited;

        event EventHandler Closed;
        event EventHandler FillWithByteCompleted;
        event EventHandler LongProcessCanceled;
        event EventHandler LongProcessChanged;
        event EventHandler LongProcessCompleted;
        event EventHandler LongProcessStarted;
        event EventHandler PositionChanged;
        event EventHandler ReadOnlyChanged;
        event EventHandler ReplaceByteCompleted;
        event EventHandler StreamOpened;
        event EventHandler Undone;

        //Methods
        void Close();

        void OpenFile();
        int Read(byte[] destination, int offset, int count);
        byte[] Read(int count);
        int ReadByte();
        void ReplaceByte(long startPosition, long length, byte original, byte replace);
        void SubmitChanges();
        bool SubmitChanges(string newFilename, bool overwrite = false);
        void Undo();
    }
}