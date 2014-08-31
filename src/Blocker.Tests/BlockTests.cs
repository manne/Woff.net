using FluentAssertions;

using Xunit.Extensions;

namespace Blocker.Tests
{
    public class BlockTests
    {
        [Theory]
        [InlineData((uint)0, (uint)2)]
        [InlineData((uint)10, (uint)2)]
        public void ShouldHaveTheCorrectStartAndEndFromStartAndDistance(uint start, uint distance)
        {
            // arrange
            var expectedResult = new Block(start, start + distance);

            // act
            var actualResult = Block.CreateFromStartAndDistance(start, distance);

            // assert
            actualResult.Should().Be(expectedResult);
        }
    }
}
