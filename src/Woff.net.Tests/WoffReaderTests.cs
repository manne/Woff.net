using System;
using System.IO;

using FluentAssertions;

using WoffDotNet.Tests.Properties;

using Xunit;

namespace WoffDotNet.Tests
{
    public class WoffReaderTests
    {
        [Fact]
        public void Read_Should_ThrowException_BecauseStreamIsTooSmall()
        {
            // arrange
            var binaryReader = new BinaryReader(new MemoryStream(new byte[10]));
            var cut = new WoffReader(binaryReader);

            // act
            Action act = cut.Process;

            // assert
            act.ShouldThrow<EndOfStreamException>();
        }
        [Fact]
        public void Read_Should_ReadHeader()
        {
            // arrange
            var binaryReader = new BinaryReader(new MemoryStream(Resources.ValidHeaderOnly));
            var cut = new WoffReader(binaryReader);

            // act
            cut.Process();

            // assert
            cut.Header.Should().NotBeNull();
        }
    }
}
