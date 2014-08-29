using System;
using System.Runtime.Serialization;

namespace WoffDotNet.Exceptions
{
    public class InvalidWoffMagicNumberException : GeneralWoffException
    {
        public InvalidWoffMagicNumberException()
        {
        }

        public InvalidWoffMagicNumberException(string message)
            : base(message)
        {
        }

        public InvalidWoffMagicNumberException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected InvalidWoffMagicNumberException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
