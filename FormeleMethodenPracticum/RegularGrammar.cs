using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormeleMethodenPracticum
{
    class RegularGrammar
    {
        private List<string> alphabet;
        private List<string> symbols;
        private string startSymbol;
        private List<ProductLine> productionLines;

        public RegularGrammar(List<string> N, List<string> E, List<ProductLine> P, string S)
        {
            symbols = N;
            alphabet = E;
            productionLines = P;
            startSymbol = S;
        }

        public string toString()
        {
            string description = "";

            //Symbols
            description += "N = {";
            for (int i = 0; i < symbols.Count; i++)
            {
                description += symbols[i];
                if (i != symbols.Count - 1)
                    description += ", ";
            }
            description += "}\n";

            //Alphabet
            description += "S = {";
            for (int i = 0; i < alphabet.Count; i++)
            {
                description += alphabet[i];
                if (i != alphabet.Count - 1)
                    description += ", ";
            }
            description += "}\n";

            //Productlines
            description += "P: {";
            for(int i = 0; i < productionLines.Count; i++)
            {
                description += productionLines[i].toString();
                if (i != productionLines.Count - 1)
                    description += ", ";
            }
            description += "}\n";

            return description;
        }
    }

    class ProductLine
    {
        public string fromSymbol;
        public string letter;
        public string toSymbol;

        public ProductLine(string fromSymbol, string letter, string toSymbol)
        {
            this.fromSymbol = fromSymbol;
            this.letter = letter;
            this.toSymbol = toSymbol;
        }

        public string toString()
        {
            return "" + fromSymbol + "->" + letter + toSymbol;
        }
    }
}
