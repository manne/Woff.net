using System;
using System.Runtime.Serialization;

namespace WoffDotNet.Exceptions
{
    [Serializable]
    public class InvalidWoffReservedValueException : GeneralWoffException
    {
        public InvalidWoffReservedValueException()
        {
        }

        public InvalidWoffReservedValueException(string message)
            : base(message)
        {
        }

        public InvalidWoffReservedValueException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected InvalidWoffReservedValueException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
