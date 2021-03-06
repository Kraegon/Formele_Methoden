﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FormeleMethodenPracticum.FiniteAutomatons;

namespace FormeleMethodenPracticum
{
    class RegularGrammar
    {
        private List<string> alphabet;
        private List<string> symbols;
        private List<string> startSymbols;
        private List<string> endSymbols;
        private List<ProductLine> productionLines;

        private string partToFill = "";
        public bool filling = false;

        public RegularGrammar()
        {
            filling = true;
            partToFill = "SYMBOLS";
            productionLines = new List<ProductLine>();
        }

        public RegularGrammar(AutomatonCore automaton)
        {
            filling = true;
            symbols = new List<string>();
            startSymbols = new List<string>();
            alphabet = new List<string>();
            endSymbols = new List<string>();
            productionLines = new List<ProductLine>();

            processAutomaton(automaton);
        }

        public bool containsSymbol(string Sym)
        {
            return symbols.Contains(Sym);
        }

        public bool containsLetter(string Let)
        {
            return alphabet.Contains(Let);
        }

        public AutomatonCore changeToNDFA()
        {
            AutomatonCore automatonCore = new AutomatonCore(true);

            foreach(string symbol in symbols)
            {
                AutomatonNodeCore ac = new AutomatonNodeCore();
                ac.stateName = symbol;
                if(startSymbols.Contains(symbol))
                {
                    ac.isBeginNode = true;
                    ac.isEndNode = false;
                }
                else if(endSymbols.Contains(symbol))
                {
                    ac.isBeginNode = false;
                    ac.isEndNode = true;
                }
                else
                {
                    ac.isBeginNode = false;
                    ac.isEndNode = false;
                }
                automatonCore.nodes.Add(ac);
            }
            
            foreach(ProductLine pl in productionLines)
            {
                foreach (AutomatonNodeCore node in automatonCore.nodes)
                {
                    if(pl.fromSymbol == node.stateName)
                    {
                        //add to children
                        bool newTrans = true;

                        //Check if there already is a transition with the samen nodecores
                        //If there is, add the extra state letter
                        foreach(AutomatonTransition tr in node.children)
                        {
                            if (tr.automatonNode.stateName == pl.toSymbol)
                            {
                                tr.acceptedSymbols.Add(pl.letter[0]);
                                newTrans = false;
                            }
                        }

                        if (newTrans)
                        {
                            AutomatonTransition trans = null;
                            foreach (AutomatonNodeCore endNode in automatonCore.nodes)
                            {
                                if (endNode.stateName == pl.toSymbol)
                                    trans = new AutomatonTransition(endNode);
                            }
                            trans.acceptedSymbols.Add(pl.letter[0]);
                            node.children.Add(trans);
                        }
                    }
                    else if (pl.toSymbol == node.stateName)
                    {
                        //add to parent

                        bool newTrans = true;

                        //Check if there already is a transition with the samen nodecores
                        //If there is, add the extra state letter
                        foreach (AutomatonTransition tr in node.parents)
                        {
                            if (tr.automatonNode.stateName == pl.toSymbol)
                            {
                                tr.acceptedSymbols.Add(pl.letter[0]);
                                newTrans = false;
                            }
                        }

                        if (newTrans)
                        {
                            AutomatonTransition trans = null;
                            foreach (AutomatonNodeCore firstNode in automatonCore.nodes)
                            {
                                if (firstNode.stateName == pl.fromSymbol)
                                    trans = new AutomatonTransition(firstNode);
                            }
                            trans.acceptedSymbols.Add(pl.letter[0]);
                            node.parents.Add(trans);
                        }
                    }
                }
            }
            return automatonCore;
        }

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
                    this.symbols = symbols;
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
                    this.alphabet = letters;
                    break;
                case "PRODUCTLINES":

                    parts = input.Split(',');

                    if (parts[0].ToUpper() == "END")
                    {
                        output = "Type the StartSymbols \n";
                        output += "Example: A,C \n";
                        partToFill = "STARTSYMBOL";
                    }
                    else
                    {
                        parts = input.Split(',');
                        if (parts.Length == 3)
                        {
                            string startSymbol = "";
                            string transitionLetter = "";
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
                                        transitionLetter = letterlist[index];
                                        break;
                                    case 2:
                                        endSymbol = letterlist[index];
                                        break;
                                }
                            }
                            if (containsSymbol(startSymbol) && containsSymbol(endSymbol) && containsLetter(transitionLetter))
                            {
                                ProductLine productLine = new ProductLine(startSymbol, transitionLetter, endSymbol);
                                productionLines.Add(productLine);
                            }
                            else
                            {
                                output = "The given values are not available in the grammar \n";
                            }
                        }
                    }
                    break;
                case "STARTSYMBOL":
                    List<string> startsymbols = new List<string>();

                    parts = input.Split(',');
                    for (int i = 0; i < parts.Length; i++)
                    {
                        if (containsSymbol(parts[i]))
                        {
                            string symbol = parts[i];
                            string[] symbollist = symbol.Split(' ');
                            if (symbollist.Length == 1)
                                symbol = symbollist[0];
                            else if (symbollist.Length == 2)
                                symbol = symbollist[1];
                            startsymbols.Add(symbol);
                        }
                    }
                    this.startSymbols = startsymbols;
                    partToFill = "ENDSYMBOLS";
                    output = "Type the EndSymbols \n";
                    output += "Example: B,C \n";
                    break;
                case "ENDSYMBOLS":
                    List<string> endsymbols = new List<string>();

                    parts = input.Split(',');
                    for (int i = 0; i < parts.Length; i++)
                    {
                        if (containsSymbol(parts[i]))
                        {
                            string symbol = parts[i];
                            string[] symbollist = symbol.Split(' ');
                            if (symbollist.Length == 1)
                                symbol = symbollist[0];
                            else if (symbollist.Length == 2)
                                symbol = symbollist[1];
                            endsymbols.Add(symbol);
                        }
                    }
                    this.endSymbols = endsymbols;

                    partToFill = "";
                    filling = false;
                    output = "Grammar filled \n";

                    break;
            }

            return output;
        }

        private void processAutomaton(AutomatonCore automaton)
        {
            foreach(AutomatonNodeCore node in automaton.nodes)
            {
                symbols.Add(node.stateName);
                if (node.isBeginNode)
                    startSymbols.Add(node.stateName);
                if (node.isEndNode)
                    endSymbols.Add(node.stateName);

                foreach (AutomatonTransition trans in node.children)
                {
                    foreach (char c in trans.acceptedSymbols)
                    {
                        ProductLine p = new ProductLine(node.stateName, c.ToString(), trans.automatonNode.stateName);
                        productionLines.Add(p);

                        if (!alphabet.Contains(c.ToString()))
                            alphabet.Add(c.ToString());
                    }
                }
            }

            filling = false;
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

            //StartSymbols
            description += "Startsymbols = {";
            for (int i = 0; i < startSymbols.Count; i++)
            {
                description += startSymbols[i];
                if (i != startSymbols.Count - 1)
                    description += ", ";
            }
            description += "}\n";
            
            //EndSymbols
            description += "Endsymbols = {";
            for (int i = 0; i < endSymbols.Count; i++)
            {
                description += endSymbols[i];
                if (i != endSymbols.Count - 1)
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
