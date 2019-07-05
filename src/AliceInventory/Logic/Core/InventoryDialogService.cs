using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using AliceInventory.Data;
using AliceInventory.Logic.Cache;
using AliceInventory.Logic.Core.Errors;
using AliceInventory.Logic.Core.Exceptions;
using AliceInventory.Logic.Email;
using AliceInventory.Logic.Parser;

namespace AliceInventory.Logic
{
    public class InventoryDialogService : IInventoryDialogService
    {
        private delegate ProcessingResult CommandProcessingMethod(Services services, ProcessingArgs args);

        private class Services
        {
            public IUserDataStorage Storage { get; set; }
            public IInputParserService Parser { get; set; }
            public IResultCache ResultCache { get; set; }
            public IInventoryEmailService EmailService { get; set; }
        }

        private class ProcessingArgs
        {
            public ProcessingResult State { get; set; }
            public object CommandData { get; set; }
            public string UserId { get; set; }
        }

        // A storage link is passed separately
        private readonly Dictionary<ParsedPhraseType, CommandProcessingMethod> _commandProcessingMethods
            = new Dictionary<ParsedPhraseType, CommandProcessingMethod>()
            {
                [ParsedPhraseType.Hello] = ProcessGreeting,
                [ParsedPhraseType.Add] = ProcessAdd,
                [ParsedPhraseType.Delete] = ProcessDelete,
                [ParsedPhraseType.More] = ProcessMore,
                [ParsedPhraseType.Cancel] = ProcessCancel,
                [ParsedPhraseType.Accept] = ProcessAccept,
                [ParsedPhraseType.Decline] = ProcessDecline,
                [ParsedPhraseType.Clear] = ProcessClear,
                [ParsedPhraseType.ReadList] = ProcessReadList,
                [ParsedPhraseType.SendMail] = ProcessSendMail,
                [ParsedPhraseType.Mail] = ProcessAddEmail,
                [ParsedPhraseType.DeleteMail] = ProcessDeleteMail,
                [ParsedPhraseType.Help] = ProcessHelp,
                [ParsedPhraseType.Exit] = ProcessExit,
                [ParsedPhraseType.UnknownCommand] = ProcessUnknownCommand,
            };

        private readonly Services _services;

        public InventoryDialogService(IUserDataStorage storage, IInputParserService parser, IResultCache resultCache, IInventoryEmailService emailService)
        {
            _services = new Services
            {
                Storage = storage,
                Parser = parser,
                ResultCache = resultCache,
                EmailService = emailService
            };
        }

        public ProcessingResult ProcessInput(string userId, string input, CultureInfo cultureInfo)
        {
            ParsedCommand command = _services.Parser.ParseInput(input, cultureInfo);

            if (!_commandProcessingMethods.ContainsKey(command.Type))
                return new ProcessingResult(new KeyNotFoundException());

            var args = new ProcessingArgs
            {
                UserId = userId,
                CommandData = command.Data,
                State = _services.ResultCache.Get(userId)
            };

            var result = _commandProcessingMethods[command.Type](_services, args);

            _services.ResultCache.Set(userId, result);

            return result;
        }

        private static ProcessingResult ProcessGreeting(Services services, ProcessingArgs args)
        {
            return new ProcessingResult(ProcessingResultType.GreetingRequested);
        }

        private static ProcessingResult ProcessAccept(Services services, ProcessingArgs args)
        {
            var state = services.ResultCache.Get(args.UserId);
            switch (state.Type)
            {
                case ProcessingResultType.ClearRequested:
                    services.Storage.DeleteAllEntries(args.UserId);
                    return new ProcessingResult(ProcessingResultType.Cleared);

                case ProcessingResultType.MailAdded:
                    return ProcessSendMail(services, args);

                default:
                    return new ProcessingResult(ProcessingResultType.Error);
            }
        }

        private static ProcessingResult ProcessDecline(Services services, ProcessingArgs args)
        {
            return new ProcessingResult(ProcessingResultType.Declined);
        }

        private static ProcessingResult ProcessAdd(Services services, ProcessingArgs args)
        {
            if (!(args.CommandData is ParsedEntry parsedEntry))
                return new UnexpectedTypeException(args.CommandData, typeof(ParsedEntry));

            var entry = ConvertToEntry(parsedEntry);
            return AddMaterial(services.Storage, args.UserId, entry);
        }

        private static ProcessingResult ProcessDelete(Services services, ProcessingArgs args)
        {
            if (!(args.CommandData is ParsedEntry parsedEntry))
                return new UnexpectedTypeException(args.CommandData, typeof(ParsedEntry));

            var entry = ConvertToEntry(parsedEntry);
            return SubtractMaterial(services.Storage, args.UserId, entry);
        }

        private static ProcessingResult ProcessCancel(Services services, ProcessingArgs args)
        {
            var state = services.ResultCache.Get(args.UserId);
            switch (state.Type)
            {
                case ProcessingResultType.Added:
                {
                    if (!(state.Data is Entry stateEntry))
                        return new UnexpectedTypeException(state.Data, typeof(Entry));

                    var result = SubtractMaterial(services.Storage, args.UserId, stateEntry);

                    if (result.Type != ProcessingResultType.Deleted)
                        return result;

                    return new ProcessingResult(ProcessingResultType.AddCanceled, stateEntry);
                }

                case ProcessingResultType.Deleted:
                {
                    if (!(state.Data is Entry stateEntry))
                        return new UnexpectedTypeException(state.Data, typeof(Entry));

                    var result = AddMaterial(services.Storage, args.UserId, stateEntry);

                    if (result.Type != ProcessingResultType.Added)
                        return result;

                    return new ProcessingResult(ProcessingResultType.DeleteCanceled, stateEntry);
                }

                default:
                    return ProcessingResultType.Error;
            }
        }

        private static ProcessingResult ProcessMore(Services services, ProcessingArgs args)
        {
            if (!(args.CommandData is ParsedEntry commandEntry))
                return new ProcessingResult(ProcessingResultType.Error);

            ProcessingResultType effectiveStateType;
            if (args.State.Type == ProcessingResultType.Added ||
                args.State.Type == ProcessingResultType.Deleted)
            {
                effectiveStateType = args.State.Type;
                if (!(args.State.Data is Entry stateEntry))
                    return new ProcessingResult(ProcessingResultType.Error);

                if (commandEntry.Name is null)
                    commandEntry.Name = stateEntry.Name;

                if (commandEntry.Unit is null)
                    commandEntry.Unit = stateEntry.UnitOfMeasure;
            }
            else
                effectiveStateType = ProcessingResultType.Added;

            var entry = ConvertToEntry(commandEntry);
            if (entry.Name == null)
                return new ProcessingResult(ProcessingResultType.Error);

            switch (effectiveStateType)
            {
                case ProcessingResultType.Added:
                    return AddMaterial(services.Storage, args.UserId, entry);
                case ProcessingResultType.Deleted:
                    return SubtractMaterial(services.Storage, args.UserId, entry);
                default:
                    return new ProcessingResult(ProcessingResultType.Error);
            }
        }

        private static ProcessingResult ProcessClear(Services services, ProcessingArgs args)
        {
            services.Storage.DeleteAllEntries(args.UserId);
            return ProcessingResultType.Cleared;
        }

        private static ProcessingResult ProcessReadList(Services services, ProcessingArgs args)
        {
            var entries = services.Storage.ReadAllEntries(args.UserId).ToLogic();
            return new ProcessingResult(ProcessingResultType.ListRead, entries);
        }

        private static ProcessingResult ProcessSendMail(Services services, ProcessingArgs args)
        {
            var email = args.CommandData as string;

            if (string.IsNullOrEmpty(email))
            {
                email = services.Storage.ReadUserEmail(args.UserId);

                if (string.IsNullOrEmpty(email))
                    return ProcessingResultType.RequestedMail;
            }
            else
            {
                services.Storage.SetUserEmail(args.UserId, email);
            }

            var entries = services.Storage.ReadAllEntries(args.UserId).ToLogic();
            if (entries.Length < 1) return new EmptyEntryListError();

            services.EmailService.SendListAsync(email, entries);
            return new ProcessingResult(ProcessingResultType.MailSent, email);
        }

        private static ProcessingResult ProcessAddEmail(Services services, ProcessingArgs args)
        {
            if (!(args.CommandData is string email))
                return new UnexpectedNullOrEmptyStringException(nameof(email));

            services.Storage.SetUserEmail(args.UserId, email);
            return new ProcessingResult(ProcessingResultType.MailAdded, email);
        }

        private static ProcessingResult ProcessDeleteMail(Services services, ProcessingArgs args)
        {
            var email = services.Storage.DeleteUserEmail(args.UserId);
            if (string.IsNullOrEmpty(email))
                return new MailIsEmptyError();

            return new ProcessingResult(ProcessingResultType.MailDeleted, email);
        }

        private static ProcessingResult ProcessHelp(Services services, ProcessingArgs args)
        {
            return ProcessingResultType.HelpRequested;
        }

        private static ProcessingResult ProcessExit(Services services, ProcessingArgs args)
        {
            return ProcessingResultType.ExitRequested;
        }

        private static ProcessingResult ProcessUnknownCommand(Services services, ProcessingArgs args)
        {
            return ProcessingResultType.Error;
        }

        private static ProcessingResult AddMaterial(IUserDataStorage storage, string userId, Entry entry)
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

                return new ProcessingResult(ProcessingResultType.Added, entry);
            }
            catch (Exception e)
            {
                return new ProcessingResult(e);
            }
        }

        private static ProcessingResult SubtractMaterial(IUserDataStorage storage, string userId, Entry entry)
        {
            try
            {
                var entries = storage.ReadAllEntries(userId);
                var dataUnitOfMeasure = entry.UnitOfMeasure.ToData();
                var dbEntry = entries.FirstOrDefault(e =>
                    e.Name == entry.Name && e.UnitOfMeasure == dataUnitOfMeasure);

                if (dbEntry is null)
                    return new EntryNotFoundInDatabaseError(entry.Name, entry.UnitOfMeasure);

                if (dbEntry.Quantity < entry.Quantity)
                    return new NotEnoughEntryToDeleteError(entry.Name, entry.Quantity, dbEntry.Quantity);

                storage.UpdateEntry(dbEntry.Id, entry.Quantity);

                return new ProcessingResult(ProcessingResultType.Deleted, entry);
            }
            catch (Exception e)
            {
                return new ProcessingResult(e);
            }
        }

        private static Entry ConvertToEntry(ParsedEntry parsedEntry)
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