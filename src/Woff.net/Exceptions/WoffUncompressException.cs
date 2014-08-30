using System;
using System.Runtime.Serialization;

namespace WoffDotNet.Exceptions
{
    [Serializable]
    public class WoffUncompressException : GeneralWoffException
    {
        public WoffUncompressException()
        {
        }

        public WoffUncompressException(string message)
            : base(message)
        {
        }

        public WoffUncompressException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected WoffUncompressException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
