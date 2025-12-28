using DeuxCentsCardGame.Services;

namespace DeuxCentsCardGame.Tests.Services
{
    public class RandomServiceTests
    {
        private readonly RandomService _randomService;

        public RandomServiceTests()
        {
            _randomService = new RandomService();
        }

        [Fact]
        public void Next_ReturnsValueInRange()
        {
            // Arrange
            int minValue = 1;
            int maxValue = 10;

            // Act
            var result = _randomService.Next(minValue, maxValue);

            // Assert
            Assert.InRange(result, minValue, maxValue - 1);
        }

        [Fact]
        public void Next_ReturnsValueGreaterThanOrEqualToMin()
        {
            // Arrange
            int minValue = 50;
            int maxValue = 100;

            // Act
            var result = _randomService.Next(minValue, maxValue);

            // Assert
            Assert.True(result >= minValue);
        }

        [Fact]
        public void Next_ReturnsValueLessThanMax()
        {
            // Arrange
            int minValue = 50;
            int maxValue = 100;

            // Act
            var result = _randomService.Next(minValue, maxValue);

            // Assert
            Assert.True(result < maxValue);
        }

        [Fact]
        public void Next_WithSameMinMax_ReturnsMin()
        {
            // Arrange
            int value = 5;

            // Act
            var result = _randomService.Next(value, value);

            // Assert
            Assert.Equal(value, result);
        }

        [Fact]
        public void Next_GeneratesDifferentValues_OverMultipleCalls()
        {
            // Arrange
            var results = new HashSet<int>();
            int attempts = 100;

            // Act
            for (int i = 0; i < attempts; i++)
            {
                results.Add(_randomService.Next(0, 1000));
            }

            // Assert - Should have generated multiple different values
            Assert.True(results.Count > 1, "Random service should generate different values");
        }

        [Theory]
        [InlineData(0, 2)]
        [InlineData(1, 10)]
        [InlineData(50, 100)]
        [InlineData(-10, 10)]
        public void Next_WorksWithVariousRanges(int min, int max)
        {
            // Act
            var result = _randomService.Next(min, max);

            // Assert
            Assert.InRange(result, min, max - 1);
        }
    }
}