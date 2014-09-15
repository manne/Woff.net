using System;
using System.Runtime.Serialization;

namespace WoffDotNet.Exceptions
{
    public class InvalidNullPaddingException : GeneralWoffException
    {
        public InvalidNullPaddingException()
        {
        }

        public InvalidNullPaddingException(string message)
            : base(message)
        {
        }

        public InvalidNullPaddingException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected InvalidNullPaddingException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
