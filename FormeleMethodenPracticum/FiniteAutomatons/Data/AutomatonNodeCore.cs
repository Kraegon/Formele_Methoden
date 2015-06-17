using FormeleMethodenPracticum.FiniteAutomatons.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormeleMethodenPracticum.FiniteAutomatons
{
    public class AutomatonNodeCore
    {
        public string stateName = "";
        public bool isEndNode = false;
        public bool isBeginNode = false;
        public List<AutomatonTransition> parents = new List<AutomatonTransition>();
        public List<AutomatonTransition> children = new List<AutomatonTransition>();
    }
}
