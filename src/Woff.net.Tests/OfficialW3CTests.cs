using System;
using System.IO;

using FluentAssertions;

using WoffDotNet.Exceptions;
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
        public void Valid_001_NoMetadata()
        {
            // arrange
            var cut = GetReader(Resources.valid_001);

            // act
            cut.Process();

            // assert
            cut.Metadata.Should().BeNull();
        }

        [Fact]
        public void Valid_001_NoPrivateData()
        {
            // arrange
            var cut = GetReader(Resources.valid_001);

            // act
            cut.Process();

            // assert
            cut.PrivateData.Should().BeNull();
        }

        [Fact]
        public void Valid_002()
        {
            // arrange
            var cut = GetReader(Resources.valid_002);

            // act
            cut.Process();

            // assert
            cut.Metadata.Should().NotBeNull();
        }

        [Fact]
        public void Valid_003()
        {
            // arrange
            var cut = GetReader(Resources.valid_003);

            // act
            cut.Process();

            // assert
            cut.PrivateData.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void Valid_004_WithMetadata()
        {
            // arrange
            var cut = GetReader(Resources.valid_004);

            // act
            cut.Process();

            // assert
            cut.Metadata.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void Valid_004_WithPrivateData()
        {
            // arrange
            var cut = GetReader(Resources.valid_004);

            // act
            cut.Process();

            // assert
            cut.PrivateData.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void Invalid_Header_Signature_001()
        {
            // arrange
            var cut = GetReader(Resources.header_signature_001);

            // act
            Action action = cut.Process;

            // assert
            action.ShouldThrow<InvalidWoffMagicNumberException>();
        }

        [Fact]
        public void Invalid_Metadata_Compression_001()
        {
            // arrange
            var cut = GetReader(Resources.metadata_compression_001);

            // act
            Action action = cut.Process;

            // assert
            action.ShouldThrow<WoffUncompressException>();
        }
    }
}
