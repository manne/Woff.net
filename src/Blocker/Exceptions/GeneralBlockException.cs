using System;

namespace Blocker.Exceptions
{
    public class GeneralBlockException : Exception
    {
        public GeneralBlockException()
        {
            // empty
        }

        public GeneralBlockException(string message)
            : base(message)
        {
            // empty
        }

        public GeneralBlockException(string message, Exception innerException)
            : base(message, innerException)
        {
            // empty
        }
    }
}
