using System;
using System.Globalization;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AliceInventory.Logic.Parser
{
    public class InputParserService : IInputParserService
    {
        private static readonly CommandTemplate[] AddRegexTemplates = 
        {
            new CommandTemplateWithEntry(
                $"{RegexHelper.AddWord} ({RegexHelper.EntryNameWord}) ({RegexHelper.Number}) ({RegexHelper.UnitOfMeasureWord})",
                0, 1, 2),
            new CommandTemplateWithEntry(
                $"({RegexHelper.EntryNameWord}) ({RegexHelper.Number}) ({RegexHelper.UnitOfMeasureWord})",
                0, 1, 2),
            new CommandTemplateWithEntry(
                $"{RegexHelper.AddWord} ({RegexHelper.Number}) ({RegexHelper.UnitOfMeasureWord}) ({RegexHelper.EntryNameWord})",
                2, 0, 1),
            new CommandTemplateWithEntry(
                $"({RegexHelper.Number}) ({RegexHelper.UnitOfMeasureWord}) ({RegexHelper.EntryNameWord})",
                2, 0, 1),
            new CommandTemplateWithEntry(
                $"{RegexHelper.AddWord} ({RegexHelper.Number}) ({RegexHelper.EntryNameWord})",
                1, 0, -1),
            new CommandTemplateWithEntry(
                $"({RegexHelper.Number}) ({RegexHelper.EntryNameWord})",
                1, 0, -1),
            new CommandTemplateWithEntry(
                $"({RegexHelper.UnitOfMeasureWord}) ({RegexHelper.EntryNameWord})",
                1, -1, 0),
            new CommandTemplateWithEntry(
                $"{RegexHelper.AddWord} ({RegexHelper.UnitOfMeasureWord}) ({RegexHelper.Number}) ({RegexHelper.EntryNameWord})",
                2, 1, 0),
            new CommandTemplateWithEntry(
                $"{RegexHelper.AddWord} ({RegexHelper.EntryNameWord}) ({RegexHelper.UnitOfMeasureWord}) ({RegexHelper.Number})",
                0, 2, 1),
            new CommandTemplateWithEntry(
                $"{RegexHelper.AddWord} ({RegexHelper.UnitOfMeasureWord}) ({RegexHelper.EntryNameWord})",
                1, -1, 0),
            new CommandTemplateWithEntry(
                $"{RegexHelper.AddWord} ({RegexHelper.EntryNameWord})",
                0, -1, -1),
        };

        private static readonly CommandTemplate[] DeleteRegexTemplates = 
        {
            new CommandTemplateWithEntry(
                $"{RegexHelper.DeleteWord} ({RegexHelper.EntryNameWord}) ({RegexHelper.Number}) ({RegexHelper.UnitOfMeasureWord})",
                0, 1, 2), 
            new CommandTemplateWithEntry(
                $"{RegexHelper.DeleteWord} ({RegexHelper.Number}) ({RegexHelper.UnitOfMeasureWord}) ({RegexHelper.EntryNameWord})",
                2, 0, 1), 
            new CommandTemplateWithEntry(
                $"{RegexHelper.DeleteWord} ({RegexHelper.Number}) ({RegexHelper.EntryNameWord})",
                1, 0, -1), 
            new CommandTemplateWithEntry(
                $"{RegexHelper.DeleteWord} ({RegexHelper.UnitOfMeasureWord}) ({RegexHelper.EntryNameWord})",
                1, -1, 0), 
            new CommandTemplateWithEntry(
                $"{RegexHelper.DeleteWord} ({RegexHelper.EntryNameWord})",
                0, -1, -1), 
        };

        private static readonly CommandTemplate[] CancelRegexTemplates = 
        {
            new CommandTemplate($"{RegexHelper.CancelWord}")
        };
        private static readonly CommandTemplate[] AcceptRegexTemplates = 
        {
            new CommandTemplate($"{RegexHelper.AcceptWord}")
        };
        private static readonly CommandTemplate[] DeclineRegexTemplates = 
        {
            new CommandTemplate($"{RegexHelper.DeclineWord}")
        };
        private static readonly CommandTemplate[] ReadListRegexTemplates = 
        {
            new CommandTemplate($"{RegexHelper.ReadListWord}")
        };
        private static readonly CommandTemplate[] ClearRegexTemplates = 
        {
            new CommandTemplate($"{RegexHelper.ClearWord}")
        };
        private static readonly CommandTemplate[] SendMailRegexTemplates = 
        {
            new CommandTemplate($"{RegexHelper.SendMailOnWord} ({RegexHelper.Email})")
        };
        private static readonly CommandTemplate[] SayHelloRegexTemplates = 
        {
            new CommandTemplate($"{RegexHelper.HelloWord}")
        };
        private static readonly CommandTemplate[] RequestHelpRegexTemplates = 
        {
            new CommandTemplate($"{RegexHelper.HelpWord}")
        };
        private static readonly CommandTemplate[] RequestExitRegexTemplates = 
        {
            new CommandTemplate($"{RegexHelper.ExitWord}")
        };

        private static readonly Dictionary<InputProcessingCommand, CommandTemplate[]> CommandDictionary;

        static InputParserService()
        {
            CommandDictionary = new Dictionary<InputProcessingCommand, CommandTemplate[]>
            {
                [InputProcessingCommand.Add] = AddRegexTemplates,
                [InputProcessingCommand.Delete] = DeleteRegexTemplates,
                [InputProcessingCommand.Cancel] = CancelRegexTemplates,
                [InputProcessingCommand.Accept] = AcceptRegexTemplates,
                [InputProcessingCommand.Decline] = DeclineRegexTemplates,
                [InputProcessingCommand.ReadList] = ReadListRegexTemplates,
                [InputProcessingCommand.Clear] = ClearRegexTemplates,
                [InputProcessingCommand.SendMail] = SendMailRegexTemplates,
                [InputProcessingCommand.SayHello] = SayHelloRegexTemplates,
                [InputProcessingCommand.RequestHelp] = RequestHelpRegexTemplates,
                [InputProcessingCommand.RequestExit] = RequestExitRegexTemplates
            };
        }

        public ProcessingCommand ParseInput(string input, CultureInfo cultureInfo)
        {
            if (string.IsNullOrEmpty(input))
                return new ProcessingCommand { Command = InputProcessingCommand.SayHello};

            foreach (var (command, templates) in CommandDictionary)
            {
                foreach (var template in templates)
                {
                    if (template.TryParse(input, out object data, cultureInfo))
                    {
                        return new ProcessingCommand()
                        {
                            Command = command,
                            Data = data
                        };
                    }
                }
            }

            return new ProcessingCommand()
            {
                Command = InputProcessingCommand.SayUnknownCommand
            };
        }
    }
}
