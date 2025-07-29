using System.Windows.Forms;
using Xunit;

namespace RefactoredCalculator.Test;

public class RefactoredCalculatorUnitTests
{
    [Fact]
    public void RefactoredCalculator_Calculate_ReturnDouble()
    {
        // Arrange
        var calculator = new Calculator("0", false, false);
        string input = "3 + 5";
        double expected = 8;

        // Act
        var result = calculator.Calculate(calculator.shuntingRPNCalc(input));

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void DivideByZero_ThrowsException()
    {
        var calculator = new Calculator("0", false, false);

        Assert.Throws<Exception>(() => calculator.Calculate(calculator.shuntingRPNCalc("9 / 0")));
    }

    [Fact]
    public void Overflow_ThrowsException()
    {
        var calculator = new Calculator("0", false, false);

        Assert.Throws<Exception>(() => calculator.Calculate(calculator.shuntingRPNCalc("99999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999 * 99999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999")));
        
    }

    [Theory]
    [InlineData("3 + 5", 8)]
    [InlineData("10 - 2", 8)]
    [InlineData("6 * 7", 42)]
    [InlineData("8 / 2", 4)]
    [InlineData("10 -2 * 6/4", 7)]
    [InlineData("    10-2 * 6/4", 7)]
    [InlineData("    10-2 *      6/      4          ", 7)]
    [InlineData("1 + 2 * 3", 7)]
    [InlineData("2 * 3 + 4", 10)]
    [InlineData("5 - 6 * 7", -37)]
    [InlineData("1.15 - 6.72 *   7.34", -48.1748)]
    public void CalculateVariousExpressions_ReturnExpectedResults(string input, double expected)
    {
        // Arrange
        var calculator = new Calculator("0", false, false);
        // Act
        var result = calculator.Calculate(calculator.shuntingRPNCalc(input));
        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("2x + 2")]
    [InlineData("2e + 2")]
    [InlineData("2e1 + 2")]
    [InlineData("2^2 + 2")]
    [InlineData("2x + 2    / 1")]
    [InlineData("2e / 2 / x")]
    [InlineData("2e1 / 2 / e")]
    [InlineData("2 / 2e")]
    [InlineData("2.2.2 + 2")]
    [InlineData("2.. + 2")]
    [InlineData("2..2 + 2")]
    [InlineData("2 ++ 2")]
    [InlineData("2 ** 2")]
    [InlineData("2    .1 + 2")]
    [InlineData("2.       1 + 2")]
    [InlineData("          2.       1 + 2")]
    public void InputHasVariables_ThrowInputException(string input)
    {
        var calculator = new Calculator("0", false, false);

        Assert.Throws<Exception>(() => calculator.Calculate(calculator.shuntingRPNCalc(input)));
    }

}


