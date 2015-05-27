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
            Commands.Create();
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

            //Input to Output with '>'.
            Write("> ");
            WriteLine(inputTextBox.Text);

            //Process input.
            processInput();

            //Clear Input.
            inputTextBox.Clear();
        }

        private void processInput()
        {
            string[] parts = inputTextBox.Text.Split(' ');

            //Garanteed to be atleast one non space character by the trigger.
            Command inputCommand = Commands.FindCommand(parts[0].ToUpper());
            if(inputCommand != null)
                inputCommand.Execute();
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
                CommandsList.Add(DFA);
                CommandsList.Add(NDFA);
                CommandsList.Add(Grammar);
                CommandsList.Add(Regex);
                CommandsList.Add(Exit);
                #endregion
            }

            private static Command
            #region Commands
                DFA = new Command("DFA",
                     "Play with DFAs.",
                     delegate()
                     {
                         FiniteAutomaton.CreateNew(false);
                     }),
                NDFA = new Command("NDFA",
                        "Play with NDFAs.",
                        delegate()
                        {
                            FiniteAutomaton.CreateNew(true);
                        }),
                Grammar = new Command("Grammar",
                        "Play with formal grammar.",
                        delegate()
                        {
                            List<string> symbols = new List<string>();
                            List<string> alphabet = new List<string>();
                            List<ProductLine> productLines = new List<ProductLine>();
                            RegularGrammar gram = new RegularGrammar(symbols, alphabet, productLines, "s");
                        }),
                Regex = new Command("Regex",
                        "Play with regular expressions.",
                        delegate()
                        {
                            WriteLine("Ik ben de regex");
                            WriteLine("Vrees mij");
                        }),
                Exit = new Command("Exit",
                       "Quit the program.",
                       delegate()
                       {
                           Program.Terminate();
                       });
            #endregion
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
