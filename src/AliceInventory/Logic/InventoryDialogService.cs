using System;
using System.Globalization;
using AliceInventory.Data;

namespace AliceInventory.Logic
{
    public class InventoryDialogService : IInventoryDialogService
    {
        private IInventoryStorage storage;
        private IInputParserService parser;
        private ICommandCache commandCache;

        public InventoryDialogService(IInventoryStorage storage, IInputParserService parser, ICommandCache commandCache)
        {
            this.storage = storage;
            this.parser = parser;
            this.commandCache = commandCache;
        }

        public ProcessingResult ProcessInput(string userId, string input, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(input))
            {
                return new ProcessingResult()
                {
                    Result = InputProcessingResult.GreetingRequested,
                    CultureInfo = culture
                };
            }

            ProcessingCommand command = parser.ParseInput(input, culture);
            var logicItem = command.Data as Entry;
            var dataItem = logicItem.ToData();

            ProcessingResult commandResult = new ProcessingResult() { CultureInfo = culture };

            switch (command.Command)
            {
                case InputProcessingCommand.Add:
                    storage.Add(userId, dataItem);
                    commandResult.Result = InputProcessingResult.Added;
                    commandResult.Data = logicItem;
                    break;

                case InputProcessingCommand.Delete:
                    storage.Delete(userId, dataItem);
                    commandResult.Result = InputProcessingResult.Deleted;
                    commandResult.Data = logicItem;
                    break;

                case InputProcessingCommand.Cancel:
                    commandResult.Result = InputProcessingResult.AddCanceled;
                    break;

                default:
                    commandResult.Result = InputProcessingResult.Error;
                    break;
            }
            commandCache.Set(userId, command);

            return commandResult;
        }
    }
}
