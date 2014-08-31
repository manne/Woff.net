using System;

namespace Blocker.Exceptions
{
    public class BlockOverlappingException : Exception
    {
        public BlockOverlappingException()
        {
            // empty
        }

        public BlockOverlappingException(string message)
            : base(message)
        {
            // empty
        }

        public BlockOverlappingException(string message, Exception innerException)
            : base(message, innerException)
        {
            // empty
        }
    }
}
