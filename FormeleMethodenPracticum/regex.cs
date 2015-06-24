using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FormeleMethodenPracticum
{
    /**
     * Let's do regex!
     * The following characters make up the regex language, excluding terminals
     *  '$'         := The empty string character
     *  '|'         := The OR operator
     *  '.'         := The DOT operator
     *  '*'         := The zero-or-more operator
     *  '+'         := The one-or-more operator
     *  '?'         := The zero-or-one operator
     *  '-'         := The range operator
     *  '{' & '}'   := The interval brackets
     *  '(' & ')'   := Your run-of-the-mill bracketty brackets.
     *  'ε'         := Epsilon (if needed/supported)
     *  
     *  We will not always use them
     */
    public static class MyRegex
    {
        private static readonly char[] validCharacters = {'$',
                                                          '?',
                                                          '|',
                                                          '*',
                                                          '+',
                                                          '.',
                                                          '{','}',
                                                          '(',')',
                                                          ' '}; //And normal letters/numerals for language


        public static void ParseRegex(string exp)
        {
            OperationTree ops;
            if (!validifyExp(exp))
            {
                Window.INSTANCE.WriteLine("Invalid expression.");
                return;
            }
            else
            {
                exp = addDotOperations(exp);
                ops = buildOperatorTree(exp);
                if (ops != null)
                    Window.INSTANCE.WriteLine(ops.ToString());
                else
                    Window.INSTANCE.WriteLine("Operator Tree could not be constructed.");
            }
        }

        //Add implied dots for parsing ease
        private static string addDotOperations(string exp)
        {
            for (int i = 0; i < exp.Length - 1; i++) //Length is already guarenteed larger than 1
            {
                if ((char.IsLetterOrDigit(exp[i]) || exp[i] == ')') && (char.IsLetterOrDigit(exp[i + 1]) || exp[i + 1] == '('))
                {
                    exp = exp.Substring(0, i + 1) + "." + exp.Substring(i + 1, exp.Length - (i + 1));
                }
            }
            Window.INSTANCE.WriteLine("Mutated to add dots: new exp = " + exp);
            Window.INSTANCE.WriteLine("Parsing " + exp + " for highest precedence operator");
            return exp;
        }

        //Build a binary tree of 
        private static OperationTree buildOperatorTree(string exp)
        {
            OperationTree operations = new OperationTree();
            if (exp.Length == 0)
                return null;
            //If the entire exp is within brackets, obsolete
            if (exp[0] == '(' && exp[exp.Length-1] == ')')
            {
                exp = exp.Substring(1, exp.Length - 2);
                Window.INSTANCE.WriteLine("Removed brackets: new exp = " + exp);
            }

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
                            Window.INSTANCE.WriteLine("Unimplemented character in regex");
                            return null;
                    }
                }
                else if(char.IsLetterOrDigit(exp[i]))//Language character
                {
                    if (bracketDepth == 0 && thisOp.Type < RegexOperator.OpType.TERMINAL)
                    {
                        thisOp = new RegexOperator(RegexOperator.OpType.TERMINAL, exp[i], i);
                    }
                }
            }
            operations.Op = thisOp;
            if (thisOp.Type != RegexOperator.OpType.TERMINAL)
            {
                Window.INSTANCE.WriteLine("Highest presedence op : " + thisOp.Character + " in " + exp);
                string leftRegex = exp.Substring(0, thisOp.Index);
                Window.INSTANCE.WriteLine("Left : " + leftRegex);
                operations.OpLeft = buildOperatorTree(leftRegex);

                if (thisOp.Index != exp.Length)
                {
                    string rightRegex = exp.Substring(thisOp.Index + 1, exp.Length - (thisOp.Index + 1));
                    Window.INSTANCE.WriteLine("Right : " + rightRegex);
                    operations.OpRight = buildOperatorTree(rightRegex);
                }
            }
            return operations;
        }


        private static bool validifyExp(string exp)
        {
            for (int i = 0; i < exp.Length; i++)
            {
                if (!char.IsLetterOrDigit(exp[i]))
                    if (!validCharacters.Contains(exp[i]))
                        return false;
            }

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
                        return false;
                    }
                    char check = brackets.Pop();
                    if (((c == '}') && (check != '{')) || ((c == ')') && (check != '(')))
                    {
                        return false;
                    }
                }
            }
            if (brackets.Count != 0)
                return false;
            return true;
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

            public bool isTerminal //Assume OpLeft & OpRight
            {
                get
                {
                    return Op.Type == RegexOperator.OpType.TERMINAL;
                }
            }

            public OperationTree OpLeft;

            public RegexOperator Op;   //Operator as struct

            public OperationTree OpRight;

            public override string ToString()
            {
                string prefix = "";
                string retval = "+"+ prefix + Op + "\n";
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
                OR = 10,              // | 
                ZERO_OR_MORE = 9,     // *
                ONE_OR_MORE = 9,      // +
                ZERO_OR_ONE = 9,      // ?
                DOT = 9,              // Implied or .
                TERMINAL = 8,         // Letter of defined alphabet (a,b .. etc)
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
                switch (Type)
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
