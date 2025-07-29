using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BasicCalculator
{

    public partial class Calculator : Form
    {
        private string left, right, op;
        private bool operatorSet = false; // To keep track of whether we are dealing with the left or right
        private bool decimalSet = false; // To keep track of whether a decimal has already been used
        private List<Tuple<string, string>> calculationHistory = new List<Tuple<string, string>>(); // List of tuples to hold the equation history

        public Calculator(string left, string right, string op)
        {
            InitializeComponent();
            setLeft(left);
            setRight(right);
            setOperator(op);
        }

        //
        // Setters
        //

        private void setLeft(string left)
        {
            try
            {
                double.Parse(left);
            }
            catch (Exception ex) when (ex is FormatException || ex is ArgumentNullException || ex is OverflowException) {
                Console.WriteLine("Cannot set Left to " + left);
            }

            // Strip leading 0 for visuals
            this.left = left.TrimStart('0');

            // Add 0 if 0 is input but then stripped
            if (this.left == "")
            {
                this.left = "0";
            }

        }

        private void setRight(string right)
        {
            try
            {
                double.Parse(right);
            }
            catch (Exception ex) when (ex is FormatException || ex is ArgumentNullException || ex is OverflowException)
            {
                Console.WriteLine("Cannot set Right to " + right);
            }

            this.right = right.TrimStart('0');

            // Add 0 if 0 is input but then stripped
            if (this.right == "")
            {
                this.right = "0";
            }
            
        }

        private void setOperator(string op)
        {
            string[] validOperators = { "+", "-", "*", "/" };
            if (! Array.Exists(validOperators, o => o == op)){
                Console.WriteLine("Invalid operator given");
                this.textBox_result.Text = "Invalid operator given";
            } else { this.op = op; }
        }

        //
        // Getters
        //

        private string getResult()
        {
            // Error handling is handled in calculate
            return calculate(this.left, this.right, this.op).ToString();
        }

        private Tuple<string, string> getPreviousResult(int index)
        {
            try
            {
                return calculationHistory[index - 1]; // Index must be an integer from 1 to 10
            }
            catch (NullReferenceException)
            {
                Console.WriteLine("Equation history index given is out of range");
                return null;
            }
        }

        //
        // Methods
        //


        private void addToHistory(string result)
        {
            calculationHistory.Insert(0, new Tuple<string, string>(left+op+right, result)); // Insert the equation and result at the start of the history

            if (calculationHistory.Count > 10)
            {
                calculationHistory.RemoveAt(10);
            }

            // Nuke the panel history and generate new ones

            panel_history.Controls.Clear();

            for (int i = 0; i < this.calculationHistory.Count; i++)
            {
                TextBox equation_textbox = new TextBox();
                equation_textbox.Text = getPreviousResult(i + 1).Item1 + " = " + getPreviousResult(i + 1).Item2;
                equation_textbox.Font = new Font("Microsoft Sans Serif", 16);
                equation_textbox.TextAlign = HorizontalAlignment.Right;
                equation_textbox.Size = new System.Drawing.Size(200, 50);
                equation_textbox.Location = new System.Drawing.Point(0, 30 * i);
                equation_textbox.ReadOnly = true;
                panel_history.Controls.Add(equation_textbox);
            }
        }

        public double calculate(string left, string right, string op)
        {
            double leftdouble;
            double rightdouble;

            try
            {
                leftdouble = double.Parse(left);
                rightdouble = double.Parse(right);
            }
            catch (OverflowException) { throw new Exception("Value Overflow"); }
            catch (FormatException) { throw new Exception("Invalid operand"); }
            catch (ArgumentNullException) { throw new Exception("Null operand"); }

            double result;

            switch (op)
            {
                case "+":
                    result = leftdouble + rightdouble;
                    break;

                case "-":
                    result = leftdouble - rightdouble;
                    break;

                case "*":
                    result = leftdouble * rightdouble;
                    break;

                case "/":
                    if (rightdouble == 0)
                    {
                        throw new Exception("Cannot divide by zero");
                    } else
                    {
                        result = leftdouble / rightdouble;
                    }
                    break;

                default:
                    throw new Exception("Invalid operator");
            }

            if (result == double.PositiveInfinity)
            {
                throw new Exception("Value Overflow");
            }

            return result;
        }

        private void clearEquation()
        {
            setLeft("0");
            setRight("0");
            setOperator("+");
            this.operatorSet = false;
            this.decimalSet = false;
        }

        //
        // Button events
        //

        private void number_button_clicked(object sender, EventArgs e)
        {
            // If a number button is clicked, update the left or right

            // Cast the sender to a button so I can grab the 'Text' attribute
            Button clickedButton = (Button)sender;

            if (this.left == "0") { textBox_result.Text = ""; }

            if (this.operatorSet == false)
            {
                setLeft(this.left + clickedButton.Text);
                this.textBox_result.Text = this.left;
            } else {
                setRight(this.right + clickedButton.Text);
                this.textBox_result.Text = this.right;
            }
        }

        private void decimal_button_clicked(Object sender, EventArgs e)
        {
            // Cast the sender to a button so I can grab the 'Text' attribute
            Button clickedButton = (Button)sender;

            if (this.decimalSet == true)
            {
                return;
            }

            if (this.operatorSet == false)
            {
                setLeft(this.left + clickedButton.Text);
                this.textBox_result.Text += clickedButton.Text;
                this.decimalSet = true;
            }
            else if (this.operatorSet == true)
            {
                setRight(this.right + clickedButton.Text);
                this.textBox_result.Text += clickedButton.Text;
                this.decimalSet = true;
            }
        }

        private void operator_button_clicked(object sender, EventArgs e)
        {
            // If an operator button is clicked and hasn't been set before, set the operator
            // Functionality of changing the operator afterward is out of scope for simple calculator

            Button clickedButton = (Button)sender;

            if (this.operatorSet == false)
            {
                this.textBox_result.Text = "";
                this.decimalSet = false;
                this.operatorSet = true;
            }
            setOperator(clickedButton.Text);
            this.textBox_intermediary.Text = this.left + " " + this.op;
        }

        private void equals_button_clicked(object sender, EventArgs e)
        {
            // If the equals button is clicked and there is a valid equation, calculate the result and clear the equation

            string result;

            if (this.left != "" && this.right != "")
            {
                try
                {
                    result = getResult();
                    this.textBox_result.Text = result;
                    addToHistory(result);
                }
                catch (Exception ex)
                {
                    this.textBox_result.Text = ex.Message;
                    return;
                }
                finally
                {
                    this.textBox_intermediary.Text = "";
                    clearEquation();
                }
               
            }
        }

        private void clear_button_clicked(Object sender, EventArgs e)
        {
            clearEquation();
            this.textBox_result.Text = "0";
            this.textBox_intermediary.Text = "";
        }
    }
}
