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
        RegularGrammar regularGrammar;
        string partToFill = "";
        Boolean grammar = false;
        public Window()
        {
            InitializeComponent();
        }

        public void WriteLine(string outputLine)
        {
            outputTextBox.Text += outputLine + "\n";

            outputTextBox.SelectionStart = outputTextBox.Text.Length;
            outputTextBox.ScrollToCaret();
        }

        public void Write(string output)
        {
            outputTextBox.Text += output;

            outputTextBox.SelectionStart = outputTextBox.Text.Length;
            outputTextBox.ScrollToCaret();
        }

        public void ClearOutput()
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

            //Input to Output with '>'.
            Write("> ");
            WriteLine(inputTextBox.Text);

            //Process input.
            processInput();

            //Clear Input.
            inputTextBox.Clear();
        }

        private void processInput() //TODO: Make usefull.
        {
            if (grammar == true)
            {
                processGrammar();
            }
            else
            {
                string[] parts = inputTextBox.Text.Split(' ');
                //Garanteed to be atleast one non space character by the trigger.
                switch (parts[0].ToUpper())
                {
                    case "DFA":
                        FiniteAutomaton.CreateNew(false);
                        break;
                    case "NDFA":
                        FiniteAutomaton.CreateNew(true);
                        break;
                    case "GRAMMAR":
                        regularGrammar = new RegularGrammar();
                        partToFill = "SYMBOLS";
                        grammar = true;
                        WriteLine("Type the symbols.");
                        WriteLine("Example: A, B");
                        break;
                    case "GRAMMARSTRING":
                        if(regularGrammar != null)
                            WriteLine(regularGrammar.toString());
                        break;
                }
            }
        }

        private void processGrammar()
        {
            switch (partToFill)
            {
                case "SYMBOLS":
                    List<string> symbols = new List<string>();

                    string[] parts = inputTextBox.Text.Split(',');
                    for (int i = 0; i < parts.Length; i++)
                    {
                        string symbol = parts[i];
                        string [] symbollist = symbol.Split(' ');
                        if(symbollist.Length == 1)
                            symbol = symbollist[0];
                        else if (symbollist.Length == 2)
                            symbol = symbollist[1]; 
                        symbols.Add(symbol);
                    }
                    WriteLine("Type the alphabet letters.");
                    WriteLine("Example: a, b");
                    partToFill = "ALPHABET";
                    regularGrammar.fillSymbols(symbols);
                    break;
                case "ALPHABET":
                    List<string> letters = new List<string>();

                    parts = inputTextBox.Text.Split(',');
                    for (int i = 0; i < parts.Length; i++)
                    {
                        string letter = parts[i];
                        string [] letterlist = letter.Split(' ');
                        if(letterlist.Length == 1)
                            letter = letterlist[0];
                        else if (letterlist.Length == 2)
                            letter = letterlist[1]; 
                        letters.Add(letter);
                    }
                    WriteLine("Type the ProductLines to add them in the list");
                    WriteLine("Example: A, a, B");
                    WriteLine("Type 'end' to end the list");
                    partToFill = "PRODUCTLINES";
                    regularGrammar.fillAlphabet(letters);
                    break;
                case "PRODUCTLINES":

                    parts = inputTextBox.Text.Split(',');

                    if (parts[0].ToUpper() == "END")
                    {
                        WriteLine("Type the StartSymbol");
                        WriteLine("Example: A");
                        partToFill = "STARTSYMBOL";
                    }
                    else
                    {
                        parts = inputTextBox.Text.Split(',');
                        if (parts.Length == 3)
                        {
                            string startSymbol = "";
                            string alphabet = "";
                            string endSymbol = "";

                            for (int i = 0; i < parts.Length; i++)
                            {
                                string letter = parts[i];
                                string[] letterlist = letter.Split(' ');
                                int index = 0;
                                if (letterlist.Length == 1)
                                    index = 0;
                                else if (letterlist.Length == 2)
                                    index = 1;
                                
                                switch (i)
                                {
                                    case 0:
                                        startSymbol = letterlist[index];
                                        break;
                                    case 1:
                                        alphabet = letterlist[index];
                                        break;
                                    case 2:
                                        endSymbol = letterlist[index];
                                        break;
                                }
                            }
                            if (regularGrammar.containsSymbol(startSymbol) && regularGrammar.containsSymbol(endSymbol) && regularGrammar.containsLetter(alphabet))
                            {
                                ProductLine productLine = new ProductLine(startSymbol, alphabet, endSymbol);
                                regularGrammar.addProductionLine(productLine);
                            }
                            else
                            {
                                WriteLine("The given values are not available in the grammar");
                            }
                        }
                    }
                    break;
                case "STARTSYMBOL":
                    parts = inputTextBox.Text.Split(' ');
                    if (regularGrammar.containsSymbol(parts[0]))
                    {
                        regularGrammar.fillStartSymbol(parts[0]);
                        partToFill = "";
                        grammar = false;
                        WriteLine("Grammar filled");
                    }
                    break;
            }
            
            /*grammar = true;
            List<string> symbols = new List<string>() { "S", "A", "B" };
            List<string> alphabet = new List<string>() { "a", "b" };
            List<ProductLine> productLines = new List<ProductLine>() {  new ProductLine("S", "a", "A"), new ProductLine("A", "b", "B"), 
                                                                                new ProductLine("A", "a", "S")};
            RegularGrammar gram = new RegularGrammar(symbols, alphabet, productLines, "s");
            Write(gram.toString());*/
        }
    }
}