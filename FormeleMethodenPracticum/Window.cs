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
        static RegularGrammar regularGrammar;
        static string partToFill = "";
        static Boolean grammar = false;

        public Window()
        {
            InitializeComponent();
            Commands.Create();
        }

        public static void WriteLine(string outputLine)
        {
            outputTextBox.Text += outputLine + "\n";

            outputTextBox.SelectionStart = outputTextBox.Text.Length;
            outputTextBox.ScrollToCaret();
        }

        public static void Write(string output)
        {
            outputTextBox.Text += output;

            outputTextBox.SelectionStart = outputTextBox.Text.Length;
            outputTextBox.ScrollToCaret();
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

            //Input to Output with '>'.
            Write("> ");
            WriteLine(inputTextBox.Text);

            //Process input.
            processInput();

            //Clear Input.
            inputTextBox.Clear();
        }

        private static void processInput()
        {
            if (grammar == true)
            {
                processGrammar();
            }
            else
            {
                string[] parts = inputTextBox.Text.Split(' ');

                //Garanteed to be atleast one non space character by the trigger.
                Command inputCommand = Commands.FindCommand(parts[0].ToUpper());
                if(inputCommand != null)
                    inputCommand.Execute();
            }
        }

        private static void processGrammar()
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
                case "REGEX":
                    WriteLine("Ik ben de regex");
                    WriteLine("Vrees mij");
                    break;
                case "EXIT":
                    Program.Terminate();
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

        /// List of commands.
        /// To add a new command do the following:
        /// 1) Create a new static Command (with a name, description and command)
        /// 2) Add the new command to the create list
        private static class Commands
        {
            private static List<Command> CommandsList = new List<Command>();

            /// Get all names of commands.
            public static string[] CommandNames()
            {
                string[] str = new String[CommandsList.Count];
                for (int i = 0; i < str.Length; i++)
                {
                    str[i] = CommandsList[i].Name;
                }
                return str;
            }

            /// Look for a certain command by name
            public static Command FindCommand(String commandName)
            {
                Command result = null;
                foreach (Command c in Commands.CommandsList)
                {
                    if (c.Name.ToUpper() == commandName)
                    {
                        result = c;
                    }
                }
                return result;
            }

            public static void Create()
            {
                #region Add commands
                CommandsList.Add(new Command("DFA",
                     "Play with DFAs.",
                     delegate()
                     {
                         FiniteAutomaton.CreateNew(false);
                     }));
                CommandsList.Add(new Command("NDFA",
                        "Play with NDFAs.",
                        delegate()
                        {
                            FiniteAutomaton.CreateNew(true);
                        }));
                CommandsList.Add(new Command("Grammar",
                        "Play with formal grammar.",
                        delegate()
                        {
                            Window.regularGrammar = new RegularGrammar();
                            Window.partToFill = "SYMBOLS";
                            Window.grammar = true;
                            WriteLine("Type the symbols.");
                            WriteLine("Example: A, B");
                        }));
                CommandsList.Add(new Command("Regex",
                        "Play with regular expressions.",
                        delegate()
                        {
                            WriteLine("Ik ben de regex");
                            WriteLine("Vrees mij");
                        }));
                CommandsList.Add(new Command("Exit",
                       "Quit the program.",
                       delegate()
                       {
                           Program.Terminate();
                       }));
                CommandsList.Add(new Command("GrammarString",
                        "Do grammar string thing.",
                        delegate()
                        {
                            if (Window.regularGrammar != null)
                                WriteLine(Window.regularGrammar.toString());
                        }));
                #endregion
            }
        }

        /// <summary>
        /// Command object containing its name, description and whether it requires parameters or not.
        /// </summary>
        private class Command
        {
            public string Name;
            public string Description;
            public Behaviour command;

            public delegate void Behaviour();
            
            public Command(string Name, string Description, Behaviour Command)
            {
                this.Name = Name;
                this.Description = Description;
                this.command = Command; //Hold command
            }

            public void Execute()
            {
                command(); //Do held back command
            }

            public override string ToString()
            {
                return "-> " + Name + " \n" + "'" + Description + "'";
            }
        } //End Command
    } //End Window
}//End namespace
