using System;
using WpfHexaEditor.Core.Bytes;

namespace WpfHexaEditor
{
    internal class CustomBackgroundBlock
    {
        private long _length;

        public CustomBackgroundBlock(long start, long length)
        {
            Start = start;
            Length = length;
        }

        public CustomBackgroundBlock(string start, long length)
        {
            var srt = ByteConverters.HexLiteralToLong(start);

            Start = srt.success ? srt.position : throw new Exception("Can't convert this string to long");
            Length = length;
        }

        /// <summary>
        /// Get or set the start offset
        /// </summary>
        public long Start { get; set; }

        /// <summary>
        /// Get the stop offset
        /// </summary>
        public long Stop => Start + Length;

        /// <summary>
        /// Get or set the lenght of background block
        /// </summary>
        public long Length
        {
            get => _length;
            set => _length = value > 0 ? value : 1;
        }

        /// <summary>
        /// D
        /// </summary>
        public string Description { get; set; }
    }
}
