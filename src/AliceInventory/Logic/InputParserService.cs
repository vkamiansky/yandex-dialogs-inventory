using System;
using System.Globalization;

namespace AliceInventory.Logic
{
    public class InputParserService : IInputParserService
    {
        public ProcessingCommand ParseInput(string input, CultureInfo culture)
        {
            ProcessingCommand resultProcessingCommand = new ProcessingCommand();
            string[] tokens = input.ToLower().Split(" ");

            Entry entry = null;
            switch (tokens[0])
            {
                case "добавь":
                    entry = ParseEntry(culture, tokens[1], tokens[2], tokens[3]);

                    if (entry != null)
                        resultProcessingCommand.Command = InputProcessingCommand.Add;
                    else
                        resultProcessingCommand.Command = InputProcessingCommand.SayUnknownCommand;
                    
                    resultProcessingCommand.Data = entry;
                    break;

                case "удали":
                    entry = ParseEntry(culture, tokens[1], tokens[2], tokens[3]);

                    if (entry != null)
                        resultProcessingCommand.Command = InputProcessingCommand.Delete;
                    else
                        resultProcessingCommand.Command = InputProcessingCommand.SayUnknownCommand;

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

        private Entry ParseEntry(CultureInfo culture, params string[] tokens)
        {
            Entry entry = null;

            string name = tokens[0];
            double count;
            bool isCount = double.TryParse(tokens[1], NumberStyles.Any, culture, out count);
            UnitOfMeasure unit = ParseUnitOfMeasure(tokens[2]);

            if (isCount)
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
