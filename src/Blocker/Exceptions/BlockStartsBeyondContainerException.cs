using System;

namespace Blocker.Exceptions
{
    public class BlockStartsBeyondContainerException : Exception
    {
        public BlockStartsBeyondContainerException()
        {
            // empty
        }

        public BlockStartsBeyondContainerException(string message)
            : base(message)
        {
            // empty
        }

        public BlockStartsBeyondContainerException(string message, Exception innerException)
            : base(message, innerException)
        {
            // empty
        }
    }
}
