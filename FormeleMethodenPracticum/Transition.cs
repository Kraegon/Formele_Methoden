using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormeleMethodenPracticum
{
    class Transition
    {
        string name;
        Dictionary<string, string> arrows;
        
        public Transition(string name)
        {
            this.name = name;
        }

        public void addArrows(string letter, string otherTransition)
        {
            arrows.Add(letter, otherTransition);
        }
    }
}
