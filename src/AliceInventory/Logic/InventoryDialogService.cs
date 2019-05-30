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

            InputProcessingResult resultType = InputProcessingResult.Error;
            object resultData = null;

            switch (parsedCommand.Command)
            {
                case InputProcessingCommand.SayHello:
                    resultType = InputProcessingResult.GreetingRequested;
                    break;

                case InputProcessingCommand.Accept:
                    if (commandCache.Get(userId).Command == InputProcessingCommand.Clear)
                    {
                        storage.Clear(userId);
                        resultType = InputProcessingResult.Cleared;
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
                            resultData = cachedCommand.Data as Logic.Entry;
                            resultType = InputProcessingResult.AddCanceled;
                            break;
                        case InputProcessingCommand.Delete:
                            storage.Add(userId, (cachedCommand.Data as Logic.Entry).ToData());
                            resultData = cachedCommand.Data as Logic.Entry;
                            resultType = InputProcessingResult.DeleteCanceled;
                            break;
                        default:
                            resultType = InputProcessingResult.Error;
                            break;
                    }
                    break;

                case InputProcessingCommand.Add:
                    storage.Add(userId, (parsedCommand.Data as Logic.Entry).ToData());
                    resultData = parsedCommand.Data;
                    resultType = InputProcessingResult.Added;
                    break;

                case InputProcessingCommand.Delete:
                    storage.Delete(userId, (parsedCommand.Data as Logic.Entry).ToData());
                    resultData = parsedCommand.Data;
                    resultType = InputProcessingResult.Deleted;
                    break;

                case InputProcessingCommand.Clear:
                    resultType = InputProcessingResult.ClearRequested;
                    break;

                case InputProcessingCommand.ReadList:
                    resultData = Array.ConvertAll(storage.ReadAll(userId), x => x.ToLogic());
                    resultType = InputProcessingResult.ListRead;
                    break;

                case InputProcessingCommand.SendMail:
                    //emailService.SendListAsync(email, storage.ReadAll(userId));
                    var email = parsedCommand.Data as string;
                    resultData = email;
                    resultType = InputProcessingResult.MailSent;
                    break;

                case InputProcessingCommand.RequestHelp:
                    resultType = InputProcessingResult.HelpRequested;
                    break;

                case InputProcessingCommand.RequestExit:
                    resultType = InputProcessingResult.ExitRequested;
                    break;

                case InputProcessingCommand.SayUnknownCommand:
                    resultData = parsedCommand.Command;
                    resultType = InputProcessingResult.Error;
                    break;

                case InputProcessingCommand.SayIllegalArguments:
                    resultData = parsedCommand.Data;
                    resultType = InputProcessingResult.Error;
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
                CultureInfo = culture,
            };
        }
    }
}
