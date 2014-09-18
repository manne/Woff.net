using System;
using System.Runtime.Serialization;

namespace WoffDotNet.Exceptions
{
    /// <summary>
    /// The invalid WOFF font table checksum exception.
    /// </summary>
    [Serializable]
    public class InvalidWoffFontTableChecksumException : GeneralWoffException
    {
        public InvalidWoffFontTableChecksumException()
        {
            // empty
        }

        public InvalidWoffFontTableChecksumException(string message)
            : base(message)
        {
            // empty
        }

        public InvalidWoffFontTableChecksumException(string message, Exception innerException)
            : base(message, innerException)
        {
            // empty
        }

        protected InvalidWoffFontTableChecksumException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            // empty
        }
    }
}
