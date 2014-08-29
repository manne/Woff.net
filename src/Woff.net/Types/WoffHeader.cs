using System;
using System.Collections.Generic;

namespace WoffDotNet.Types
{

    /// <summary>
    /// File header with basic font type and version, along with offsets to metadata and private data blocks.
    /// </summary>
    public sealed class WoffHeader : IEquatable<WoffHeader>, ICloneable
    {
        public const uint Size = 44;

        /// <summary>
        /// The magic number for WOFF files.
        /// </summary>
        public const UInt32 MagicNumberUInt = 0x774F4646;

        /// <summary>
        /// Creates a new instance of <see cref="WoffHeader"/>.
        /// </summary>
        /// <param name="sig">Signature, must be <c>0x774F4646</c>.</param>
        /// <param name="flav">The "sfnt version" of the input font.</param>
        /// <param name="len">Total size of the WOFF file.</param>
        /// <param name="numTables">Number of entries in directory of font tables.</param>
        /// <param name="reserved">Reserved; set to zero.</param>
        /// <param name="totalSfntSize">Total size needed for the uncompressed font data, including the sfnt header, directory, and font tables (including padding).</param>
        /// <param name="majorVersion">Major version of the WOFF file.</param>
        /// <param name="minorVersion">Minor version of the WOFF file.</param>
        /// <param name="metaOffset">Offset to metadata block, from beginning of WOFF file.</param>
        /// <param name="metaLength">Length of compressed metadata block.</param>
        /// <param name="metaOrigLength">Uncompressed size of metadata block.</param>
        /// <param name="privOffset">Offset to private data block, from beginning of WOFF file.</param>
        /// <param name="privLength">Length of private data block.</param>
        public WoffHeader(UInt32 sig, UInt32 flav, UInt32 len, UInt16 numTables, UInt16 reserved, UInt32 totalSfntSize,
            UInt32 majorVersion, UInt32 minorVersion, UInt32 metaOffset, UInt32 metaLength, UInt32 metaOrigLength,
            UInt32 privOffset, UInt32 privLength)
        {
            Signature = sig;
            Flavor = flav;
            Length = len;
            NumTables = numTables;
            Reserved = reserved;
            TotalSfntSize = totalSfntSize;
            MajorVersion = majorVersion;
            MinorVersion = minorVersion;
            MetaOffset = metaOffset;
            MetaLength = metaLength;
            MetaOrigLength = metaOrigLength;
            PrivOffset = privOffset;
            PrivLength = privLength;
        }

        /// <summary>
        /// The magic number of WOFF.
        /// </summary>
        public static readonly byte[] MagicNumber = { 0x77, 0x4F, 0x46, 0x46 };

        #region WoffHeaderEqualityComparer

        private sealed class WoffHeaderEqualityComparer : IEqualityComparer<WoffHeader>
        {
            public bool Equals(WoffHeader x, WoffHeader y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.Signature == y.Signature && x.Flavor == y.Flavor && x.Length == y.Length && x.NumTables == y.NumTables && x.Reserved == y.Reserved && x.TotalSfntSize == y.TotalSfntSize && x.MajorVersion == y.MajorVersion && x.MinorVersion == y.MinorVersion && x.MetaOffset == y.MetaOffset && x.MetaLength == y.MetaLength && x.MetaOrigLength == y.MetaOrigLength && x.PrivOffset == y.PrivOffset && x.PrivLength == y.PrivLength;
            }

            public int GetHashCode(WoffHeader obj)
            {
                unchecked
                {
                    int hashCode = (int)obj.Signature;
                    hashCode = (hashCode * 397) ^ (int)obj.Flavor;
                    hashCode = (hashCode * 397) ^ (int)obj.Length;
                    hashCode = (hashCode * 397) ^ obj.NumTables.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.Reserved.GetHashCode();
                    hashCode = (hashCode * 397) ^ (int)obj.TotalSfntSize;
                    hashCode = (hashCode * 397) ^ (int)obj.MajorVersion;
                    hashCode = (hashCode * 397) ^ (int)obj.MinorVersion;
                    hashCode = (hashCode * 397) ^ (int)obj.MetaOffset;
                    hashCode = (hashCode * 397) ^ (int)obj.MetaLength;
                    hashCode = (hashCode * 397) ^ (int)obj.MetaOrigLength;
                    hashCode = (hashCode * 397) ^ (int)obj.PrivOffset;
                    hashCode = (hashCode * 397) ^ (int)obj.PrivLength;
                    return hashCode;
                }
            }
        }

        private static readonly IEqualityComparer<WoffHeader> WoffHeaderComparerInstance = new WoffHeaderEqualityComparer();

        public static IEqualityComparer<WoffHeader> WoffHeaderComparer
        {
            get { return WoffHeaderComparerInstance; }
        }

        #endregion

        #region Equality members

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(WoffHeader other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Signature == other.Signature && Flavor == other.Flavor && Length == other.Length && NumTables == other.NumTables && Reserved == other.Reserved && TotalSfntSize == other.TotalSfntSize && MajorVersion == other.MajorVersion && MinorVersion == other.MinorVersion && MetaOffset == other.MetaOffset && MetaLength == other.MetaLength && MetaOrigLength == other.MetaOrigLength && PrivOffset == other.PrivOffset && PrivLength == other.PrivLength;
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified object  is equal to the current object; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current object. </param><filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(WoffHeader)) return false;
            return Equals((WoffHeader)obj);
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (int)Signature;
                hashCode = (hashCode * 397) ^ (int)Flavor;
                hashCode = (hashCode * 397) ^ (int)Length;
                hashCode = (hashCode * 397) ^ NumTables.GetHashCode();
                hashCode = (hashCode * 397) ^ Reserved.GetHashCode();
                hashCode = (hashCode * 397) ^ (int)TotalSfntSize;
                hashCode = (hashCode * 397) ^ (int)MajorVersion;
                hashCode = (hashCode * 397) ^ (int)MinorVersion;
                hashCode = (hashCode * 397) ^ (int)MetaOffset;
                hashCode = (hashCode * 397) ^ (int)MetaLength;
                hashCode = (hashCode * 397) ^ (int)MetaOrigLength;
                hashCode = (hashCode * 397) ^ (int)PrivOffset;
                hashCode = (hashCode * 397) ^ (int)PrivLength;
                return hashCode;
            }
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public object Clone()
        {
            return new WoffHeader(Signature, Flavor, Length, NumTables, Reserved, TotalSfntSize, MajorVersion, MinorVersion, MetaOffset, MetaLength, MetaOrigLength, PrivOffset, PrivLength);
        }

        public static bool operator ==(WoffHeader left, WoffHeader right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(WoffHeader left, WoffHeader right)
        {
            return !Equals(left, right);
        }

        #endregion

        /// <summary>
        /// Must be 0x774F4646 ('wOFF').
        /// </summary>
        public UInt32 Signature { get; private set; }

        /// <summary>
        /// The "sfnt version" of the input font.
        /// </summary>
        public UInt32 Flavor { get; private set; }

        /// <summary>
        /// Total size of the WOFF file.
        /// </summary>
        public UInt32 Length { get; private set; }

        /// <summary>
        /// Number of entries in directory of font tables.
        /// </summary>
        public UInt16 NumTables { get; private set; }

        /// <summary>
        /// Reserved; set to zero.
        /// </summary>
        public UInt16 Reserved { get; private set; }

        /// <summary>
        /// Total size needed for the uncompressed font data, including the sfnt header, directory, and font tables (including padding).
        /// </summary>
        public UInt32 TotalSfntSize { get; private set; }

        /// <summary>
        /// Major version of the WOFF file.
        /// </summary>
        public UInt32 MajorVersion { get; private set; }

        /// <summary>
        /// Minor version of the WOFF file.
        /// </summary>
        public UInt32 MinorVersion { get; private set; }

        /// <summary>
        /// Offset to metadata block, from beginning of WOFF file.
        /// </summary>
        public UInt32 MetaOffset { get; private set; }

        /// <summary>
        /// Length of compressed metadata block.
        /// </summary>
        public UInt32 MetaLength { get; private set; }

        /// <summary>
        /// Uncompressed size of metadata block.
        /// </summary>
        public UInt32 MetaOrigLength { get; private set; }

        /// <summary>
        /// Offset to private data block, from beginning of WOFF file.
        /// </summary>
        public UInt32 PrivOffset { get; private set; }

        /// <summary>
        /// Length of private data block.
        /// </summary>
        public UInt32 PrivLength { get; private set; }
    }
}
