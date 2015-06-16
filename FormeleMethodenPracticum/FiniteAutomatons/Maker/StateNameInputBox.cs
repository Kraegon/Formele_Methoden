using FormeleMethodenPracticum.FiniteAutomatons.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FormeleMethodenPracticum.FiniteAutomatons.Maker
{
    public partial class StateNameInputBox : Form
    {
        AutomatonNodeMaker automatonNodeMaker;

        public StateNameInputBox(AutomatonNodeMaker automatonNodeMaker)
        {
            this.automatonNodeMaker = automatonNodeMaker;
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
            if (Regex.IsMatch(textBox1.Text, @"^[a-zA-Z]$"))
            {
                automatonNodeMaker.Text = textBox1.Text;
                automatonNodeMaker.createdAutomatonNodeCore.stateName = textBox1.Text;
            }
            else
            {
                MessageBox.Show("Your string was not accepted, please try again.", "Error");
            }
            this.Close();
        }
    }
}
