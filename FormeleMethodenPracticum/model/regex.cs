using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace FormeleMethodenPracticum.Model
{
    /**
     * Let's do regex!
     * The following characters make up our language
     *  '$'         := The empty string character
     *  '|'         := The OR operator
     *  '*'         := The zero-or-more operator
     *  '+'         := The one-or-more operator
     *  '{' & '}'   := The collections brackets
     *  '(' & ')'   := Your run-off-the-mill bracketty brackets.
     */
    public static class Regex
    {
        private static readonly char[] validCharacters = {'$', '|','*','+','{','}','(',')', ' '}; //And normal letters/numerals for language

        /**
         * Automata/stack should fix this nicely. 
         * Not necessary now, TODO
         /*
        bool isValidRegex(string expression)
        {
            bool isValid = false;
            return isValid;
        }
        /**/
        
        public static void ParseRegex(string exp)
        {
            if (!validifyExp(exp))
            {
                Window.INSTANCE.WriteLine("Invalid expression.");
                return;
            }
            else
            {
                //Build operator tree
            }
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
    }
}
