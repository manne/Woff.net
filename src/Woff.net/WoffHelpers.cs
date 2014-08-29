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
    }
}
