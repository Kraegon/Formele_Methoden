using FormeleMethodenPracticum.FiniteAutomatons;
using FormeleMethodenPracticum.FiniteAutomatons.Data;
using FormeleMethodenPracticum.FiniteAutomatons.Maker;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
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
            Timer refreshTimer = new Timer();
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
                                            txt += ", ";
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

        public static bool isDFA(AutomatonCore automatonCore)
        {
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

        private void button1_KeyUp(object sender, KeyEventArgs e)
        {
            OnKeyUp(e);
        }
    }
}
