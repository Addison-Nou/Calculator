namespace BasicCalculator.Test
{
    public class BasicCalculatorUnitTests
    {

        [Fact]
        public void BasicCalculator_DivideByZero_ThrowsException()
        {
            var calculator = new Calculator("0", "0", "+");

            Assert.Throws<Exception>(() => calculator.calculate("9", "0", "/"));
        }

        [Fact]
        public void BasicCalculator_Overflow_ThrowsException()
        {
            var calculator = new Calculator("0", "0", "+");

            Assert.Throws<Exception>(() => calculator.calculate("99999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999", "99999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999", "*"));
        }

        [Theory]
        [InlineData("9", "1", " *")]
        [InlineData("9", "1", "**")]
        [InlineData("4", "-7", "- ")]
        [InlineData("8", "8", "  *  ")]
        [InlineData("12", "3", "7")]
        [InlineData("5", "1", "e")]
        [InlineData("3", "9", "x")]
        [InlineData("3", "9", "xx")]
        public void InvalidOperatorProvided_ReturnException(string left, string right, string op)
        {
            var calculator = new Calculator("0", "0", "+");

            Assert.Throws<Exception>(() => calculator.calculate(left, right, op));
        }

        [Theory]
        [InlineData("9", "1", "+", 10)]
        [InlineData("4", "-7", "-", 11)]
        [InlineData("8", "8", "*", 64)]
        [InlineData("12", "3", "/", 4)]
        [InlineData("5", "1", "+", 6)]
        [InlineData("3", "9", "*", 27)]
        public void CalculateVariousExpressions_ReturnExpectedResults(string left, string right, string op, double expected)
        {
            // Arrange
            var calculator = new Calculator("0", "0", "+");
            // Act
            var result = calculator.calculate(left, right, op);
            // Assert
            Assert.Equal(expected, result);
        }
    }
}
