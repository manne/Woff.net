using System;
using System.Runtime.Serialization;

namespace WoffDotNet.Exceptions
{
    [Serializable]
    public class GeneralWoffException : Exception
    {
        public GeneralWoffException()
        {
        }

        public GeneralWoffException(string message)
            : base(message)
        {
        }

        public GeneralWoffException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected GeneralWoffException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
