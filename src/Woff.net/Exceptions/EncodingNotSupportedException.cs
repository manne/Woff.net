﻿using System;
using System.Runtime.Serialization;

namespace WoffDotNet.Exceptions
{
    [Serializable]
    public class EncodingNotSupportedException : Exception
    {
        public EncodingNotSupportedException() : this("the encoding is not supported")
        {
        }

        public EncodingNotSupportedException(string message)
            : base(message)
        {
        }

        public EncodingNotSupportedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected EncodingNotSupportedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
