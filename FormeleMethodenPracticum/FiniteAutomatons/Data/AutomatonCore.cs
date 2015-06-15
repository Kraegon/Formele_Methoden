﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormeleMethodenPracticum.FiniteAutomatons
{
    public class AutomatonCore
    {
        public List<AutomatonNodeCore> nodes = new List<AutomatonNodeCore>();
        public bool nondeterministic;

        public AutomatonCore(bool nondeterministic)
        {
            this.nondeterministic = nondeterministic;
        }
    }
}
