using System.Diagnostics.Contracts;
using System.IO;

using Mono;

using WoffDotNet.Types;

namespace WoffDotNet
{
    public class WoffReader
    {
        private readonly BinaryReader _binaryReader;

        private WoffHeader _header;

        public WoffReader(BinaryReader binaryReader)
        {
            Contract.Requires(binaryReader != null);

            _binaryReader = binaryReader;
        }


        public void Process()
        {
            var bytes = new byte[WoffHeader.Size];
            if (_binaryReader.Read(bytes, 0, bytes.Length) != WoffHeader.Size)
            {
                throw new EndOfStreamException("The stream does not have a header");
            }

            var enc = DataConverter.BigEndian;
            var signature = enc.GetUInt32(bytes, 0);

            if (!signature.Equals(WoffHeader.MagicNumberUInt))
            {
                throw new InvalidDataException("There must be the Magic Number in the beginning of the stream");
            }

            var flavor = enc.GetUInt32(bytes, 4);
            var length = enc.GetUInt32(bytes, 8);
            var numTables = enc.GetUInt16(bytes, 12);
            var reserved = enc.GetUInt16(bytes, 14);
            if (reserved != 0)
            {
                throw new InvalidDataException("The reserved field must be zero");
            }

            var totalSfntSize = enc.GetUInt32(bytes, 16);
            var majorVersion = enc.GetUInt16(bytes, 20);
            var minorVersion = enc.GetUInt16(bytes, 22);
            var metaOffset = enc.GetUInt32(bytes, 24);
            var metaLength = enc.GetUInt32(bytes, 28);
            var metaOrigLength = enc.GetUInt32(bytes, 32);
            var privOffset = enc.GetUInt32(bytes, 36);
            var privLength = enc.GetUInt32(bytes, 40);

            _header = new WoffHeader(signature, flavor, length, numTables, reserved, totalSfntSize, majorVersion, minorVersion, metaOffset, metaLength, metaOrigLength, privOffset, privLength);
        }

        public WoffHeader Header
        {
            get
            {
                return _header;
            }
        }
    }
}
