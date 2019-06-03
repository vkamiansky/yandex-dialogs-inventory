using System;
using System.Globalization;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AliceInventory.Logic.Parser
{
    public class InputParserService : IInputParserService
    {
        // Order by priority
        private static readonly CommandTemplate[] CommandsTemplates = 
        {
            new CommandTemplateWithSingleEntry(InputProcessingCommand.Add,
                $"{RegexHelper.AddWord} {RegexHelper.EntryName} {RegexHelper.EntryCount} {RegexHelper.EntryUnit}"),
            new CommandTemplateWithSingleEntry(InputProcessingCommand.Add,
                $"{RegexHelper.AddWord} {RegexHelper.EntryCount} {RegexHelper.EntryUnit} {RegexHelper.EntryName}"),
            new CommandTemplateWithSingleEntry(InputProcessingCommand.Add,
                $"{RegexHelper.EntryCount} {RegexHelper.EntryUnit} {RegexHelper.EntryName}"),
            new CommandTemplateWithSingleEntry(InputProcessingCommand.Add,
                $"{RegexHelper.EntryUnit} {RegexHelper.EntryName}"),
            new CommandTemplateWithSingleEntry(InputProcessingCommand.Add,
                $"{RegexHelper.AddWord} {RegexHelper.EntryCount} {RegexHelper.EntryName}"),
            new CommandTemplateWithSingleEntry(InputProcessingCommand.Add,
                $"{RegexHelper.AddWord} {RegexHelper.EntryUnit} {RegexHelper.EntryCount} {RegexHelper.EntryName}"),
            new CommandTemplateWithSingleEntry(InputProcessingCommand.Add,
                $"{RegexHelper.AddWord} {RegexHelper.EntryName} {RegexHelper.EntryUnit} {RegexHelper.EntryCount}"),
            new CommandTemplateWithSingleEntry(InputProcessingCommand.Add,
                $"{RegexHelper.AddWord} {RegexHelper.EntryUnit} {RegexHelper.EntryName}"),
            new CommandTemplateWithSingleEntry(InputProcessingCommand.Add,
                $"{RegexHelper.AddWord} {RegexHelper.EntryName}"),
            new CommandTemplate(InputProcessingCommand.SendMail,
                $"{RegexHelper.SendWord} {RegexHelper.MailWord}"),
            new CommandTemplate(InputProcessingCommand.SendMail,
                $"{RegexHelper.SendWord} на {RegexHelper.MailWord}"),
            new CommandTemplateWithEmail(InputProcessingCommand.SendMailTo,
                $"{RegexHelper.SendWord} на {RegexHelper.Email}"),
            new CommandTemplateWithEmail(InputProcessingCommand.AddMail,
                $"{RegexHelper.Email}"),
            new CommandTemplate(InputProcessingCommand.DeleteMail,
                $"{RegexHelper.DeleteWord} {RegexHelper.MailWord}"),
            new CommandTemplateWithSingleEntry(InputProcessingCommand.Delete,
                $"{RegexHelper.DeleteWord} {RegexHelper.EntryName} {RegexHelper.EntryCount} {RegexHelper.EntryUnit}"),
            new CommandTemplateWithSingleEntry(InputProcessingCommand.Delete,
                $"{RegexHelper.DeleteWord} {RegexHelper.EntryCount} {RegexHelper.EntryUnit} {RegexHelper.EntryName}"),
            new CommandTemplateWithSingleEntry(InputProcessingCommand.Delete,
                $"{RegexHelper.DeleteWord} {RegexHelper.EntryCount} {RegexHelper.EntryName}"),
            new CommandTemplateWithSingleEntry(InputProcessingCommand.Delete,
                $"{RegexHelper.DeleteWord} {RegexHelper.EntryUnit} {RegexHelper.EntryName}"),
            new CommandTemplateWithSingleEntry(InputProcessingCommand.Delete,
                $"{RegexHelper.DeleteWord} {RegexHelper.EntryName}"),
            new CommandTemplate(InputProcessingCommand.Cancel,
                $"{RegexHelper.CancelWord}"),
            new CommandTemplate(InputProcessingCommand.Accept,
                $"{RegexHelper.AcceptWord}"),
            new CommandTemplate(InputProcessingCommand.Decline,
                $"{RegexHelper.DeclineWord}"),
            new CommandTemplate(InputProcessingCommand.ReadList,
                $"{RegexHelper.ReadWord}"),
            new CommandTemplate(InputProcessingCommand.ReadList,
                $"{RegexHelper.ReadWord} {RegexHelper.ListWord}"),
            new CommandTemplate(InputProcessingCommand.Clear,
                $"{RegexHelper.ClearWord}"),
            new CommandTemplate(InputProcessingCommand.Clear,
                $"{RegexHelper.ClearWord} {RegexHelper.ListWord}"),
            new CommandTemplate(InputProcessingCommand.SayHello,
                $"{RegexHelper.HelloPattern}"),
            new CommandTemplate(InputProcessingCommand.RequestHelp,
                $"{RegexHelper.HelpWord}"),
            new CommandTemplate(InputProcessingCommand.RequestExit,
                $"{RegexHelper.ExitWord}"),
            new CommandTemplateWithSingleEntry(InputProcessingCommand.Add,
                $"{RegexHelper.EntryName} {RegexHelper.EntryCount} {RegexHelper.EntryUnit}"),
            new CommandTemplateWithSingleEntry(InputProcessingCommand.Add,
                $"{RegexHelper.EntryCount} {RegexHelper.EntryName}"),
        };

        public ProcessingCommand ParseInput(string input, CultureInfo cultureInfo)
        {
            if (string.IsNullOrEmpty(input))
                return new ProcessingCommand { Command = InputProcessingCommand.SayHello};

            input = input.ToLower(cultureInfo);

            var result = ParseInput(input, true, cultureInfo);

            if (result.Command == InputProcessingCommand.SayUnknownCommand)
                return ParseInput(input, false, cultureInfo);
            
            return result;
        }

        private ProcessingCommand ParseInput(string input, bool removeIgnoreWords, CultureInfo cultureInfo)
        {
            if (removeIgnoreWords)
                input = RegexHelper.RemoveIgnoreWords(input);

            foreach (var template in CommandsTemplates)
            {
                if (template.TryParse(input, out object data, cultureInfo))
                {
                    return new ProcessingCommand()
                    {
                        Command = template.Command,
                        Data = data
                    };
                }
            }

            return new ProcessingCommand()
            {
                Command = InputProcessingCommand.SayUnknownCommand
            };
        }
    }
}
