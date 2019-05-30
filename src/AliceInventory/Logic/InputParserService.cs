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
                [InputProcessingCommand.SayHello] = new Regex(@"доброе утро|добрый (день|вечер)|здравствуй(те|)|привет(ствую|)|хеллоу|хай", RegexOptions.Compiled),
                [InputProcessingCommand.Accept] = new Regex(@"подтвер(ждаю|дить|ди)|несомненно|конечно|именно|точно|верно|давай|хочу|^да$", RegexOptions.Compiled),
                [InputProcessingCommand.Decline] = new Regex(@"не (надо|хочу)|отвали|отстань|^нет$", RegexOptions.Compiled),
                [InputProcessingCommand.Cancel] = new Regex(@"отмен(ить|яю|яй|а|и)", RegexOptions.Compiled),
                [InputProcessingCommand.Add] = new Regex(@"присоедини(ть|)|(над|при|до)бав(ляй|ить|ь)|по(ложи|мести)|(за|в)(сунь|пихай|пихни)", RegexOptions.Compiled),
                [InputProcessingCommand.Delete] = new Regex(@"вы((тащить|(таскивать|нуть))|(брось|броси|тащи|суни|сунь|нь)(те|))|у(брать|далить|(дали|ничтожь)(те|))|с(тереть|тирай|отри)|изъять", RegexOptions.Compiled),
                [InputProcessingCommand.Clear] = new Regex(@"(вы|по|о)(чист)(ите|и|ь)", RegexOptions.Compiled),
                [InputProcessingCommand.ReadList] = new Regex(@"(продемонстрируй|покажи|расскажи)(те|)", RegexOptions.Compiled),
                [InputProcessingCommand.SendMail] = new Regex(@"отправ(ить|(ляй|ь))(те|)|(вы|по)(слать|шли)", RegexOptions.Compiled),
                [InputProcessingCommand.RequestHelp] = new Regex(@"что ты (можешь|умеешь)|помо(гите|ги|щь)|(спасай|спаси)(те|)|(выручай|выручи)(те|)|хелп", RegexOptions.Compiled),
                [InputProcessingCommand.RequestExit] = new Regex(@"выход|пока|хватит|прощай", RegexOptions.Compiled),
            };
            AvailableUnitsOfMeasure = new Dictionary<UnitOfMeasure, Regex>()
            {
                [UnitOfMeasure.Unit] = new Regex(@"(^|\s)единиц(а|у|ы|)($|\s)|(^|\s)шту(чек|к(овин|и|а|у|))($|\s)", RegexOptions.Compiled),
                [UnitOfMeasure.Kg] = new Regex(@"(^|\s)килограмм(ов|а|)($|\s)|(^|\s)кг($|\s)", RegexOptions.Compiled),
                [UnitOfMeasure.L] = new Regex(@"(^|\s)литр(ов|а|)($|\s)|(^|\s)л($|\s)", RegexOptions.Compiled),
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
                    Regex mailRegex = new Regex(@"[\w+\.-]+@[\w+\-]+\.\w{2,4}", RegexOptions.Compiled);
                    Match mailMatch = mailRegex.Match(input.Trim());
                    if (mailMatch.Success)
                        data = mailMatch.Value;
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
