using FormeleMethodenPracticum.FiniteAutomatons;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FormeleMethodenPracticum
{
    public partial class Window : Form
    {
        private static Window _instance;
        public static Window INSTANCE
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Window();
                }
                return _instance;
            }
        }

        public object lastProcessedResult;
        RegularGrammar regularGrammar;

        public Window()
        {
            InitializeComponent();
            Commands.Create();
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

        private void processInput()
        {
            if (Window.INSTANCE.regularGrammar != null && Window.INSTANCE.regularGrammar.filling == true)
            {
                Write( Window.INSTANCE.regularGrammar.processGrammar(inputTextBox.Text));
            }
            else
            {
                string[] parts = inputTextBox.Text.Split(' ');
                
                //Garanteed to be atleast one non space character by the trigger.
                Command inputCommand = Commands.FindCommand(parts[0].ToUpper());
                if(inputCommand != null)
                    inputCommand.Execute(inputTextBox.Text.Substring(parts[0].Length).Trim());
            }
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
                #region Add command
                CommandsList.Add(new Command("DFA",
                     "Play with (N)DFAs.\n" +
                     "Controls:\n" +
                     "Leftern Double Click - New Node\n" +
                     "Rightern Double Click - Remove Node\n" +
                     "Drag - Move Node\n" +
                     "Click - Select / Deselect Node\n" +
                     "Enter when selecting - Make Start Node / Undo Making Start Node\n" +
                     "Tab Then Selecting - Transition Maker Enabled / Transition Maker Disabled\n" +
                     "Click When Transition Making - New Transition / Remove Transition\n" +
                     "Keyup - Assign State Name",
                     delegate(string paramaters)
                     {
                         Window.INSTANCE.lastProcessedResult = AutomatonMaker.CreateNew(false);
                     }));
                CommandsList.Add(new Command("NDFA",
                     "Play with (N)DFAs.\n" +
                     "Controls:\n" +
                     "Leftern Double Click - New Node\n" +
                     "Rightern Double Click - Remove Node\n" +
                     "Drag - Move Node\n" +
                     "Click - Select / Deselect Node\n" +
                     "Enter when selecting - Make Start Node / Undo Making Start Node\n" +
                     "Tab Then Selecting - Transition Maker Enabled / Transition Maker Disabled\n" +
                     "Click When Transition Making - New Transition / Remove Transition\n" +
                     "Keyup - Assign State Name",
                     delegate(string paramaters)
                     {
                         Window.INSTANCE.lastProcessedResult = AutomatonMaker.CreateNew(true);
                     }));
                CommandsList.Add(new Command("isPreviousItemDFA",
                     "Check whether the last made object is a DFA",
                     delegate(string paramaters)
                     {
                         if (Window.INSTANCE.lastProcessedResult is AutomatonCore)
                             Window.INSTANCE.WriteLine("Was the previous automaton deterministic?: " + ((!(Window.INSTANCE.lastProcessedResult as AutomatonCore).nondeterministic) ? "true" : (AutomatonMaker.isDFA(Window.INSTANCE.lastProcessedResult as AutomatonCore) ? "true" : "false")));
                         else
                             Window.INSTANCE.WriteLine("The previous result was not an automaton.");//TODO: Check for other needs
                     }));
                CommandsList.Add(new Command("Grammar",
                        "Play with formal grammar.",
                        delegate(string paramaters)
                        {
                            Window.INSTANCE.regularGrammar = new RegularGrammar();
                            Window.INSTANCE.regularGrammar.partToFill = "SYMBOLS";
                            Window.INSTANCE.regularGrammar.filling = true;
                            Window.INSTANCE.WriteLine("Type the symbols.");
                            Window.INSTANCE.WriteLine("Example: A, B");
                        }));
                CommandsList.Add(new Command("Regex",
                        "Play with regular expressions.",
                        delegate(string paramaters)
                        {
                            Window.INSTANCE.WriteLine("Params: " + paramaters);
                            //Regex.ParseRegex(paramaters);
                        }));
                CommandsList.Add(new Command("Exit",
                       "Quit the program.",
                       delegate(string paramaters)
                       {
                           Program.Terminate();
                       }));
                CommandsList.Add(new Command("GrammarString",
                        "Do grammar string thing.",
                        delegate(string paramaters)
                        {
                            if (Window.INSTANCE.regularGrammar != null)
                                Window.INSTANCE.WriteLine(Window.INSTANCE.regularGrammar.toString());
                        }));
                CommandsList.Add(new Command("GrammarToNDFA",
                        "Convert Grammar to NDFA",
                        delegate(string paramaters)
                        {
                            if (Window.INSTANCE.regularGrammar != null)
                            {
                               Window.INSTANCE.lastProcessedResult = Window.INSTANCE.regularGrammar.changeToNDFA();
                               Window.INSTANCE.WriteLine("The NDFA has made from the grammar");
                            }
                        }));
                CommandsList.Add(new Command("ShowAutomaton",
                        "Shows the automaton in a table",
                        delegate(string paramaters)
                        {
                            if (Window.INSTANCE.lastProcessedResult != null)
                            {
                                //Window.INSTANCE.lastProcessedResult;
                            }
                        }));
                CommandsList.Add(new Command("DFAReverse",
                        "Reverse the DFA, new Automaton will be NDFA",
                        delegate(string paramaters)
                        {
                            if (Window.INSTANCE.lastProcessedResult != null)
                            {
                                //Window.INSTANCE.lastProcessedResult;
                            }
                        }));
                CommandsList.Add(new Command("DFAMinimize",
                        "Minimize the DFA",
                        delegate(string paramaters)
                        {
                            if (Window.INSTANCE.lastProcessedResult != null)
                            {
                                //Window.INSTANCE.lastProcessedResult;
                            }
                        }));
                CommandsList.Add(new Command("AutomatonToGrammar",
                        "Convert Automaton to Grammar",
                        delegate(string paramaters)
                        {
                            if (Window.INSTANCE.lastProcessedResult != null)
                            {
                                //Window.INSTANCE.lastProcessedResult;
                            }
                        }));
                CommandsList.Add(new Command("Help",
                        "This help command.",
                        delegate(string parameters)
                        {
                            foreach(Command c in CommandsList)
                            {
                                Window.INSTANCE.WriteLine(c.ToString());
                                Window.INSTANCE.WriteLine("");
                            }
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

            public delegate void Behaviour(string paramaters);
            
            public Command(string Name, string Description, Behaviour Command)
            {
                this.Name = Name;
                this.Description = Description;
                this.command = Command; //Hold command
            }

            public void Execute(string paramaters)
            {
                command(paramaters); //Do held back command
            }

            public override string ToString()
            {
                return "-> " + Name + " \n" + "'" + Description + "'";
            }
        } //End Command
    } //End Window
}//End namespace
