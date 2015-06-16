using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormeleMethodenPracticum
{
    class Automaton
    {
        List<Transition> transitions;
        bool nondeterministic;

        public Automaton(bool nondeterministic)
        {
            this.nondeterministic = nondeterministic;
            transitions = new List<Transition>();
        }

        public Automaton(bool nondeterministic, List<Transition> transitions)
        {
            this.nondeterministic = nondeterministic;
            this.transitions = transitions;
        }

        public void reverse()
        {
            if(!nondeterministic)
            {
                List<Transition> newTransitions = new List<Transition>();

                foreach(Transition tr in transitions)
                {
                    Transition t = null;
                    if (tr.isBeginState() && tr.isEndState())
                    {
                        t = new Transition(tr.getName(), true, true);
                        t.setArrows(tr.getArrows());
                        newTransitions.Add(t);
                    }
                    else if (tr.isBeginState() && !tr.isEndState())
                    {
                        t = new Transition(tr.getName(), false, true);
                        t.setArrows(tr.getArrows());
                        newTransitions.Add(t);
                    }
                    else if (!tr.isBeginState() && tr.isEndState())
                    {
                        t = new Transition(tr.getName(), true, false);
                        t.setArrows(tr.getArrows());
                        newTransitions.Add(t);
                    }
                    else
                        newTransitions.Add(tr);
                }

                transitions = newTransitions;
                nondeterministic = true;
            }
        }

        public void toDFA()
        {
            if(nondeterministic)
            { 

            }
        }

        public void minimalise()
        {
            if(!nondeterministic)
            { }
        }

        public string toString()
        {
            string text = "";

            return text;
        }
    }
}
