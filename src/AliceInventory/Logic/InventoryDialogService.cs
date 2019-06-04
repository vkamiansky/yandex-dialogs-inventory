using System;
using System.Globalization;
using System.Linq;
using AliceInventory.Data;
using AliceInventory.Logic.Email;

namespace AliceInventory.Logic
{
    public class InventoryDialogService : IInventoryDialogService
    {
        private readonly IUserDataStorage storage;
        private readonly IInputParserService parser;
        private readonly ICommandCache commandCache;
        private readonly IInventoryEmailService emailService;

        public InventoryDialogService(IUserDataStorage storage, IInputParserService parser, ICommandCache commandCache, IInventoryEmailService emailService)
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
            ProcessingCommand prevCommand = commandCache.Get(userId);

            switch (parsedCommand.Command)
            {
                case InputProcessingCommand.SayHello:
                {
                    resultType = InputProcessingResult.GreetingRequested;
                    break;
                }

                case InputProcessingCommand.Accept when prevCommand.Command == InputProcessingCommand.Clear:
                {
                    storage.ClearInventory(userId);
                    resultType = InputProcessingResult.Cleared;

                    break;
                }

                case InputProcessingCommand.Accept when prevCommand.Command == InputProcessingCommand.AddMail:
                {
                    goto case InputProcessingCommand.SendMail;
                }

                case InputProcessingCommand.Decline:
                {
                    resultType = InputProcessingResult.Declined;
                    break;
                }

                case InputProcessingCommand.Cancel when prevCommand.Command == InputProcessingCommand.Add:
                {
                    if (!(prevCommand.Data is SingleEntry singleEntry)) goto default;
                    storage.DeleteEntry(userId, singleEntry.Name, singleEntry.Count, singleEntry.Unit.ToData());
                    resultData = (Logic.SingleEntry) prevCommand.Data;
                    resultType = InputProcessingResult.AddCanceled;
                    break;
                }
                
                case InputProcessingCommand.Cancel when prevCommand.Command == InputProcessingCommand.Delete:
                {
                    if (!(prevCommand.Data is SingleEntry singleEntry)) goto default;
                    storage.AddEntry(userId, singleEntry.Name, singleEntry.Count, singleEntry.Unit.ToData());
                    resultData = (Logic.SingleEntry) prevCommand.Data;
                    resultType = InputProcessingResult.DeleteCanceled;
                    break;
                }

                case InputProcessingCommand.Add:
                {
                    if (!(parsedCommand.Data is SingleEntry singleEntry)) goto default;
                    storage.AddEntry(userId, singleEntry.Name, singleEntry.Count, singleEntry.Unit.ToData());
                    resultData = parsedCommand.Data;
                    resultType = InputProcessingResult.Added;
                    break;
                }

                case InputProcessingCommand.Delete:
                {
                    if (!(parsedCommand.Data is SingleEntry singleEntry)) goto default;
                    storage.DeleteEntry(userId, singleEntry.Name, singleEntry.Count, singleEntry.Unit.ToData());
                    resultData = parsedCommand.Data;
                    resultType = InputProcessingResult.Deleted;
                    break;
                }

                case InputProcessingCommand.Clear:
                {
                    resultType = InputProcessingResult.ClearRequested;
                    break;
                }

                case InputProcessingCommand.ReadList:
                {
                    resultData = Array.ConvertAll(storage.ReadAllEntries(userId), x => x.ToLogic());
                    resultType = InputProcessingResult.ListRead;
                    break;
                }

                case InputProcessingCommand.SendMailTo:
                {
                    var entries = Array.ConvertAll(storage.ReadAllEntries(userId), x => x.ToLogic());

                    if (entries.Length < 1)
                    {
                        resultType = InputProcessingResult.ListRead;
                        resultData = entries;
                    }
                    else if (parsedCommand.Data is string email)
                    {
                        emailService.SendListAsync(email, entries);
                        storage.SetUserEmail(userId, email);
                        resultType = InputProcessingResult.MailSent;
                        resultData = email;
                    }

                    break;
                }

                case InputProcessingCommand.SendMail:
                {
                    var entries = Array.ConvertAll(storage.ReadAllEntries(userId), x => x.ToLogic());
                    var email = storage.GetUserEmail(userId);

                    if (entries.Length < 1)
                    {
                        resultType = InputProcessingResult.ListRead;
                        resultData = entries;
                    }
                    else if (string.IsNullOrEmpty(email))
                    {
                        resultType = InputProcessingResult.RequestedMail;
                    }
                    else
                    {
                        emailService.SendListAsync(email, entries);
                        resultType = InputProcessingResult.MailSent;
                        resultData = email;
                    }

                    break;
                }
                
                case InputProcessingCommand.AddMail:
                {
                    if (parsedCommand.Data is string email)
                    {
                        storage.SetUserEmail(userId, email);
                        resultType = InputProcessingResult.MailAdded;
                        resultData = email;
                    }
                    break;
                }

                case InputProcessingCommand.DeleteMail:
                {
                    var email = storage.DeleteUserEmail(userId);
                    resultType = InputProcessingResult.MailDeleted;
                    resultData = email;
                    break;
                }

                case InputProcessingCommand.RequestHelp:
                {
                    resultType = InputProcessingResult.HelpRequested;
                    break;
                }

                case InputProcessingCommand.RequestExit:
                {
                    resultType = InputProcessingResult.ExitRequested;
                    break;
                }

                case InputProcessingCommand.SayUnknownCommand:
                {
                    resultData = parsedCommand.Command;
                    resultType = InputProcessingResult.Error;
                    break;
                }

                default:
                {
                    resultType = InputProcessingResult.Error;
                    break;
                }
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