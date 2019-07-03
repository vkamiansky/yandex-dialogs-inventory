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
            new CommandTemplate(ParsedCommandType.SendMail,
                RegexHelper.SendWord, RegexHelper.MailWord),
            new CommandTemplate(ParsedCommandType.SendMail,
                RegexHelper.SendWord, "на", RegexHelper.MailWord),
            new CommandTemplateWithEmail(ParsedCommandType.SendMailTo,
                RegexHelper.SendWord, "на", RegexHelper.Email),
            new CommandTemplateWithEmail(ParsedCommandType.AddMail,
                RegexHelper.Email),
            new CommandTemplate(ParsedCommandType.DeleteMail,
                RegexHelper.DeleteWord, RegexHelper.MailWord),

            //Cancel
            new CommandTemplate(ParsedCommandType.Cancel, RegexHelper.CancelWord),

            //Accept and decline
            new CommandTemplate(ParsedCommandType.Accept, RegexHelper.AcceptWord),
            new CommandTemplate(ParsedCommandType.Decline, RegexHelper.DeclineWord),

            //Read List
            new CommandTemplate(ParsedCommandType.ReadList, $"что в {RegexHelper.ListWord}"),
            new CommandTemplate(ParsedCommandType.ReadList, RegexHelper.ReadWord),
            new CommandTemplate(ParsedCommandType.ReadList, RegexHelper.ListWord),
            new CommandTemplate(ParsedCommandType.ReadList, RegexHelper.ReadWord, RegexHelper.ListWord),

            //Clear
            new CommandTemplate(ParsedCommandType.Clear, RegexHelper.ClearWord),
            new CommandTemplate(ParsedCommandType.Clear, RegexHelper.ClearWord, RegexHelper.ListWord),

            //Greeting
            new CommandTemplate(ParsedCommandType.SayHello, RegexHelper.HelloPattern),
            new CommandTemplate(ParsedCommandType.RequestHelp, RegexHelper.HelpWord),
            new CommandTemplate(ParsedCommandType.RequestExit, RegexHelper.ExitWord),

            //Delete
            new CommandTemplateWithEntry(ParsedCommandType.Delete,
                RegexHelper.DeleteWord, RegexHelper.EntryName, RegexHelper.EntryQuantity, RegexHelper.EntryUnit),
            new CommandTemplateWithEntry(ParsedCommandType.Delete,
                RegexHelper.DeleteWord, RegexHelper.EntryName, RegexHelper.EntryQuantity),
            new CommandTemplateWithEntry(ParsedCommandType.Delete,
                RegexHelper.DeleteWord, RegexHelper.EntryQuantity, RegexHelper.EntryUnit, RegexHelper.EntryName),
            new CommandTemplateWithEntry(ParsedCommandType.Delete,
                RegexHelper.DeleteWord, RegexHelper.EntryQuantity, RegexHelper.EntryName),
            new CommandTemplateWithEntry(ParsedCommandType.Delete,
                RegexHelper.DeleteWord, RegexHelper.EntryUnit, RegexHelper.EntryName),
            new CommandTemplateWithEntry(ParsedCommandType.Delete,
                RegexHelper.DeleteWord, RegexHelper.EntryName),
            
            //More
            new CommandTemplateWithEntry(ParsedCommandType.More,
                RegexHelper.MoreWord, RegexHelper.EntryQuantity),
            new CommandTemplateWithEntry(ParsedCommandType.More,
                RegexHelper.MoreWord, RegexHelper.EntryUnit),
            new CommandTemplateWithEntry(ParsedCommandType.More,
                RegexHelper.MoreWord, RegexHelper.EntryQuantity, RegexHelper.EntryUnit),
            new CommandTemplateWithEntry(ParsedCommandType.More,
                RegexHelper.MoreWord, RegexHelper.EntryUnit, RegexHelper.EntryQuantity),
            new CommandTemplateWithEntry(ParsedCommandType.More,
                RegexHelper.MoreWord, RegexHelper.EntryName, RegexHelper.EntryQuantity, RegexHelper.EntryUnit),
            new CommandTemplateWithEntry(ParsedCommandType.More,
                RegexHelper.MoreWord, RegexHelper.EntryName, RegexHelper.EntryQuantity),
            new CommandTemplateWithEntry(ParsedCommandType.More,
                RegexHelper.MoreWord, RegexHelper.EntryQuantity, RegexHelper.EntryUnit, RegexHelper.EntryName),
            new CommandTemplateWithEntry(ParsedCommandType.More,
                RegexHelper.MoreWord, RegexHelper.EntryQuantity, RegexHelper.EntryName),
            new CommandTemplateWithEntry(ParsedCommandType.More,
                RegexHelper.MoreWord, RegexHelper.EntryUnit, RegexHelper.EntryName),
            new CommandTemplateWithEntry(ParsedCommandType.More,
                RegexHelper.MoreWord, RegexHelper.EntryName),

            // Add
            new CommandTemplateWithEntry(ParsedCommandType.Add,
                RegexHelper.AddWord, RegexHelper.EntryName, RegexHelper.EntryQuantity, RegexHelper.EntryUnit),
            new CommandTemplateWithEntry(ParsedCommandType.Add,
                RegexHelper.AddWord, RegexHelper.EntryQuantity, RegexHelper.EntryUnit, RegexHelper.EntryName),
            new CommandTemplateWithEntry(ParsedCommandType.Add,
                RegexHelper.EntryQuantity, RegexHelper.EntryUnit, RegexHelper.EntryName),
            new CommandTemplateWithEntry(ParsedCommandType.Add,
                RegexHelper.EntryUnit, RegexHelper.EntryName),
            new CommandTemplateWithEntry(ParsedCommandType.Add,
                RegexHelper.AddWord, RegexHelper.EntryQuantity, RegexHelper.EntryName),
            new CommandTemplateWithEntry(ParsedCommandType.Add,
                RegexHelper.AddWord, RegexHelper.EntryUnit, RegexHelper.EntryQuantity, RegexHelper.EntryName),
            new CommandTemplateWithEntry(ParsedCommandType.Add,
                RegexHelper.AddWord, RegexHelper.EntryName, RegexHelper.EntryUnit, RegexHelper.EntryQuantity),
            new CommandTemplateWithEntry(ParsedCommandType.Add,
                RegexHelper.AddWord, RegexHelper.EntryUnit, RegexHelper.EntryName),
            new CommandTemplateWithEntry(ParsedCommandType.Add,
                RegexHelper.AddWord, RegexHelper.EntryName, RegexHelper.EntryQuantity),
            new CommandTemplateWithEntry(ParsedCommandType.Add,
                RegexHelper.AddWord, RegexHelper.EntryName),

            //Low priority command
            new CommandTemplateWithEntry(ParsedCommandType.Add,
                RegexHelper.EntryName, RegexHelper.EntryQuantity, RegexHelper.EntryUnit),
            new CommandTemplateWithEntry(ParsedCommandType.Add,
                RegexHelper.EntryQuantity, RegexHelper.EntryName),
        };

        public ParsedCommand ParseInput(string input, CultureInfo cultureInfo)
        {
            if (string.IsNullOrEmpty(input))
                return new ParsedCommand() { Type = ParsedCommandType.SayHello};

            input = NormalizeString(input, cultureInfo);

            foreach (var template in CommandsTemplates)
            {
                if (template.TryParse(input, out var data, cultureInfo))
                {
                    return new ParsedCommand()
                    {
                        Type = template.CommandType,
                        Data = data
                    };
                }
            }

            return new ParsedCommand()
            {
                Type = ParsedCommandType.SayUnknownCommand
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
