using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormeleMethodenPracticum.FiniteAutomatons
{
    public class AutomatonCore
    {
        public List<AutomatonNodeMaker> nodes = new List<AutomatonNodeMaker>();
        public bool nondeterministic;

        public bool parse = false;
        public List<AutomatonNodeCore> noMakerNodes;
    }
}
