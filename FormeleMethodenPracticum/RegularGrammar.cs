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
        private List<ProductLine> productionLines = new List<ProductLine>();

        public RegularGrammar()
        {
        }

        public RegularGrammar(List<string> N, List<string> E, List<ProductLine> P, string S)
        {
            symbols = N;
            alphabet = E;
            productionLines = P;
            startSymbol = S;
        }

        public void fillAlphabet(List<string> E)
        {
            alphabet = E;
        }

        public void fillSymbols(List<string> N)
        {
            symbols = N;
        }

        public void addProductionLine(ProductLine P)
        {
            productionLines.Add(P);
        }

        public void fillStartSymbol(string S)
        {
            startSymbol = S;
        }

        public bool containsSymbol(string Sym)
        {
            return symbols.Contains(Sym);
        }

        public bool containsLetter(string Let)
        {
            return alphabet.Contains(Let);
        }

        public void changeToNDFA()
        {
            //Fill in transition and make a FiniteAutomaton
        }

        public string toString()
        {
            if (alphabet.Count == 0 || symbols.Count == 0 || productionLines.Count == 0)
                return "Not all the variables of the grammar are filled.";
            
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
