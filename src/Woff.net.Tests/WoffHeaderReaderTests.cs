using System;
using System.IO;

using FluentAssertions;

using WoffDotNet.Exceptions;
using WoffDotNet.Readers;
using WoffDotNet.Tests.Properties;
using WoffDotNet.Types;

using Xunit;

namespace WoffDotNet.Tests
{
    public class WoffHeaderReaderTests
    {
        private byte[] GetHeader(byte[] bytes)
        {
            var result = new byte[WoffHeader.Size];
            Array.Copy(bytes, result, result.Length);
            return result;
        }

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
        public void Read_Should_ThrowException_BecauseTheMagicNumerIsIncorrect()
        {
            // arrange
            var cut = new WoffHeaderReader(GetHeader(Resources.InvalidHeader_WrongMagicNumber));

            // act
            Action act = cut.Process;

            // assert
            act.ShouldThrow<InvalidWoffMagicNumberException>();
        }

        [Fact]
        public void Read_Should_ThrowException_BecauseTheReservedValueIsNotZero()
        {
            // arrange
            var cut = new WoffHeaderReader(GetHeader(Resources.InvalidHeader_WrongReservedValue));

            // act
            Action act = cut.Process;

            // assert
            act.ShouldThrow<InvalidWoffReservedValueException>();
        }

        [Fact]
        public void Read_Should_ThrowException_BecauseThSfntSizeValueIsNotDividableByFour()
        {
            // arrange
            var cut = new WoffHeaderReader(GetHeader(Resources.InvalidHeader_WrongSfntSizeValue));

            // act
            Action act = cut.Process;

            // assert
            act.ShouldThrow<InvalidWoffTotalSfntSizeException>();
        }

        [Fact]
        public void Read_Should_ReadHeader()
        {
            // arrange
            var cut = new WoffHeaderReader(GetHeader(Resources.ValidHeaderOnly));

            // act
            cut.Process();

            // assert
            cut.Header.Should().NotBeNull();
        }
    }
}
