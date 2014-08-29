﻿using WoffDotNet.Types;

namespace WoffDotNet.Validators
{
    public static class WoffHeaderValidator
    {
        /// <summary>
        /// Checks if extended metadata statements in the <paramref name="header"/> are illegal or not.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <returns><c>true</c> if the extended metadata statements are illegal, otherwise <c>false</c>.</returns>
        public static bool HasIllegalMetadata(WoffHeader header)
        {
            return (header.MetaOffset == 0 && header.MetaLength != 0) || (header.MetaLength == 0 && header.MetaOffset != 0);
        }

        /// <summary>
        /// Checks if privatedata statements in the <paramref name="header"/> are illegal or not.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <returns><c>true</c> if the privatedata statements are illegal, otherwise <c>false</c>.</returns>
        public static bool HasIllegalPrivateData(WoffHeader header)
        {
            return (header.PrivOffset == 0 && header.PrivLength != 0) || (header.PrivLength == 0 && header.PrivOffset != 0);
        }
    }
}
