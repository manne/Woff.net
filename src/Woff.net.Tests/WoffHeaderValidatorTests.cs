using FluentAssertions;

using WoffDotNet.Types;
using WoffDotNet.Validators;

using Xunit;

namespace WoffDotNet.Tests
{
    public class WoffHeaderValidatorTests
    {
        [Fact]
        public void HasIllegalMetadata_Should_BeTrue_BecauseItHasAnOffsetAndNoLength()
        {
            // arrange
            uint offset = 200;
            uint length = 0;
            var header = new WoffHeader(0, 0, 0, 0, 0, 0, 0, 0, offset, length, 0, 0, 0);

            // act
            var actualResult = WoffHeaderValidator.HasIllegalMetadata(header);

            // assert
            actualResult.Should().BeTrue();
        }

        [Fact]
        public void HasIllegalMetadata_Should_BeTrue_BecauseItHasNoOffsetAndALength()
        {
            // arrange
            uint offset = 0;
            uint length = 200;
            var header = new WoffHeader(0, 0, 0, 0, 0, 0, 0, 0, offset, length, 0, 0, 0);

            // act
            var actualResult = WoffHeaderValidator.HasIllegalMetadata(header);

            // assert
            actualResult.Should().BeTrue();
        }

        [Fact]
        public void HasIllegalMetadata_Should_BeFalse_BecauseItHasAnOffsetAndALength()
        {
            // arrange
            uint offset = 200;
            uint length = 200;
            var header = new WoffHeader(0, 0, 0, 0, 0, 0, 0, 0, offset, length, 0, 0, 0);

            // act
            var actualResult = WoffHeaderValidator.HasIllegalMetadata(header);

            // assert
            actualResult.Should().BeFalse();
        }

        [Fact]
        public void HasIllegalMetadata_Should_BeFalse_BecauseItHasNoOffsetAndNoLength()
        {
            // arrange
            uint offset = 0;
            uint length = 0;
            var header = new WoffHeader(0, 0, 0, 0, 0, 0, 0, 0, offset, length, 0, 0, 0);

            // act
            var actualResult = WoffHeaderValidator.HasIllegalMetadata(header);

            // assert
            actualResult.Should().BeFalse();
        }

        [Fact]
        public void HasIllegalPrivateData_Should_BeTrue_BecauseItHasAnOffsetAndNoLength()
        {
            // arrange
            uint offset = 200;
            uint length = 0;
            var header = new WoffHeader(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, offset, length);

            // act
            var actualResult = WoffHeaderValidator.HasIllegalPrivateData(header);

            // assert
            actualResult.Should().BeTrue();
        }

        [Fact]
        public void HasIllegalPrivateData_Should_BeTrue_BecauseItHasNoOffsetAndALength()
        {
            // arrange
            uint offset = 0;
            uint length = 200;
            var header = new WoffHeader(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, offset, length);

            // act
            var actualResult = WoffHeaderValidator.HasIllegalPrivateData(header);

            // assert
            actualResult.Should().BeTrue();
        }

        [Fact]
        public void HasIllegalPrivateData_Should_BeFalse_BecauseItHasAnOffsetAndALength()
        {
            // arrange
            uint offset = 200;
            uint length = 200;
            var header = new WoffHeader(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, offset, length);

            // act
            var actualResult = WoffHeaderValidator.HasIllegalPrivateData(header);

            // assert
            actualResult.Should().BeFalse();
        }

        [Fact]
        public void HasIllegalPrivateData_Should_BeFalse_BecauseItHasNoOffsetAndNoLength()
        {
            // arrange
            uint offset = 0;
            uint length = 0;
            var header = new WoffHeader(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, offset, length);

            // act
            var actualResult = WoffHeaderValidator.HasIllegalPrivateData(header);

            // assert
            actualResult.Should().BeFalse();
        }
    }
}
