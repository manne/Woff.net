using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Text;
using System.Xml;

using WoffDotNet.Types;
using WoffDotNet.Validators;

namespace WoffDotNet
{
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

            UInt32 mod = position % 4;
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

        public static Encoding GetEncoding(this byte[] bytes)
        {
            using (var reader = new StreamReader(new MemoryStream(bytes), true))
            {
                reader.Read();
                var currentEncoding = reader.CurrentEncoding;
                return currentEncoding;
            }
        }
    }
}
