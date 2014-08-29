using System;
using System.Diagnostics.Contracts;

using Mono;

using WoffDotNet.Types;

namespace WoffDotNet.Readers
{
    public class WoffTableDirectoryReader
    {
        private readonly byte[] _bytes;

        public WoffTableDirectoryReader(byte[] bytes)
        {
            Contract.Requires(bytes != null);
            Contract.Requires(bytes.Length == 20);

            _bytes = bytes;
        }

        public WoffTableDirectory Process()
        {
            DataConverter enc = DataConverter.BigEndian;
            UInt32 tag = enc.GetUInt32(_bytes, 0);
            UInt32 offset = enc.GetUInt32(_bytes, 4);
            UInt32 compLength = enc.GetUInt32(_bytes, 8);
            UInt32 origLength = enc.GetUInt32(_bytes, 12);
            UInt32 origChecksum = enc.GetUInt32(_bytes, 16);
            var result = new WoffTableDirectory(tag, offset, compLength, origLength, origChecksum);
            return result;
        }
    }
}
