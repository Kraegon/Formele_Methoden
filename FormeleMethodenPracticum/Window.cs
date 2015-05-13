using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FormeleMethodenPracticum
{
    public partial class Window : Form
    {
        public Window()
        {
            InitializeComponent();
        }

        public static void WriteLine(string outputLine)
        {
            outputTextBox.Text += outputLine + "\n";
        }

        public static void Write(string output)
        {
            outputTextBox.Text += output;
        }

        public static void ClearOutput()
        {
            outputTextBox.Clear();
        }

        private void inputTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            //Execute the rest only if enter is pressed.
            if (e.KeyCode != Keys.Enter)
                return;

            //If empty, clear.
            if (inputTextBox.Text.Trim() == "")
            {
                inputTextBox.Clear();
                return;
            }

            //Remove enter.
            inputTextBox.Text = inputTextBox.Text.Remove(inputTextBox.Text.Length - 1, 1);

            //Process input.
            processInput();

            //Input to Output with '>'.
            Write("> ");
            WriteLine(inputTextBox.Text);

            //Clear Input.
            inputTextBox.Clear();
        }

        private void processInput()
        { 
            //TODO: Make usefull.
        }
    }
}