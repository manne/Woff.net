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
    }
}
