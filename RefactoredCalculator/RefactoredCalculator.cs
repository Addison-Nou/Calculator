using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RefactoredCalculator
{

    public partial class Calculator : Form
    {
        private string input;
        private bool decimalSet; // If the decimal button has been set for the current number
        private bool operatorSet; // If the operator has been set for the current operand
        private List<Tuple<string, string>> calculationHistory = new List<Tuple<string, string>>(); // List of tuples to hold the equation history

        public Calculator(string input, bool decimalSet, bool operatorSet)
        {
            InitializeComponent();
            this.input = input;
            this.decimalSet = false;
            this.operatorSet = false;
        }

        // Shunting Yard RPN algorithm to process user input in a way that allows for operator precedence calculation
        public List<string> shuntingRPNCalc(string input)
        {

            // Regex to validate the input - unnecessary due to the limited UI but I've decided to include it for safety
            string regex = @"^\s*-?(?:\d+(\.\d*)?|\.\d+)(\s*[\+\-\*/]\s*-?(?:\d+(\.\d*)?|\.\d+))*\s*$";

            // Regex breakdown:
            // ^                                            beginning line assertion  
            // \s*                                          allows leading whitespaces
            // -?                                           allows leading negative sign
            // (?:\d+(\.\d*)?|\.\d+)                        allows for an integer or a decimal i.e. 5, 5.0, .5
            // (\s*[\+\-\*/]\s*-?(?:\d+(\.\d*)?|\.\d+))     allows for an operator followed by another number (allowing negative numbers), with optional whitespace around the operator
            // *\s*$                                        allows for optional ending whitespace and asserts the end of the line

            if (!Regex.IsMatch(input, regex))
            {
                throw new Exception("Invalid input format");
            }

            // If the input is valid, trim any whitespace and apply Shunting Yard Algorithm

            input = input.Replace(" ", ""); // Remove all whitespace from input for easier processing
            string token = "";
            Stack<string> operator_stack = new Stack<string>(); // Stack for operators
            List<string> output_queue = new List<string>(); // Final output queue for calculation
            Dictionary<string, int> operator_precedence = new Dictionary<string, int> // Dictionary of operator precedence
            {
                { "+", 1 },
                { "-", 1 },
                { "*", 2 },
                { "/", 2 }
            };

            // Iterate through the input for tokens to process
            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];

                // If the character is part of a number then keep going
                if (char.IsDigit(c) || c == '.')
                {
                    token += c;
                    continue;
                }

                if (string.IsNullOrEmpty(token)) { continue; } // If the current token is empty, skip

                // If current token is not empty (indicating that the previous token was a number), add it to the output queue
                output_queue.Add(token);

                while (operator_stack.Count > 0 && operator_precedence[operator_stack.Peek()] >= operator_precedence[c.ToString()])
                {
                    // Pop operators from the stack to the output queue
                    output_queue.Add(operator_stack.Pop());
                }

                operator_stack.Push(c.ToString());
                token = "";

            }

            // Add the remaining token and operators to the output queue
            output_queue.Add(token);

            for (int j = operator_stack.Count - 1; j >= 0; j--)
            {
                output_queue.Add(operator_stack.Pop());
            }

            return output_queue;
        }

        public double Calculate(List<string> rpnOutput)
        {
            string[] validOperators = { "+", "-", "*", "/" };
            Stack<double> calculationStack = new Stack<double>(); // Stack to hold numbers for calculation

            // Iterate through the output queue
            for (int i = 0; i < rpnOutput.Count; i++)
            {
                string token = rpnOutput[i];

                // If the token is a number, push it onto the stack
                if (double.TryParse(token, out double number))
                {
                    calculationStack.Push(number);
                }
                else if (validOperators.Contains(token)) // If the token is a valid operator, pop two numbers from the stack and apply the operator, then push the result to the stack
                {
                    double rightOperand;
                    double leftOperand;

                    try
                    {
                        rightOperand = calculationStack.Pop();
                        leftOperand = calculationStack.Pop();
                    }
                    catch (OverflowException) { throw new Exception("Value Overflow"); }
                    catch (FormatException) { throw new Exception("Invalid operator"); }
                    catch (ArgumentNullException) { throw new Exception("Null operator"); }


                    switch (token)
                    {
                        case "+":
                            calculationStack.Push(leftOperand + rightOperand);
                            break;
                        case "-":
                            calculationStack.Push(leftOperand - rightOperand);
                            break;
                        case "*":
                            calculationStack.Push(leftOperand * rightOperand);
                            break;
                        case "/":
                            if (rightOperand == 0)
                            {
                                throw new Exception("Cannot divide by zero");
                            } else { calculationStack.Push(leftOperand / rightOperand); }
                            break;
                        default:
                            throw new Exception("Unknown operator: " + token);
                    }
                } else throw new Exception("Invalid input"); // The token is neither a number nor an operator; the input is (somehow) invalid and got past the regex check
            }

            // double and double type arithmetic that overflows will return positive or negative infinity as per IEEE 754 standard
            double result = calculationStack.Pop();
            
            if (result == double.PositiveInfinity)
            {
                throw new Exception("Value Overflow");
            }

            return result;
        }

        private Tuple<string, string> getPreviousResult(int index)
        {
            try
            {
                return calculationHistory[index-1]; // Index must be an integer from 1 to 10
            }
            catch (ArgumentOutOfRangeException)
            {

                Console.WriteLine("Equation history index given is out of range");
                return null;
            }
        }
        
        private void addToHistory(string result)
        {

            calculationHistory.Insert(0, new Tuple<string, string>(input, result)); // Insert the equation and result at the start of the history

            if (calculationHistory.Count > 10)
            {
                calculationHistory.RemoveAt(10);
            }

            // Nuke the panel history and generate new ones

            panel_history.Controls.Clear();

            for (int i = 0; i < this.calculationHistory.Count; i++)
            {
                TextBox equation_textbox = new TextBox();
                equation_textbox.Text = getPreviousResult(i+1).Item1 + " = " + getPreviousResult(i+1).Item2;
                equation_textbox.Font = new Font("Microsoft Sans Serif", 16);
                equation_textbox.TextAlign = HorizontalAlignment.Right;
                equation_textbox.Size = new System.Drawing.Size(200, 50);
                equation_textbox.Location = new System.Drawing.Point(0, 30*i);
                equation_textbox.ReadOnly = true;
                panel_history.Controls.Add(equation_textbox);
            }
        }

        private void clearEquation()
        {
            input = "0";
            operatorSet = false;
            decimalSet = false;
        }

        //
        // Button events
        //

        private void number_button_clicked(object sender, EventArgs e) // If a number button is clicked, update the left or right
        {
            // Cast the sender to a button to grab the 'Text' attribute
            Button clickedButton = (Button)sender;

            if (input == "0") {
                input = "";
                textBox_result.Text = "";
            }

            input += clickedButton.Text;
            textBox_result.Text = input;
            operatorSet = false;
        }

        private void decimal_button_clicked(Object sender, EventArgs e)
        {
            // Cast the sender to a button to grab the 'Text' attribute
            Button clickedButton = (Button)sender;

            if (decimalSet == false)
            {
                input += clickedButton.Text;
                decimalSet = true;
                textBox_result.Text = input;
            }
        }

        private void operator_button_clicked(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;

            // If operator has not been set
            if (operatorSet == false)
            {
                input += clickedButton.Text;
                textBox_result.Text = input;
                decimalSet = false;
                operatorSet = true;
            }
            else //Operator has already been set so change it to the newly clicked operator
            {
                input = input.TrimEnd(input[input.Length - 1]) + clickedButton.Text; // Replace the last operator with the new operator
                textBox_result.Text = input;
            }
        }

        private void equals_button_clicked(object sender, EventArgs e)
        {
            // If the equals button is clicked and there is a valid equation, calculate the result and clear the equation
            string result;

            try
            {
                if (input != "0" && input != "")
                {
                    List<string> tokens = shuntingRPNCalc(input);
                    result = Calculate(tokens).ToString();
                    addToHistory(result);
                    textBox_result.Text = result;
                }
            }
            catch (Exception ex)
            {
                textBox_result.Text = ex.Message;
            }
            finally
            {
                clearEquation();
            }
        }

        private void clear_button_clicked(Object sender, EventArgs e)
        {
            clearEquation();
            textBox_result.Text = "0";
        }
    }
}
