using Blocker.Exceptions;

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

        [Fact]
        public void Validate_ShouldBeFalse_BecauseTheMaxPaddingIsExceeded()
        {
            // arrange
            var cut = new NestedBlock(0, 20, new NestedBlockOptions(3, 4));
            cut.AddChild(Block.CreateFromStartAndDistance(0, 4));
            cut.AddChild(Block.CreateFromStartAndDistance(8, 4));

            // act
            var actualResult = cut.Validate(true);

            // assert
            actualResult.Should().BeFalse();
            cut.Exceptions.Should().ContainItemsAssignableTo<BlockMaxPaddingExceededException>();
        }

        [Fact]
        public void Validate_ShouldBetrue_BecauseTheMaxPaddingIsNotExceeded()
        {
            // arrange
            var cut = new NestedBlock(0, 20, new NestedBlockOptions(3, 4));
            cut.AddChild(Block.CreateFromStartAndDistance(0, 1));
            cut.AddChild(Block.CreateFromStartAndDistance(4, 4));

            // act
            var actualResult = cut.Validate(true);

            // assert
            actualResult.Should().BeTrue();
            cut.Exceptions.Should().BeEmpty();
        }
    }
}
