using FormeleMethodenPracticum.FiniteAutomatons.Data;
using FormeleMethodenPracticum.FiniteAutomatons.Maker;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FormeleMethodenPracticum.FiniteAutomatons
{
    public class AutomatonNodeMaker : Label
    {
        public AutomatonNodeCore createdAutomatonNodeCore = new AutomatonNodeCore();
        private AutomatonMaker automatonMaker;

        //For Dragging
        public bool buttondown = false;
        public Point offset;
        public Point selectionPoint;

        public AutomatonNodeMaker(AutomatonMaker automatonMaker, bool isEndNode)
        {
            this.automatonMaker = automatonMaker;
            createdAutomatonNodeCore.isEndNode = isEndNode;
            this.Font = new Font(this.Font.SystemFontName, 20, FontStyle.Bold);
            this.BackColor = Color.Transparent;
            this.TabStop = false;
            this.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TextAlign = ContentAlignment.MiddleCenter;
        }

        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            buttondown = true;
            offset = PointToScreen(mevent.Location);
            selectionPoint = Location;
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            buttondown = false;
            offset = new Point(0, 0);
            if (selectionPoint != Location)
            {
                return;
            }

            if (!automatonMaker.transitionCreator)
            {
                if (automatonMaker.selectedAutomatonNodeMaker == this)
                {
                    automatonMaker.selectedAutomatonNodeMaker = null;
                }
                else
                {
                    automatonMaker.selectedAutomatonNodeMaker = this;
                }
            }
            else
            {
                bool doesAlreadyContain = false;
                foreach (AutomatonTransition transition in automatonMaker.selectedAutomatonNodeMaker.createdAutomatonNodeCore.children)
                {
                    if (transition.automatonNode == this.createdAutomatonNodeCore)
                    {
                        doesAlreadyContain = true;
                        break;
                    }
                }

                if (!doesAlreadyContain)
                {
                    AutomatonTransition trans1 = new AutomatonTransition(automatonMaker.selectedAutomatonNodeMaker.createdAutomatonNodeCore);
                    AutomatonTransition trans2 = new AutomatonTransition(this.createdAutomatonNodeCore);
                    createdAutomatonNodeCore.parents.Add(trans1);
                    automatonMaker.selectedAutomatonNodeMaker.createdAutomatonNodeCore.children.Add(trans2);
                    new AcceptedSymbolsInputBox(new List<AutomatonTransition>(){trans1, trans2}).Show();

                }
                else
                {
                    foreach (AutomatonTransition trans in createdAutomatonNodeCore.parents)
                    {
                        if (trans.automatonNode == automatonMaker.selectedAutomatonNodeMaker.createdAutomatonNodeCore)
                        {
                            createdAutomatonNodeCore.parents.Remove(trans);
                            break;
                        }
                    }
                    foreach (AutomatonTransition trans in automatonMaker.selectedAutomatonNodeMaker.createdAutomatonNodeCore.children)
                    {
                        if (trans.automatonNode == this.createdAutomatonNodeCore)
                        {
                            automatonMaker.selectedAutomatonNodeMaker.createdAutomatonNodeCore.children.Remove(trans);
                            break;
                        }
                    }
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs mevent)
        {
            if (buttondown)
            {
                Point p = PointToScreen(mevent.Location);
                int xoff = p.X - offset.X;
                int yoff = p.Y - offset.Y;
                Location = new Point(Location.X + xoff, Location.Y + yoff);
                offset = new Point(p.X, p.Y);
            }
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                createdAutomatonNodeCore.isEndNode = !createdAutomatonNodeCore.isEndNode;
            else if (e.Button == System.Windows.Forms.MouseButtons.Right)
                (this.Parent as AutomatonMaker).removeNode(this);
        }
    }
}
