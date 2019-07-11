using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace AliceInventory.Logic.Parser
{
    public class CommandTemplate
    {
        private static string InsertWordAroundWords(string[] words, string newWord)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append(@"(?:");
            builder.Append(newWord);
            builder.Append(@"\s)?");

            bool isFirst = true;

            foreach (var word in words)
            {
                if (!isFirst) builder.Append(@"\s");
                builder.Append(word);
                builder.Append(@"(?:\s");
                builder.Append(newWord);
                builder.Append(@")?");

                isFirst = false;
            }

            return builder.ToString();
        }

        public ParsedPhraseType PhraseType { get; }
        protected readonly Regex Regex;

        public CommandTemplate(ParsedPhraseType phraseType, params string[] regexParts)
        {
            PhraseType = phraseType;
            var expression = InsertWordAroundWords(regexParts, RegexHelper.CanBeIgnoreWordPattern);
            Regex = new Regex(@"^" + expression + @"$", RegexOptions.None);
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
