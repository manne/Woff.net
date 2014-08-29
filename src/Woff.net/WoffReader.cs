using System.IO;

namespace WoffDotNet
{
    public class WoffReader
    {
        private readonly BinaryReader _binaryReader;

        public WoffReader(BinaryReader binaryReader)
        {
            _binaryReader = binaryReader;
        }
    }
}
