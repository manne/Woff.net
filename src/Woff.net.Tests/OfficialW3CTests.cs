using System;
using System.IO;

using WoffDotNet.Tests.Properties;

using Xunit;

namespace WoffDotNet.Tests
{
    public class OfficialW3CTests
    {
        private static WoffReader GetReader(byte[] bytes)
        {
            return new WoffReader(new BinaryReader(new MemoryStream(bytes)));
        }


        [Fact]
        public void Valid_001()
        {
            // arrange
            var cut = GetReader(Resources.valid_001);

            // act
            Action action = cut.Process;

            // assert
            action();
        }
    }
}
