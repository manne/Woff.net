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
        public void Valid_001()
        {
            // arrange
            var cut = GetReader(Resources.valid_001);

            // act
            Action action = cut.Process;

            // assert
            action();
        }

        [Fact]
        public void Valid_002()
        {
            // arrange
            var cut = GetReader(Resources.valid_002);

            // act
            Action action = cut.Process;

            // assert
            action();
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
        public void Invalid_Header_Signature_001()
        {
            // arrange
            var cut = GetReader(Resources.header_signature_001);

            // act
            Action action = cut.Process;

            // assert
            action.ShouldThrow<InvalidWoffMagicNumberException>();
        }
    }
}
