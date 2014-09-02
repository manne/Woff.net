using System.Diagnostics.Contracts;

namespace Blocker
{
    public static class BlockExtensions
    {
        public static bool IsOverlapping(this Block first, Block second)
        {
            Contract.Requires(first != null);
            Contract.Requires(second != null);

            var result = first.Start.IsBetween(second.Start, second.End);
            if (result)
            {
                return true;
            }

            result = first.End.IsBetween(second.Start, second.End);
            if (result)
            {
                return true;
            }

            result = second.Start.IsBetween(first.Start, first.End);
            if (result)
            {
                return true;
            }

            result = second.End.IsBetween(first.Start, first.End);
            if (result)
            {
                return true;
            }

            return false;
        }

        private static bool IsBetween(this uint val, uint low, uint high)
        {
               return val > low && val < high;
        }

        public static uint CalculateNextBytePadding(uint position, uint boundary)
        {
            Contract.Ensures(Contract.Result<uint>() <= boundary);

            uint mod = position % boundary;
            return (mod == 0) ? 0 : boundary - mod;
        }
    }
}
