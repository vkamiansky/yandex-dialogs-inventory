using System;
using System.Collections.Generic;
using System.Globalization;
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
                [ParsedPhraseType.Multiply] = ProcessMultiply,
                [ParsedPhraseType.Division] = ProcessDivision,
                [ParsedPhraseType.DeleteAllExcept] = ProcessDeleteAllExcept,
                [ParsedPhraseType.More] = ProcessMore,
                [ParsedPhraseType.Cancel] = ProcessCancel,
                [ParsedPhraseType.Accept] = ProcessAccept,
                [ParsedPhraseType.Decline] = ProcessDecline,
                [ParsedPhraseType.Clear] = ProcessClear,
                [ParsedPhraseType.ReadList] = ProcessReadList,
                [ParsedPhraseType.ReadItem] = ProcessReadItem,
                [ParsedPhraseType.SendMail] = ProcessSendMail,
                [ParsedPhraseType.ShowMail] = ProcessShowMail,
                [ParsedPhraseType.Mail] = ProcessAddMail,
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

        public ProcessingResult ProcessInput(string userId, UserInput input)
        {
            ParsedCommand command = _services.Parser.ParseInput(input);

            if (!_commandProcessingMethods.ContainsKey(command.Type))
                return new ProcessingResult(new KeyNotFoundException());

            var args = new ProcessingArgs
            {
                UserId = userId,
                CommandData = command.Data,
                State = _services.ResultCache.Get(userId)
            };

            var result = _commandProcessingMethods[command.Type](_services, args);
            result.CultureInfo = input.CultureInfo;

            _services.ResultCache.Set(userId, result);

            return result;
        }

        private static ProcessingResult ProcessGreeting(Services services, ProcessingArgs args)
        {
            return new ProcessingResult(ProcessingResultType.GreetingRequested);
        }

        private static ProcessingResult ProcessAccept(Services services, ProcessingArgs args)
        {
            switch (args.State.Type)
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

            // если запись прошла парсинг, но была преобразована к виду:
            // "добавь €блоки 2 шт и бананы 3 шт" => "€блоки 2 шт и бананы", 3, "шт"

            var possibleEntries = entry.Name.Split(new string[] { " и " }, StringSplitOptions.None);
            if (possibleEntries.Length > 1)
            {
                var entries = new List<Entry>();
                foreach (var e in possibleEntries)
                {
                    var info = e.Split(' ');
                    double res;
                    switch (info.Length)
                    {
                        case 1: // "добавь бананы и €блоки"
                            entries.Add(new Entry()
                            {
                                Name = info[0],
                                Quantity = (e == possibleEntries.First() || e == possibleEntries.Last()) ? entry.Quantity : 1,
                                UnitOfMeasure = (e == possibleEntries.First() || e == possibleEntries.Last()) ?
                                    Enum.GetValues(typeof(UnitOfMeasure))
                                                        .Cast<UnitOfMeasure>()
                                                        .Where(m => m.ToHtml() == RegexHelper.ParseUnitOfMeasure(info[1]).ToHtml())
                                                        .FirstOrDefault()
                                    : UnitOfMeasure.Unit
                            });
                            break;
                        case 2: // "добавь 2 €блока и 3 банана"
                            if (double.TryParse(info[0], out res))
                                entries.Add(new Entry() 
                                { 
                                    Name = info[1], 
                                    Quantity = double.Parse(info[0]), 
                                    UnitOfMeasure = UnitOfMeasure.Unit 
                                });
                            else
                                entries.Add(new Entry() 
                                { 
                                    Name = info[0], 
                                    Quantity = double.Parse(info[1]), 
                                    UnitOfMeasure = UnitOfMeasure.Unit 
                                });
                            break;
                        case 3:
                            if (double.TryParse(info[0], out res)) // "2 штуки бананов"
                                entries.Add(new Entry()
                                {
                                    Name = info[2],
                                    Quantity = double.Parse(info[0]),
                                    UnitOfMeasure = Enum.GetValues(typeof(UnitOfMeasure))
                                                        .Cast<UnitOfMeasure>()
                                                        .Where(m => m.ToHtml() == RegexHelper.ParseUnitOfMeasure(info[1]).ToHtml())
                                                        .FirstOrDefault()
                                });
                            else // "бананы 2 штуки"
                                entries.Add(new Entry()
                                {
                                    Name = info[0],
                                    Quantity = double.Parse(info[1]),
                                    UnitOfMeasure = Enum.GetValues(typeof(UnitOfMeasure))
                                                        .Cast<UnitOfMeasure>()
                                                        .Where(m => m.ToHtml() == RegexHelper.ParseUnitOfMeasure(info[2]).ToHtml())
                                                        .FirstOrDefault()
                                });
                            break;
                    }
                }
                return AddMaterials(services.Storage, args.UserId, entries);
            }
            return AddMaterial(services.Storage, args.UserId, entry);
        }

        private static ProcessingResult ProcessDelete(Services services, ProcessingArgs args)
        {
            if (!(args.CommandData is ParsedEntry parsedEntry))
                return new UnexpectedTypeException(args.CommandData, typeof(ParsedEntry));

            var entry = ConvertToEntry(parsedEntry);
            return SubtractMaterial(services.Storage, args.UserId, entry);
        }

        private static ProcessingResult ProcessMultiply(Services services, ProcessingArgs args)
        {
            if (!(args.CommandData is ParsedEntry parsedEntry))
                return new UnexpectedTypeException(args.CommandData, typeof(ParsedEntry));

            var entry = ConvertToEntry(parsedEntry);
            return MultiplyMaterial(services.Storage, args.UserId, entry);
        }

        private static ProcessingResult ProcessDivision(Services services, ProcessingArgs args)
        {
            if (!(args.CommandData is ParsedEntry parsedEntry))
                return new UnexpectedTypeException(args.CommandData, typeof(ParsedEntry));

            var entry = ConvertToEntry(parsedEntry);
            return DivisionMaterial(services.Storage, args.UserId, entry);
        }

        private static ProcessingResult ProcessDeleteAllExcept(Services services, ProcessingArgs args)
        {
            try
            {
                if (!(args.CommandData is ParsedEntry parsedEntry))
                    return new UnexpectedTypeException(args.CommandData, typeof(ParsedEntry));

                var entry = ConvertToEntry(parsedEntry);

                var entries = services.Storage.ReadAllEntries(args.UserId);
                var dataUnitOfMeasure = entry.UnitOfMeasure.ToData();
                var dbEntry = entries.FirstOrDefault(e =>
                    e.Name == entry.Name && e.UnitOfMeasure == dataUnitOfMeasure);

                if (dbEntry is null)
                    return new EntryNotFoundInDatabaseError(entry.Name, entry.UnitOfMeasure);

                services.Storage.DeleteAllEntries(args.UserId);
                services.Storage.CreateEntry(args.UserId, entry.Name, entry.Quantity, entry.UnitOfMeasure.ToData());
                return new ProcessingResult(ProcessingResultType.AllExceptDeleted, entry);
            }
            catch (Exception e)
            {
                return new ProcessingResult(e);
            }
        }

        private static ProcessingResult ProcessCancel(Services services, ProcessingArgs args)
        {
            var state = args.State;
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

                case ProcessingResultType.Multiplied:
                {
                    if (!(state.Data is Entry stateEntry))
                        return new UnexpectedTypeException(state.Data, typeof(Entry));

                    var result = DivisionMaterial(services.Storage, args.UserId, stateEntry);

                    if (result.Type != ProcessingResultType.Divided)
                        return result;

                    return new ProcessingResult(ProcessingResultType.MultiplyCanceled, stateEntry);
                }
                case ProcessingResultType.Divided:
                {
                    if (!(state.Data is Entry stateEntry))
                        return new UnexpectedTypeException(state.Data, typeof(Entry));

                    var result = MultiplyMaterial(services.Storage, args.UserId, stateEntry);

                    if (result.Type != ProcessingResultType.Multiplied)
                        return result;

                    return new ProcessingResult(ProcessingResultType.DivisionCanceled, stateEntry);
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
                    return ProcessingResultType.Error;
            }
        }

        private static ProcessingResult ProcessClear(Services services, ProcessingArgs args)
        {
            return ProcessingResultType.ClearRequested;
        }

        private static ProcessingResult ProcessReadList(Services services, ProcessingArgs args)
        {
            var entries = services.Storage.ReadAllEntries(args.UserId).ToLogic();
            return new ProcessingResult(ProcessingResultType.ListRead, entries);
        }
        private static ProcessingResult ProcessShowMail(Services services, ProcessingArgs args)
        {
            if(services.Storage.ReadUserMail(args.UserId)!=null)
            {
                 var email = services.Storage.ReadUserMail(args.UserId);
                 return new ProcessingResult(ProcessingResultType.ShowMail,email);

            }else return ProcessingResultType.RequestedMail;
           
        }

        private static ProcessingResult ProcessReadItem(Services services, ProcessingArgs args)
        {
            if (!(args.CommandData is ParsedEntry parsedEntry))
                return new UnexpectedTypeException(args.CommandData, typeof(ParsedEntry));

            var entry = ConvertToEntry(parsedEntry);

            var entries = services.Storage.ReadAllEntries(args.UserId);
            var res = entries.Where(x => x.Name == entry.Name).ToArray().ToLogic();
            return new ProcessingResult(ProcessingResultType.ItemRead, res);
        }

        private static ProcessingResult ProcessSendMail(Services services, ProcessingArgs args)
        {
            var email = args.CommandData as string;

            if (string.IsNullOrEmpty(email))
            {
                email = services.Storage.ReadUserMail(args.UserId);

                if (string.IsNullOrEmpty(email))
                    return ProcessingResultType.RequestedMail;
            }
            else
            {
                services.Storage.SetUserMail(args.UserId, email);
            }

            var entries = services.Storage.ReadAllEntries(args.UserId).ToLogic();
            if (entries.Length < 1) return new EmptyEntryListError();

            services.EmailService.SendListAsync(email, entries);
            return new ProcessingResult(ProcessingResultType.MailSent, email);
        }

        private static ProcessingResult ProcessAddMail(Services services, ProcessingArgs args)
        {
            if (!(args.CommandData is string email))
                return new UnexpectedNullOrEmptyStringException(nameof(email));

            services.Storage.SetUserMail(args.UserId, email);
            return new ProcessingResult(ProcessingResultType.MailAdded, email);
        }

        private static ProcessingResult ProcessDeleteMail(Services services, ProcessingArgs args)
        {
            var email = services.Storage.ReadUserMail(args.UserId);
            if (string.IsNullOrEmpty(email))
                return new MailIsEmptyError();

            services.Storage.DeleteUserMail(args.UserId);
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
                if (entry.Quantity <= 0) return new ProcessingResult(ProcessingResultType.InvalidCount);
                var entries = storage.ReadAllEntries(userId);
                var dbEntry = entries.FirstOrDefault(e =>
                    e.Name == entry.Name && e.UnitOfMeasure == entry.UnitOfMeasure.ToData());

                if (dbEntry is null)
                {
                    storage.CreateEntry(userId, entry.Name, entry.Quantity, entry.UnitOfMeasure.ToData());
                }
                else
                {
                    storage.UpdateEntry(dbEntry.Id, dbEntry.Quantity + entry.Quantity);
                }

                return new ProcessingResult(ProcessingResultType.Added, entry);
            }
            catch (Exception e)
            {
                return new ProcessingResult(e);
            }
        }

        private static ProcessingResult AddMaterials(IUserDataStorage storage, string userId, List<Entry> origEntries)
        {
            try
            {
                foreach (var entry in origEntries)
                {
                    if (entry.Quantity <= 0) return new ProcessingResult(ProcessingResultType.InvalidCount);
                    var entries = storage.ReadAllEntries(userId);
                    var dbEntry = entries.FirstOrDefault(e =>
                        e.Name == entry.Name && e.UnitOfMeasure == entry.UnitOfMeasure.ToData());

                    if (dbEntry is null)
                    {
                        storage.CreateEntry(userId, entry.Name, entry.Quantity, entry.UnitOfMeasure.ToData());
                    }
                    else
                    {
                        storage.UpdateEntry(dbEntry.Id, dbEntry.Quantity + entry.Quantity);
                    }
                }
                return new ProcessingResult(ProcessingResultType.AddedMany, origEntries);
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
                if (entry.Quantity <= 0) 
                    return new ProcessingResult(ProcessingResultType.InvalidCount);
                var entries = storage.ReadAllEntries(userId);
                var dataUnitOfMeasure = entry.UnitOfMeasure.ToData();
                var dbEntry = entries.FirstOrDefault(e =>
                    e.Name == entry.Name && e.UnitOfMeasure == dataUnitOfMeasure);

                if (dbEntry is null)
                    return new EntryNotFoundInDatabaseError(entry.Name, entry.UnitOfMeasure);

                var updateValue = dbEntry.Quantity - entry.Quantity;

                if (updateValue < 0)
                    return new NotEnoughEntryToDeleteError(entry.Name, entry.Quantity, dbEntry.Quantity);

                if (updateValue == 0)
                    storage.DeleteEntry(dbEntry.Id);
                else
                    storage.UpdateEntry(dbEntry.Id, dbEntry.Quantity - entry.Quantity);

                return new ProcessingResult(ProcessingResultType.Deleted, entry);
            }
            catch (Exception e)
            {
                return new ProcessingResult(e);
            }
        }

        private static ProcessingResult MultiplyMaterial(IUserDataStorage storage, string userId, Entry entry)
        {
            try
            {
                if (entry.Quantity <= 0) return new ProcessingResult(ProcessingResultType.InvalidCount);
                var entries = storage.ReadAllEntries(userId);
                var dbEntry = entries.FirstOrDefault(e => e.Name == entry.Name);

                if (dbEntry is null)
                    return new EntryNotFoundInDatabaseError(entry.Name, entry.UnitOfMeasure);

                storage.UpdateEntry(dbEntry.Id, dbEntry.Quantity * entry.Quantity);
                entry.UnitOfMeasure = dbEntry.UnitOfMeasure.ToLogic();

                return new ProcessingResult(ProcessingResultType.Multiplied, entry);
            }
            catch (Exception e)
            {
                return new ProcessingResult(e);
            }
        }

        private static ProcessingResult DivisionMaterial(IUserDataStorage storage, string userId, Entry entry)
        {
            try
            {
                if (entry.Quantity <= 0) return new ProcessingResult(ProcessingResultType.InvalidCount);
                var entries = storage.ReadAllEntries(userId);
                var dbEntry = entries.FirstOrDefault(e => e.Name == entry.Name);

                if (dbEntry is null)
                    return new EntryNotFoundInDatabaseError(entry.Name, entry.UnitOfMeasure);

                storage.UpdateEntry(dbEntry.Id, dbEntry.Quantity / entry.Quantity);

                entry.UnitOfMeasure = dbEntry.UnitOfMeasure.ToLogic();

                return new ProcessingResult(ProcessingResultType.Divided, entry);
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