using System;
using System.Runtime.Serialization;

namespace WoffDotNet.Exceptions
{
    [Serializable]
    public class InvalidWoffTotalSfntSizeException : GeneralWoffException
    {
        public InvalidWoffTotalSfntSizeException()
        {
        }

        public InvalidWoffTotalSfntSizeException(string message)
            : base(message)
        {
        }

        public InvalidWoffTotalSfntSizeException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected InvalidWoffTotalSfntSizeException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
