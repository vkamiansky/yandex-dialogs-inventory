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

        protected readonly Func<UserInput, string> GetInputField;

        public CommandTemplate(ParsedPhraseType phraseType, Func<UserInput, string> inputField, params string[] regexParts)
        {
            GetInputField = inputField;
            PhraseType = phraseType;
            var expression = InsertWordAroundWords(regexParts, RegexHelper.CanBeIgnoreWordPattern);
            Regex = new Regex(@"^" + expression + @"$", RegexOptions.None);
        }

        public bool TryParse(UserInput input, out object data)
        {
            var inputFieldValue = GetInputField(input);
            if(string.IsNullOrEmpty(inputFieldValue))
            {
                data = null;
                return false;
            }

            var result = Regex.Match(inputFieldValue);
            if (!result.Success)
            {
                data = null;
                return false;
            }
            data = GetObject(result, input.CultureInfo);
            return true;
        }

        protected virtual object GetObject(Match match, CultureInfo cultureInfo)
        {
            return null;
        }
    }
}
