using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Calculator
{

    public partial class Calculator : Form
    {
        private string left, right, op;
        private bool operatorSet = false; // To keep track of whether we are dealing with the left or right
        private bool decimalSet = false; // To keep track of whether a decimal has already been used
        private List<string> equation_history = new List<string>();

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
                float.Parse(left);
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
                float.Parse(right);
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
                Console.Write("Invalid operator given");
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

        private string getPreviousResult(int index)
        {
            try
            {
                // index - 1 because the integer is from 1 to 10
                return this.equation_history[index - 1];
            }
            catch (ArgumentOutOfRangeException) {

                //Console.Write("Equation history index given is out of range");
                return null;
            }
        }

        //
        // Methods
        //

        
        private void addToHistory(string result)
        {

            this.equation_history.Insert(0, result);

            if (this.equation_history.Count > 10)
            {
                this.equation_history.RemoveAt(10);
            }

            // Nuke the panel history and generate new ones

            panel_history.Controls.Clear();

            for (int i = 0; i < this.equation_history.Count; i++)
            {
                TextBox equation_textbox = new TextBox();
                equation_textbox.Text = getPreviousResult(i+1);
                equation_textbox.Font = new Font("Microsoft Sans Serif", 16);
                equation_textbox.TextAlign = HorizontalAlignment.Right;
                equation_textbox.Size = new System.Drawing.Size(200, 50);
                equation_textbox.Location = new System.Drawing.Point(0, 30*i);
                equation_textbox.ReadOnly = true;
                panel_history.Controls.Add(equation_textbox);
            }
        }

        private float calculate(string left, string right, string op)
        {
            float leftFloat = 0;
            float rightFloat = 0;

            if (left == "" || right == "")
            {
                throw new Exception("Operands missing");
            }

            try
            {
                leftFloat = float.Parse(left);
                rightFloat = float.Parse(right);
            } catch (OverflowException) {
                throw new Exception("Value Overflow");
            }

            float result = 0;

            switch (op)
            {
                case "+":
                    result = leftFloat + rightFloat;
                    break;

                case "-":
                    result = leftFloat - rightFloat;
                    break;

                case "*":
                    result = leftFloat * rightFloat;
                    break;

                case "/":
                    if (rightFloat == 0)
                    {
                        throw new Exception("Cannot divide by zero");
                    } else
                    {
                        result = leftFloat / rightFloat;
                    }

                    break;
            }

            // Float and double type arithmetic that overflows will return positive or negative infinity as per IEEE 754 standard

            if (result == float.PositiveInfinity)
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
                Console.WriteLine("Left: " + this.left);
            } else {
                setRight(this.right + clickedButton.Text);
                this.textBox_result.Text = this.right;
                Console.WriteLine("Right: " + this.right);
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
                Console.WriteLine("Right: " + this.right);
            }
            else if (this.operatorSet == true)
            {
                setRight(this.right + clickedButton.Text);
                this.textBox_result.Text += clickedButton.Text;
                this.decimalSet = true;
                Console.WriteLine("Right: " + this.right);
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
                
                addToHistory(result);

                //Console.Write("History Index at 0: " + getPreviousResult(0));
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
