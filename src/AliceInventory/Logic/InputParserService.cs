using System;
using System.Globalization;
using System.Collections.Generic;

namespace AliceInventory.Logic
{
    public class InputParserService : IInputParserService
    {
        public ProcessingCommand ParseInput(string input, CultureInfo culture)
        {
            Queue<string> tokens = new Queue<string>(input.ToLower().Split(" "));

            InputProcessingCommand command = ExtractCommand(tokens);
            Entry entry = ExtractEntry(culture, tokens);

            switch (command)
            {
                case InputProcessingCommand.Add:
                    if (entry == null)
                        command = InputProcessingCommand.SayIllegalArguments;
                    break;

                case InputProcessingCommand.Delete:
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
            if (tokens.Count == 0)
                return InputProcessingCommand.SayUnknownCommand;

            switch (tokens.Dequeue())
            {
                case "привет":
                    return InputProcessingCommand.SayHello;

                case "добавь":
                    return InputProcessingCommand.Add;

                case "удали":
                    return InputProcessingCommand.Delete;

                case "очисти":
                    return InputProcessingCommand.Clear;

                case "покажи":
                    return InputProcessingCommand.ReadList;

                default:
                    return InputProcessingCommand.SayUnknownCommand;
            }
        }

        private Entry ExtractEntry(CultureInfo culture, Queue<string> tokens)
        {
            Entry entry = null;

            string name = tokens.Dequeue();
            double count;
            bool isCount = double.TryParse(tokens.Dequeue(), NumberStyles.Any, culture, out count);
            UnitOfMeasure unit = ParseUnitOfMeasure(tokens.Dequeue());

            if (isCount && count > 0)
                entry = new Entry
                {
                    Name = name,
                    Count = count,
                    Unit = unit
                };

            return entry;
        }

        private UnitOfMeasure ParseUnitOfMeasure(string unitOfMeasure)
        {
            switch (unitOfMeasure)
            {
                case "кг":
                case "килограмм":
                case "килограмма":
                case "килограммов":
                    return UnitOfMeasure.Kg;

                case "штука":
                case "штуки":
                case "штук":
                case "штуку":
                case "штуковин":
                case "единиц":
                case "единица":
                case "единицу":
                case "единицы":
                    return UnitOfMeasure.Unit;

                case "литр":
                case "литра":
                case "литров":
                    return UnitOfMeasure.L;

                default:
                    return UnitOfMeasure.Unit;
            }
        }
    }
}
