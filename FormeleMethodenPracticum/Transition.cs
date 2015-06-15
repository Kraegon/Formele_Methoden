using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormeleMethodenPracticum
{
    public class Transition
    {
        string name;
        bool beginState;
        bool endState;
        Dictionary<string, string> arrows;
        
        public Transition(string name, bool beginState, bool endState)
        {
            this.name = name;
            arrows = new Dictionary<string, string>();
        }

        public void addArrows(string letter, string otherTransition)
        {
            arrows.Add(letter, otherTransition);
        }

        public string getName()
        {
            return name;
        }

        public bool isBeginState()
        {
            return beginState;
        }

        public bool isEndState()
        {
            return endState;
        }

        public Dictionary<string, string> getArrows()
        {
            return arrows;
        }

        public void setArrows(Dictionary<string, string> arrows)
        {
            this.arrows = arrows;
        }
    }
}
