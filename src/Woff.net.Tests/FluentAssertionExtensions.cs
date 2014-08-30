using System;
using System.Linq;

namespace WoffDotNet.Tests
{
    public static class FluentAssertionExtensions
    {
        public static void ShouldHaveCorrectlyAssignedMetadata(this WoffReader reader)
        {
            bool isCorrectlyAssigned = reader.Metadata != null;
            if (isCorrectlyAssigned)
            {
                if (reader.MetadataExceptions != null && reader.MetadataExceptions.Any())
                {
                    isCorrectlyAssigned = false;
                }
            }

            if (!isCorrectlyAssigned)
            {
                throw new Exception("The reader should have metadata and no exception for processing the metadata");
            }
        }
    }
}
