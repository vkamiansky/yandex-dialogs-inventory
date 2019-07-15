using System;
using System.Globalization;
using System.Linq;
using AliceInventory.Data;
using AliceInventory.Logic;
using AliceInventory.Logic.Cache;
using AliceInventory.Logic.Email;
using AliceInventory.Logic.Parser;
using Moq;
using Xunit;
using Entry = AliceInventory.Data.Entry;
using UnitOfMeasure = AliceInventory.Data.UnitOfMeasure;

namespace AliceInventory.UnitTests
{
    public class InventoryDialogServiceTests
    {
        private const double TOLERANCE = 0.01d;

        [Theory]
        [InlineData(
            "яблоки", 123, 15.5f, UnitOfMeasure.Kg,
            ProcessingResultType.Added, "груши", 3, Logic.UnitOfMeasure.Unit,
            "добавь яблоки 12,511 кг", ParsedPhraseType.Add, "яблоки", 12.511d, Logic.UnitOfMeasure.Kg,
            28.011d,
            ProcessingResultType.Added, "яблоки", 12.511d, Logic.UnitOfMeasure.Kg)]
        [InlineData(
            "яблоки", 123, 15.5f, UnitOfMeasure.Kg,
            ProcessingResultType.Added, "груши", 3, Logic.UnitOfMeasure.Unit,
            "убери яблоки 12,2 кг", ParsedPhraseType.Delete, "яблоки", 12.2d, Logic.UnitOfMeasure.Kg,
            3.3d,
            ProcessingResultType.Deleted, "яблоки", 12.2d, Logic.UnitOfMeasure.Kg)]
        [InlineData(
            "груши", 123, 15.5f, UnitOfMeasure.Kg,
            ProcessingResultType.Added, "груши", 3, Logic.UnitOfMeasure.Kg,
            "ещё 12,2", ParsedPhraseType.More, null, 12.2d, null,
            27.7d,
            ProcessingResultType.Added, "груши", 12.2d, Logic.UnitOfMeasure.Kg)]
        [InlineData(
            "груши", 123, 15.5f, UnitOfMeasure.Kg,
            ProcessingResultType.Deleted, "груши", 3, Logic.UnitOfMeasure.Kg,
            "ещё 12,2", ParsedPhraseType.More, null, 12.2d, null,
            3.3d,
            ProcessingResultType.Deleted, "груши", 12.2d, Logic.UnitOfMeasure.Kg)]
        [InlineData(
            "груши", 123, 15.5f, UnitOfMeasure.Kg,
            ProcessingResultType.Deleted, "груши", 3, Logic.UnitOfMeasure.Kg,
            "отмена", ParsedPhraseType.Cancel, null, null, null,
            18.5f,
            ProcessingResultType.DeleteCanceled, "груши", 3, Logic.UnitOfMeasure.Kg)]
        [InlineData(
            "груши", 123, 15.5f, UnitOfMeasure.Kg,
            ProcessingResultType.Added, "груши", 3, Logic.UnitOfMeasure.Kg,
            "отмена", ParsedPhraseType.Cancel, null, null, null,
            12.5f,
            ProcessingResultType.AddCanceled, "груши", 3, Logic.UnitOfMeasure.Kg)]
        public void ProcessEntryUpdateCommands(
            string storageEntryName,
            int storageEntryId,
            double storageEntryQuantity,
            UnitOfMeasure storageEntryUnit,
            ProcessingResultType stateType,
            string stateName,
            double stateQuantity,
            Logic.UnitOfMeasure stateUnit,
            string userInput,
            ParsedPhraseType entryType,
            string entryName,
            double? entryQuantity,
            Logic.UnitOfMeasure? entryUnit,
            double updateQuantity,
            ProcessingResultType resultType,
            string resultName,
            double resultQuantity,
            Logic.UnitOfMeasure resultUnit
        )
        {
            var userId = "user1";

            // These are the entries already in storage
            var entries = new[]
            {
                new Entry
                {
                    Id = storageEntryId,
                    UserId = userId,
                    Name = storageEntryName,
                    UnitOfMeasure = storageEntryUnit,
                    Quantity = storageEntryQuantity
                },
                new Entry
                {
                    Id = storageEntryId + 1,
                    UserId = userId,
                    Name = storageEntryName + "text",
                    UnitOfMeasure = storageEntryUnit,
                    Quantity = storageEntryQuantity + 1
                }
            };

            var storageMock = new Mock<IUserDataStorage>(MockBehavior.Strict);

            // Returning entries from storage
            storageMock.Setup(x => x.ReadAllEntries(
                    It.Is<string>(y => y == userId)))
                .Returns(entries);
            // Accepting request for entry update
            storageMock.Setup(x => x.UpdateEntry(
                It.Is<int>(y => y == storageEntryId),
                It.Is<double>(y => Math.Abs(y - updateQuantity) < TOLERANCE)));

            var russianCulture = new CultureInfo("ru-RU");
            // The entry as recognized by the parser
            var parsedCommand = new ParsedCommand
            {
                Type = entryType,
                Data = new ParsedEntry
                {
                    Unit = entryUnit,
                    Name = entryName,
                    Quantity = entryQuantity
                }
            };

            // Returning a parsed user command
            var parserMock = new Mock<IInputParserService>(MockBehavior.Strict);
            parserMock.Setup(x => x.ParseInput(
                    It.Is<string>(y => y == userInput),
                    It.Is<CultureInfo>(y => y == russianCulture)))
                .Returns(parsedCommand);

            var cacheMock = new Mock<IResultCache>(MockBehavior.Strict);
            // Returning the result of last successful logical operation
            cacheMock.Setup(x => x.Get(
                    It.Is<string>(y => y == userId)))
                .Returns(new ProcessingResult(
                    stateType,
                    new Logic.Entry
                    {
                        Name = stateName,
                        Quantity = stateQuantity,
                        UnitOfMeasure = stateUnit
                    }));
            // Accepting the result of the current completed logical operation
            cacheMock.Setup(x => x.Set(
                It.Is<string>(y => y == userId),
                It.Is<ProcessingResult>(y =>
                    y.Type == resultType
                    && (y.Data as Logic.Entry).Name == resultName
                    && (y.Data as Logic.Entry).Quantity == resultQuantity
                    && (y.Data as Logic.Entry).UnitOfMeasure == resultUnit)));

            var sut = new InventoryDialogService(
                storageMock.Object,
                parserMock.Object,
                cacheMock.Object,
                null);

            var result = sut.ProcessInput(userId, userInput, russianCulture);

            // Checking the result
            var resultEntry = result.Data as Logic.Entry;
            Assert.Equal(resultType, result.Type);
            Assert.Equal(resultName, resultEntry.Name);
            Assert.Equal(resultQuantity, resultEntry.Quantity);
            Assert.Equal(resultUnit, resultEntry.UnitOfMeasure);

            // Making sure no unnecessary calls have been made
            parserMock.Verify(x => x.ParseInput(
                It.IsAny<string>(),
                It.IsAny<CultureInfo>()), Times.Once);

            cacheMock.Verify(x => x.Get(
                It.IsAny<string>()), Times.Once);
            cacheMock.Verify(x => x.Set(
                It.IsAny<string>(),
                It.IsAny<ProcessingResult>()), Times.Once);

            storageMock.Verify(x => x.ReadAllEntries(
                It.IsAny<string>()), Times.Once);
            storageMock.Verify(x => x.UpdateEntry(
                It.IsAny<int>(),
                It.IsAny<double>()), Times.Once);
        }

        [Theory]
        [InlineData("привет", ProcessingResultType.GreetingRequested, ParsedPhraseType.Hello,
            ProcessingResultType.GreetingRequested)]
        [InlineData("помощь", ProcessingResultType.GreetingRequested, ParsedPhraseType.Help,
            ProcessingResultType.HelpRequested)]
        [InlineData("выход", ProcessingResultType.GreetingRequested, ParsedPhraseType.Exit,
            ProcessingResultType.ExitRequested)]
        [InlineData("нет", ProcessingResultType.ClearRequested, ParsedPhraseType.Decline,
            ProcessingResultType.Declined)]
        [InlineData("очисти список", ProcessingResultType.GreetingRequested, ParsedPhraseType.Clear,
            ProcessingResultType.ClearRequested)]
        public void ProcessSimpleCommands(
            string userInput,
            ProcessingResultType stateType,
            ParsedPhraseType entryType,
            ProcessingResultType resultType)
        {
            var userId = "user1";
            var russianCulture = new CultureInfo("ru-RU");
            // The entry as recognized by the parser
            var parsedCommand = new ParsedCommand
            {
                Type = entryType
            };

            // Returning a parsed user command
            var parserMock = new Mock<IInputParserService>(MockBehavior.Strict);
            parserMock.Setup(x => x.ParseInput(
                    It.Is<string>(y => y == userInput),
                    It.Is<CultureInfo>(y => y == russianCulture)))
                .Returns(parsedCommand);

            var cacheMock = new Mock<IResultCache>(MockBehavior.Strict);
            // Returning the result of last successful logical operation
            cacheMock.Setup(x => x.Get(
                    It.Is<string>(y => y == userId)))
                .Returns(new ProcessingResult(stateType));
            // Accepting the result of the current completed logical operation
            cacheMock.Setup(x => x.Set(
                It.Is<string>(y => y == userId),
                It.Is<ProcessingResult>(y => y.Type == resultType)));

            var sut = new InventoryDialogService(
                null,
                parserMock.Object,
                cacheMock.Object,
                null);

            var result = sut.ProcessInput(userId, userInput, russianCulture);

            // Checking the result
            Assert.Equal(resultType, result.Type);
            Assert.Null(result.Data);

            // Making sure no unnecessary calls have been made
            parserMock.Verify(x => x.ParseInput(
                It.IsAny<string>(),
                It.IsAny<CultureInfo>()), Times.Once);

            cacheMock.Verify(x => x.Get(
                It.IsAny<string>()), Times.Once);
            cacheMock.Verify(x => x.Set(
                It.IsAny<string>(),
                It.IsAny<ProcessingResult>()), Times.Once);
        }

        [Theory]
        [InlineData("отправь на почту", ProcessingResultType.HelpRequested, ParsedPhraseType.SendMail,
            ProcessingResultType.MailSent)]
        [InlineData("да", ProcessingResultType.MailAdded, ParsedPhraseType.SendMail, ProcessingResultType.MailSent)]
        public void ProcessSendListWithEmailStored(string userInput, ProcessingResultType stateType,
            ParsedPhraseType entryType, ProcessingResultType resultType)
        {
            var emailAddress = "some.mail@test.org";
            var userId = "user1";
            var entries = new[]
            {
                new Entry
                {
                    Id = 255,
                    UserId = userId,
                    Name = "груши",
                    UnitOfMeasure = UnitOfMeasure.Kg,
                    Quantity = 15.2f
                },
                new Entry
                {
                    Id = 256,
                    UserId = userId,
                    Name = "яблоки",
                    UnitOfMeasure = UnitOfMeasure.Unit,
                    Quantity = 12
                },
                new Entry
                {
                    Id = 257,
                    UserId = userId,
                    Name = "яблоки",
                    UnitOfMeasure = UnitOfMeasure.Kg,
                    Quantity = 12.5f
                }
            };

            var resultEntries = new[]
            {
                new Logic.Entry
                {
                    Name = "груши",
                    UnitOfMeasure = Logic.UnitOfMeasure.Kg,
                    Quantity = 15.2f
                },
                new Logic.Entry
                {
                    Name = "яблоки",
                    UnitOfMeasure = Logic.UnitOfMeasure.Unit,
                    Quantity = 12
                },
                new Logic.Entry
                {
                    Name = "яблоки",
                    UnitOfMeasure = Logic.UnitOfMeasure.Kg,
                    Quantity = 12.5f
                }
            };

            var storageMock = new Mock<IUserDataStorage>(MockBehavior.Strict);

            storageMock.Setup(x => x.ReadUserMail(
                    It.Is<string>(y => y == userId)))
                .Returns(emailAddress);
            // Returning entries from the storage
            storageMock.Setup(x => x.ReadAllEntries(
                    It.Is<string>(y => y == userId)))
                .Returns(entries);

            var russianCulture = new CultureInfo("ru-RU");
            // The entry as recognized by the parser
            var parsedCommand = new ParsedCommand
            {
                Type = entryType
            };

            // Returning a parsed user command
            var parserMock = new Mock<IInputParserService>(MockBehavior.Strict);
            parserMock.Setup(x => x.ParseInput(
                    It.Is<string>(y => y == userInput),
                    It.Is<CultureInfo>(y => y == russianCulture)))
                .Returns(parsedCommand);

            var cacheMock = new Mock<IResultCache>(MockBehavior.Strict);
            // Returning the result of last successful logical operation
            cacheMock.Setup(x => x.Get(
                    It.Is<string>(y => y == userId)))
                .Returns(new ProcessingResult(stateType));
            // Accepting the result of the current completed logical operation
            cacheMock.Setup(x => x.Set(
                It.Is<string>(y => y == userId),
                It.Is<ProcessingResult>(y =>
                    y.Type == resultType)));

            var emailerMock = new Mock<IInventoryEmailService>(MockBehavior.Strict);
            // Send an email with all the user's entries
            emailerMock.Setup(x => x.SendListAsync(
                It.Is<string>(y => y == emailAddress),
                It.Is<Logic.Entry[]>(y => y.Zip(resultEntries, (z1, z2) =>
                    z1.Name == z2.Name
                    && z1.Quantity == z2.Quantity
                    && z1.UnitOfMeasure == z2.UnitOfMeasure).All(z => z))));

            var sut = new InventoryDialogService(
                storageMock.Object,
                parserMock.Object,
                cacheMock.Object,
                emailerMock.Object);

            var result = sut.ProcessInput(userId, userInput, russianCulture);

            // Checking the result
            Assert.Equal(resultType, result.Type);
            Assert.Equal(emailAddress, result.Data);

            // Making sure no unnecessary calls have been made
            parserMock.Verify(x => x.ParseInput(
                It.IsAny<string>(),
                It.IsAny<CultureInfo>()), Times.Once);

            cacheMock.Verify(x => x.Get(
                It.IsAny<string>()), Times.Once);
            cacheMock.Verify(x => x.Set(
                It.IsAny<string>(),
                It.IsAny<ProcessingResult>()), Times.Once);

            storageMock.Verify(x => x.ReadUserMail(
                It.IsAny<string>()), Times.Once);
            storageMock.Verify(x => x.ReadAllEntries(
                It.IsAny<string>()), Times.Once);

            emailerMock.Verify(x => x.SendListAsync(
                It.IsAny<string>(),
                It.IsAny<Logic.Entry[]>()), Times.Once);
        }

        [Fact]
        public void ProcessAcceptClearList()
        {
            var userInput = "да";
            var userId = "user1";

            var storageMock = new Mock<IUserDataStorage>(MockBehavior.Strict);

            // Deleting all the user's entries from storage
            storageMock.Setup(x => x.DeleteAllEntries(
                It.Is<string>(y => y == userId)));

            var russianCulture = new CultureInfo("ru-RU");
            // The entry as recognized by the parser
            var parsedCommand = new ParsedCommand
            {
                Type = ParsedPhraseType.Accept
            };

            // Returning a parsed user command
            var parserMock = new Mock<IInputParserService>(MockBehavior.Strict);
            parserMock.Setup(x => x.ParseInput(
                    It.Is<string>(y => y == userInput),
                    It.Is<CultureInfo>(y => y == russianCulture)))
                .Returns(parsedCommand);

            var cacheMock = new Mock<IResultCache>(MockBehavior.Strict);
            // Returning the result of last successful logical operation
            cacheMock.Setup(x => x.Get(
                    It.Is<string>(y => y == userId)))
                .Returns(new ProcessingResult(ProcessingResultType.ClearRequested));
            // Accepting the result of the current completed logical operation
            cacheMock.Setup(x => x.Set(
                It.Is<string>(y => y == userId),
                It.Is<ProcessingResult>(y => y.Type == ProcessingResultType.Cleared)));

            var sut = new InventoryDialogService(
                storageMock.Object,
                parserMock.Object,
                cacheMock.Object,
                null);

            var result = sut.ProcessInput(userId, userInput, russianCulture);

            // Checking the result
            Assert.Equal(ProcessingResultType.Cleared, result.Type);

            // Making sure no unnecessary calls have been made
            parserMock.Verify(x => x.ParseInput(
                It.IsAny<string>(),
                It.IsAny<CultureInfo>()), Times.Once);

            cacheMock.Verify(x => x.Get(
                It.IsAny<string>()), Times.Once);
            cacheMock.Verify(x => x.Set(
                It.IsAny<string>(),
                It.IsAny<ProcessingResult>()), Times.Once);

            storageMock.Verify(x => x.DeleteAllEntries(
                It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void ProcessAddMail()
        {
            var userInput = "some.mail@test.org";
            var emailAddress = "some.mail@test.org";
            var userId = "user1";

            var storageMock = new Mock<IUserDataStorage>(MockBehavior.Strict);

            // A new email address is stored
            storageMock.Setup(x => x.SetUserMail(
                It.Is<string>(y => y == userId),
                It.Is<string>(y => y == emailAddress)));

            var russianCulture = new CultureInfo("ru-RU");
            // The entry as recognized by the parser
            var parsedCommand = new ParsedCommand
            {
                Type = ParsedPhraseType.Mail,
                Data = emailAddress
            };

            // Returning a parsed user command
            var parserMock = new Mock<IInputParserService>(MockBehavior.Strict);
            parserMock.Setup(x => x.ParseInput(
                    It.Is<string>(y => y == userInput),
                    It.Is<CultureInfo>(y => y == russianCulture)))
                .Returns(parsedCommand);

            var cacheMock = new Mock<IResultCache>(MockBehavior.Strict);
            // Returning the result of last successful logical operation
            cacheMock.Setup(x => x.Get(
                    It.Is<string>(y => y == userId)))
                .Returns(new ProcessingResult(ProcessingResultType.RequestedMail));
            // Accepting the result of the current completed logical operation
            cacheMock.Setup(x => x.Set(
                It.Is<string>(y => y == userId),
                It.Is<ProcessingResult>(y =>
                    y.Type == ProcessingResultType.MailAdded
                    && y.Data.ToString() == emailAddress)));

            var sut = new InventoryDialogService(
                storageMock.Object,
                parserMock.Object,
                cacheMock.Object,
                null);

            var result = sut.ProcessInput(userId, userInput, russianCulture);

            // Checking the result
            Assert.Equal(ProcessingResultType.MailAdded, result.Type);
            Assert.Equal(emailAddress, result.Data);

            // Making sure no unnecessary calls have been made
            parserMock.Verify(x => x.ParseInput(
                It.IsAny<string>(),
                It.IsAny<CultureInfo>()), Times.Once);

            cacheMock.Verify(x => x.Get(
                It.IsAny<string>()), Times.Once);
            cacheMock.Verify(x => x.Set(
                It.IsAny<string>(),
                It.IsAny<ProcessingResult>()), Times.Once);

            storageMock.Verify(x => x.SetUserMail(
                It.IsAny<string>(),
                It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void ProcessAddNewEntry()
        {
            var userId = "user1";
            var entryName = "яблоки";
            var entryQuantity = 12.123d;
            var entryId = 123;
            var dataUnitOfMeasure = UnitOfMeasure.Kg;
            var entries = new[]
            {
                new Entry
                {
                    Id = 255,
                    UserId = userId,
                    Name = "груши",
                    UnitOfMeasure = UnitOfMeasure.Kg,
                    Quantity = 15.2f
                },
                new Entry
                {
                    Id = 256,
                    UserId = userId,
                    Name = entryName,
                    UnitOfMeasure = UnitOfMeasure.Unit,
                    Quantity = 12
                }
            };

            var storageMock = new Mock<IUserDataStorage>(MockBehavior.Strict);
            // Returning all the user's entries
            storageMock.Setup(x => x.ReadAllEntries(
                    It.Is<string>(y => y == userId)))
                .Returns(entries);
            // Creating a new entry and returning its ID
            storageMock.Setup(x => x.CreateEntry(
                    It.Is<string>(y => y == userId),
                    It.Is<string>(y => y == entryName),
                    It.Is<double>(y => Math.Abs(y - entryQuantity) < TOLERANCE),
                    It.Is<UnitOfMeasure>(y => y == dataUnitOfMeasure)))
                .Returns(entryId);

            var userInput = "добавь яблоки 12,123 кг";
            var russianCulture = new CultureInfo("ru-RU");
            // The command as returned from the parser
            var parsedCommand = new ParsedCommand
            {
                Type = ParsedPhraseType.Add,
                Data = new ParsedEntry
                {
                    Name = entryName,
                    Quantity = entryQuantity,
                    Unit = Logic.UnitOfMeasure.Kg
                }
            };

            var parserMock = new Mock<IInputParserService>(MockBehavior.Strict);
            parserMock.Setup(x => x.ParseInput(
                    It.Is<string>(y => y == userInput),
                    It.Is<CultureInfo>(y => y == russianCulture)))
                .Returns(parsedCommand);

            var cacheMock = new Mock<IResultCache>(MockBehavior.Strict);
            // Returning the last successful operation result from the cache
            cacheMock.Setup(x => x.Get(
                    It.Is<string>(y => y == userId)))
                .Returns(new ProcessingResult());
            // Saving the newly produced successful result to the cache
            cacheMock.Setup(x => x.Set(
                It.Is<string>(y => y == userId),
                It.Is<ProcessingResult>(y =>
                    y.Type == ProcessingResultType.Added
                    && (y.Data as Logic.Entry).Name == entryName
                    && (y.Data as Logic.Entry).Quantity == entryQuantity
                    && (y.Data as Logic.Entry).UnitOfMeasure == Logic.UnitOfMeasure.Kg)));

            var sut = new InventoryDialogService(
                storageMock.Object,
                parserMock.Object,
                cacheMock.Object,
                null);

            var result = sut.ProcessInput(userId, userInput, russianCulture);

            // Checking the result
            var resultEntry = result.Data as Logic.Entry;
            Assert.Equal(ProcessingResultType.Added, result.Type);
            Assert.Equal(entryName, resultEntry.Name);
            Assert.Equal(entryQuantity, resultEntry.Quantity);
            Assert.Equal(Logic.UnitOfMeasure.Kg, resultEntry.UnitOfMeasure);

            // Making sure no unnecessary calls have been made
            parserMock.Verify(x => x.ParseInput(
                It.IsAny<string>(),
                It.IsAny<CultureInfo>()), Times.Once);

            cacheMock.Verify(x => x.Get(
                It.IsAny<string>()), Times.Once);
            cacheMock.Verify(x => x.Set(
                It.IsAny<string>(),
                It.IsAny<ProcessingResult>()), Times.Once);

            storageMock.Verify(x => x.ReadAllEntries(
                It.IsAny<string>()), Times.Once);
            storageMock.Verify(x => x.CreateEntry(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<double>(),
                It.IsAny<UnitOfMeasure>()), Times.Once);
        }

        [Fact]
        public void ProcessDeleteEntry()
        {
            var userId = "user1";
            var userInput = "удали груши 15,2 кг";

            var entryName = "груши";
            var stateQuantity = 15.2f;
            var entryQuantity = 15.2f;
            var resultQuantity = 15.2f;
            var stateId = 123;
            var stateUnitOfMeasure = UnitOfMeasure.Kg;
            var entryUnitOfMeasure = Logic.UnitOfMeasure.Kg;
            var resultUnitOfMeasure = Logic.UnitOfMeasure.Kg;
            var entries = new[]
            {
                new Entry
                {
                    Id = stateId,
                    UserId = userId,
                    Name = "груши",
                    UnitOfMeasure = stateUnitOfMeasure,
                    Quantity = stateQuantity
                }
            };

            var storageMock = new Mock<IUserDataStorage>(MockBehavior.Strict);
            // Returning all the user's entries
            storageMock.Setup(x => x.ReadAllEntries(
                    It.Is<string>(y => y == userId)))
                .Returns(entries);
            // Deleting the entry by its ID
            storageMock.Setup(x => x.DeleteEntry(
                It.Is<int>(y => y == stateId)));

            var russianCulture = new CultureInfo("ru-RU");
            // The command as returned from the parser
            var parsedCommand = new ParsedCommand
            {
                Type = ParsedPhraseType.Delete,
                Data = new ParsedEntry
                {
                    Name = entryName,
                    Quantity = entryQuantity,
                    Unit = entryUnitOfMeasure
                }
            };

            var parserMock = new Mock<IInputParserService>(MockBehavior.Strict);
            parserMock.Setup(x => x.ParseInput(
                    It.Is<string>(y => y == userInput),
                    It.Is<CultureInfo>(y => y == russianCulture)))
                .Returns(parsedCommand);

            var cacheMock = new Mock<IResultCache>(MockBehavior.Strict);
            // Returning the last successful operation result from the cache
            cacheMock.Setup(x => x.Get(
                    It.Is<string>(y => y == userId)))
                .Returns(new ProcessingResult());
            // Saving the newly produced successful result to the cache
            cacheMock.Setup(x => x.Set(
                It.Is<string>(y => y == userId),
                It.Is<ProcessingResult>(y =>
                    y.Type == ProcessingResultType.Deleted
                    && (y.Data as Logic.Entry).Name == entryName
                    && (y.Data as Logic.Entry).Quantity == resultQuantity
                    && (y.Data as Logic.Entry).UnitOfMeasure == resultUnitOfMeasure)));

            var sut = new InventoryDialogService(
                storageMock.Object,
                parserMock.Object,
                cacheMock.Object,
                null);

            var result = sut.ProcessInput(userId, userInput, russianCulture);

            // Checking the result
            var resultEntry = result.Data as Logic.Entry;
            Assert.Equal(ProcessingResultType.Deleted, result.Type);
            Assert.Equal(entryName, resultEntry.Name);
            Assert.Equal(resultQuantity, resultEntry.Quantity);
            Assert.Equal(resultUnitOfMeasure, resultEntry.UnitOfMeasure);

            // Making sure no unnecessary calls have been made
            parserMock.Verify(x => x.ParseInput(
                It.IsAny<string>(),
                It.IsAny<CultureInfo>()), Times.Once);

            cacheMock.Verify(x => x.Get(
                It.IsAny<string>()), Times.Once);
            cacheMock.Verify(x => x.Set(
                It.IsAny<string>(),
                It.IsAny<ProcessingResult>()), Times.Once);

            storageMock.Verify(x => x.ReadAllEntries(
                It.IsAny<string>()), Times.Once);
            storageMock.Verify(x => x.DeleteEntry(
                It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public void ProcessDeleteMail()
        {
            var userInput = "удалить почту";
            var emailAddress = "some.mail@test.org";
            var userId = "user1";

            var storageMock = new Mock<IUserDataStorage>(MockBehavior.Strict);

            // The deleted email address is returned as a result
            storageMock.Setup(x => x.DeleteUserMail(
                    It.Is<string>(y => y == userId)))
                .Returns(emailAddress);

            var russianCulture = new CultureInfo("ru-RU");
            // The entry as recognized by the parser
            var parsedCommand = new ParsedCommand
            {
                Type = ParsedPhraseType.DeleteMail
            };

            // Returning a parsed user command
            var parserMock = new Mock<IInputParserService>(MockBehavior.Strict);
            parserMock.Setup(x => x.ParseInput(
                    It.Is<string>(y => y == userInput),
                    It.Is<CultureInfo>(y => y == russianCulture)))
                .Returns(parsedCommand);

            var cacheMock = new Mock<IResultCache>(MockBehavior.Strict);
            // Returning the result of last successful logical operation
            cacheMock.Setup(x => x.Get(
                    It.Is<string>(y => y == userId)))
                .Returns(new ProcessingResult(ProcessingResultType.HelpRequested));
            // Accepting the result of the current completed logical operation
            cacheMock.Setup(x => x.Set(
                It.Is<string>(y => y == userId),
                It.Is<ProcessingResult>(y =>
                    y.Type == ProcessingResultType.MailDeleted
                    && y.Data.ToString() == emailAddress)));

            var sut = new InventoryDialogService(
                storageMock.Object,
                parserMock.Object,
                cacheMock.Object,
                null);

            var result = sut.ProcessInput(userId, userInput, russianCulture);

            // Checking the result
            Assert.Equal(ProcessingResultType.MailDeleted, result.Type);
            Assert.Equal(emailAddress, result.Data);

            // Making sure no unnecessary calls have been made
            parserMock.Verify(x => x.ParseInput(
                It.IsAny<string>(),
                It.IsAny<CultureInfo>()), Times.Once);

            cacheMock.Verify(x => x.Get(
                It.IsAny<string>()), Times.Once);
            cacheMock.Verify(x => x.Set(
                It.IsAny<string>(),
                It.IsAny<ProcessingResult>()), Times.Once);

            storageMock.Verify(x => x.DeleteUserMail(
                It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void ProcessReadList()
        {
            var userInput = "прочти список";
            var userId = "user1";

            var entries = new[]
            {
                new Entry
                {
                    Id = 255,
                    UserId = userId,
                    Name = "груши",
                    UnitOfMeasure = UnitOfMeasure.Kg,
                    Quantity = 15.2f
                },
                new Entry
                {
                    Id = 256,
                    UserId = userId,
                    Name = "яблоки",
                    UnitOfMeasure = UnitOfMeasure.Unit,
                    Quantity = 12
                },
                new Entry
                {
                    Id = 257,
                    UserId = userId,
                    Name = "яблоки",
                    UnitOfMeasure = UnitOfMeasure.Kg,
                    Quantity = 12.5f
                }
            };

            var resultEntries = new[]
            {
                new Logic.Entry
                {
                    Name = "груши",
                    UnitOfMeasure = Logic.UnitOfMeasure.Kg,
                    Quantity = 15.2f
                },
                new Logic.Entry
                {
                    Name = "яблоки",
                    UnitOfMeasure = Logic.UnitOfMeasure.Unit,
                    Quantity = 12
                },
                new Logic.Entry
                {
                    Name = "яблоки",
                    UnitOfMeasure = Logic.UnitOfMeasure.Kg,
                    Quantity = 12.5f
                }
            };

            var storageMock = new Mock<IUserDataStorage>(MockBehavior.Strict);

            // Returning entries from storage
            storageMock.Setup(x => x.ReadAllEntries(
                    It.Is<string>(y => y == userId)))
                .Returns(entries);

            var russianCulture = new CultureInfo("ru-RU");
            // The entry as recognized by the parser
            var parsedCommand = new ParsedCommand
            {
                Type = ParsedPhraseType.ReadList
            };

            // Returning a parsed user command
            var parserMock = new Mock<IInputParserService>(MockBehavior.Strict);
            parserMock.Setup(x => x.ParseInput(
                    It.Is<string>(y => y == userInput),
                    It.Is<CultureInfo>(y => y == russianCulture)))
                .Returns(parsedCommand);

            var cacheMock = new Mock<IResultCache>(MockBehavior.Strict);
            // Returning the result of last successful logical operation
            cacheMock.Setup(x => x.Get(
                    It.Is<string>(y => y == userId)))
                .Returns(new ProcessingResult(ProcessingResultType.HelpRequested));
            // Accepting the result of the current completed logical operation
            cacheMock.Setup(x => x.Set(
                It.Is<string>(y => y == userId),
                It.Is<ProcessingResult>(y =>
                    y.Type == ProcessingResultType.ListRead
                    && ((Logic.Entry[]) y.Data).Length == entries.Length
                    && ((Logic.Entry[]) y.Data).Zip(resultEntries, (z1, z2) =>
                        z1.Name == z2.Name
                        && z1.Quantity == z2.Quantity
                        && z1.UnitOfMeasure == z2.UnitOfMeasure).All(z => z)
                )));

            var sut = new InventoryDialogService(
                storageMock.Object,
                parserMock.Object,
                cacheMock.Object,
                null);

            var result = sut.ProcessInput(userId, userInput, russianCulture);

            // Checking the result
            Assert.Equal(ProcessingResultType.ListRead, result.Type);
            Assert.Equal(resultEntries.Length, ((Logic.Entry[]) result.Data).Length);
            Assert.All(resultEntries.Zip((Logic.Entry[]) result.Data, (expected, actual) => (expected, actual)),
                x =>
                {
                    Assert.Equal(x.expected.Name, x.actual.Name);
                    Assert.Equal(x.expected.Quantity, x.actual.Quantity);
                    Assert.Equal(x.expected.UnitOfMeasure, x.actual.UnitOfMeasure);
                });

            // Making sure no unnecessary calls have been made
            parserMock.Verify(x => x.ParseInput(
                It.IsAny<string>(),
                It.IsAny<CultureInfo>()), Times.Once);

            cacheMock.Verify(x => x.Get(
                It.IsAny<string>()), Times.Once);
            cacheMock.Verify(x => x.Set(
                It.IsAny<string>(),
                It.IsAny<ProcessingResult>()), Times.Once);

            storageMock.Verify(x => x.ReadAllEntries(
                It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void ProcessRequestSendListWithEmailSpecified()
        {
            var userInput = "отправь на почту some.mail@test.org";
            var emailAddress = "some.mail@test.org";
            var userId = "user1";
            var entries = new[]
            {
                new Entry
                {
                    Id = 255,
                    UserId = userId,
                    Name = "груши",
                    UnitOfMeasure = UnitOfMeasure.Kg,
                    Quantity = 15.2f
                },
                new Entry
                {
                    Id = 256,
                    UserId = userId,
                    Name = "яблоки",
                    UnitOfMeasure = UnitOfMeasure.Unit,
                    Quantity = 12
                },
                new Entry
                {
                    Id = 257,
                    UserId = userId,
                    Name = "яблоки",
                    UnitOfMeasure = UnitOfMeasure.Kg,
                    Quantity = 12.5f
                }
            };

            var resultEntries = new[]
            {
                new Logic.Entry
                {
                    Name = "груши",
                    UnitOfMeasure = Logic.UnitOfMeasure.Kg,
                    Quantity = 15.2f
                },
                new Logic.Entry
                {
                    Name = "яблоки",
                    UnitOfMeasure = Logic.UnitOfMeasure.Unit,
                    Quantity = 12
                },
                new Logic.Entry
                {
                    Name = "яблоки",
                    UnitOfMeasure = Logic.UnitOfMeasure.Kg,
                    Quantity = 12.5f
                }
            };

            var storageMock = new Mock<IUserDataStorage>(MockBehavior.Strict);

            // Saving the user's email
            storageMock.Setup(x => x.SetUserMail(
                It.Is<string>(y => y == userId),
                It.Is<string>(y => y == emailAddress)));
            // Returning entries from storage
            storageMock.Setup(x => x.ReadAllEntries(
                    It.Is<string>(y => y == userId)))
                .Returns(entries);

            var russianCulture = new CultureInfo("ru-RU");
            // The entry as recognized by the parser
            var parsedCommand = new ParsedCommand
            {
                Type = ParsedPhraseType.SendMail,
                Data = emailAddress
            };

            // Returning a parsed user command
            var parserMock = new Mock<IInputParserService>(MockBehavior.Strict);
            parserMock.Setup(x => x.ParseInput(
                    It.Is<string>(y => y == userInput),
                    It.Is<CultureInfo>(y => y == russianCulture)))
                .Returns(parsedCommand);

            var cacheMock = new Mock<IResultCache>(MockBehavior.Strict);
            // Returning the result of last successful logical operation
            cacheMock.Setup(x => x.Get(
                    It.Is<string>(y => y == userId)))
                .Returns(new ProcessingResult(ProcessingResultType.HelpRequested));
            // Accepting the result of the current completed logical operation
            cacheMock.Setup(x => x.Set(
                It.Is<string>(y => y == userId),
                It.Is<ProcessingResult>(y =>
                    y.Type == ProcessingResultType.MailSent
                    && y.Data.ToString() == emailAddress)));

            var emailerMock = new Mock<IInventoryEmailService>(MockBehavior.Strict);
            // Send an email with all the user's entries
            emailerMock.Setup(x => x.SendListAsync(
                It.Is<string>(y => y == emailAddress),
                It.Is<Logic.Entry[]>(y => y.Zip(resultEntries, (z1, z2) =>
                    z1.Name == z2.Name
                    && z1.Quantity == z2.Quantity
                    && z1.UnitOfMeasure == z2.UnitOfMeasure).All(z => z))));

            var sut = new InventoryDialogService(
                storageMock.Object,
                parserMock.Object,
                cacheMock.Object,
                emailerMock.Object);

            var result = sut.ProcessInput(userId, userInput, russianCulture);

            // Checking the result
            Assert.Equal(ProcessingResultType.MailSent, result.Type);
            Assert.Equal(emailAddress, result.Data);

            // Making sure no unnecessary calls have been made
            parserMock.Verify(x => x.ParseInput(
                It.IsAny<string>(),
                It.IsAny<CultureInfo>()), Times.Once);

            cacheMock.Verify(x => x.Get(
                It.IsAny<string>()), Times.Once);
            cacheMock.Verify(x => x.Set(
                It.IsAny<string>(),
                It.IsAny<ProcessingResult>()), Times.Once);

            storageMock.Verify(x => x.SetUserMail(
                It.IsAny<string>(),
                It.IsAny<string>()), Times.Once);
            storageMock.Verify(x => x.ReadAllEntries(
                It.IsAny<string>()), Times.Once);

            emailerMock.Verify(x => x.SendListAsync(
                It.IsAny<string>(),
                It.IsAny<Logic.Entry[]>()), Times.Once);
        }

        [Fact]
        public void ProcessRequestSendListWithoutEmailSpecified()
        {
            var userInput = "отправь на почту";
            var userId = "user1";

            var storageMock = new Mock<IUserDataStorage>(MockBehavior.Strict);

            // No email stored for user
            storageMock.Setup(x => x.ReadUserMail(
                    It.Is<string>(y => y == userId)))
                .Returns(string.Empty);

            var russianCulture = new CultureInfo("ru-RU");
            // The entry as recognized by the parser
            var parsedCommand = new ParsedCommand
            {
                Type = ParsedPhraseType.SendMail
            };

            // Returning a parsed user command
            var parserMock = new Mock<IInputParserService>(MockBehavior.Strict);
            parserMock.Setup(x => x.ParseInput(
                    It.Is<string>(y => y == userInput),
                    It.Is<CultureInfo>(y => y == russianCulture)))
                .Returns(parsedCommand);

            var cacheMock = new Mock<IResultCache>(MockBehavior.Strict);
            // Returning the result of last successful logical operation
            cacheMock.Setup(x => x.Get(
                    It.Is<string>(y => y == userId)))
                .Returns(new ProcessingResult(ProcessingResultType.HelpRequested));
            // Accepting the result of the current completed logical operation
            cacheMock.Setup(x => x.Set(
                It.Is<string>(y => y == userId),
                It.Is<ProcessingResult>(y => y.Type == ProcessingResultType.RequestedMail)));

            var sut = new InventoryDialogService(
                storageMock.Object,
                parserMock.Object,
                cacheMock.Object,
                null);

            var result = sut.ProcessInput(userId, userInput, russianCulture);

            // Checking the result
            Assert.Equal(ProcessingResultType.RequestedMail, result.Type);

            // Making sure no unnecessary calls have been made
            parserMock.Verify(x => x.ParseInput(
                It.IsAny<string>(),
                It.IsAny<CultureInfo>()), Times.Once);

            cacheMock.Verify(x => x.Get(
                It.IsAny<string>()), Times.Once);
            cacheMock.Verify(x => x.Set(
                It.IsAny<string>(),
                It.IsAny<ProcessingResult>()), Times.Once);

            storageMock.Verify(x => x.ReadUserMail(
                It.IsAny<string>()), Times.Once);
        }
    }
}