using FormeleMethodenPracticum.FiniteAutomatons.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FormeleMethodenPracticum.FiniteAutomatons.Maker
{
    public partial class AcceptedSymbolsInputBox : Form
    {
        List<AutomatonTransition> automatonTransitions;

        public AcceptedSymbolsInputBox(List<AutomatonTransition> automatonTransitions)
        {
            this.automatonTransitions = automatonTransitions;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            finalize();
        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                finalize();
        }

        private void finalize()
        {
            if (textBox1.Text == "")
                return;

            string[] parts = textBox1.Text.Trim().Split(',');
            foreach (string str in parts)
            {
                if (char.IsLetter(str, 0) || char.IsNumber(str, 0))
                {
                    foreach (AutomatonTransition automatonTransition in automatonTransitions)
                    {
                        automatonTransition.acceptedSymbols.Add(str[0]);
                    }
                }
                else
                {
                    foreach (AutomatonTransition automatonTransition in automatonTransitions)
                    {
                        automatonTransition.acceptedSymbols.Clear();
                    }
                    MessageBox.Show("Your string was not accepted, please remove the transition and try again.", "Error");
                    break;
                }
            }
            this.Close();
        }
    }
}
