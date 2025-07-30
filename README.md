## Calculator Challenge

These projects were developed using:

\- Visual Studio 2022 v17.14.7

\- .NET 9.0.303 (WinForms)

\- xUnit v2.9.3

-----------------------

&nbsp;SIMPLE CALCULATOR

-----------------------

The simple calculator follows these rules:

\- The calculator accepts a formula that follows the rule of <operand><operator><operand>

\- If a user attempts to input multiple operators in a row, the calculator will replace the current operator with the newly entered one. For example, if the current input is '9+' and the user presses the '*' key, the input will change to '9*'.

-----------------------

&nbsp;REFACTORED CALCULATOR

-----------------------

The refactored calculator follows these rules:

\- The calculator accepts a formula that follows the rule of <operand 1><operator 1><operand 2>...<operator n><operand n>

\- If a user attempts to input multiple operators in a row, the calculator will replace the current working operator with the newly entered one, until a new operand is entered. For example, if the current input is '9+8-' and the user presses the '*' key, the input will change to '9+8*'.

-----------------------

The following assumptions were made about the acceptance criteria of the project:

\- The calculator does not support inputting scientific notation (e), exponentiation (^) or variables.

\- The calculator should support floating point numbers, and thus uses doubles as they are the standard for a majority of floating-point calculations in C# that require precision.

\- Division by zero and value overflow should return usable exceptions rather than positive or negative infinity as per IEEE 754.

\- Due to the limited nature of the GUI, the operators and operands cannot be set by the user to be an invalid input. However, these possible errors should be caught regardless.

-----------------------

The following exceptions are caught in both calculators:

\- Divide By Zero

\- Value overflow for input and output

\- Invalid operands and operators

-----------------------

How to run the projects:
1. Open the solution file 'Calculator.sln' in Visual Studio 2022
2. In the Startup Item dropdown menu located to the left of the Run option in the toolbar, choose between 'BasicCalculator' or 'RefactoredCalculator'
3. Run the selected program

How to run the tests:
1. In the toolbar under 'Test', click 'Run all tests'