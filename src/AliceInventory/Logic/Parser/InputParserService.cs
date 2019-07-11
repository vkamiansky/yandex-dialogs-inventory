using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AliceInventory.Logic.Parser
{
    public class InputParserService : IInputParserService
    {
        // Order by priority
        private static readonly CommandTemplate[] CommandsTemplates = 
        {
            //Email
            new CommandTemplate(ParsedPhraseType.SendMail,
                RegexHelper.SendWord, RegexHelper.MailWord),
            new CommandTemplate(ParsedPhraseType.SendMail,
                RegexHelper.SendWord, "на", RegexHelper.MailWord),
            new CommandTemplateWithEmail(ParsedPhraseType.SendMail,
                RegexHelper.SendWord, "на", RegexHelper.Email),
            new CommandTemplateWithEmail(ParsedPhraseType.Mail,
                RegexHelper.Email),
            new CommandTemplate(ParsedPhraseType.DeleteMail,
                RegexHelper.DeleteWord, RegexHelper.MailWord),

            //Cancel
            new CommandTemplate(ParsedPhraseType.Cancel, RegexHelper.CancelWord),

            //Accept and decline
            new CommandTemplate(ParsedPhraseType.Accept, RegexHelper.AcceptWord),
            new CommandTemplate(ParsedPhraseType.Decline, RegexHelper.DeclineWord),

            //Read List
            new CommandTemplate(ParsedPhraseType.ReadList, $"что в {RegexHelper.ListWord}"),
            new CommandTemplate(ParsedPhraseType.ReadList, RegexHelper.ReadWord),
            new CommandTemplate(ParsedPhraseType.ReadList, RegexHelper.ListWord),
            new CommandTemplate(ParsedPhraseType.ReadList, RegexHelper.ReadWord, RegexHelper.ListWord),

            //Clear
            new CommandTemplate(ParsedPhraseType.Clear, RegexHelper.ClearWord),
            new CommandTemplate(ParsedPhraseType.Clear, RegexHelper.ClearWord, RegexHelper.ListWord),

            //Greeting
            new CommandTemplate(ParsedPhraseType.Hello, RegexHelper.HelloPattern),
            new CommandTemplate(ParsedPhraseType.Help, RegexHelper.HelpWord),
            new CommandTemplate(ParsedPhraseType.Exit, RegexHelper.ExitWord),

            //Delete
            new CommandTemplateWithEntry(ParsedPhraseType.Delete,
                RegexHelper.DeleteWord, RegexHelper.EntryName, RegexHelper.EntryQuantity, RegexHelper.EntryUnit),
            new CommandTemplateWithEntry(ParsedPhraseType.Delete,
                RegexHelper.DeleteWord, RegexHelper.EntryName, RegexHelper.EntryQuantity),
            new CommandTemplateWithEntry(ParsedPhraseType.Delete,
                RegexHelper.DeleteWord, RegexHelper.EntryQuantity, RegexHelper.EntryUnit, RegexHelper.EntryName),
            new CommandTemplateWithEntry(ParsedPhraseType.Delete,
                RegexHelper.DeleteWord, RegexHelper.EntryQuantity, RegexHelper.EntryName),
            new CommandTemplateWithEntry(ParsedPhraseType.Delete,
                RegexHelper.DeleteWord, RegexHelper.EntryUnit, RegexHelper.EntryName),
            new CommandTemplateWithEntry(ParsedPhraseType.Delete,
                RegexHelper.DeleteWord, RegexHelper.EntryName),
            
            //More
            new CommandTemplateWithEntry(ParsedPhraseType.More,
                RegexHelper.MoreWord, RegexHelper.EntryQuantity),
            new CommandTemplateWithEntry(ParsedPhraseType.More,
                RegexHelper.MoreWord, RegexHelper.EntryUnit),
            new CommandTemplateWithEntry(ParsedPhraseType.More,
                RegexHelper.MoreWord, RegexHelper.EntryQuantity, RegexHelper.EntryUnit),
            new CommandTemplateWithEntry(ParsedPhraseType.More,
                RegexHelper.MoreWord, RegexHelper.EntryUnit, RegexHelper.EntryQuantity),
            new CommandTemplateWithEntry(ParsedPhraseType.More,
                RegexHelper.MoreWord, RegexHelper.EntryName, RegexHelper.EntryQuantity, RegexHelper.EntryUnit),
            new CommandTemplateWithEntry(ParsedPhraseType.More,
                RegexHelper.MoreWord, RegexHelper.EntryName, RegexHelper.EntryQuantity),
            new CommandTemplateWithEntry(ParsedPhraseType.More,
                RegexHelper.MoreWord, RegexHelper.EntryQuantity, RegexHelper.EntryUnit, RegexHelper.EntryName),
            new CommandTemplateWithEntry(ParsedPhraseType.More,
                RegexHelper.MoreWord, RegexHelper.EntryQuantity, RegexHelper.EntryName),
            new CommandTemplateWithEntry(ParsedPhraseType.More,
                RegexHelper.MoreWord, RegexHelper.EntryUnit, RegexHelper.EntryName),
            new CommandTemplateWithEntry(ParsedPhraseType.More,
                RegexHelper.MoreWord, RegexHelper.EntryName),

            // Add
            new CommandTemplateWithEntry(ParsedPhraseType.Add,
                RegexHelper.AddWord, RegexHelper.EntryName, RegexHelper.EntryQuantity, RegexHelper.EntryUnit),
            new CommandTemplateWithEntry(ParsedPhraseType.Add,
                RegexHelper.AddWord, RegexHelper.EntryQuantity, RegexHelper.EntryUnit, RegexHelper.EntryName),
            new CommandTemplateWithEntry(ParsedPhraseType.Add,
                RegexHelper.EntryQuantity, RegexHelper.EntryUnit, RegexHelper.EntryName),
            new CommandTemplateWithEntry(ParsedPhraseType.Add,
                RegexHelper.EntryUnit, RegexHelper.EntryName),
            new CommandTemplateWithEntry(ParsedPhraseType.Add,
                RegexHelper.AddWord, RegexHelper.EntryQuantity, RegexHelper.EntryName),
            new CommandTemplateWithEntry(ParsedPhraseType.Add,
                RegexHelper.AddWord, RegexHelper.EntryUnit, RegexHelper.EntryQuantity, RegexHelper.EntryName),
            new CommandTemplateWithEntry(ParsedPhraseType.Add,
                RegexHelper.AddWord, RegexHelper.EntryName, RegexHelper.EntryUnit, RegexHelper.EntryQuantity),
            new CommandTemplateWithEntry(ParsedPhraseType.Add,
                RegexHelper.AddWord, RegexHelper.EntryUnit, RegexHelper.EntryName),
            new CommandTemplateWithEntry(ParsedPhraseType.Add,
                RegexHelper.AddWord, RegexHelper.EntryName, RegexHelper.EntryQuantity),
            new CommandTemplateWithEntry(ParsedPhraseType.Add,
                RegexHelper.AddWord, RegexHelper.EntryName),

            //Low priority command
            new CommandTemplateWithEntry(ParsedPhraseType.Add,
                RegexHelper.EntryName, RegexHelper.EntryQuantity, RegexHelper.EntryUnit),
            new CommandTemplateWithEntry(ParsedPhraseType.Add,
                RegexHelper.EntryQuantity, RegexHelper.EntryName),
        };

        public ParsedCommand ParseInput(string input, CultureInfo cultureInfo)
        {
            if (string.IsNullOrEmpty(input))
                return new ParsedCommand() { Type = ParsedPhraseType.Hello};

            input = NormalizeString(input, cultureInfo);

            foreach (var template in CommandsTemplates)
            {
                if (template.TryParse(input, out var data, cultureInfo))
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

        private static string NormalizeString(string input, CultureInfo cultureInfo)
        {
            input = input.ToLower(cultureInfo);
            input = RegexHelper.IgnoreWord.Replace(input, "");
            input = RegexHelper.MultipleSpaces.Replace(input, " ");
            if (input.StartsWith(" "))
                input = input.Substring(1, input.Length - 1);
            if (input.EndsWith(" "))
                input = input.Substring(0, input.Length - 1);
            return input;
        }
    }
}
