using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace FormeleMethodenPracticum.model
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
        private static readonly char[] validCharacters = {'$', '|','*','+','{','}','(',')'}; //And normal letters/numerals for language

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
        
        public static void ParseRegex(string expression)
        {
             
        }
    }

}
