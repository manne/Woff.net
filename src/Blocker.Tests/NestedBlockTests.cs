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
        public void Validate_ShouldBeTrue_BecauseTheMaxPaddingIsNotExceeded()
        {
            // arrange
            var cut = new NestedBlock(0, 8, new NestedBlockOptions(3, 4));
            cut.AddChild(Block.CreateFromStartAndDistance(0, 1));
            cut.AddChild(Block.CreateFromStartAndDistance(4, 4));

            // act
            var actualResult = cut.Validate(true);

            // assert
            actualResult.Should().BeTrue();
            cut.Exceptions.Should().BeEmpty();
        }

        [Fact]
        public void Validate_ShouldBeFalse_BecauseTheLastBlockIsBehindTheContainer()
        {
            // arrange
            var cut = new NestedBlock(0, 8, new NestedBlockOptions(3, 4));
            cut.AddChild(Block.CreateFromStartAndDistance(0, 10));

            // act
            var actualResult = cut.Validate(true);

            // assert
            actualResult.Should().BeFalse();
            cut.Exceptions.Should().Contain(e => e.GetType() == typeof(BlockOverlappingException));
        }

        [Fact]
        public void Validate_ShouldBeFalse_BecauseTheLastBlockIsTooFarAwayFromTheEndOfTheContainer()
        {
            // arrange
            var cut = new NestedBlock(0, 12, new NestedBlockOptions(3, 4));
            cut.AddChild(Block.CreateFromStartAndDistance(0, 10));

            // act
            var actualResult = cut.Validate(true);

            // assert
            actualResult.Should().BeFalse();
            cut.Exceptions.Should().Contain(e => e.GetType() == typeof(BlockMaxPaddingExceededException));
        }

        [Fact]
        public void Validate_ShouldBeFalse_BecauseTheLastBlockAfterTheEndOfTheContainer()
        {
            // arrange
            var cut = new NestedBlock(0, 12, new NestedBlockOptions(3, 4));
            cut.AddChild(Block.CreateFromStartAndDistance(0, 12));
            cut.AddChild(Block.CreateFromStartAndDistance(14, 10));

            // act
            var actualResult = cut.Validate(true);

            // assert
            actualResult.Should().BeFalse();
            cut.Exceptions.Should().Contain(e => e.GetType() == typeof(BlockStartsBeyondContainerException));
        }

        [Fact]
        public void Validate_ShouldBeTrue_BecauseTheLastBlockIsOnTheEdgeOfTheEndOfTheContainer()
        {
            // arrange
            var cut = new NestedBlock(0, 12, new NestedBlockOptions(3, 4));
            cut.AddChild(Block.CreateFromStartAndDistance(0, 12));

            // act
            var actualResult = cut.Validate(true);

            // assert
            actualResult.Should().BeTrue();
            cut.Exceptions.Should().BeEmpty();
        }
    }
}
