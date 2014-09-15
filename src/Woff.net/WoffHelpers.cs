using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

using WoffDotNet.Types;
using WoffDotNet.Validators;

namespace WoffDotNet
{
    /// <summary>
    /// The WOFF helpers.
    /// </summary>
    public static class WoffHelpers
    {
        public static bool HasMetadata(this WoffHeader header)
        {
            if (WoffHeaderValidator.HasIllegalMetadata(header))
            {
                return false;
            }

            return header.MetaOffset > 0;
        }

        public static bool HasPrivateDate(this WoffHeader header)
        {
            if (WoffHeaderValidator.HasIllegalPrivateData(header))
            {
                return false;
            }

            return header.PrivOffset > 0;
        }

        /// <summary>
        /// Calculates the additional needed bytes to get to the next 4 byte boundary.
        /// </summary>
        /// <param name="position"></param>
        /// <returns>The next 4 byte boundary, can be the same as <paramref name="position"/>.</returns>
        public static UInt32 Calculate4BytePadding(UInt32 position)
        {
            Contract.Ensures(Contract.Result<UInt32>() <= 3);

            uint mod = position % 4;
            return (mod == 0) ? 0 : 4 - mod;
        }

        public static bool HasEncoding(this XmlDocument document, string encoding)
        {

            if (document.FirstChild.NodeType == XmlNodeType.XmlDeclaration)
            {
                var xmlDeclaration = (XmlDeclaration)document.FirstChild;
                if (string.IsNullOrEmpty(xmlDeclaration.Encoding))
                {
                    return true;
                }

                if (string.Equals(xmlDeclaration.Encoding, "UTF-8", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Calculates the encoding of the byte array.
        /// </summary>
        /// <param name="bytes">
        /// The bytes.
        /// </param>
        /// <returns>
        /// The <see cref="Encoding"/>.
        /// </returns>
        public static Encoding GetEncoding(this byte[] bytes)
        {
            using (var reader = new StreamReader(new MemoryStream(bytes), true))
            {
                reader.Read();
                var currentEncoding = reader.CurrentEncoding;
                return currentEncoding;
            }
        }

        public static char[] UInt32BEToCharArray(this UInt32 value)
        {
            Contract.Ensures(Contract.Result<char[]>() != null);
            Contract.Ensures(Contract.Result<char[]>().Length == 4);

            // only for big endian
            var c = new char[4];
            c[3] = (char)(value & 0x000000ff);
            c[2] = (char)((value & 0x0000ff00) >> 8);
            c[1] = (char)((value & 0x00ff0000) >> 16);
            c[0] = (char)((value & 0xff000000) >> 24);
            return c;
        }

        /// <summary>
        /// Convert a UInt32 into strings, <param name="value" /> must be in big endian.
        /// 0x0 are replaced with an x20. Therefore the string is valid and printable.
        /// </summary>
        public static string UInt32ToString(this UInt32 value)
        {
            Contract.Ensures(Contract.Result<string>() != null);
            Contract.Ensures(Contract.Result<string>().Length == 4);

            var c = UInt32BEToCharArray(value);
            if (c[3] == char.MinValue)
                c[3] = ' ';
            if (c[2] == char.MinValue)
                c[2] = ' ';
            if (c[1] == char.MinValue)
                c[1] = ' ';
            if (c[0] == char.MinValue)
                c[0] = ' ';
            return new string(c);
        }

        public static bool IsNullByteArray(byte[] bytes)
        {
            return bytes.Any(b => b == 0);
        }
    }
}
