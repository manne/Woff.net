using System;
using System.Diagnostics.Contracts;

using WoffDotNet.Types;
using WoffDotNet.Validators;

namespace WoffDotNet
{
    public static class WoffHelpers
    {
        public static bool HasMetadata(this WoffHeader header)
        {
            if (WoffHeaderValidator.HasIllegalMetadata(header))
            {
                return false;
            }

            return header.MetaOffset > 0;
        }

        public static bool HasPrivateDate(this WoffHeader header)
        {
            if (WoffHeaderValidator.HasIllegalPrivateData(header))
            {
                return false;
            }

            return header.PrivOffset > 0;
        }

        /// <summary>
        /// Calculates the additional needed bytes to get to the next 4 byte boundary.
        /// </summary>
        /// <param name="position"></param>
        /// <returns>The next 4 byte boundary, can be the same as <paramref name="position"/>.</returns>
        public static UInt32 Calculate4BytePadding(UInt32 position)
        {
            Contract.Ensures(Contract.Result<UInt32>() <= 3);

            UInt32 mod = position % 4;
            return (mod == 0) ? 0 : 4 - mod;
        }
    }
}
