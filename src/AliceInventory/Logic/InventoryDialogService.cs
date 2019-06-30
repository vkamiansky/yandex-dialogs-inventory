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

            ProcessingCommand currentCommand = new ProcessingCommand();
            ProcessingCommand prevCommand = commandCache.Get(userId);

            ProcessingResult result = null;

            switch (parsedCommand.Type)
            {
                case ParsedCommandType.SayHello:
                    {
                        currentCommand.Type = ProcessingCommandType.SayHello;
                        result = new ProcessingResult(ProcessingResultType.GreetingRequested);
                        break;
                    }

                case ParsedCommandType.Accept when prevCommand.Type == ProcessingCommandType.RequestClear:
                    {
                        currentCommand.Type = ProcessingCommandType.Clear;
                        storage.ClearInventory(userId);
                        result = new ProcessingResult(ProcessingResultType.Cleared);
                        break;
                    }

                case ParsedCommandType.Accept when prevCommand.Type == ProcessingCommandType.AddMail:
                    {
                        goto case ParsedCommandType.SendMail;
                    }

                case ParsedCommandType.Decline:
                    {
                        currentCommand.Type = ProcessingCommandType.None;
                        result = new ProcessingResult(ProcessingResultType.Declined);
                        break;
                    }

                case ParsedCommandType.Cancel when prevCommand.Type == ProcessingCommandType.Add:
                    {
                        if (!(prevCommand.Data is SingleEntry prevEntry)) goto default;

                        DeleteEntry(userId, ref currentCommand, prevEntry);
                        result = new ProcessingResult(ProcessingResultType.AddCanceled, prevEntry);
                        break;
                    }

                case ParsedCommandType.Cancel when prevCommand.Type == ProcessingCommandType.Delete:
                    {
                        if (!(prevCommand.Data is SingleEntry prevEntry)) goto default;

                        AddEntry(userId, ref currentCommand, prevEntry);
                        result = new ProcessingResult(ProcessingResultType.DeleteCanceled, prevEntry);
                        break;
                    }

                case ParsedCommandType.Add:
                    {
                        if (!(parsedCommand.Data is ParsedSingleEntry parsedEntry)) goto default;
                        if (parsedEntry.Name is null) goto default;

                        var entry = ConvertToSingleEntry(parsedEntry);
                        result = AddEntry(userId, ref currentCommand, entry);
                        break;
                    }

                case ParsedCommandType.Delete:
                    {
                        if (!(parsedCommand.Data is ParsedSingleEntry parsedEntry)) goto default;
                        if (parsedEntry.Name is null) goto default;

                        var entry = ConvertToSingleEntry(parsedEntry);
                        result = DeleteEntry(userId, ref currentCommand, entry);
                        break;
                    }

                case ParsedCommandType.More:
                    {
                        if (!(parsedCommand.Data is ParsedSingleEntry parsedEntry)) goto default;

                        ProcessingCommandType type;
                        if (prevCommand.Type == ProcessingCommandType.Add ||
                            prevCommand.Type == ProcessingCommandType.Delete)
                        {
                            type = prevCommand.Type;
                            if (!(prevCommand.Data is SingleEntry prevEntry)) goto default;

                            if (parsedEntry.Name is null)
                            {
                                parsedEntry.Name = prevEntry.Name;

                                if (parsedEntry.Unit is null) parsedEntry.Unit = prevEntry.Unit;
                            }
                        }
                        else
                            type = ProcessingCommandType.Add;

                        var entry = ConvertToSingleEntry(parsedEntry);

                        if (entry.Name == null) goto default;

                        if (type == ProcessingCommandType.Add)
                            result = AddEntry(userId, ref currentCommand, entry);
                        else if (type == ProcessingCommandType.Delete)
                            result = DeleteEntry(userId, ref currentCommand, entry);
                        else goto default;

                        break;
                    }

                case ParsedCommandType.Clear:
                    {
                        currentCommand.Type = ProcessingCommandType.RequestClear;
                        result = new ProcessingResult(ProcessingResultType.ClearRequested);
                        break;
                    }

                case ParsedCommandType.ReadList:
                    {
                        currentCommand.Type = ProcessingCommandType.ReadList;
                        var data = Array.ConvertAll(storage.ReadAllEntries(userId), x => x.ToLogic());
                        result = new ProcessingResult(ProcessingResultType.ListRead, data);
                        break;
                    }

                case ParsedCommandType.SendMailTo:
                    {
                        currentCommand.Type = ProcessingCommandType.SendMail;
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
                        currentCommand.Type = ProcessingCommandType.SendMail;
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
                        currentCommand.Type = ProcessingCommandType.AddMail;

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
                        currentCommand.Type = ProcessingCommandType.DeleteMail;
                        var email = storage.DeleteUserEmail(userId);
                        result = new ProcessingResult(ProcessingResultType.MailDeleted, email);
                        break;
                    }

                case ParsedCommandType.RequestHelp:
                    {
                        currentCommand.Type = ProcessingCommandType.RequestHelp;
                        result = new ProcessingResult(ProcessingResultType.HelpRequested);
                        break;
                    }

                case ParsedCommandType.RequestExit:
                    {
                        currentCommand.Type = ProcessingCommandType.RequestExit;
                        result = new ProcessingResult(ProcessingResultType.ExitRequested);
                        break;
                    }

                default:
                    {
                        currentCommand.Type = ProcessingCommandType.None;
                        result = new ProcessingResult(ProcessingResultType.Error);
                        break;
                    }
            }

            commandCache.Set(userId, currentCommand);

            return result;
        }

        private ProcessingResult AddEntry(string userId, ref ProcessingCommand currentCommand, SingleEntry entry)
        {
            currentCommand.Type = ProcessingCommandType.Add;
            currentCommand.Data = entry;

            storage.AddEntry(userId, entry.Name, entry.Count, entry.Unit.ToData());

            return new ProcessingResult(ProcessingResultType.Added, entry);
        }

        private ProcessingResult DeleteEntry(string userId, ref ProcessingCommand currentCommand, SingleEntry entry)
        {
            currentCommand.Type = ProcessingCommandType.Delete;
            currentCommand.Data = entry;

            storage.DeleteEntry(userId, entry.Name, entry.Count, entry.Unit.ToData());

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