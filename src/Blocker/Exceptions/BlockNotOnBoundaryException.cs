using System;

namespace Blocker.Exceptions
{
    public class BlockNotOnBoundaryException : GeneralBlockException
    {
        public BlockNotOnBoundaryException()
        {
            // empty
        }

        public BlockNotOnBoundaryException(string message)
            : base(message)
        {
            // empty
        }

        public BlockNotOnBoundaryException(string message, Exception innerException)
            : base(message, innerException)
        {
            // empty
        }
    }
}
