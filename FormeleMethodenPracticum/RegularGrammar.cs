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
        private List<string> endSymbols;
        private List<ProductLine> productionLines = new List<ProductLine>();

        public string partToFill = "";
        public bool filling = false;

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

        public void fillEndSymbols(List<string> N)
        {
            endSymbols = N;
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

        //public List<Transition> changeToNDFA()
        //{
        //    List<Transition> transitions = new List<Transition>();

        //    foreach(string symbol in symbols)
        //    {
        //        Transition tr = null;
                
        //        if(symbol == startSymbol && endSymbols.Contains(symbol))
        //                tr = new Transition(symbol, true, true);
        //        else if (symbol == startSymbol && !endSymbols.Contains(symbol))
        //                tr = new Transition(symbol, true, false);
        //        else if (symbol != startSymbol && endSymbols.Contains(symbol))
        //            tr = new Transition(symbol, false, true);
        //        else if (symbol != startSymbol && !endSymbols.Contains(symbol))
        //            tr = new Transition(symbol, false, false);

        //        transitions.Add(tr);
        //    }

        //    foreach(ProductLine line in productionLines)
        //    {
        //        foreach(Transition tr in transitions)
        //        {
        //            if (tr.getName() == line.fromSymbol)
        //            {
        //                tr.addArrows(line.letter, line.toSymbol);
        //            }    
        //        }
        //    }

        //    return transitions;
        //}

        public string processGrammar(string input)
        {
            string output = "";
            switch (partToFill)
            {
                case "SYMBOLS":
                    List<string> symbols = new List<string>();

                    string[] parts = input.Split(',');
                    for (int i = 0; i < parts.Length; i++)
                    {
                        string symbol = parts[i];
                        string[] symbollist = symbol.Split(' ');
                        if (symbollist.Length == 1)
                            symbol = symbollist[0];
                        else if (symbollist.Length == 2)
                            symbol = symbollist[1];
                        symbols.Add(symbol);
                    }
                    output = "Type the alphabet letters. \n";
                    output += "Example: a, b \n";
                    partToFill = "ALPHABET";
                    fillSymbols(symbols);
                    break;
                case "ALPHABET":
                    List<string> letters = new List<string>();

                    parts = input.Split(',');
                    for (int i = 0; i < parts.Length; i++)
                    {
                        string letter = parts[i];
                        string[] letterlist = letter.Split(' ');
                        if (letterlist.Length == 1)
                            letter = letterlist[0];
                        else if (letterlist.Length == 2)
                            letter = letterlist[1];
                        letters.Add(letter);
                    }
                    output = "Type the ProductLines to add them in the list \n";
                    output += "Example: A, a, B \n";
                    output += "Type 'end' to end the list \n";
                    partToFill = "PRODUCTLINES";
                    fillAlphabet(letters);
                    break;
                case "PRODUCTLINES":

                    parts = input.Split(',');

                    if (parts[0].ToUpper() == "END")
                    {
                        output = "Type the StartSymbol \n";
                        output += "Example: A \n";
                        partToFill = "STARTSYMBOL";
                    }
                    else
                    {
                        parts = input.Split(',');
                        if (parts.Length == 3)
                        {
                            string startSymbol = "";
                            string alphabet = "";
                            string endSymbol = "";

                            for (int i = 0; i < parts.Length; i++)
                            {
                                string letter = parts[i];
                                string[] letterlist = letter.Split(' ');
                                int index = 0;
                                if (letterlist.Length == 1)
                                    index = 0;
                                else if (letterlist.Length == 2)
                                    index = 1;

                                switch (i)
                                {
                                    case 0:
                                        startSymbol = letterlist[index];
                                        break;
                                    case 1:
                                        alphabet = letterlist[index];
                                        break;
                                    case 2:
                                        endSymbol = letterlist[index];
                                        break;
                                }
                            }
                            if (containsSymbol(startSymbol) && containsSymbol(endSymbol) && containsLetter(alphabet))
                            {
                                ProductLine productLine = new ProductLine(startSymbol, alphabet, endSymbol);
                                addProductionLine(productLine);
                            }
                            else
                            {
                                output = "The given values are not available in the grammar \n";
                            }
                        }
                    }
                    break;
                case "STARTSYMBOL":
                    parts = input.Split(' ');
                    if (containsSymbol(parts[0]))
                    {
                        fillStartSymbol(parts[0]);
                        partToFill = "ENDSYMBOLS";
                        output = "Type the EndSymbols \n";
                        output += "Example: B,C \n";
                    }
                    break;
                case "ENDSYMBOLS":
                    List<string> endsymbols = new List<string>();

                    parts = input.Split(',');
                    for (int i = 0; i < parts.Length; i++)
                    {
                        string symbol = parts[i];
                        string[] symbollist = symbol.Split(' ');
                        if (symbollist.Length == 1)
                            symbol = symbollist[0];
                        else if (symbollist.Length == 2)
                            symbol = symbollist[1];
                        endsymbols.Add(symbol);
                    }
                    fillEndSymbols(endsymbols);

                    partToFill = "";
                    filling = false;
                    output = "Grammar filled \n";

                    break;
            }

            return output;
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
            description += "}";

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
