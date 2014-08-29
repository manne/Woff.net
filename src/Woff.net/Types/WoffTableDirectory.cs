using System;

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

        public uint Tag { get; set; }

        public uint Offset { get; set; }

        public uint CompLength { get; set; }

        public uint OrigLength { get; set; }

        public uint OrigCheckSum { get; set; }
    }
}
