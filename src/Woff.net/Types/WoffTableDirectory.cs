using System;
using System.Diagnostics;

namespace WoffDotNet.Types
{
    public class WoffTableDirectory
    {
        /// <summary>
        /// The size of a table directory.
        /// </summary>
        public const uint Size = 20;

        public WoffTableDirectory(UInt32 tag, UInt32 offset, UInt32 compLength, UInt32 origLength, UInt32 origCheckSum)
        {
            Tag = tag;
            Offset = offset;
            CompLength = compLength;
            OrigLength = origLength;
            OrigCheckSum = origCheckSum;
        }

        /// <summary>
        /// Gets or sets the 4-byte sfnt table identifier.
        /// </summary>
        [DebuggerDisplay("{TagAsString()}, {Tag}")]
        public uint Tag { get; set; }

        /// <summary>
        /// Gets or sets the offset to the data, from beginning of WOFF file.
        /// </summary>
        public uint Offset { get; set; }

        /// <summary>
        /// Gets or sets the length of the compressed data, excluding padding.
        /// </summary>
        public uint CompLength { get; set; }

        /// <summary>
        /// Gets or sets the length of the uncompressed table, excluding padding.
        /// </summary>
        public uint OrigLength { get; set; }

        /// <summary>
        /// Gets or sets the checksum of the uncompressed table.
        /// </summary>
        public uint OrigCheckSum { get; set; }

        /// <summary>
        /// Gets or sets the padding to the next block.
        /// </summary>
        internal uint Padding { get; set; }

        /// <summary>
        /// Returns the tag as a printable string.
        /// </summary>
        /// <returns>The tag as a printable string.</returns>
        public string TagAsString()
        {
            return new string(Tag.UInt32BEToCharArray());
        }
    }
}
