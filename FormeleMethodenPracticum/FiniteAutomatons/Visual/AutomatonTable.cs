using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FormeleMethodenPracticum.FiniteAutomatons.Data;

namespace FormeleMethodenPracticum.FiniteAutomatons.Visual
{
    public partial class AutomatonTable : Form
    {
        int totalColumns = 2;
        int totalRows = 0;
        List<string> columnAlphabet;

        public AutomatonTable(AutomatonCore automaton)
        {
            InitializeComponent();

            columnAlphabet = new List<string>();

            processTable(automaton);
            fillTable(automaton);  
        }

        private void processTable(AutomatonCore automaton)
        {
            totalRows = automaton.nodes.Count;

            foreach(AutomatonNodeCore node in automaton.nodes)
            {
                foreach (AutomatonTransition trans in node.children)
                {
                    foreach (char c in trans.acceptedSymbols)
                    {
                        if (!columnAlphabet.Contains(c.ToString()))
                            columnAlphabet.Add(c.ToString());
                    }
                }
            }

            totalColumns = columnAlphabet.Count;

            for(int i = 0; i < totalColumns; i++)
            {
                if(i < 2)
                {
                    dataTable.Columns[i].Name = columnAlphabet[i];
                    dataTable.Columns[i].HeaderText = columnAlphabet[i];
                }
                else
                {
                    DataGridViewColumn column = new DataGridViewColumn();
                    column.Name = columnAlphabet[i];
                    column.HeaderText = columnAlphabet[i];
                    dataTable.Columns.Add(column);
                }
            }
            
        }

        private void fillTable(AutomatonCore automaton)
        {
            foreach (AutomatonNodeCore node in automaton.nodes)
            {
                string nodeName = "";

                if (node.isBeginNode)
                    nodeName = ">";
                if (node.isEndNode)
                    nodeName += "*";

                nodeName += " " + node.stateName;

                int rowIndex = dataTable.Rows.Add();

                DataGridViewRow row = dataTable.Rows[rowIndex];
                row.HeaderCell.Value = nodeName;

                Dictionary<string,List<string>> dictionary = new Dictionary<string,List<string>>();
                foreach (string letter in columnAlphabet)
                {
                    dictionary.Add(letter, new List<string>());
                }


                foreach (AutomatonTransition trans in node.children)
                {
                    foreach (char c in trans.acceptedSymbols)
                    {
                        dictionary[c.ToString()].Add(trans.automatonNode.stateName);
                    }
                }

                foreach(string letter in columnAlphabet)
                {
                    string columnName = "/";
                    List<string> symbolList = dictionary[letter];
                    for(int i = 0; i < symbolList.Count; i++)
                    {
                        if(i > 0)
                        {
                            columnName += ", " + symbolList[i];
                        }
                        else
                        {
                            columnName = symbolList[i];
                        }
                    }

                    row.Cells[letter].Value = columnName;
                }
            }
        }
    }
}
