using System;
using System.Globalization;
using System.Linq;
using AliceInventory.Data;
using AliceInventory.Logic.Core.Errors;
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
                        storage.DeleteAllEntries(userId);
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
                        if (!(prevResult.Data is Entry prevEntry)) goto default;

                        SubtractEntry(userId, prevEntry);
                        result = new ProcessingResult(ProcessingResultType.AddCanceled, prevEntry);
                        break;
                    }

                case ParsedCommandType.Cancel when prevResult.Type == ProcessingResultType.Deleted:
                    {
                        if (!(prevResult.Data is Entry prevEntry)) goto default;

                        AddEntry(userId, prevEntry);
                        result = new ProcessingResult(ProcessingResultType.DeleteCanceled, prevEntry);
                        break;
                    }

                case ParsedCommandType.Add:
                    {
                        if (!(parsedCommand.Data is ParsedEntry parsedEntry)) goto default;
                        if (parsedEntry.Name is null) goto default;

                        var entry = ConvertToEntry(parsedEntry);
                        result = AddEntry(userId, entry);
                        break;
                    }

                case ParsedCommandType.Delete:
                    {
                        if (!(parsedCommand.Data is ParsedEntry parsedEntry)) goto default;
                        if (parsedEntry.Name is null) goto default;

                        var entry = ConvertToEntry(parsedEntry);
                        result = SubtractEntry(userId, entry);
                        break;
                    }

                case ParsedCommandType.More:
                    {
                        if (!(parsedCommand.Data is ParsedEntry parsedEntry)) goto default;

                        ProcessingResultType type;
                        if (prevResult.Type == ProcessingResultType.Added ||
                            prevResult.Type == ProcessingResultType.Deleted)
                        {
                            type = prevResult.Type;
                            if (!(prevResult.Data is Entry prevEntry)) goto default;

                            if (parsedEntry.Name is null)
                            {
                                parsedEntry.Name = prevEntry.Name;

                                if (parsedEntry.Unit is null) parsedEntry.Unit = prevEntry.UnitOfMeasure;
                            }
                        }
                        else
                            type = ProcessingResultType.Added;

                        var entry = ConvertToEntry(parsedEntry);

                        if (entry.Name == null) goto default;

                        if (type == ProcessingResultType.Added)
                            result = AddEntry(userId, entry);
                        else if (type == ProcessingResultType.Deleted)
                            result = SubtractEntry(userId, entry);
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
                        var entries = storage.ReadAllEntries(userId);

                        var data = Array.ConvertAll(entries, x => x.ToLogic());
                        result = new ProcessingResult(ProcessingResultType.ListRead, data);
                        break;
                    }

                case ParsedCommandType.SendMailTo:
                    {
                        var entries = Array.ConvertAll(storage.ReadAllEntries(userId), x => x.ToLogic());

                        
                        else goto default;

                        break;
                    }

                case ParsedCommandType.SendMail:
                    {
                        var entries = Array.ConvertAll(storage.ReadAllEntries(userId), x => x.ToLogic());
                        var email = storage.ReadUserEmail(userId);

                        result = SendMailTo(email, entries);
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

        private ProcessingResult AddEntry(string userId, Entry entry)
        {
            try
            {
                var entries = storage.ReadAllEntries(userId);
                var dbEntry = entries.FirstOrDefault(e =>
                    e.Name == entry.Name && e.UnitOfMeasure == entry.UnitOfMeasure.ToData());

                if (dbEntry is null)
                {
                    storage.CreateEntry(userId, entry.Name, entry.Quantity, entry.UnitOfMeasure.ToData());
                }
                else
                {
                    storage.UpdateEntry(dbEntry.Id, entry.Quantity);
                }
            }
            catch (Exception e)
            {
                return new ProcessingResult(e);
            }

            return new ProcessingResult(ProcessingResultType.Added, entry);
        }

        private ProcessingResult SubtractEntry(string userId, Entry entry)
        {
            try
            {
                var entries = storage.ReadAllEntries(userId);
                var dbEntry = entries.FirstOrDefault(e =>
                    e.Name == entry.Name && e.UnitOfMeasure == entry.UnitOfMeasure.ToData());

                if (dbEntry is null)
                    return new EntryNotFoundError(userId, entry.Name);
                
                if (dbEntry.Quantity < entry.Quantity)
                    return new NotEnoughEntryToDeleteError(entry.Quantity, dbEntry);

                storage.UpdateEntry(dbEntry.Id, entry.Quantity);
            }
            catch (Exception e)
            {
                return new ProcessingResult(e);
            }

            return new ProcessingResult(ProcessingResultType.Deleted, entry);
        }

        private ProcessingResult SendMailTo(string email, Entry[] entries)
        {
            if (entries.Length < 1)
                return new EmptyEntryListError();
            
            if (string.IsNullOrEmpty(email))
                return new ProcessingResult(ProcessingResultType.RequestedMail);

            emailService.SendListAsync(email, entries);
            return new ProcessingResult(ProcessingResultType.MailSent, email);
        }

        private Entry ConvertToEntry(ParsedEntry parsedEntry)
        {
            return new Entry()
            {
                Name = parsedEntry.Name,
                Quantity = parsedEntry.Quantity ?? 1,
                UnitOfMeasure = parsedEntry.Unit ?? UnitOfMeasure.Unit
            };
        }
    }
}