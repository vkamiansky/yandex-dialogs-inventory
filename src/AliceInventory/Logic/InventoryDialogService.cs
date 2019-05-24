using System;
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

        public ProcessingResult ProcessInput(string userId, string input)
        {
            ProcessingCommand command = parser.ParseInput(input);
            var logicItem = command.Data as Entry;
            var dataItem = logicItem.ToData();

            ProcessingResult commandResult;
            switch (command.Command)
            {
                case InputProcessingCommand.Add:
                    storage.Add(userId, dataItem);
                    commandResult = new ProcessingResult
                    {
                        Result = InputProcessingResult.Added,
                        Data = logicItem,
                    };
                    break;

                case InputProcessingCommand.Delete:
                    storage.Delete(userId, dataItem);
                    commandResult = new ProcessingResult
                    {
                        Result = InputProcessingResult.Deleted,
                        Data = logicItem,
                    };
                    break;

                case InputProcessingCommand.Cancel:
                    commandResult = new ProcessingResult
                    {
                        Result = InputProcessingResult.AddCanceled,
                    };
                    break;

                default:
                    commandResult = new ProcessingResult
                    {
                        Result = InputProcessingResult.Error,
                        Data = null,
                    };
                    break;
            }
            commandCache.Set(userId, command);

            return commandResult;
        }
    }
}
