using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FormeleMethodenPracticum.Model
{
    /**
     * Let's do regex!
     * The following characters make up the regex language, excluding terminals
     *  '$'         := The empty string character
     *  '|'         := The OR operator
     *  '*'         := The zero-or-more operator
     *  '+'         := The one-or-more operator
     *  '?'         := The zero-or-one operator
     *  '.'         := The any character operator
     *  '-'         := The range operator
     *  '{' & '}'   := The interval brackets
     *  '(' & ')'   := Your run-of-the-mill bracketty brackets.
     *  'ε'         := Epsilon (if needed/supported)
     *  
     *  We will not always use them
     */
    public static class Regex
    {
        private static readonly char[] validCharacters = {'$',
                                                          '?',
                                                          '|',
                                                          '*',
                                                          '+',
                                                          '{','}',
                                                          '(',')',
                                                          ' '}; //And normal letters/numerals for language


        public static void ParseRegex(string exp)
        {
            if (!validifyExp(exp))
            {
                Window.INSTANCE.WriteLine("Invalid expression.");
                return;
            }
            else
            {
                OperationTree ops = buildOperatorTree(exp);
            }
        }

        private static OperationTree buildOperatorTree(string exp)
        {
            OperationTree operations = new OperationTree();
            if (exp.Length == 0)
                return null;
            //Set first operator to start operation
            operations.Op = OpType.START_OP;

            if (validCharacters.Contains<char>(exp[0])) //Is an operator, add to tree
            {
                switch (exp[0])
                {
                    case '{':
                        //Skip to matching bracket, remove them 
                        break;
                    case '(':
                        //Skip to matching bracket, remove them
                        break;
                    case '+':
                        //Add to tree
                        break;
                    case '*':
                        //Add to tree
                        break;
                    case '|':
                        //Add to tree
                        break;
                }
            }
            else if(char.IsLetterOrDigit(exp[0]))
            {
                //Terminal
            }
            else //Not a valid character (Should have been detected by validifyExp
            {
                return null;
            }

            //*Should* not be reachable
           if (operations.Count == 0)
                return null; //If there's no operations, just break everything
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
                    Window.INSTANCE.WriteLine(check + " : " + c);
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
                    if(OpLeft != null)
                    {
                        if (OpLeft.Op == OpType.TERMINAL)
                            n += 1;
                        else
                            n += OpLeft.Count + 1;
                    }
                    if(OpRight != null)
                    {
                        if (OpRight.Op == OpType.TERMINAL)
                            n += 1;
                        else
                            n += OpRight.Count + 1;
                    }
                    return n;
                }
            }

            public OperationTree OpLeft;
            
            public char OpChar; //Operator's char representation (handy for terminals)
            public OpType Op;   //Operator as type

            public OperationTree OpRight; 
        }

        public enum OpType
        {
            START_OP,       // Not a char
            ZERO_OR_MORE,   // *
            ONE_OR_MORE,    // +
            OR,             // | 
            DOT,            // Implied or .
            TERMINAL        // Letter of defined alphabet (a,b .. etc)
        }
    }
}
