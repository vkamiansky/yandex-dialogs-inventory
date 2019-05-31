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
            new CommandTemplateWithEntry(InputProcessingCommand.Add,
                $"{RegexHelper.AddPattern} {RegexHelper.EntryName} {RegexHelper.EntryCount} {RegexHelper.EntryUnit}"),
            new CommandTemplateWithEntry(InputProcessingCommand.Add,
                $"{RegexHelper.AddPattern} {RegexHelper.EntryCount} {RegexHelper.EntryUnit} {RegexHelper.EntryName}"),
            new CommandTemplateWithEntry(InputProcessingCommand.Add,
                $"{RegexHelper.EntryCount} {RegexHelper.EntryUnit} {RegexHelper.EntryName}"),
            new CommandTemplateWithEntry(InputProcessingCommand.Add,
                $"{RegexHelper.EntryUnit} {RegexHelper.EntryName}"),
            new CommandTemplateWithEntry(InputProcessingCommand.Add,
                $"{RegexHelper.AddPattern} {RegexHelper.EntryCount} {RegexHelper.EntryName}"),
            new CommandTemplateWithEntry(InputProcessingCommand.Add,
                $"{RegexHelper.AddPattern} {RegexHelper.EntryUnit} {RegexHelper.EntryCount} {RegexHelper.EntryName}"),
            new CommandTemplateWithEntry(InputProcessingCommand.Add,
                $"{RegexHelper.AddPattern} {RegexHelper.EntryName} {RegexHelper.EntryUnit} {RegexHelper.EntryCount}"),
            new CommandTemplateWithEntry(InputProcessingCommand.Add,
                $"{RegexHelper.AddPattern} {RegexHelper.EntryUnit} {RegexHelper.EntryName}"),
            new CommandTemplateWithEntry(InputProcessingCommand.Add,
                $"{RegexHelper.AddPattern} {RegexHelper.EntryName}"),
            new CommandTemplateWithEntry(InputProcessingCommand.Delete,
                $"{RegexHelper.DeletePattern} {RegexHelper.EntryName} {RegexHelper.EntryCount} {RegexHelper.EntryUnit}"),
            new CommandTemplateWithEntry(InputProcessingCommand.Delete,
                $"{RegexHelper.DeletePattern} {RegexHelper.EntryCount} {RegexHelper.EntryUnit} {RegexHelper.EntryName}"),
            new CommandTemplateWithEntry(InputProcessingCommand.Delete,
                $"{RegexHelper.DeletePattern} {RegexHelper.EntryCount} {RegexHelper.EntryName}"),
            new CommandTemplateWithEntry(InputProcessingCommand.Delete,
                $"{RegexHelper.DeletePattern} {RegexHelper.EntryUnit} {RegexHelper.EntryName}"),
            new CommandTemplateWithEntry(InputProcessingCommand.Delete,
                $"{RegexHelper.DeletePattern} {RegexHelper.EntryName}"),
            new CommandTemplate(InputProcessingCommand.Cancel,
                $"{RegexHelper.CancelPattern}"),
            new CommandTemplate(InputProcessingCommand.Accept,
                $"{RegexHelper.AcceptPattern}"),
            new CommandTemplate(InputProcessingCommand.Decline,
                $"{RegexHelper.DeclinePattern}"),
            new CommandTemplate(InputProcessingCommand.ReadList,
                $"{RegexHelper.ReadListPattern}"),
            new CommandTemplate(InputProcessingCommand.Clear,
                $"{RegexHelper.ClearPattern}"),
            new CommandTemplateWithEmail(InputProcessingCommand.SendMail,
                $"{RegexHelper.SendMailOnPattern} {RegexHelper.Email}"),
            new CommandTemplate(InputProcessingCommand.SayHello,
                $"{RegexHelper.HelloPattern}"),
            new CommandTemplate(InputProcessingCommand.RequestHelp,
                $"{RegexHelper.HelpPattern}"),
            new CommandTemplate(InputProcessingCommand.RequestExit,
                $"{RegexHelper.ExitPattern}"),
            new CommandTemplateWithEntry(InputProcessingCommand.Add,
                $"{RegexHelper.EntryName} {RegexHelper.EntryCount} {RegexHelper.EntryUnit}"),
            new CommandTemplateWithEntry(InputProcessingCommand.Add,
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
