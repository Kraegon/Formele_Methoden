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
        public AutomatonCore createdAutomatonCore = new AutomatonCore();
        public AutomatonNodeMaker selectedAutomatonNodeMaker;
        public bool transitionCreator = false;

        public AutomatonMaker(bool nondeterministic)
        {
            createdAutomatonCore.nondeterministic = nondeterministic;

            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "AutomatonMaker";
            this.Text = "AutomatonMaker";
            this.ResumeLayout(false);
            this.DoubleBuffered = true;
            this.DoubleClick += delegate(object sender, EventArgs e)
            {
                MouseEventArgs me = e as MouseEventArgs;
                AutomatonNodeMaker b = new AutomatonNodeMaker(this, false);
                b.Location = new Point(me.Location.X - 25, me.Location.Y - 25);
                b.Size = new Size(50, 50);
                b.FlatStyle = FlatStyle.Popup;
                b.Parent = this;
                this.Controls.Add(b);
                createdAutomatonCore.nodes.Add(b);
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

                    foreach (AutomatonNodeMaker n1 in Controls)
                    {
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
                            foreach (AutomatonNodeMaker n2 in Controls)
                            {
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

            createdAutomatonCore.nodes.Remove(node);
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
                new StateNameInputBox(selectedAutomatonNodeMaker).Show();
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
            automatonMaker.Show();
            return automatonMaker.createdAutomatonCore;
        }
    }
}
