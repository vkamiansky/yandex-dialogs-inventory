using System;
using System.Globalization;
using System.Collections.Generic;

namespace AliceInventory.Logic
{
    public class InputParserService : IInputParserService
    {
        public ProcessingCommand ParseInput(string input, CultureInfo culture)
        {
            ProcessingCommand resultProcessingCommand = new ProcessingCommand();
            Queue<string> tokens = new Queue<string>(input.ToLower().Split(" "));

            Entry entry = null;
            switch (tokens.Dequeue())
            {
                case "добавь":
                    entry = ExtractEntry(culture, tokens);

                    if (entry != null)
                        resultProcessingCommand.Command = InputProcessingCommand.Add;
                    else
                        resultProcessingCommand.Command = InputProcessingCommand.SayIllegalArguments;
                    
                    resultProcessingCommand.Data = entry;
                    break;

                case "удали":
                    entry = ExtractEntry(culture, tokens);

                    if (entry != null)
                        resultProcessingCommand.Command = InputProcessingCommand.Delete;
                    else
                        resultProcessingCommand.Command = InputProcessingCommand.SayIllegalArguments;

                    resultProcessingCommand.Data = entry;
                    break;

                case "очисти":
                    resultProcessingCommand.Command = InputProcessingCommand.Clear;
                    break;

                case "покажи":
                    resultProcessingCommand.Command = InputProcessingCommand.ReadList;
                    break;

                default:
                    resultProcessingCommand.Command = InputProcessingCommand.SayUnknownCommand;
                    break;
            }

            return resultProcessingCommand;
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
