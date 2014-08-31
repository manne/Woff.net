using FluentAssertions;

using Xunit;

namespace Blocker.Tests
{
    public class NestedBlockTests
    {
        [Fact]
        public void Validate_Should()
        {
            // arrange
            var cut = new NestedBlock(0, 20, new NestedBlockOptions(3, 4));
            cut.AddChild(Block.CreateFromStartAndDistance(0, 4));

            // act
            var actualResult = cut.Validate();

            // assert
            actualResult.Should().BeTrue();
        }

        [Fact]
        public void Validate_Should2()
        {
            // arrange
            var cut = new NestedBlock(0, 20, new NestedBlockOptions(3, 4));
            cut.AddChild(Block.CreateFromStartAndDistance(2, 4));

            // act
            var actualResult = cut.Validate();

            // assert
            actualResult.Should().BeFalse();
        }
    }
}
