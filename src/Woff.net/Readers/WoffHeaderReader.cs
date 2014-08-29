using System.Diagnostics.Contracts;
using System.Globalization;

using Mono;

using WoffDotNet.Exceptions;
using WoffDotNet.Types;

namespace WoffDotNet.Readers
{
    public class WoffHeaderReader
    {
        private readonly byte[] _bytes;

        public WoffHeaderReader(byte[] bytes)
        {
            Contract.Requires(bytes != null);
            Contract.Requires(bytes.Length == WoffHeader.Size);

            _bytes = bytes;
        }

        public void Process()
        {
            var enc = DataConverter.BigEndian;
            var signature = enc.GetUInt32(_bytes, 0);

            if (!signature.Equals(WoffHeader.MagicNumberUInt))
            {
                throw new InvalidWoffMagicNumberException("There must be the Magic Number in the beginning of the stream");
            }

            var flavor = enc.GetUInt32(_bytes, 4);
            var length = enc.GetUInt32(_bytes, 8);
            var numTables = enc.GetUInt16(_bytes, 12);
            var reserved = enc.GetUInt16(_bytes, 14);
            if (reserved != 0)
            {
                throw new InvalidWoffReservedValueException("The reserved field must be zero");
            }

            var totalSfntSize = enc.GetUInt32(_bytes, 16);
            if (totalSfntSize % 4 != 0)
            {
                throw new InvalidWoffTotalSfntSizeException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "The total sfnt size ({0}) is not a multiple of four.",
                        totalSfntSize));
            }

            var majorVersion = enc.GetUInt16(_bytes, 20);
            var minorVersion = enc.GetUInt16(_bytes, 22);
            var metaOffset = enc.GetUInt32(_bytes, 24);
            var metaLength = enc.GetUInt32(_bytes, 28);
            var metaOrigLength = enc.GetUInt32(_bytes, 32);
            var privOffset = enc.GetUInt32(_bytes, 36);
            var privLength = enc.GetUInt32(_bytes, 40);

            var header = new WoffHeader(
                signature,
                flavor,
                length,
                numTables,
                reserved,
                totalSfntSize,
                majorVersion,
                minorVersion,
                metaOffset,
                metaLength,
                metaOrigLength,
                privOffset,
                privLength);
            Header = header;
        }

        public WoffHeader Header { get; private set; }
    }
}
