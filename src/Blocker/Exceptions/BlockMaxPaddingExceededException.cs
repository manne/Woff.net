using System;

namespace Blocker.Exceptions
{
    public class BlockMaxPaddingExceededException : GeneralBlockException
    {
        public BlockMaxPaddingExceededException()
        {
            // empty
        }

        public BlockMaxPaddingExceededException(string message)
            : base(message)
        {
            // empty
        }

        public BlockMaxPaddingExceededException(string message, Exception innerException)
            : base(message, innerException)
        {
            // empty
        }
    }
}
