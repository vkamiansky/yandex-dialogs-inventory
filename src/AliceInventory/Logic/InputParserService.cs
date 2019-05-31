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
        private Regex AvailableCountRegex { get; }

        public InputParserService()
        {
            AvailableCommands = new Dictionary<InputProcessingCommand, Regex>()
            {
                [InputProcessingCommand.SayHello] = new Regex(@"(^|\s)(доброе утро|добрый (день|вечер)|здравствуй(те|)|привет(ствую|)|хеллоу|хай)($|\s)", RegexOptions.Compiled),
                [InputProcessingCommand.Accept] = new Regex(@"(^|\s)(подтвер(ждаю|дить|ди)|несомненно|конечно|именно|точно|верно|давай|хочу|да)($|\s)", RegexOptions.Compiled),
                [InputProcessingCommand.Decline] = new Regex(@"(^|\s)(не (надо|хочу)|нет)($|\s)", RegexOptions.Compiled),
                [InputProcessingCommand.Cancel] = new Regex(@"(^|\s)(отмен(ить|яю|яй|а|и))($|\s)", RegexOptions.Compiled),
                [InputProcessingCommand.Add] = new Regex(@"(^|\s)(присоедини(ть|м|)|(над|при|до)бав(ляй|ить|им|ь)|по(ложи|мести)(м|)|(за|в)(пихай|пихни|сунь|кинь)|плюс)($|\s)", RegexOptions.Compiled),
                [InputProcessingCommand.Delete] = new Regex(@"(^|\s)(вы((тащить|(таскивать|нуть))|(брось|броси|тащи|суни|сунь|нь)(те|))|у(брать|далить|(ничтожь|дали|бери)(те|))|с(тереть|тирай|отри)|изъять|минус)($|\s)", RegexOptions.Compiled),
                [InputProcessingCommand.Clear] = new Regex(@"(^|\s)((вы|по|о)(чист)(ите|ить|и|ь))($|\s)", RegexOptions.Compiled),
                [InputProcessingCommand.ReadList] = new Regex(@"(^|\s)((продемонстрируй|покажи|расскажи)(те|))($|\s)", RegexOptions.Compiled),
                [InputProcessingCommand.SendMail] = new Regex(@"(^|\s)(отправ(ить|(ляй|ь))(те|)|(вы|по)(слать|шли))($|\s)", RegexOptions.Compiled),
                [InputProcessingCommand.RequestHelp] = new Regex(@"(^|\s)(что ты (можешь|умеешь)|помо(гите|ги|щь)|(спасай|спаси)(те|)|(выручай|выручи)(те|)|хелп)($|\s)", RegexOptions.Compiled),
                [InputProcessingCommand.RequestExit] = new Regex(@"(^|\s)(до (свидания|встречи|скорого)|выход(жу|и|)|пока|хватит|прощай|отвали|отстань)($|\s)", RegexOptions.Compiled),
            };
            AvailableUnitsOfMeasure = new Dictionary<UnitOfMeasure, Regex>()
            {
                [UnitOfMeasure.Unit] = new Regex(@"(^|\s)(единиц(а|у|ы|)|шту(чек|к(овин|и|а|у|)))($|\s)", RegexOptions.Compiled),
                [UnitOfMeasure.Kg] = new Regex(@"(^|\s)(килограмм(ов|а|)|кг)($|\s)", RegexOptions.Compiled),
                [UnitOfMeasure.L] = new Regex(@"(^|\s)(литр(ов|а|)|л)($|\s)", RegexOptions.Compiled),
            };
            AvailableCountRegex = new Regex(@"(^|\s)-?(\d+|)([\.,]|)\d+(\s|$)", RegexOptions.Compiled);
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

                case InputProcessingCommand.SayUnknownCommand:
                    UnitOfMeasure? unitOfMeasureNull = ExtractUnitOfMeasure(ref input);
                    double? countNull = ExtractCount(ref input, culture);
                    string name = ExtractName(ref input);

                    bool isCommandAdd = !(unitOfMeasureNull == null && countNull == null);
                    if (isCommandAdd)
                    {
                        UnitOfMeasure unitOfMeasure = unitOfMeasureNull ?? UnitOfMeasure.Unit;
                        double count = countNull ?? 1;

                        data = CreateEntry(name, count, unitOfMeasure);
                        if (data != null)
                            command = InputProcessingCommand.Add;
                    }
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
            UnitOfMeasure unitOfMeasure = ExtractUnitOfMeasure(ref input) ?? UnitOfMeasure.Unit;
            double count = ExtractCount(ref input, culture) ?? 1;
            string name = ExtractName(ref input);

            Entry entry = CreateEntry(name, count, unitOfMeasure);

            return entry;
        }

        private UnitOfMeasure? ExtractUnitOfMeasure(ref string input)
        {
            UnitOfMeasure? unitOfMeasure = null;

            foreach (var availableUnitOfMeasure in AvailableUnitsOfMeasure)
            {
                if (availableUnitOfMeasure.Value.IsMatch(input))
                {
                    unitOfMeasure = availableUnitOfMeasure.Key;
                    input = availableUnitOfMeasure.Value.Replace(input, " ");
                    break;
                }
            }

            return unitOfMeasure;
        }

        private double? ExtractCount(ref string input, CultureInfo culture)
        {
            double? count = null;
            Match countMatch = AvailableCountRegex.Match(input);

            if (countMatch.Success)
            {
                count = double.Parse(countMatch.Value, culture);
                input = AvailableCountRegex.Replace(input, " ");
            }

            return count;
        }

        private string ExtractName(ref string input)
        {
            return input.Trim();
        }

        private Entry CreateEntry(string name, double count, UnitOfMeasure unitOfMeasure)
        {
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
