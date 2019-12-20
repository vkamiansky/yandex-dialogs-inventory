using System.Globalization;

namespace AliceInventory.Logic.Parser
{
    public class InputParserService : IInputParserService
    {
        // Order by priority
        private static readonly CommandTemplate[] CommandsTemplates =
        {
            //Email
            new CommandTemplate(ParsedPhraseType.SendMail, x => x.Prepared,
                RegexHelper.SendWord, RegexHelper.MailWord),
            new CommandTemplateWithEmail(ParsedPhraseType.SendMail, x => x.Raw,
                RegexHelper.SendWord, RegexHelper.Email),
            new CommandTemplate(ParsedPhraseType.SendMail, x => x.Prepared,
                RegexHelper.SendWord, "на", RegexHelper.MailWord),
            new CommandTemplate(ParsedPhraseType.SendMail, x => x.Prepared,
                RegexHelper.SendWord, RegexHelper.ListWord, "на", RegexHelper.MailWord),
            new CommandTemplateWithEmail(ParsedPhraseType.SendMail, x => x.Raw,
                RegexHelper.SendWord, "на", RegexHelper.Email),
            new CommandTemplateWithEmail(ParsedPhraseType.SendMail, x => x.Raw,
                RegexHelper.SendWord, RegexHelper.ListWord, "на", RegexHelper.Email),
            new CommandTemplateWithEmail(ParsedPhraseType.SendMail, x => x.Raw,
                RegexHelper.SendWord, RegexHelper.ListWord, RegexHelper.Email),
            new CommandTemplateWithEmail(ParsedPhraseType.Mail, x => x.Raw,
                RegexHelper.Email),
            new CommandTemplate(ParsedPhraseType.DeleteMail, x => x.Prepared,
                RegexHelper.DeleteWord, RegexHelper.MailWord),

            //Cancel
            new CommandTemplate(ParsedPhraseType.Cancel, x => x.Prepared, RegexHelper.CancelWord),
            new CommandTemplate(ParsedPhraseType.Cancel, x => x.Button, RegexHelper.CancelWord),

            //Accept and decline
            new CommandTemplate(ParsedPhraseType.Accept,  x => x.Button, RegexHelper.AcceptWord),
            new CommandTemplate(ParsedPhraseType.Accept,  x => x.Prepared, RegexHelper.AcceptWord),
            new CommandTemplate(ParsedPhraseType.Decline, x => x.Button, RegexHelper.DeclineWord),
            new CommandTemplate(ParsedPhraseType.Decline, x => x.Prepared, RegexHelper.DeclineWord),

            //Read List
            new CommandTemplate(ParsedPhraseType.ReadList, x => x.Prepared, $"что в {RegexHelper.ListWord}"),
            new CommandTemplate(ParsedPhraseType.ReadList, x => x.Prepared, RegexHelper.ReadWord),
            new CommandTemplate(ParsedPhraseType.ReadList, x => x.Prepared, RegexHelper.ListWord),
            new CommandTemplate(ParsedPhraseType.ReadList, x => x.Prepared, RegexHelper.ReadWord, RegexHelper.ListWord),
            new CommandTemplate(ParsedPhraseType.ReadList, x => x.Button, RegexHelper.ReadWord, RegexHelper.ListWord),

            //Clear
            new CommandTemplate(ParsedPhraseType.Clear, x => x.Prepared, RegexHelper.ClearWord),
            new CommandTemplate(ParsedPhraseType.Clear, x => x.Button, RegexHelper.ClearWord),
            new CommandTemplate(ParsedPhraseType.Clear, x => x.Prepared, RegexHelper.ClearWord, RegexHelper.ListWord),
            new CommandTemplate(ParsedPhraseType.Clear, x => x.Button, RegexHelper.ClearWord, RegexHelper.ListWord),

            //Greeting
            new CommandTemplate(ParsedPhraseType.Hello, x => x.Prepared, RegexHelper.HelloPattern),
            new CommandTemplate(ParsedPhraseType.Help, x => x.Prepared, RegexHelper.HelpWord),
            new CommandTemplate(ParsedPhraseType.Help, x => x.Button, RegexHelper.HelpWord),
            new CommandTemplate(ParsedPhraseType.Exit, x => x.Prepared, RegexHelper.ExitWord),
            new CommandTemplate(ParsedPhraseType.Exit, x => x.Button, RegexHelper.ExitWord),

            //Delete
            new CommandTemplateWithEntry(ParsedPhraseType.Delete, x => x.Prepared,
                RegexHelper.DeleteWord, RegexHelper.EntryName, RegexHelper.EntryQuantity, RegexHelper.EntryUnit),
            new CommandTemplateWithEntry(ParsedPhraseType.Delete, x => x.Prepared,
                RegexHelper.DeleteWord, RegexHelper.EntryName, RegexHelper.EntryQuantity),
            new CommandTemplateWithEntry(ParsedPhraseType.Delete, x => x.Prepared,
                RegexHelper.DeleteWord, RegexHelper.EntryQuantity, RegexHelper.EntryUnit, RegexHelper.EntryName),
            new CommandTemplateWithEntry(ParsedPhraseType.Delete, x => x.Prepared,
                RegexHelper.DeleteWord, RegexHelper.EntryQuantity, RegexHelper.EntryName),
            new CommandTemplateWithEntry(ParsedPhraseType.Delete, x => x.Prepared,
                RegexHelper.DeleteWord, RegexHelper.EntryUnit, RegexHelper.EntryName),
            new CommandTemplateWithEntry(ParsedPhraseType.Delete, x => x.Prepared,
                RegexHelper.DeleteWord, RegexHelper.EntryName),
            
            //More
            new CommandTemplateWithEntry(ParsedPhraseType.More, x => x.Prepared,
                RegexHelper.MoreWord, RegexHelper.EntryQuantity),
            new CommandTemplateWithEntry(ParsedPhraseType.More, x => x.Prepared,
                RegexHelper.MoreWord, RegexHelper.EntryUnit),
            new CommandTemplateWithEntry(ParsedPhraseType.More, x => x.Prepared,
                RegexHelper.MoreWord, RegexHelper.EntryQuantity, RegexHelper.EntryUnit),
            new CommandTemplateWithEntry(ParsedPhraseType.More, x => x.Prepared,
                RegexHelper.MoreWord, RegexHelper.EntryUnit, RegexHelper.EntryQuantity),
            new CommandTemplateWithEntry(ParsedPhraseType.More, x => x.Prepared,
                RegexHelper.MoreWord, RegexHelper.EntryName, RegexHelper.EntryQuantity, RegexHelper.EntryUnit),
            new CommandTemplateWithEntry(ParsedPhraseType.More, x => x.Prepared,
                RegexHelper.MoreWord, RegexHelper.EntryName, RegexHelper.EntryQuantity),
            new CommandTemplateWithEntry(ParsedPhraseType.More, x => x.Prepared,
                RegexHelper.MoreWord, RegexHelper.EntryQuantity, RegexHelper.EntryUnit, RegexHelper.EntryName),
            new CommandTemplateWithEntry(ParsedPhraseType.More, x => x.Prepared,
                RegexHelper.MoreWord, RegexHelper.EntryQuantity, RegexHelper.EntryName),
            new CommandTemplateWithEntry(ParsedPhraseType.More, x => x.Prepared,
                RegexHelper.MoreWord, RegexHelper.EntryUnit, RegexHelper.EntryName),
            new CommandTemplateWithEntry(ParsedPhraseType.More, x => x.Prepared,
                RegexHelper.MoreWord, RegexHelper.EntryName),

            // Add
            new CommandTemplateWithEntry(ParsedPhraseType.Add, x => x.Prepared,
                RegexHelper.AddWord, RegexHelper.EntryName, RegexHelper.EntryQuantity, RegexHelper.EntryUnit),
            new CommandTemplateWithEntry(ParsedPhraseType.Add, x => x.Prepared,
                RegexHelper.AddWord, RegexHelper.EntryQuantity, RegexHelper.EntryUnit, RegexHelper.EntryName),
            new CommandTemplateWithEntry(ParsedPhraseType.Add, x => x.Prepared,
                RegexHelper.EntryQuantity, RegexHelper.EntryUnit, RegexHelper.EntryName),
            new CommandTemplateWithEntry(ParsedPhraseType.Add, x => x.Prepared,
                RegexHelper.EntryUnit, RegexHelper.EntryName),
            new CommandTemplateWithEntry(ParsedPhraseType.Add, x => x.Prepared,
                RegexHelper.AddWord, RegexHelper.EntryQuantity, RegexHelper.EntryName),
            new CommandTemplateWithEntry(ParsedPhraseType.Add, x => x.Prepared,
                RegexHelper.AddWord, RegexHelper.EntryUnit, RegexHelper.EntryQuantity, RegexHelper.EntryName),
            new CommandTemplateWithEntry(ParsedPhraseType.Add, x => x.Prepared,
                RegexHelper.AddWord, RegexHelper.EntryName, RegexHelper.EntryUnit, RegexHelper.EntryQuantity),
            new CommandTemplateWithEntry(ParsedPhraseType.Add, x => x.Prepared,
                RegexHelper.AddWord, RegexHelper.EntryUnit, RegexHelper.EntryName),
            new CommandTemplateWithEntry(ParsedPhraseType.Add, x => x.Prepared,
                RegexHelper.AddWord, RegexHelper.EntryName, RegexHelper.EntryQuantity),
            new CommandTemplateWithEntry(ParsedPhraseType.Add, x => x.Prepared,
                RegexHelper.AddWord, RegexHelper.EntryName),

            //Low priority command
            new CommandTemplateWithEntry(ParsedPhraseType.Add, x => x.Prepared,
                RegexHelper.EntryName, RegexHelper.EntryQuantity, RegexHelper.EntryUnit),
            new CommandTemplateWithEntry(ParsedPhraseType.Add, x => x.Prepared,
                RegexHelper.EntryQuantity, RegexHelper.EntryName),
        };

        public ParsedCommand ParseInput(UserInput input)
        {
            if (IsEmptyInput(input))
                return new ParsedCommand() { Type = ParsedPhraseType.Hello };

            input = NormalizeInput(input, input.CultureInfo);

            foreach (var template in CommandsTemplates)
            {
                if (template.TryParse(input, out var data))
                {
                    return new ParsedCommand()
                    {
                        Type = template.PhraseType,
                        Data = data
                    };
                }
            }

            return new ParsedCommand()
            {
                Type = ParsedPhraseType.UnknownCommand
            };
        }

        private static string NormalizeString(string str, CultureInfo cultureInfo)
        {
            if (string.IsNullOrWhiteSpace(str))
                return string.Empty;
            str = str.ToLower(cultureInfo);
            str = RegexHelper.IgnoreWord.Replace(str, "");
            str = RegexHelper.MultipleSpaces.Replace(str, " ");
            str = str.Trim();
            return str;
        }

        private static UserInput NormalizeInput(UserInput input, CultureInfo cultureInfo)
        {
            return new UserInput
            {
                Raw = NormalizeString(input.Raw, cultureInfo),
                Prepared = NormalizeString(input.Prepared, cultureInfo),
                Button = NormalizeString(input.Button, cultureInfo),
                CultureInfo = input.CultureInfo
            };
        }

        private static bool IsEmptyInput(UserInput input)
        {
            return string.IsNullOrEmpty(input.Raw)
                && string.IsNullOrEmpty(input.Prepared)
                && string.IsNullOrEmpty(input.Button);
        }
    }
}
