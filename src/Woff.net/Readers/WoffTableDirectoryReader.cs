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
            var tag = enc.GetUInt32(_bytes, 0);
            var offset = enc.GetUInt32(_bytes, 4);
            var compLength = enc.GetUInt32(_bytes, 8);
            var origLength = enc.GetUInt32(_bytes, 12);
            var origChecksum = enc.GetUInt32(_bytes, 16);
            var result = new WoffTableDirectory(tag, offset, compLength, origLength, origChecksum);
            return result;
        }
    }
}
