using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormeleMethodenPracticum.FiniteAutomatons.Data
{
    public class AutomatonTransition
    {
        public AutomatonNodeCore automatonNode;
        public List<char> acceptedSymbols = new List<char>();

        public AutomatonTransition(AutomatonNodeCore automatonNode)
        {
            this.automatonNode = automatonNode;
        }
    }
}
