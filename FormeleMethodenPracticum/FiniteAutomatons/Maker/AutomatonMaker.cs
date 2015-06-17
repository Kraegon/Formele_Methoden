using FormeleMethodenPracticum.FiniteAutomatons;
using FormeleMethodenPracticum.FiniteAutomatons.Data;
using FormeleMethodenPracticum.FiniteAutomatons.Maker;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FormeleMethodenPracticum
{
    public class AutomatonMaker : Form
    {
        public AutomatonCore createdAutomatonCore;
        public AutomatonNodeMaker selectedAutomatonNodeMaker;
        private Button button1;
        public bool transitionCreator = false;

        public AutomatonMaker(bool nondeterministic)
        {
            createdAutomatonCore = new AutomatonCore(nondeterministic);

            InitializeComponent();

            this.DoubleClick += delegate(object sender, EventArgs e)
            {
                MouseEventArgs me = e as MouseEventArgs;
                AutomatonNodeMaker b = new AutomatonNodeMaker(this, false);
                b.Location = new Point(me.Location.X - 25, me.Location.Y - 25);
                b.Size = new Size(50, 50);
                b.FlatStyle = FlatStyle.Popup;
                b.Parent = this;
                this.Controls.Add(b);
                createdAutomatonCore.nodes.Add(b.createdAutomatonNodeCore);
            };
            System.Windows.Forms.Timer refreshTimer = new System.Windows.Forms.Timer();
            refreshTimer.Interval = (int)Math.Round(1000.0f / 30.0f);
            refreshTimer.Tick += delegate
            {
                this.Refresh();
                this.Invalidate();
            };
            refreshTimer.Start();
        }

        //Draws the automaton in progress of being made
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.CompositingQuality = CompositingQuality.HighQuality;
            base.OnPaint(e);

            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;
            using (Font f = new Font(this.Font.SystemFontName, 20, FontStyle.Bold))
            {
                using (Pen a = new Pen(Color.Teal, 5))
                {
                    a.EndCap = LineCap.ArrowAnchor;

                    foreach (object obj in Controls)
                    {
                        AutomatonNodeMaker n1 = obj as AutomatonNodeMaker;

                        if (n1 == null)
                            continue;

                        //End
                        using (Pen p = new Pen(n1 == selectedAutomatonNodeMaker ? (transitionCreator ? Color.Pink : Color.Blue) : Color.Red, 4))
                        {
                            e.Graphics.DrawEllipse(p, n1.Location.X + 2, n1.Location.Y + 2, n1.Size.Width - 4, n1.Size.Height - 4);
                            if (n1.createdAutomatonNodeCore.isEndNode)
                                e.Graphics.DrawEllipse(p, n1.Location.X + 2 + 6, n1.Location.Y + 2 + 6, n1.Size.Width - 4 - 12, n1.Size.Height - 4 - 12);
                        }

                        //Start
                        if (n1.createdAutomatonNodeCore.isBeginNode)
                        {

                            e.Graphics.DrawCurve(a, new Point[] { new Point(n1.Location.X + (n1.Size.Width / 2) - 100, n1.Location.Y + (n1.Size.Height / 2)),
                                                                  new Point(n1.Location.X + (n1.Size.Width / 2), n1.Location.Y + (n1.Size.Height / 2))});
                        }

                        //Transition
                        foreach (AutomatonTransition transition in n1.createdAutomatonNodeCore.children)
                        {
                            foreach (object obj2 in Controls)
                            {
                                AutomatonNodeMaker n2 = obj2 as AutomatonNodeMaker;

                                if (n2 == null)
                                    continue;

                                if (transition.automatonNode == n2.createdAutomatonNodeCore)
                                {
                                    string txt = "";
                                    for (int i = 0; i < transition.acceptedSymbols.Count; i++)
                                    {
                                        txt += transition.acceptedSymbols[i];
                                        if ((i + 1) < transition.acceptedSymbols.Count)
                                            txt += ",";
                                    }

                                    if (n1.createdAutomatonNodeCore == n2.createdAutomatonNodeCore)
                                    {
                                        e.Graphics.DrawCurve(a, new Point[] { new Point(n1.Location.X, n1.Location.Y + (n1.Size.Height / 2)),
                                                                          new Point(n1.Location.X, n1.Location.Y - (n1.Size.Height / 2)),
                                                                          new Point(n1.Location.X + n1.Width, n1.Location.Y - (n1.Size.Height / 2)),
                                                                          new Point(n1.Location.X + n1.Width, n1.Location.Y + (n1.Size.Height / 2))});

                                        e.Graphics.DrawString(txt, f, Brushes.Black, new Point(n1.Location.X + (n1.Width / 2), n1.Location.Y - (n1.Size.Height / 4)), stringFormat);
                                    }
                                    else
                                    {
                                        e.Graphics.DrawCurve(a, new Point[] { new Point(n1.Location.X + (n1.Size.Width / 2), n1.Location.Y + (n1.Size.Height / 2)),
                                                                          new Point(n2.Location.X + (n2.Size.Width / 2), n2.Location.Y + (n2.Size.Height / 2))});

                                        e.Graphics.DrawString(txt, f, Brushes.Black, new Point(
                                                        (n1.Location.X + (n1.Size.Width / 2)) - (((n1.Location.X + (n1.Size.Width / 2) - (n2.Location.X + (n2.Size.Width / 2))) / 2)),
                                                        (n1.Location.Y + (n1.Size.Height / 2)) - (((n1.Location.Y + (n1.Size.Height / 2) - (n2.Location.Y + (n2.Size.Height / 2))) / 2))),
                                                        stringFormat);
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        //Used to remove nodes from both the screen and the core
        public void removeNode(AutomatonNodeMaker node)
        {
            //Remove itself from its parents and childs
            node.createdAutomatonNodeCore.parents.ForEach(parent => parent.automatonNode.children.RemoveAll(n => n.automatonNode == node.createdAutomatonNodeCore));
            node.createdAutomatonNodeCore.children.ForEach(child => child.automatonNode.parents.RemoveAll(n => n.automatonNode == node.createdAutomatonNodeCore));

            createdAutomatonCore.nodes.Remove(node.createdAutomatonNodeCore);
            Controls.Remove(node);
        }

        //Used to assign letters to states and switch from and to relationCreation
        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            if (selectedAutomatonNodeMaker == null)
                return;

            if (char.IsLetterOrDigit((char)e.KeyCode))
            {
                new StateNameInputBox(selectedAutomatonNodeMaker).ShowDialog();
            }
            else if (e.KeyCode == Keys.Tab)
                transitionCreator = !transitionCreator;
            else if (e.KeyCode == Keys.Enter)
                selectedAutomatonNodeMaker.createdAutomatonNodeCore.isBeginNode = !selectedAutomatonNodeMaker.createdAutomatonNodeCore.isBeginNode;
        }

        public static AutomatonCore CreateNew(bool nondeterministic)
        {
            return ShowAutomatonMaker(nondeterministic);
        }

        public static AutomatonCore ShowAutomatonMaker(bool nondeterministic)
        {
            AutomatonMaker automatonMaker = new AutomatonMaker(nondeterministic);
            automatonMaker.ShowDialog();
            return automatonMaker.createdAutomatonCore;
        }

        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(1041, 539);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.TabStop = false;
            this.button1.Text = "Done";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            this.button1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.button1_KeyUp);
            // 
            // AutomatonMaker
            // 
            this.ClientSize = new System.Drawing.Size(1128, 574);
            this.ControlBox = false;
            this.Controls.Add(this.button1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AutomatonMaker";
            this.Text = "AutomatonMaker";
            this.ResumeLayout(false);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Remove focus
            this.ActiveControl = null;

            bool hasBeginNode = false;
            foreach (var item in createdAutomatonCore.nodes)
            {
                if(item.isBeginNode)
                {
                    hasBeginNode = true;
                }

                if (item.stateName == "")
                {
                    MessageBox.Show("Not all nodes have a statename.", "Error");
                    return;
                }
            }

            if (!hasBeginNode)
            {
                MessageBox.Show("No beginnode.", "Error");
                return;
            }

            HashSet<char> alphabet = new HashSet<char>();
            foreach (AutomatonNodeCore node in createdAutomatonCore.nodes)
            {
                foreach (AutomatonTransition trans in node.children)
                {
                    foreach (char alpha in trans.acceptedSymbols)
                    {
                        alphabet.Add(alpha);
                    }

                }
            }

            if (alphabet.Count == 0)
            {
                MessageBox.Show("Must use atleast one character.", "Error");
                return;
            }

            if (createdAutomatonCore.nondeterministic)
            {
                Window.INSTANCE.lastProcessedResult = createdAutomatonCore;
                this.Close();
                return;
            }

            if (isDFA(createdAutomatonCore))
            {
                Window.INSTANCE.lastProcessedResult = createdAutomatonCore;
                this.Close();
            }
            else
            {
                MessageBox.Show("Not a dfa.", "Error");
            }
        }

        public static AutomatonCore toDFA(AutomatonCore automatonCore)
        {
            convertNames(automatonCore);

            //Stop if it's already deterministic
            if (!automatonCore.nondeterministic)
                return automatonCore;

            //Once again, stop if it's already deterministic
            if(isDFA(automatonCore))
            {
                automatonCore.nondeterministic = false;
                return automatonCore;
            }

            //Add all beginNodes this this list
            List<AutomatonNodeCore> beginNodes = new List<AutomatonNodeCore>();
            foreach (AutomatonNodeCore node in automatonCore.nodes)
	        {
                if (node.isBeginNode)
                    beginNodes.Add(node);
	        }

            //Magic TODO: Add comments
            //      statename         alphabet  destination-states
            Dictionary<string, Dictionary<char, HashSet<string>>> states = new Dictionary<string, Dictionary<char, HashSet<string>>>();
            List<AutomatonNodeCore> iterationNodes = new List<AutomatonNodeCore>();
            iterationNodes.AddRange(beginNodes);

            addChildrenOfBeginNodes(iterationNodes, states);
            makeNewStatesAfterBeginNodes(iterationNodes, states);

            int amountOfStateNames = 0;
            while(amountOfStateNames != iterationNodes.Count)
            {
                amountOfStateNames = iterationNodes.Count;

                addChildrenofNonBeginNodes(automatonCore.nodes, states);
                makeNewStatesAfterBeginNodes(iterationNodes, states);//TODO: Change name
            }
            
            return statesToAutomatonCore(automatonCore.nodes, states);
        }

        public static void convertNames(AutomatonCore automatonCore)
        {
            for (int i = 0; i < automatonCore.nodes.Count; ++i)
            {
                if (i > 25)
                    automatonCore.nodes[i].stateName = "A" + (Convert.ToChar(65 + i)).ToString();
                else
                    automatonCore.nodes[i].stateName = (Convert.ToChar(65 + i)).ToString();
            }
        }

        private static AutomatonCore statesToAutomatonCore(List<AutomatonNodeCore> originalAutomatonNodes, Dictionary<string, Dictionary<char, HashSet<string>>> states)
        {
            AutomatonCore automatonCore = new AutomatonCore(true);
            foreach (KeyValuePair<string, Dictionary<char, HashSet<string>>> state in states)
            {
                AutomatonNodeCore node = new AutomatonNodeCore();
                string[] parts = state.Key.Trim().Split(',');
                foreach (AutomatonNodeCore ogNode in originalAutomatonNodes)
                {
                    bool contains = false;
                    foreach (string item in parts)
                    {
                        if (item == ogNode.stateName)
                        {
                            contains = true;
                            break;
                        }
                    }

                    if (!contains)
                        continue;

                    if (ogNode.isBeginNode)
                        node.isBeginNode = true;

                    if (ogNode.isEndNode)
                        node.isEndNode = true;
                }

                node.stateName = state.Key;
                automatonCore.nodes.Add(node);
            }

            foreach (AutomatonNodeCore node in automatonCore.nodes)
            {
                Dictionary<char, HashSet<string>> state = states[node.stateName];
                
                foreach (KeyValuePair<char, HashSet<string>> alphaStatePair in state)
                {
                    string childStateName = "";
                    bool first = true;
                    foreach (string childNamePart in alphaStatePair.Value)
	                {
                        if(!first)
                            childStateName += ",";

                        childStateName += childNamePart;
                        first = false;
                    }

                    foreach (AutomatonNodeCore node2 in automatonCore.nodes)
                    {
                        if (node2.stateName != childStateName)
                            continue;

                        AutomatonTransition trans1 = new AutomatonTransition(node2);
                        trans1.acceptedSymbols.Add(alphaStatePair.Key);
                        node.children.Add(trans1);

                        AutomatonTransition trans2 = new AutomatonTransition(node);
                        trans2.acceptedSymbols.Add(alphaStatePair.Key);
                        node2.parents.Add(trans2);
                        break;
                    }  
                }
            }

            HashSet<char> alphabet = new HashSet<char>();
            foreach (AutomatonNodeCore node in originalAutomatonNodes)
            {
                foreach (AutomatonTransition trans in node.children)
                {
                    foreach (char alpha in trans.acceptedSymbols)
                    {
                        alphabet.Add(alpha);
                    }
                }
            }

            bool errorStateMade = false;
            AutomatonNodeCore errorState = null;
            foreach (AutomatonNodeCore node1 in automatonCore.nodes)
            {
                HashSet<char> containingAlphabet = new HashSet<char>();
                foreach (AutomatonTransition trans5 in node1.children)
                {
                    foreach (char alpha in trans5.acceptedSymbols)
                    {
                        containingAlphabet.Add(alpha);
                    }
                }

                if(containingAlphabet.Count != alphabet.Count && !errorStateMade)
                {
                    errorState = new AutomatonNodeCore();
                    errorState.stateName = "ø";
                    AutomatonTransition trans1 = new AutomatonTransition(errorState);
                    foreach (char alpha in alphabet)
	                {
		                trans1.acceptedSymbols.Add(alpha);
	                }
                    errorState.children.Add(trans1);

                    errorStateMade = true;
                }

                if (containingAlphabet.Count != alphabet.Count)
                {
                    foreach (char alpha in alphabet)
                    {
                        bool found = false;
                        foreach (AutomatonTransition trans6 in node1.children)
                        {
                            foreach (char alpha2 in trans6.acceptedSymbols)
                            {
                                if (alpha2 == alpha)
                                {
                                    found = true;
                                    break;
                                }
                            }
                            if(found)
                                break;
                        }

                        if (!found)
                        {
                            AutomatonTransition trans1 = new AutomatonTransition(node1);
                            trans1.acceptedSymbols.Add(alpha);
                            errorState.parents.Add(trans1);

                            AutomatonTransition trans2 = new AutomatonTransition(errorState);
                            trans2.acceptedSymbols.Add(alpha);
                            node1.children.Add(trans2);
                        }
                    }
                }
            }

            if (errorStateMade)
                automatonCore.nodes.Add(errorState);

            automatonCore.nondeterministic = false;
            return automatonCore;
        }

        private static void addChildrenofNonBeginNodes(List<AutomatonNodeCore> originalAutomatonNodes, Dictionary<string, Dictionary<char, HashSet<string>>> states)
        {
            foreach (KeyValuePair<string, Dictionary<char, HashSet<string>>> node in states)
            {
                if (!states.ContainsKey(node.Key))
                    states[node.Key] = new Dictionary<char, HashSet<string>>();

                addChildrenOfNonBeginNodesIterative(originalAutomatonNodes, states, node.Key);
            }
        }

        private static void addChildrenOfNonBeginNodesIterative(List<AutomatonNodeCore> originalAutomatonNodes, Dictionary<string, Dictionary<char, HashSet<string>>> states, string stateName)
        {
            List<AutomatonNodeCore> origins = new List<AutomatonNodeCore>();
            string[] originParts = stateName.Split(',');
            foreach (string originPart in originParts)
            {
                foreach (AutomatonNodeCore core in originalAutomatonNodes)
                {
                    if (core.stateName == originPart)
                    {
                        origins.Add(core);
                        break;
                    }
                }
            }

            foreach (AutomatonNodeCore origin in origins)
            {
                foreach (AutomatonTransition trans in origin.children)
                {
                    if (!states.ContainsKey(stateName))
                        states[stateName] = new Dictionary<char, HashSet<string>>();

                    if (trans.acceptedSymbols.Count > 0)
                    {
                        foreach (char alpha in trans.acceptedSymbols)
                        {
                            if (!states[stateName].ContainsKey(alpha))
                                states[stateName][alpha] = new HashSet<string>();

                            states[stateName][alpha].Add(trans.automatonNode.stateName);
                        }
                    }
                    else
                    {
                        addChildrenOfNonBeginNodesIterative(new List<AutomatonNodeCore>() {trans.automatonNode}, states, stateName);
                    }
                }
            }
        }

        private static void makeNewStatesAfterBeginNodes(List<AutomatonNodeCore> iterationNodes, Dictionary<string, Dictionary<char, HashSet<string>>> states)
        {
            foreach (KeyValuePair<string, Dictionary<char, HashSet<string>>> state in states)
            {
                SortedSet<string> newStateParts = new SortedSet<string>();
                string newStateName = "";

                foreach (KeyValuePair<char, HashSet<string>> stateNameParts in state.Value)
	            {
                    foreach (string part in stateNameParts.Value)
                    {
                        newStateParts.Add(part);
                    }

                    foreach (string part in newStateParts)
                    {
                        if(newStateName != "")
                            newStateName += ",";
                        newStateName += part;
                    }

                    bool alreadyExists = false;
                    foreach (AutomatonNodeCore node in iterationNodes)
                    {
                        if (node.stateName == newStateName)
                        {
                            alreadyExists = true;
                            break;
                        }
                    }

                    if (!alreadyExists)
                    {
                        AutomatonNodeCore newCore = new AutomatonNodeCore();
                        newCore.stateName = newStateName;
                        iterationNodes.Add(newCore);
                    }

                    newStateName = "";
                    newStateParts.Clear();
	            }
            }

            foreach (AutomatonNodeCore node in iterationNodes)
            {
                if (!states.ContainsKey(node.stateName))
                    states[node.stateName] = new Dictionary<char, HashSet<string>>();
            }
        }

        private static void addChildrenOfBeginNodes(List<AutomatonNodeCore> iterationNodes, Dictionary<string, Dictionary<char, HashSet<string>>> states)
        {
            foreach (AutomatonNodeCore node in iterationNodes)
	        {
                if(!states.ContainsKey(node.stateName))
                    states[node.stateName] = new Dictionary<char, HashSet<string>>();

                addChildrenOfBeginNodesIterative(node, states, node.stateName);
	        }
        }

        private static void addChildrenOfBeginNodesIterative(AutomatonNodeCore origin, Dictionary<string, Dictionary<char, HashSet<string>>> states, string stateName)
        {
            foreach (AutomatonTransition trans in origin.children)
            {
                if (!states.ContainsKey(stateName))
                    states[stateName] = new Dictionary<char, HashSet<string>>();

                if (trans.acceptedSymbols.Count > 0)
                {
                    foreach (char alpha in trans.acceptedSymbols)
                    {
                        if (!states[stateName].ContainsKey(alpha))
                            states[stateName][alpha] = new HashSet<string>();

                        states[stateName][alpha].Add(trans.automatonNode.stateName);
                    }
                }
                else
                {
                    addChildrenOfBeginNodesIterative(trans.automatonNode, states, stateName);
                }
            }
        }

        public static bool isDFA(AutomatonCore automatonCore)
        {
            if (!automatonCore.nondeterministic)
               return true;

            List<char> symbols = new List<char>();
            List<char> foundSymbols = new List<char>();
            bool isFirst = true;

            foreach (AutomatonNodeCore node in automatonCore.nodes)
            {
                if (isFirst) 
                {
                    isFirst = false;
                    foreach (AutomatonTransition automationTransition in node.children)//Fill symbol list
                    {
                        symbols.AddRange(automationTransition.acceptedSymbols);
                    }

                    if (containsDuplicate(symbols)) //Look for duplicates
                        return false;
                }
                else
                {
                    foreach (AutomatonTransition automationTransition in node.children)//Fill foundsymbols, check for new ones
                    {
                        foreach (char c in automationTransition.acceptedSymbols)
                        {
                            if (symbols.Contains(c))
                                foundSymbols.Add(c);
                            else
                                return false;
                        }
                    }

                    if (containsDuplicate(foundSymbols)) //Check for duplicates
                        return false;

                    if (foundSymbols.Count != symbols.Count) //Check whether they are all used
                        return false;

                    foundSymbols = new List<char>();
                }
            }

            return true;
        }

        private static bool containsDuplicate(List<char> list)
        {
            Dictionary<char, int> item2ItemCount = list.GroupBy(item => item).ToDictionary(x => x.Key, x => x.Count()); 
            foreach (KeyValuePair<char, int> item in item2ItemCount)
            {
                if (item.Value > 1)
                    return true;
            }
            return false;
        }

        public static AutomatonCore reverseDFA(AutomatonCore automatonCore)
        {
            AutomatonCore reverse = new AutomatonCore(true);

            //Fill in the nodes and BeginStates become Endstates and Endstates become Beginstates
            foreach(AutomatonNodeCore node in automatonCore.nodes)
            {
                AutomatonNodeCore newNode = new AutomatonNodeCore();
                newNode.stateName = node.stateName;

                if (node.isBeginNode)
                {
                    newNode.isEndNode = true;
                    newNode.isBeginNode = false;
                }
                if (node.isEndNode)
                {
                    newNode.isEndNode = false;
                    newNode.isBeginNode = true;
                }

                reverse.nodes.Add(newNode);
            }

            foreach (AutomatonNodeCore node in automatonCore.nodes)
            {
                foreach (AutomatonTransition trans in node.children)
	            {
                    foreach (AutomatonNodeCore node1 in reverse.nodes)
                    {
                        if (node.stateName != node1.stateName)//Matches OG parent
                            continue;

                        foreach (AutomatonNodeCore node2 in reverse.nodes)
                        {
                            if (node2.stateName != trans.automatonNode.stateName)//Matches OG child
                                continue;
                            
                            AutomatonTransition trans1 = new AutomatonTransition(node1);
                            trans1.acceptedSymbols = trans.acceptedSymbols;
                            node2.children.Add(trans1);

                            AutomatonTransition trans2 = new AutomatonTransition(node2);
                            trans2.acceptedSymbols = trans.acceptedSymbols;
                            node1.parents.Add(trans2);
                            break;
                        }
                        break;
                    }
                }
            }

            return reverse;
        }

        private void button1_KeyUp(object sender, KeyEventArgs e)
        {
            OnKeyUp(e);
        }

        public static AutomatonCore DFAMinimize(AutomatonCore automatonCore)
        {
            return toDFA(reverseDFA(toDFA(reverseDFA(toDFA(reverseDFA(automatonCore))))));
        }
    }
}
