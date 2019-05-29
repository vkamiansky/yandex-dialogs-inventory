using System;
using System.Globalization;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AliceInventory.Logic
{
    public class InputParserService : IInputParserService
    {
        private Dictionary<InputProcessingCommand, Regex> AvailableCommands { get; }

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
        }

        public ProcessingCommand ParseInput(string input, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(input))
            {
                return new ProcessingCommand
                {
                    Command = InputProcessingCommand.SayHello,
                };
            }
            else if (input.Contains("что ты можешь"))
            {
                return new ProcessingCommand
                {
                    Command = InputProcessingCommand.RequestHelp,
                };
            }

            Queue<string> tokens = new Queue<string>(input.ToLower().Split(" ", StringSplitOptions.RemoveEmptyEntries));

            InputProcessingCommand command = ExtractCommand(tokens);

            Entry entry = null;
            switch (command)
            {
                case InputProcessingCommand.Add:
                case InputProcessingCommand.Delete:
                case InputProcessingCommand.SendMail:
                case InputProcessingCommand.SayIllegalArguments:
                    entry = ExtractEntry(culture, tokens);
                    if (entry == null)
                        command = InputProcessingCommand.SayIllegalArguments;
                    break;
            }

            return new ProcessingCommand
            {
                Command = command,
                Data = entry,
            };
        }

        private InputProcessingCommand ExtractCommand(Queue<string> tokens)
        {
            switch (tokens.Dequeue())
            {
                case "привет":
                case "приветствую":
                case "здравствуй":
                case "здравствуйте":
                case "хай":
                case "хеллоу":
                    return InputProcessingCommand.SayHello;
                
                case "да":
                case "конечно":
                case "несомненно":
                case "точно":
                case "именно":
                case "верно":
                case "хочу":
                case "давай":
                case "подтверждаю":
                case "подтвердить":
                    return InputProcessingCommand.Accept;

                case "нет":
                case "отвали":
                case "отстань":
                    return InputProcessingCommand.Decline;

                case "отмена":
                case "отмени":
                case "отменяй":
                case "отменяю":
                case "отменить":
                    return InputProcessingCommand.Cancel;

                case "добавь":
                case "положи":
                case "засунь":
                case "помести":
                case "вмести":
                case "впихни":
                case "всунь":
                case "запихай":
                case "добавить":
                case "добавляй":
                case "прибавить":
                case "прибавь":
                case "надбавь":
                case "накинь":
                case "присоедини":
                    return InputProcessingCommand.Add;

                case "удали":
                case "убери":
                case "высунь":
                case "высуни":
                case "выброси":
                case "выбрось":
                case "удалить":
                case "убрать":
                case "вытащи":
                case "вытащить":
                case "сотри":
                case "изъять":
                case "уничтожь":
                case "стереть":
                case "стирай":
                case "вынуть":
                case "вынь":
                case "вытаскивать":
                    return InputProcessingCommand.Delete;

                case "очисти":
                case "очисть":
                case "вычисти":
                    return InputProcessingCommand.Clear;

                case "покажи":
                case "продемонстрируй":
                case "расскажи":
                    return InputProcessingCommand.ReadList;

                case "отправь":
                case "отправить":
                case "отправляй":
                case "вышли":
                case "пошли":
                case "выслать":
                case "послать":
                    return InputProcessingCommand.SendMail;

                case "помоги":
                case "помогите":
                case "помощь":
                case "хелп":
                case "спасай":
                case "спасайте":
                case "выручай":
                case "выручайте":
                    return InputProcessingCommand.RequestHelp;

                case "выход":
                case "пока":
                case "хватит":
                case "прощай":
                    return InputProcessingCommand.RequestExit;

                default:
                    return InputProcessingCommand.SayUnknownCommand;
            }
        }

        private Entry ExtractEntry(CultureInfo culture, Queue<string> tokens)
        {
            int tokensCount = (tokens.Count > 3) ? 3 : tokens.Count;

            string name = null;
            double count = 1;
            bool isCountExist = false;
            UnitOfMeasure unitOfMeasure = UnitOfMeasure.Unit;
            bool isUnitOfMeasureExist = false;

            for (int i = 0; i < tokensCount; i++)
            {
                string token = tokens.Dequeue();

                if (!isCountExist)
                {
                    if (isCountExist = double.TryParse(token, NumberStyles.Any, culture, out count))
                        continue;
                    else
                        count = 1;
                }
                if (!isUnitOfMeasureExist)
                {
                    if (isUnitOfMeasureExist = TryParseUnitOfMeasure(token, out unitOfMeasure))
                        continue;
                }
                name = token;
            }

            Entry entry = null;
            if (name != null
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

        private bool TryParseUnitOfMeasure(string unitOfMeasure, out UnitOfMeasure result)
        {
            switch (unitOfMeasure)
            {
                case "кг":
                case "килограмм":
                case "килограмма":
                case "килограммов":
                    result = UnitOfMeasure.Kg;
                    return true;

                case "штука":
                case "штуки":
                case "штук":
                case "штуку":
                case "штуковин":
                case "единиц":
                case "единица":
                case "единицу":
                case "единицы":
                    result = UnitOfMeasure.Unit;
                    return true;

                case "литр":
                case "литра":
                case "литров":
                    result = UnitOfMeasure.L;
                    return true;

                default:
                    result = UnitOfMeasure.Unit;
                    return false;
            }
        }
    }
}
