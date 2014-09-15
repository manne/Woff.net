using System;
using System.Diagnostics;

namespace WoffDotNet.Types
{
    public class WoffTableDirectory
    {
        public const uint Size = 20;

        public WoffTableDirectory(UInt32 tag, UInt32 offset, UInt32 compLength, UInt32 origLength, UInt32 origCheckSum)
        {
            Tag = tag;
            Offset = offset;
            CompLength = compLength;
            OrigLength = origLength;
            OrigCheckSum = origCheckSum;
        }

        [DebuggerDisplay("{TagAsString()}")]
        public uint Tag { get; set; }

        public uint Offset { get; set; }

        public uint CompLength { get; set; }

        public uint OrigLength { get; set; }

        public uint OrigCheckSum { get; set; }

        internal uint Padding { get; set; }

        /// <summary>
        /// Returns the tag as a printable string.
        /// </summary>
        /// <returns>The tag as a printable string.</returns>
        public String TagAsString()
        {
            return new string(Tag.UInt32BEToCharArray());
        }
    }
}
