using System;
using System.Globalization;
using System.Linq;
using AliceInventory.Data;
using AliceInventory.Logic.Email;
using AliceInventory.Logic.Parser;

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

        public ProcessingResult ProcessInput(string userId, string input, CultureInfo cultureInfo)
        {
            ParsedCommand parsedCommand = parser.ParseInput(input, cultureInfo);

            ProcessingResult prevResult = commandCache.Get(userId);

            ProcessingResult result = null;

            switch (parsedCommand.Type)
            {
                case ParsedCommandType.SayHello:
                    {
                        result = new ProcessingResult(ProcessingResultType.GreetingRequested);
                        break;
                    }

                case ParsedCommandType.Accept when prevResult.Type == ProcessingResultType.ClearRequested:
                    {
                        storage.ClearInventory(userId);
                        result = new ProcessingResult(ProcessingResultType.Cleared);
                        break;
                    }

                case ParsedCommandType.Accept when prevResult.Type == ProcessingResultType.RequestedMail:
                    {
                        goto case ParsedCommandType.SendMail;
                    }

                case ParsedCommandType.Decline:
                    {
                        result = new ProcessingResult(ProcessingResultType.Declined);
                        break;
                    }

                case ParsedCommandType.Cancel when prevResult.Type == ProcessingResultType.Added:
                    {
                        if (!(prevResult.Data is SingleEntry prevEntry)) goto default;

                        DeleteEntry(userId, prevEntry);
                        result = new ProcessingResult(ProcessingResultType.AddCanceled, prevEntry);
                        break;
                    }

                case ParsedCommandType.Cancel when prevResult.Type == ProcessingResultType.Deleted:
                    {
                        if (!(prevResult.Data is SingleEntry prevEntry)) goto default;

                        AddEntry(userId, prevEntry);
                        result = new ProcessingResult(ProcessingResultType.DeleteCanceled, prevEntry);
                        break;
                    }

                case ParsedCommandType.Add:
                    {
                        if (!(parsedCommand.Data is ParsedSingleEntry parsedEntry)) goto default;
                        if (parsedEntry.Name is null) goto default;

                        var entry = ConvertToSingleEntry(parsedEntry);
                        result = AddEntry(userId, entry);
                        break;
                    }

                case ParsedCommandType.Delete:
                    {
                        if (!(parsedCommand.Data is ParsedSingleEntry parsedEntry)) goto default;
                        if (parsedEntry.Name is null) goto default;

                        var entry = ConvertToSingleEntry(parsedEntry);
                        result = DeleteEntry(userId, entry);
                        break;
                    }

                case ParsedCommandType.More:
                    {
                        if (!(parsedCommand.Data is ParsedSingleEntry parsedEntry)) goto default;

                        ProcessingResultType type;
                        if (prevResult.Type == ProcessingResultType.Added ||
                            prevResult.Type == ProcessingResultType.Deleted)
                        {
                            type = prevResult.Type;
                            if (!(prevResult.Data is SingleEntry prevEntry)) goto default;

                            if (parsedEntry.Name is null)
                            {
                                parsedEntry.Name = prevEntry.Name;

                                if (parsedEntry.Unit is null) parsedEntry.Unit = prevEntry.Unit;
                            }
                        }
                        else
                            type = ProcessingResultType.Added;

                        var entry = ConvertToSingleEntry(parsedEntry);

                        if (entry.Name == null) goto default;

                        if (type == ProcessingResultType.Added)
                            result = AddEntry(userId, entry);
                        else if (type == ProcessingResultType.Deleted)
                            result = DeleteEntry(userId, entry);
                        else goto default;

                        break;
                    }

                case ParsedCommandType.Clear:
                    {
                        result = new ProcessingResult(ProcessingResultType.ClearRequested);
                        break;
                    }

                case ParsedCommandType.ReadList:
                    {
                        var data = Array.ConvertAll(storage.ReadAllEntries(userId), x => x.ToLogic());
                        result = new ProcessingResult(ProcessingResultType.ListRead, data);
                        break;
                    }

                case ParsedCommandType.SendMailTo:
                    {
                        var entries = Array.ConvertAll(storage.ReadAllEntries(userId), x => x.ToLogic());

                        if (entries.Length < 1)
                        {
                            result = new ProcessingResult(ProcessingResultType.ListRead, entries);
                        }
                        else if (parsedCommand.Data is string email)
                        {
                            emailService.SendListAsync(email, entries);
                            storage.SetUserEmail(userId, email);
                            result = new ProcessingResult(ProcessingResultType.MailSent, email);
                        }
                        else goto default;

                        break;
                    }

                case ParsedCommandType.SendMail:
                    {
                        var entries = Array.ConvertAll(storage.ReadAllEntries(userId), x => x.ToLogic());
                        var email = storage.GetUserEmail(userId);

                        if (entries.Length < 1)
                        {
                            result = new ProcessingResult(ProcessingResultType.ListRead, entries);
                        }
                        else if (string.IsNullOrEmpty(email))
                        {
                            result = new ProcessingResult(ProcessingResultType.RequestedMail);
                        }
                        else
                        {
                            emailService.SendListAsync(email, entries);
                            result = new ProcessingResult(ProcessingResultType.MailSent, email);
                        }

                        break;
                    }

                case ParsedCommandType.AddMail:
                    {
                        if (parsedCommand.Data is string email)
                        {
                            storage.SetUserEmail(userId, email);
                            result = new ProcessingResult(ProcessingResultType.MailAdded, email);
                        }
                        else goto default;

                        break;
                    }

                case ParsedCommandType.DeleteMail:
                    {
                        var email = storage.DeleteUserEmail(userId);
                        result = new ProcessingResult(ProcessingResultType.MailDeleted, email);
                        break;
                    }

                case ParsedCommandType.RequestHelp:
                    {
                        result = new ProcessingResult(ProcessingResultType.HelpRequested);
                        break;
                    }

                case ParsedCommandType.RequestExit:
                    {
                        result = new ProcessingResult(ProcessingResultType.ExitRequested);
                        break;
                    }

                default:
                    {
                        result = new ProcessingResult(ProcessingResultType.Error);
                        break;
                    }
            }

            commandCache.Set(userId, result);

            return result;
        }

        private ProcessingResult AddEntry(string userId, SingleEntry entry)
        {
            try
            {
                storage.AddEntry(userId, entry.Name, entry.Count, entry.Unit.ToData());
            }
            catch (Exception e)
            {
                return new ProcessingResult(e);
            }

            return new ProcessingResult(ProcessingResultType.Added, entry);
        }

        private ProcessingResult DeleteEntry(string userId, SingleEntry entry)
        {
            try
            {
                storage.DeleteEntry(userId, entry.Name, entry.Count, entry.Unit.ToData());
            }
            catch (Exception e)
            {
                return new ProcessingResult(e);
            }

            return new ProcessingResult(ProcessingResultType.Deleted, entry);
        }

        private SingleEntry ConvertToSingleEntry(ParsedSingleEntry parsedSingleEntry)
        {
            return new SingleEntry()
            {
                Name = parsedSingleEntry.Name,
                Count = parsedSingleEntry.Count ?? 1,
                Unit = parsedSingleEntry.Unit ?? UnitOfMeasure.Unit
            };
        }
    }
}