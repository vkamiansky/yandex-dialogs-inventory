using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AliceInventory.Logic.Parser
{
    public class CommandTemplateWithEmail : CommandTemplate
    {
        public CommandTemplateWithEmail(string expression) : base(expression) { }

        protected override object GetObject(Match match, CultureInfo cultureInfo)
        {
            return match.Groups[0];
        }
    }
}
