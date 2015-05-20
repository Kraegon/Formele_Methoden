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
    public class FiniteAutomaton
    {
        public static FiniteAutomaton CreateNew(bool nondeterministic)
        {
            ShowAutomatonMaker(nondeterministic);

            return new FiniteAutomaton();
        }

        public class Node : Button
        {
            bool buttondown = false;
            bool selected = false;
            Point offset;
            Point selectionPoint;

            protected override void OnPaint(PaintEventArgs pevent)
            {
                //base.OnPaint(pevent);
                Graphics g = pevent.Graphics;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;

                using (SolidBrush b = new SolidBrush(Form.DefaultBackColor))
                    g.FillRectangle(b, 0, 0, this.Size.Width, this.Size.Height);
                using (Pen p = new Pen(selected ? Color.Blue : Color.Red, 2))
                    g.DrawEllipse(p, 0, 0, this.Size.Width, this.Size.Height);
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
                if (selectionPoint == Location)
                {
                    if (selected)
                    {
                        selected = false;
                    }
                    else
                    {
                        foreach (Node n in Parent.Controls)
                        {
                            if (n == null)
                                continue;

                            n.selected = false;
                        }
                        selected = true;
                    }
                }
                offset = new Point(0, 0);
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
        }

        //Prompt a screen to create nondeterministic ? ndfa : dfa
        private static FiniteAutomaton ShowAutomatonMaker(bool nondeterministic)
        {
            List<Node> nodes = new List<Node>();
            Form automatonMaker = new Form();
            automatonMaker.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            automatonMaker.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            automatonMaker.ClientSize = new System.Drawing.Size(284, 261);
            automatonMaker.Name = "AutomatonMaker";
            automatonMaker.Text = "AutomatonMaker";
            automatonMaker.ResumeLayout(false);
            automatonMaker.DoubleClick += delegate(object sender, EventArgs e)
            {
                MouseEventArgs me = e as MouseEventArgs;
                Node b = new Node();
                b.Location = new Point(me.Location.X - 25, me.Location.Y - 25);
                b.Size = new Size(50, 50);
                b.FlatStyle = FlatStyle.Popup;
                b.Parent = automatonMaker;
                automatonMaker.Controls.Add(b);
                nodes.Add(b);
            };
            Timer refreshTimer = new Timer();
            refreshTimer.Interval = (int)Math.Round(1000.0f / 30.0f);
            refreshTimer.Tick += delegate
            {
                automatonMaker.Refresh();
                automatonMaker.Invalidate();
            };
            refreshTimer.Start();
            
            automatonMaker.ShowDialog();
            //TODO: Try component instead of drawing for events and stop being unable to doublebuffer.
            return new FiniteAutomaton();
        }

        public void draw(System.Drawing.Graphics g)
        {
            //TODO: Let this make sense.
            g.DrawLine(System.Drawing.Pens.AliceBlue, new System.Drawing.Point(0, 0), new System.Drawing.Point(1, 0));
        }
    }
}
