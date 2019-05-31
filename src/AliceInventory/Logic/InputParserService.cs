using System;
using System.Globalization;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AliceInventory.Logic
{
    public class InputParserService : IInputParserService
    {
        private Dictionary<InputProcessingCommand, Regex> AvailableCommands { get; }
        private Dictionary<UnitOfMeasure, Regex> AvailableUnitsOfMeasure { get; }

        public InputParserService()
        {
            AvailableCommands = new Dictionary<InputProcessingCommand, Regex>()
            {
                [InputProcessingCommand.SayHello] = new Regex(@"(^|\s)(доброе утро|добрый (день|вечер)|здравствуй(те|)|привет(ствую|)|хеллоу|хай)($|\s)", RegexOptions.Compiled),
                [InputProcessingCommand.Accept] = new Regex(@"(^|\s)(подтвер(ждаю|дить|ди)|несомненно|конечно|именно|точно|верно|давай|хочу|да)($|\s)", RegexOptions.Compiled),
                [InputProcessingCommand.Decline] = new Regex(@"(^|\s)(не (надо|хочу)|отвали|отстань|нет)($|\s)", RegexOptions.Compiled),
                [InputProcessingCommand.Cancel] = new Regex(@"(^|\s)(отмен(ить|яю|яй|а|и))($|\s)", RegexOptions.Compiled),
                [InputProcessingCommand.Add] = new Regex(@"(^|\s)(присоедини(ть|)|(над|при|до)бав(ляй|ить|ь)|по(ложи|мести)|(за|в)(сунь|пихай|пихни))($|\s)", RegexOptions.Compiled),
                [InputProcessingCommand.Delete] = new Regex(@"(^|\s)(вы((тащить|(таскивать|нуть))|(брось|броси|тащи|суни|сунь|нь)(те|))|у(брать|далить|(дали|ничтожь)(те|))|с(тереть|тирай|отри)|изъять)($|\s)", RegexOptions.Compiled),
                [InputProcessingCommand.Clear] = new Regex(@"(^|\s)((вы|по|о)(чист)(ите|и|ь))($|\s)", RegexOptions.Compiled),
                [InputProcessingCommand.ReadList] = new Regex(@"(^|\s)((продемонстрируй|покажи|расскажи)(те|))($|\s)", RegexOptions.Compiled),
                [InputProcessingCommand.SendMail] = new Regex(@"(^|\s)(отправ(ить|(ляй|ь))(те|)|(вы|по)(слать|шли))($|\s)", RegexOptions.Compiled),
                [InputProcessingCommand.RequestHelp] = new Regex(@"(^|\s)(что ты (можешь|умеешь)|помо(гите|ги|щь)|(спасай|спаси)(те|)|(выручай|выручи)(те|)|хелп)($|\s)", RegexOptions.Compiled),
                [InputProcessingCommand.RequestExit] = new Regex(@"(^|\s)(до (свидания|встречи|скорого)|выход(жу|и|)|пока|хватит|прощай)($|\s)", RegexOptions.Compiled),
            };
            AvailableUnitsOfMeasure = new Dictionary<UnitOfMeasure, Regex>()
            {
                [UnitOfMeasure.Unit] = new Regex(@"(^|\s)(единиц(а|у|ы|)|шту(чек|к(овин|и|а|у|)))($|\s)", RegexOptions.Compiled),
                [UnitOfMeasure.Kg] = new Regex(@"(^|\s)(килограмм(ов|а|)|кг)($|\s)", RegexOptions.Compiled),
                [UnitOfMeasure.L] = new Regex(@"(^|\s)(литр(ов|а|)|л)($|\s)", RegexOptions.Compiled),
            };
        }

        public ProcessingCommand ParseInput(string input, CultureInfo culture)
        {
            input = input?.ToLower();

            InputProcessingCommand command = ExtractCommand(ref input);

            object data = null;
            switch (command)
            {
                case InputProcessingCommand.Add:
                case InputProcessingCommand.Delete:
                case InputProcessingCommand.SayIllegalArguments:
                    data = ExtractEntry(input, culture);
                    if (data == null)
                        command = InputProcessingCommand.SayIllegalArguments;
                    break;

                case InputProcessingCommand.SendMail:
                    Regex mailRegex = new Regex(@"(^|\s)[\w+\.-]+@[\w+\-]+\.\w{2,4}($|\s)", RegexOptions.Compiled);
                    Match mailMatch = mailRegex.Match(input);
                    if (mailMatch.Success)
                        data = mailMatch.Value.Trim();
                    else
                        command = InputProcessingCommand.SayIllegalArguments; 
                    break;
            }

            return new ProcessingCommand
            {
                Command = command,
                Data = data,
            };
        }

        private InputProcessingCommand ExtractCommand(ref string input)
        {
            InputProcessingCommand command = InputProcessingCommand.SayUnknownCommand;

            if (string.IsNullOrEmpty(input))
            {
                command = InputProcessingCommand.SayHello;
            }
            else
            {
                foreach (var commandRegex in AvailableCommands)
                {
                    if (commandRegex.Value.IsMatch(input))
                    {
                        command = commandRegex.Key;
                        input = commandRegex.Value.Replace(input, " ");
                        break;
                    }
                }
            }
            
            return command;
        }

        private Entry ExtractEntry(string input, CultureInfo culture)
        {
            UnitOfMeasure unitOfMeasure = UnitOfMeasure.Unit;
            foreach (var availableUnitOfMeasure in AvailableUnitsOfMeasure)
            {
                if (availableUnitOfMeasure.Value.IsMatch(input))
                {
                    unitOfMeasure = availableUnitOfMeasure.Key;
                    input = availableUnitOfMeasure.Value.Replace(input, " ");
                    break;
                }
            }

            double count = 1;
            Regex countRegex = new Regex(@"(^|\s|-)(\d+|)([\.,]|)\d+(\s|$)", RegexOptions.Compiled);
            Match countMatch = countRegex.Match(input);
            if (countMatch.Success)
            {
                count = double.Parse(countMatch.Value, culture);
                input = countRegex.Replace(input, " ");
            }

            string name = input.Trim();

            Entry entry = null;
            if (!string.IsNullOrEmpty(name)
                && count > 0)
            {
                entry = new Entry
                {
                    Name = name,
                    Count = count,
                    Unit = unitOfMeasure
                };
            }

            return entry;
        }
    }
}
