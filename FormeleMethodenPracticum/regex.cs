using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FormeleMethodenPracticum.FiniteAutomatons;

namespace FormeleMethodenPracticum
{
    /**
     * Let's do regex!
     * The following characters could make up the regex language, excluding terminals...
     * 
     *  '$'         := The empty string character
     *  '|'         := The OR operator
     *  '.'         := The DOT operator
     *  '*'         := The zero-or-more operator
     *  '+'         := The one-or-more operator
     *  '?'         := The zero-or-one operator
     *  '-'         := The range operator
     *  '{' & '}'   := The interval brackets
     *  '(' & ')'   := Your run-of-the-mill bracketty brackets.
     *  'ε'         := Epsilon
     *  
     *  ...but we don't use all of them 
     */
    public static class MyRegex //An actual regex class already exists (System.Text.RegularExpressions)
    {
        private static readonly char[] validCharacters = {//'$', 
                                                          '?',
                                                          '|',
                                                          '*',
                                                          '+',
                                                          '.',
                                                          //'{','}',
                                                          '(',')',
                                                          ' '}; //And normal letters/numerals for language


        public static AutomatonCore ParseRegex(string exp)
        {
            OperationTree ops;
            mutateExp(exp, out exp); //Remove whitespaces & add dots
            if (!validifyExp(exp)) //Check for bracet consistency and invalid characters
            {
                Console.INSTANCE.WriteLine("ERR: Invalid expression.");
                return null;
            }
            ops = buildOperatorTree(exp);
            if (ops == null)
                Console.INSTANCE.WriteLine("ERR: Operator Tree could not be constructed.");
            Console.INSTANCE.WriteLine(ops.ToString()); //Dump
            AutomatonCore ndfa = buildNDFA(ops);
            if (ndfa == null)
                Console.INSTANCE.WriteLine("Constructing NDFA failed");
            return ndfa;
        }

        /**
         * Construct an NDFA using the Thompson construction method.
         */
        private static AutomatonCore buildNDFA(OperationTree parsedRegex)
        {
            //The (NDFA) core to hang our creation on.
            AutomatonCore core = new AutomatonCore(true);
            //The following variable is initialised incomplete
            LinkedList<AutomatonNodeCore> NDFA = thompsonSubset(parsedRegex);
            //We need to note the entry state and final state
            NDFA.First.Value.isBeginNode = true;
            NDFA.Last.Value.isEndNode = true;
            //And names to the states            
            int nameNumber = 1;
            foreach (AutomatonNodeCore n in NDFA)
            {
                int tmp = nameNumber; //Modifiable clone
                string name = string.Empty;
                while (--tmp >= 0)
                {
                    name = (char)('A' + tmp % 26) + name;
                    tmp /= 26;
                }
                n.stateName = name;
                nameNumber++;//next name
            }
            //Add to core
            foreach (AutomatonNodeCore n in NDFA)
            {
                core.nodes.Add(n);
            }
            return core;
        }

        /**
         * Assistant method for recursive Thompson construction
         * Do not call me outside of the above constructNDFA
         * The returned list has a number of requirements:
         * 1) the first state is the part that connects on the entry end of a black box
         * 2) the last state is the part that connects to the exit end of a black box
         * 3) All blackboxes are resolved on returning (recursive)
         */
        private static LinkedList<AutomatonNodeCore> thompsonSubset(OperationTree parsedRegex)
        {
            LinkedList<AutomatonNodeCore> subset = new LinkedList<AutomatonNodeCore>();
            //Assume the characters are parsed correctly
            switch (parsedRegex.Op.Character)
            {
                case '|':
                    /*                          BLACK BOX A 
                     *                       ε/             \ε
                     * OR construction  -->[A]               [B]--> 
                     *                       ε\             /ε
                     *                          BLACK BOX B 
                     */
                    #region OR
                    LinkedList<AutomatonNodeCore> orBlackBoxA, orBlackBoxB;
                    //An OR operation should always have a left branch and a right branch
                    orBlackBoxA = thompsonSubset(parsedRegex.OpLeft);
                    orBlackBoxB = thompsonSubset(parsedRegex.OpRight);
                    AutomatonNodeCore orStateA = new AutomatonNodeCore(),//Divergence point
                                      orStateB = new AutomatonNodeCore();//Convergence point
                    AutomatonTransition orEpsilonA = new AutomatonTransition(orBlackBoxA.First.Value), 
                                        orEpsilonB = new AutomatonTransition(orBlackBoxB.First.Value);
                    AutomatonTransition orEpsilonC = new AutomatonTransition(orStateB), 
                                        orEpsilonD = new AutomatonTransition(orStateB);
                    //The parts are ready, put them together
                    orStateA.children.Add(orEpsilonA);
                    orStateA.children.Add(orEpsilonB);
                    orBlackBoxA.Last.Value.children.Add(orEpsilonC);
                    orBlackBoxB.Last.Value.children.Add(orEpsilonD);
                    subset.AddFirst(orStateA);
                    foreach (AutomatonNodeCore n in orBlackBoxA)
                        subset.AddLast(n);
                    foreach (AutomatonNodeCore n in orBlackBoxB)
                        subset.AddLast(n);
                    subset.AddLast(orStateB);
#endregion
                    break;
                case '?':
                case '*':
                case '+':
                    /*                               + -------------------ε------------------> +
                    * ZERO-OR-ONE construction:  -->[A] -ε-> [B] -ε-> BLACK BOX -ε-> [C] -ε-> [D]-->
                    *                                       
                    *                                + -------------------ε------------------> +
                    * ZERO-OR-MORE construction: -->[A] -ε-> [B] -ε-> BLACK BOX -ε-> [C] -ε-> [D]-->   Notice the similarities?
                    *                                         + <---------ε---------- +               
                    *                                
                    * ONE-OR-MORE construction:  -->[A] -ε-> [B] -ε-> BLACK BOX -ε-> [C] -ε-> [D]-->
                    *                                         + <---------ε---------- +
                    */
                    #region ZERO-OR-MORE+ONE-OR-MORE+ZERO-OR-ONE
                    LinkedList<AutomatonNodeCore> thisBlackBox;
                    thisBlackBox = thompsonSubset(parsedRegex.OpLeft); //a #-OR-# op contains ONLY a left branch
                    #region DEBUG
#if DEBUG
                    if (parsedRegex.OpRight != null)
                    {
                        Console.INSTANCE.WriteLine("WARN: Right hand side of ZERO-OR-MORE operation present");
                        Console.INSTANCE.WriteLine("Duly noted, thoroughly ignored.");
                    }
#endif
                    #endregion
                    AutomatonNodeCore thisStateA = new AutomatonNodeCore(),
                                      thisStateB = new AutomatonNodeCore(),
                                      thisStateC = new AutomatonNodeCore(),
                                      thisStateD = new AutomatonNodeCore();
                    AutomatonTransition thisEpsilonA = new AutomatonTransition(thisStateB),
                                        specialEpsilonB = new AutomatonTransition(thisStateD),//Used in ? and *
                                        thisEpsilonC = new AutomatonTransition(thisBlackBox.First.Value),
                                        thisEpsilonD = new AutomatonTransition(thisStateC),
                                        thisEpsilonE = new AutomatonTransition(thisStateD),
                                        specialEpsilonF = new AutomatonTransition(thisStateB); //Used in + and *
                    thisStateA.children.Add(thisEpsilonA);
                    if(parsedRegex.Op.Character == '*' || parsedRegex.Op.Character == '?')
                        thisStateA.children.Add(specialEpsilonB);
                    thisStateB.children.Add(thisEpsilonC);
                    thisBlackBox.Last.Value.children.Add(thisEpsilonD);
                    thisStateC.children.Add(thisEpsilonE);
                    if (parsedRegex.Op.Character == '*' || parsedRegex.Op.Character == '+')                    
                        thisStateC.children.Add(specialEpsilonF);
                    subset.AddLast(thisStateA);
                    subset.AddLast(thisStateB);
                    foreach (AutomatonNodeCore n in thisBlackBox)
                        subset.AddLast(n);
                    subset.AddLast(thisStateC);
                    subset.AddLast(thisStateD);
#endregion
                    break;
                case '.':
                    /*
                     * DOT construction: --> BLACK BOX A -ε-> BLACK BOX B -->
                     */
                    #region DOT
                    LinkedList<AutomatonNodeCore> dotBlackBoxA, dotBlackBoxB;
                    //A DOT operation should always have a left branch and a right branch
                    dotBlackBoxA = thompsonSubset(parsedRegex.OpLeft);
                    dotBlackBoxB = thompsonSubset(parsedRegex.OpRight);
                    AutomatonTransition dotTransition = new AutomatonTransition(dotBlackBoxB.First.Value);
                    dotBlackBoxA.Last.Value.children.Add(dotTransition);
                    subset = dotBlackBoxA;
                    foreach (AutomatonNodeCore n in dotBlackBoxB)
                        subset.AddLast(n);
#endregion
                    break;
                default:
                    /*
                     * TERMINAL construction -'character'-> 
                     */
                    #region TERM
                    if (parsedRegex.isTerminal)
                    {
                        AutomatonNodeCore termStateA = new AutomatonNodeCore();
                        AutomatonNodeCore termStateB = new AutomatonNodeCore();
                        AutomatonTransition termTransition = new AutomatonTransition(termStateB);
                        termTransition.acceptedSymbols.Add(parsedRegex.Op.Character);
                        termStateA.children.Add(termTransition);
                        subset.AddLast(termStateA);
                        subset.AddLast(termStateB);
                    }
                    else
                        return null; //Terminal that's not a terminal
                    #endregion
                    break;
            }
            if (subset.Count == 0)
                return null;
            return subset;
        }

        //Build a binary tree of operations in the regex
        private static OperationTree buildOperatorTree(string exp)
        {
            OperationTree operations = new OperationTree();
            if (exp.Length == 0)
                return null;
            //If the entire exp is within brackets, obsolete
            if (exp[0] == '(' && exp[exp.Length - 1] == ')')
            {
                exp = exp.Substring(1, exp.Length - 2);
                #region DEBUG
#if DEBUG
                Console.INSTANCE.WriteLine("Removed brackets: new exp = " + exp);
#endif
                #endregion
            }
            #region DEBUG
#if DEBUG
            Console.INSTANCE.WriteLine("Parsing " + exp + " for highest precedence operator");
#endif
            #endregion
            //The operator for this iteration
            RegexOperator thisOp = new RegexOperator();

            if (exp.Length == 1)
            {
                thisOp = new RegexOperator(RegexOperator.OpType.TERMINAL, exp[0], 0);
            }


            //loop variables
            int bracketDepth = 0;
            Stack<char> brackets = new Stack<char>();

            //Behaviour per character
            for (int i = 0; i < exp.Length; i++)
            {
                if (validCharacters.Contains<char>(exp[i])) //Is an operator, check if it's the precedent operator
                {
                    switch (exp[i])
                    {
                        case '(': //BRACKET
                            //Remember bracket with index
                            bracketDepth--;
                            brackets.Push(exp[i]);
                            break;
                        case ')': //BRACKET
                            bracketDepth++;
                            char match = brackets.Pop();
                            if (match != '(')
                                return null; //Something went wrong
                            break;
                        case '+': //ONE-OR-MORE the #-OR-# operators actually go in sequence
                            if (bracketDepth == 0 && (thisOp.Type < RegexOperator.OpType.ONE_OR_MORE))
                            {
                                thisOp = new RegexOperator(RegexOperator.OpType.ONE_OR_MORE, exp[i], i);
                            }
                            break;
                        case '*': //ZERO-OR-MORE
                            if (bracketDepth == 0 && (thisOp.Type < RegexOperator.OpType.ZERO_OR_MORE))
                            {
                                thisOp = new RegexOperator(RegexOperator.OpType.ZERO_OR_MORE, exp[i], i);
                            }
                            break;
                        case '?': //ZERO-OR-ONE
                            if (bracketDepth == 0 && (thisOp.Type < RegexOperator.OpType.ZERO_OR_ONE))
                            {
                                thisOp = new RegexOperator(RegexOperator.OpType.ZERO_OR_ONE, exp[i], i);
                            }
                            break;
                        case '|': //OR
                            if (bracketDepth == 0 && (thisOp.Type < RegexOperator.OpType.OR))
                            {
                                thisOp = new RegexOperator(RegexOperator.OpType.OR, exp[i], i);
                            }
                            break;
                        case '.':
                            if (bracketDepth == 0 && thisOp.Type < RegexOperator.OpType.DOT)
                            {
                                thisOp = new RegexOperator(RegexOperator.OpType.DOT, '.', i);
                            }
                            break;
                        default:
                            Console.INSTANCE.WriteLine("ERR: Unimplemented character in regex");
                            return null;
                    }
                }
                else if (char.IsLetterOrDigit(exp[i]))//Language character
                {
                    if (bracketDepth == 0 && thisOp.Type < RegexOperator.OpType.TERMINAL)
                    {
                        thisOp = new RegexOperator(RegexOperator.OpType.TERMINAL, exp[i], i);
                    }
                }
            }
            operations.Op = thisOp;
            string leftRegex = null;
            string rightRegex = null;
            if (thisOp.Type != RegexOperator.OpType.TERMINAL)
            {

                leftRegex = exp.Substring(0, thisOp.Index);
                operations.OpLeft = buildOperatorTree(leftRegex);
                if (thisOp.Index != exp.Length)
                {
                    rightRegex = exp.Substring(thisOp.Index + 1, exp.Length - (thisOp.Index + 1));
                    operations.OpRight = buildOperatorTree(rightRegex);
                }
            }
            #region DEBUG
#if DEBUG
            Console.INSTANCE.WriteLine("Highest presedence op : " + thisOp.Character + " in " + exp);
            if (leftRegex != null)
                Console.INSTANCE.WriteLine("Left : " + leftRegex);
            else
                Console.INSTANCE.WriteLine("No left hand side");
            if (rightRegex != null)
                Console.INSTANCE.WriteLine("Right : " + rightRegex);
            else
                Console.INSTANCE.WriteLine("No right hand side");
#endif
            #endregion
            return operations;
        }

        /**
         * Validify expression on the following criteria (in no specific order)
         * 1) Valid characters
         * 2) Bracket consistency
         * 3) Is there atleast one character
         * 4) Does the expression start with a character?
         * 5) Are there any constructions like "a**" or "b+*+"?
         */
        private static bool validifyExp(string exp)
        {
            bool isValid = false;

            //Disallow starting with an operator
            if (exp[0] != '(' && !char.IsLetterOrDigit(exp[0]))
            {
                #region DEBUG
#if DEBUG
                Console.INSTANCE.WriteLine("ERR: Expression does not start with a terminal");
#endif
                #endregion
                return isValid;
            }
            for (int i = 0; i < exp.Length; i++)
            {
                //Disallow immediate chaining of *,+ or ?
                if (i + 1 < exp.Length) //It IS allowed to be the last character
                {
                    if ((exp[i] == '*' ||
                       exp[i] == '+' ||
                       exp[i] == '?')
                       &&
                       (exp[i + 1] == '*' ||
                       exp[i + 1] == '+' ||
                       exp[i + 1] == '?'))
                    {
                        #region DEBUG
#if DEBUG
                        Console.INSTANCE.WriteLine("ERR: Chained #-or-# operators");
#endif
                        #endregion
                        return isValid;
                    }
                }

                //Disallow unknown characters
                if (!char.IsLetterOrDigit(exp[i]))
                    if (!validCharacters.Contains(exp[i]))
                    {
                        #region DEBUG
#if DEBUG
                        Console.INSTANCE.WriteLine("ERR: regex contains invalid characters");
#endif
                        #endregion
                        return isValid; 
                    }
            }
            //Disallow mismatched brackets
            Stack<char> brackets = new Stack<char>();
            foreach (char c in exp)
            {
                if ((c == '{') || (c == '('))
                {
                    brackets.Push(c);
                }
                if ((c == '}') || (c == ')'))
                {
                    if (brackets.Count == 0)
                    {
                        #region DEBUG
#if DEBUG
                        Console.INSTANCE.WriteLine("ERR: brackets mismatched");
#endif
                        #endregion
                        return isValid;
                    }
                    char check = brackets.Pop();
                    if (((c == '}') && (check != '{')) || ((c == ')') && (check != '(')))
                    {
                        #region DEBUG
#if DEBUG
                        Console.INSTANCE.WriteLine("ERR: brackets mismatched");
#endif
                        #endregion
                        return isValid;
                    }
                }
            }
            if (brackets.Count != 0)
            {
                #region DEBUG
#if DEBUG
                Console.INSTANCE.WriteLine("ERR: brackets mismatched");
#endif
                #endregion
                return isValid;
            }

            //Disallow statement without terminals
            foreach (char c in exp)
            {
                isValid = char.IsLetterOrDigit(c) ? true : false;
                if (isValid)
                    break;
            }
            if (!isValid)
                #region DEBUG
#if DEBUG
                Console.INSTANCE.WriteLine("ERR: No terminals");
#endif
                #endregion
            return isValid;
        }

        /**
         * Let's assert we can trim any whitespaces in the regex
         * without impacting the expression.
         */
        private static void mutateExp(string exp, out string mutatedExp)
        {
            mutatedExp = new string(exp.ToCharArray()
                .Where(c => !Char.IsWhiteSpace(c))
                .ToArray());
            addDotOperations(mutatedExp, out mutatedExp);
            //That will be all
        }

        /**
         * Add implied dots for parsing ease
         */
        private static void addDotOperations(string exp, out string mutatedExp)
        {
            mutatedExp = exp;
            for (int i = 0; i < mutatedExp.Length - 1; i++) //Length is already guarenteed larger than 1
            {
                if ((char.IsLetterOrDigit(mutatedExp[i]) || 
                    mutatedExp[i] == ')' || 
                    mutatedExp[i] == '*' ||
                    mutatedExp[i] == '+' ||
                    mutatedExp[i] == '?')
                    && (char.IsLetterOrDigit(mutatedExp[i + 1]) || mutatedExp[i + 1] == '('))
                {
                    mutatedExp = mutatedExp.Substring(0, i + 1) + "." + mutatedExp.Substring(i + 1, mutatedExp.Length - (i + 1));
                }
            }

            #region DEBUG
#if DEBUG
            Console.INSTANCE.WriteLine("Mutated to add dots: new exp = " + exp);
#endif
            #endregion
        }

        private class OperationTree
        {
            public int Count
            {
                get //We'll do this every time we count because perfomance can bite me
                {
                    int n = 0;
                    if (OpLeft != null)
                    {
                        if (isTerminal)
                            n += 1;
                        else
                            n += OpLeft.Count + 1;
                    }
                    if (OpRight != null)
                    {
                        if (isTerminal)
                            n += 1;
                        else
                            n += OpRight.Count + 1;
                    }
                    return n;
                }
            }

            public bool isTerminal //Assumes OpLeft & OpRight are not present
            {
                get
                {
                    return Op.Type == RegexOperator.OpType.TERMINAL;
                }
            }

            public OperationTree OpLeft = null; //Staying null is common for terminals

            public RegexOperator Op;   //Operator as 'struct'

            public OperationTree OpRight = null;

            public override string ToString()
            {
                string prefix = "";
                string retval = "+" + prefix + Op + "\n";
                if (OpLeft != null)
                    retval += OpLeft.ToString(prefix + "   |");
                if (OpRight != null)
                    retval += OpRight.ToString(prefix + "   |");

                return retval;
            }

            private string ToString(string prefix)
            {
                string retval = prefix + Op + "\n";
                if (OpLeft != null)
                    retval += OpLeft.ToString(prefix + "   |");
                if (OpRight != null)
                    retval += OpRight.ToString(prefix + "   |");
                return retval;
            }
        }

        //Container for operators
        private class RegexOperator
        {
            public enum OpType //And their precedence
            {
                OR = 10,              // |, actually the only truly precedent operator
                ZERO_OR_MORE = 8,     // *
                ONE_OR_MORE = 8,      // +
                ZERO_OR_ONE = 8,      // ?
                DOT = 9,              // Implied or .
                TERMINAL = 7,         // Letter of defined alphabet (a,b .. etc)
                NOT_AN_OPERATOR = 0   // null replacement
            }

            public OpType Type;
            public char Character;
            public int Index;

            public RegexOperator(OpType type, char character, int index)
            {
                this.Type = type;
                this.Character = character;
                this.Index = index;
            }

            public RegexOperator()
            {
                this.Type = OpType.NOT_AN_OPERATOR;
                this.Character = '\0';
                this.Index = 0;
            }

            public override string ToString()
            {
                string retval = "op: ";
                switch (Type) //Switches on their enum value, not handy
                {
                    case OpType.OR:
                        retval += " OR ";
                        break;
                    case OpType.ZERO_OR_MORE:
                        if (Character == '*')
                            retval += " ZERO OR MORE ";
                        if (Character == '+')
                            retval += " ONE OR MORE ";
                        if (Character == '?')
                            retval += " ZERO OR ONE ";
                        if (Character == '.')
                            retval += " DOT ";
                        break;
                    case OpType.TERMINAL:
                        retval += " TERM ";
                        break;
                    case OpType.NOT_AN_OPERATOR:
                        retval += " NaN ";
                        return retval;
                }
                retval += '\'';
                retval += Character;
                retval += '\'';
                return retval;
            }
        }
    }
}
