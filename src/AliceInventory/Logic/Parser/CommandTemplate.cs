using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AliceInventory.Logic.Parser
{
    public class CommandTemplate
    {
        private Regex Regex;

        public string RegularExpression
        {
            set
            {
                Regex = new Regex("^" + value + "$", RegexOptions.Compiled);
            }
        }

        public CommandTemplate(string expression)
        {
            RegularExpression = expression;
        }

        public bool TryParse(string input, out object data, CultureInfo cultureInfo)
        {
            var result = Regex.Match(input);

            if (!result.Success)
            {
                data = null;
                return false;
            }
            
            data = GetObject(result, cultureInfo);
            return true;
        }

        protected virtual object GetObject(Match match, CultureInfo cultureInfo)
        {
            return null;
        }
    }
}
