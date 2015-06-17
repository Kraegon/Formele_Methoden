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
        //int totalColumns = 2;
        //int totalRows = 0;
        //List<string> columnAlphabet;

        public AutomatonTable(AutomatonCore automaton)
        {
            InitializeComponent();
            dataTable.AutoGenerateColumns = false;

            HashSet<char> alphabet = new HashSet<char>();
            foreach (AutomatonNodeCore node in automaton.nodes)
	        {
                foreach (AutomatonTransition transition in node.children)
	            {
                    foreach (char alpha in transition.acceptedSymbols)
                    {
                        alphabet.Add(alpha);   
                    }
	            }
	        }
            List<char> alphabetList = alphabet.OrderBy(alpha=>alpha).ToList<char>();

            for (int i = 1; i <= alphabet.Count; i++)
            {
                dataTable.Columns.Add(alphabetList[i - 1].ToString(), alphabetList[i - 1].ToString());
            }

            if(automaton.nondeterministic)
                dataTable.Columns.Add("epsilon", "ε");

            for (int j = 0; j < automaton.nodes.Count; j++)
            {
                dataTable.Rows.Add();
                Dictionary<char, string> cellValueByAlphabet = new Dictionary<char, string>();

                foreach(AutomatonTransition trans in automaton.nodes[j].children)
                {
                    if (trans.acceptedSymbols.Count != 0)
                    {
                        foreach (char alpha in trans.acceptedSymbols)
                        {
                            for (int i = 0; i < alphabetList.Count; i++)
                            {
                                if (alphabetList[i] == alpha)
                                {
                                    if (cellValueByAlphabet.ContainsKey(alpha))
                                        cellValueByAlphabet[alpha] += ", ";
                                    else
                                        cellValueByAlphabet[alpha] = "";

                                    cellValueByAlphabet[alpha] += trans.automatonNode.stateName;
                                }
                            }
                        }
                    }
                    else if(automaton.nondeterministic)
                    {
                        if (cellValueByAlphabet.ContainsKey('ε'))
                            cellValueByAlphabet['ε'] += ", ";
                        else
                            cellValueByAlphabet['ε'] = "";

                        cellValueByAlphabet['ε'] += trans.automatonNode.stateName;
                    }
                }

                foreach (KeyValuePair<char, string> item in cellValueByAlphabet)
                {
                    for (int i = 0; i <= alphabet.Count - (automaton.nondeterministic ? 0 : 1); i++)
                    {
                        if (dataTable.Columns[i].HeaderText == item.Key.ToString())
                        {
                            dataTable.Rows[j].Cells[i].Value = item.Value;
                        }
                    }
                }
                dataTable.Rows[j].HeaderCell.Value = (automaton.nodes[j].isBeginNode ? "->" : "") + (automaton.nodes[j].isEndNode ? "*" : "") + automaton.nodes[j].stateName;
            }
        }
    }
}
