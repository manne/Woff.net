using FluentAssertions;

using Xunit;

namespace Blocker.Tests
{
    public class BlockOverlappingTests
    {
        [Fact]
        public void ShouldOverlapBecauseTheSecondBlockIsOnTheStart()
        {
            // arrange
            var block = new Block(40, 100);

            // act
            var actualResult = block.IsOverlapping(new Block(100, 120));

            // assert
            actualResult.Should().BeFalse();
        }

        [Fact]
        public void ShouldOverlapBecauseTheSecondBlockIsInside()
        {
            // arrange
            var block = new Block(40, 100);

            // act
            var actualResult = block.IsOverlapping(new Block(50, 80));

            // assert
            actualResult.Should().BeTrue();
        }

        [Fact]
        public void ShouldOverlapBecauseTheSecondBlocksOnTheRight()
        {
            // arrange
            var block = new Block(40, 100);

            // act
            var actualResult = block.IsOverlapping(new Block(80, 120));

            // assert
            actualResult.Should().BeTrue();
        }

        [Fact]
        public void ShouldOverlapBecauseTheSecondBlockIsOnTheLeft()
        {
            // arrange
            var block = new Block(80, 120);

            // act
            var actualResult = block.IsOverlapping(new Block(40, 100));

            // assert
            actualResult.Should().BeTrue();
        }


        [Fact]
        public void ShouldNotOverlapBecauseTheMainBlockIsOnTheRight()
        {
            // arrange
            var block = new Block(80, 120);

            // act
            var actualResult = block.IsOverlapping(new Block(0, 40));

            // assert
            actualResult.Should().BeFalse();
        }

        [Fact]
        public void ShouldNotOverlapBecauseTheMainBlockIsOnTheLeft()
        {
            // arrange
            var block = new Block(0, 40);

            // act
            var actualResult = block.IsOverlapping(new Block(80, 120));

            // assert
            actualResult.Should().BeFalse();
        }

        [Fact]
        public void ShouldOverlapBecauseTheSecondBlockIsOutside()
        {
            // arrange
            var block = new Block(80, 100);

            // act
            var actualResult = block.IsOverlapping(new Block(0, 120));

            // assert
            actualResult.Should().BeTrue();
        }

        [Fact]
        public void ShouldNotOverlapBecauseTheSecondBlockIsOnTheLeft()
        {
            // arrange
            var block = new Block(0, 40);

            // act
            var actualResult = block.IsOverlapping(new Block(80, 120));

            // assert
            actualResult.Should().BeFalse();
        }

        [Fact]
        public void ShouldNotOverlapBecauseThe()
        {
            // arrange
            var block = new Block(0, 40);

            // act
            var actualResult = block.IsOverlapping(new Block(40, 120));

            // assert
            actualResult.Should().BeFalse();
        }
    }
}
