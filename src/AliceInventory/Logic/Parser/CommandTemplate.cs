using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace AliceInventory.Logic.Parser
{
    public class CommandTemplate
    {
        public InputProcessingCommand Command { get; }
        protected readonly Regex Regex;

        public CommandTemplate(InputProcessingCommand command, string expression)
        {
            Command = command;
            Regex = new Regex(@"^" + expression + @"(?:\s+|$)", RegexOptions.Compiled);
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
