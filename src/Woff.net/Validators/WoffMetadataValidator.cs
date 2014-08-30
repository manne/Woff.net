using System;
using System.Xml;

namespace WoffDotNet.Validators
{
    public static class WoffMetadataValidator
    {
        public static AggregateException ValidateWoffMetadata(this XmlDocument document)
        {
           // if()
            return null;
        }

        public static bool ValidateLengths(uint statedLength, uint actualLength)
        {
            return statedLength == actualLength;
        }
    }
}
