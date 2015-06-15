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
                #region Add commands
                CommandsList.Add(new Command("DFA",
                     "Play with DFAs.",
                     delegate(string paramaters)
                     {
                         AutomatonMaker.CreateNew(false);
                     }));
                CommandsList.Add(new Command("NDFA",
                        "Play with NDFAs.",
                        delegate(string paramaters)
                        {
                            AutomatonMaker.CreateNew(true);
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
                            //if (Window.INSTANCE.regularGrammar != null)
                            //{
                            //    Window.INSTANCE.automaton = new Automaton(true, Window.INSTANCE.regularGrammar.changeToNDFA());
                            //}
                        }));
                CommandsList.Add(new Command("Help",
                        "This help command.",
                        delegate(string parameters)
                        {
                            foreach(Command c in CommandsList)
                            {
                                Window.INSTANCE.WriteLine(c.ToString());
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
