using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace AliceInventory.Logic.Parser
{
    public class CommandTemplateWithEmail : CommandTemplate
    {
        public CommandTemplateWithEmail(ParsedPhraseType phraseType, Func<UserInput, string> inputField, params string[] regexParts) : base(phraseType, inputField, regexParts) { }

        protected override object GetObject(Match match, CultureInfo cultureInfo)
        {
            return match.Groups[RegexHelper.EmailGroupName].Value;
        }
    }
}
