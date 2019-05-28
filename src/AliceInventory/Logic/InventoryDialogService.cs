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
            ProcessingCommand parsedCommand = parser.ParseInput(input, culture);
            var logicItem = parsedCommand.Data as Entry;
            var dataItem = logicItem.ToData();

            InputProcessingResult resultType;
            object resultData = logicItem;
            switch (parsedCommand.Command)
            {
                case InputProcessingCommand.SayHello:
                    resultType = InputProcessingResult.GreetingRequested;
                    break;

                case InputProcessingCommand.Accept:
                    if (commandCache.Get(userId).Command == InputProcessingCommand.Clear)
                    {
                        resultType = InputProcessingResult.Cleared;
                        storage.Clear(userId);
                    }
                    else
                    {
                        resultType = InputProcessingResult.Error;
                    }
                    break;

                case InputProcessingCommand.Decline:
                    resultType = InputProcessingResult.Declined;
                    break;
                    
                case InputProcessingCommand.Cancel: 
                    ProcessingCommand cachedCommand = commandCache.Get(userId);
                    Data.Entry cachedCommandData = (cachedCommand.Data as Logic.Entry).ToData();
                    if (cachedCommand.Command == InputProcessingCommand.Add)
                    {
                        storage.Delete(userId, cachedCommandData);
                        resultType = InputProcessingResult.AddCanceled;
                        resultData = cachedCommandData;
                    }
                    else if (cachedCommand.Command == InputProcessingCommand.Delete)
                    {
                        storage.Add(userId, cachedCommandData);
                        resultType = InputProcessingResult.DeleteCanceled;
                        resultData = cachedCommandData;
                    }
                    else
                    {
                        resultType = InputProcessingResult.Error;
                    }
                    break;

                case InputProcessingCommand.Add:
                    resultType = InputProcessingResult.Added;
                    storage.Add(userId, dataItem);
                    break;

                case InputProcessingCommand.Delete:
                    resultType = InputProcessingResult.Deleted;
                    storage.Delete(userId, dataItem);
                    break;

                case InputProcessingCommand.Clear:
                    resultType = InputProcessingResult.ClearRequested;
                    break;

                case InputProcessingCommand.ReadList:
                    resultType = InputProcessingResult.ListRead;
                    resultData = storage.ReadAll(userId);
                    break;

                case InputProcessingCommand.SendMail:
                    resultType = InputProcessingResult.MailSent;
                    break;

                case InputProcessingCommand.RequestHelp:
                    resultType = InputProcessingResult.HelpRequested;
                    break;

                case InputProcessingCommand.RequestExit:
                    resultType = InputProcessingResult.ExitRequested;
                    break;

                case InputProcessingCommand.SayUnknownCommand:
                    resultType = InputProcessingResult.Error;
                    resultData = parsedCommand.Command;
                    break;

                case InputProcessingCommand.SayIllegalArguments:
                    resultType = InputProcessingResult.Error;
                    break;

                default:
                    resultType = InputProcessingResult.Error;
                    resultData = "Команда не была обработана должным образом";
                    break;
            }

            commandCache.Set(userId, parsedCommand);

            return new ProcessingResult
            {
                Result = resultType,
                Data = resultData,
            };
        }
    }
}
