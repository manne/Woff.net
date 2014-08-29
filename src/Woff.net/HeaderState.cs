namespace WoffDotNet
{
    public class HeaderState
    {
        public HeaderState(bool metadataIsValid, bool privateDataIsValid)
        {
            MetadataIsValid = metadataIsValid;
            PrivateDataIsValid = privateDataIsValid;
        }

        public bool MetadataIsValid { get; private set; }
        public bool PrivateDataIsValid { get; private set; }
    }
}
