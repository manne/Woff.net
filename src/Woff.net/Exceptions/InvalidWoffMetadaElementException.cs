using System;
using System.Runtime.Serialization;

namespace WoffDotNet.Exceptions
{
    [Serializable]
    public class InvalidWoffMetadaElementException : Exception
    {
        public InvalidWoffMetadaElementException()
        {
        }

        public InvalidWoffMetadaElementException(string message)
            : base(message)
        {
        }

        public InvalidWoffMetadaElementException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected InvalidWoffMetadaElementException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
