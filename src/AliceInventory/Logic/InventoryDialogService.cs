using System;
using System.Globalization;
using System.Linq;
using AliceInventory.Data;
using AliceInventory.Logic.Email;

namespace AliceInventory.Logic
{
    public class InventoryDialogService : IInventoryDialogService
    {
        private IInventoryStorage storage;
        private IInputParserService parser;
        private ICommandCache commandCache;
        private IAliceEmailService emailService;

        public InventoryDialogService(IInventoryStorage storage, IInputParserService parser, ICommandCache commandCache, IAliceEmailService emailService)
        {
            this.storage = storage;
            this.parser = parser;
            this.commandCache = commandCache;
            this.emailService = emailService;
        }

        public ProcessingResult ProcessInput(string userId, string input, CultureInfo culture)
        {
            ProcessingCommand parsedCommand = parser.ParseInput(input, culture);
            var logicItem = parsedCommand.Data as Logic.Entry;

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
                    switch (cachedCommand.Command)
                    {
                        case InputProcessingCommand.Add:
                            storage.Delete(userId, (cachedCommand.Data as Logic.Entry).ToData());
                            resultType = InputProcessingResult.AddCanceled;
                            resultData = cachedCommand.Data as Logic.Entry;
                            break;
                        case InputProcessingCommand.Delete:
                            storage.Add(userId, (cachedCommand.Data as Logic.Entry).ToData());
                            resultType = InputProcessingResult.DeleteCanceled;
                            resultData = cachedCommand.Data as Logic.Entry;
                            break;
                        default:
                            resultType = InputProcessingResult.Error;
                            break;
                    }
                    break;

                case InputProcessingCommand.Add:
                    if (logicItem != null && logicItem.Count > 0)
                    {
                        resultType = InputProcessingResult.Added;
                        storage.Add(userId, logicItem.ToData());
                    }
                    else
                    {
                        resultType = InputProcessingResult.Error;
                    }
                    break;

                case InputProcessingCommand.Delete:
                    if (logicItem != null && logicItem.Count > 0)
                    {
                        resultType = InputProcessingResult.Deleted;
                        storage.Delete(userId, logicItem.ToData());
                    }
                    else
                    {
                        resultType = InputProcessingResult.Error;
                    }
                    break;

                case InputProcessingCommand.Clear:
                    resultType = InputProcessingResult.ClearRequested;
                    break;

                case InputProcessingCommand.ReadList:
                    resultType = InputProcessingResult.ListRead;
                    resultData = Array.ConvertAll(storage.ReadAll(userId), x => x.ToLogic());
                    break;

                case InputProcessingCommand.SendMail:
                    //emailService.SendListAsync(email, storage.ReadAll(userId));
                    resultType = InputProcessingResult.MailSent;
                    var email = parsedCommand.Data as string;
                    resultData = email;
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

                default:
                    resultType = InputProcessingResult.Error;
                    break;
            }

            commandCache.Set(userId, parsedCommand);

            return new ProcessingResult
            {
                Result = resultType,
                Data = resultData,
                CultureInfo = culture
            };
        }
    }
}
