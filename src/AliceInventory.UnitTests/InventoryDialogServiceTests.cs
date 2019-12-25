using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AliceInventory.Logic.Parser;
using Xunit;
using Moq;

namespace AliceInventory.UnitTests
{
    public class InventoryDialogServiceTests
    {
        private const double TOLERANCE = 0.01d;

        [Fact]
        public void ProcessAddNewEntry()
        {
            var userId = "user1";
            var entryName = WordNormalizer.Normalize("яблоки");
            var entryQuantity = 12.123d;
            var entryId = Guid.NewGuid();
            var dataUnitOfMeasure = Data.UnitOfMeasure.Kg;
            var entries = new[]
            {
                new Data.Entry()
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Name = "груша",
                    UnitOfMeasure = Data.UnitOfMeasure.Kg,
                    Quantity = 15.2f
                },
                new Data.Entry()
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Name = entryName,
                    UnitOfMeasure = Data.UnitOfMeasure.Unit,
                    Quantity = 12
                },
            };

            var storageMock = new Mock<Data.IUserDataStorage>(MockBehavior.Strict);
            // Returning all the user's entries
            storageMock.Setup(x => x.ReadAllEntries(
                    It.Is<string>(y => y == userId)))
                .Returns(entries);
            // Creating a new entry and returning its ID
            storageMock.Setup(x => x.CreateEntry(
                    It.Is<string>(y => y == userId),
                    It.Is<string>(y => y == entryName),
                    It.Is<double>(y => Math.Abs(y - entryQuantity) < TOLERANCE),
                    It.Is<Data.UnitOfMeasure>(y => y == dataUnitOfMeasure)))
                .Returns(entryId);

            var russianCulture = new CultureInfo("ru-RU");
            var userInput = new Logic.UserInput
            { Raw = "raw", Prepared = "prepared", Button = "button", CultureInfo = russianCulture };
            // The command as returned from the parser
            var parsedCommand = new Logic.Parser.ParsedCommand
            {
                Type = Logic.Parser.ParsedPhraseType.Add,
                Data = new Logic.ParsedEntry
                {
                    Name = entryName,
                    Quantity = entryQuantity,
                    Unit = Logic.UnitOfMeasure.Kg
                }
            };

            var parserMock = new Mock<Logic.IInputParserService>(MockBehavior.Strict);
            parserMock.Setup(x => x.ParseInput(
                    It.Is<Logic.UserInput>(y =>
                        y.Raw == userInput.Raw
                        && y.Prepared == userInput.Prepared
                        && y.Button == userInput.Button
                        && y.CultureInfo == userInput.CultureInfo)))
                .Returns(parsedCommand);

            var cacheMock = new Mock<Logic.Cache.IResultCache>(MockBehavior.Strict);
            // Returning the last successful operation result from the cache
            cacheMock.Setup(x => x.Get(
                    It.Is<string>(y => y == userId)))
                .Returns(new Logic.ProcessingResult());
            // Saving the newly produced successful result to the cache
            cacheMock.Setup(x => x.Set(
                It.Is<string>(y => y == userId),
                It.Is<Logic.ProcessingResult>(y =>
                    y.Type == Logic.ProcessingResultType.Added
                    && (y.Data as Logic.Entry).Name == entryName
                    && (y.Data as Logic.Entry).Quantity == entryQuantity
                    && (y.Data as Logic.Entry).UnitOfMeasure == Logic.UnitOfMeasure.Kg)));

            var sut = new Logic.InventoryDialogService(
                storageMock.Object,
                parserMock.Object,
                cacheMock.Object,
                null);

            var result = sut.ProcessInput(userId, userInput);

            // Checking the result
            var resultEntry = result.Data as Logic.Entry;
            Assert.Equal(Logic.ProcessingResultType.Added, result.Type);
            Assert.Equal(entryName, resultEntry.Name);
            Assert.Equal(entryQuantity, resultEntry.Quantity);
            Assert.Equal(Logic.UnitOfMeasure.Kg, resultEntry.UnitOfMeasure);

            // Making sure no unnecessary calls have been made
            parserMock.Verify(x => x.ParseInput(
                It.IsAny<Logic.UserInput>()), Times.Once);

            cacheMock.Verify(x => x.Get(
                It.IsAny<string>()), Times.Once);
            cacheMock.Verify(x => x.Set(
                It.IsAny<string>(),
                It.IsAny<Logic.ProcessingResult>()), Times.Once);

            storageMock.Verify(x => x.ReadAllEntries(
                It.IsAny<string>()), Times.Once);
            storageMock.Verify(x => x.CreateEntry(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<double>(),
                It.IsAny<Data.UnitOfMeasure>()), Times.Once);
        }

       [Fact]
        public void ProcessDeleteEntry()
        {
            var userId = "user1";
            var entryName = WordNormalizer.Normalize("груш");
            var stateQuantity = 15.2f;
            var entryQuantity = 15.2f;
            var resultQuantity = 15.2f;
            var stateId = Guid.NewGuid();
            var stateUnitOfMeasure = Data.UnitOfMeasure.Kg;
            var entryUnitOfMeasure = Logic.UnitOfMeasure.Kg;
            var resultUnitOfMeasure = Logic.UnitOfMeasure.Kg;
            var entries = new[]
            {
                new Data.Entry()
                {
                    Id = stateId,
                    UserId = userId,
                    Name = "груша",
                    UnitOfMeasure = stateUnitOfMeasure,
                    Quantity = stateQuantity
                }
            };

            var storageMock = new Mock<Data.IUserDataStorage>(MockBehavior.Strict);
            // Returning all the user's entries
            storageMock.Setup(x => x.ReadAllEntries(
                    It.Is<string>(y => y == userId)))
                .Returns(entries);
            // Deleting the entry by its ID
            storageMock.Setup(x => x.DeleteEntry(
                It.Is<Guid>(y => y == stateId)));

            var russianCulture = new CultureInfo("ru-RU");
            var userInput = new Logic.UserInput
            { Raw = "raw", Prepared = "prepared", Button = "button", CultureInfo = russianCulture };

            // The command as returned from the parser
            var parsedCommand = new ParsedCommand
            {
                Type = ParsedPhraseType.Delete,
                Data = new Logic.ParsedEntry
                {
                    Name = entryName,
                    Quantity = entryQuantity,
                    Unit = entryUnitOfMeasure
                }
            };

            var parserMock = new Mock<Logic.IInputParserService>(MockBehavior.Strict);
            parserMock.Setup(x => x.ParseInput(
                    It.Is<Logic.UserInput>(y =>
                        y.Raw == userInput.Raw
                        && y.Prepared == userInput.Prepared
                        && y.Button == userInput.Button
                        && y.CultureInfo == userInput.CultureInfo)))
                .Returns(parsedCommand);

            var cacheMock = new Mock<Logic.Cache.IResultCache>(MockBehavior.Strict);
            // Returning the last successful operation result from the cache
            cacheMock.Setup(x => x.Get(
                    It.Is<string>(y => y == userId)))
                .Returns(new Logic.ProcessingResult());
            // Saving the newly produced successful result to the cache
            cacheMock.Setup(x => x.Set(
                It.Is<string>(y => y == userId),
                It.Is<Logic.ProcessingResult>(y =>
                    y.Type == Logic.ProcessingResultType.Deleted
                    && (y.Data as Logic.Entry).Name == entryName
                    && (y.Data as Logic.Entry).Quantity == resultQuantity
                    && (y.Data as Logic.Entry).UnitOfMeasure == resultUnitOfMeasure)));

            var sut = new Logic.InventoryDialogService(
                storageMock.Object,
                parserMock.Object,
                cacheMock.Object,
                null);

            var result = sut.ProcessInput(userId, userInput);

            // Checking the result
            var resultEntry = result.Data as Logic.Entry;
            Assert.Equal(Logic.ProcessingResultType.Deleted, result.Type);
            Assert.Equal(entryName, resultEntry.Name);
            Assert.Equal(resultQuantity, resultEntry.Quantity);
            Assert.Equal(resultUnitOfMeasure, resultEntry.UnitOfMeasure);

            // Making sure no unnecessary calls have been made
            parserMock.Verify(x => x.ParseInput(
                It.IsAny<Logic.UserInput>()), Times.Once);

            cacheMock.Verify(x => x.Get(
                It.IsAny<string>()), Times.Once);
            cacheMock.Verify(x => x.Set(
                It.IsAny<string>(),
                It.IsAny<Logic.ProcessingResult>()), Times.Once);

            storageMock.Verify(x => x.ReadAllEntries(
                It.IsAny<string>()), Times.Once);
            storageMock.Verify(x => x.DeleteEntry(
                It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public void ProcessDeleteAllExceptEntry()
        {
            var userId = "user1";
            
            var storageMock = new Mock<Data.IUserDataStorage>(MockBehavior.Strict);

            // Deleting all the user's entries from storage
            storageMock.Setup(x => x.DeleteAllEntries(
                It.Is<string>(y => y == userId)));

            var entryName = WordNormalizer.Normalize("яблоки");
            var entryQuantity = 12.123d;
            var entryId = Guid.NewGuid();
            var dataUnitOfMeasure = Data.UnitOfMeasure.Kg;
            var entries = new[]
            {
                new Data.Entry(){ }
            };

            storageMock.Setup(x => x.ReadAllEntries(
                    It.Is<string>(y => y == userId)))
                .Returns(entries);
            // Creating a new entry and returning its ID
            storageMock.Setup(x => x.CreateEntry(
                    It.Is<string>(y => y == userId),
                    It.Is<string>(y => y == entryName),
                    It.Is<double>(y => Math.Abs(y - entryQuantity) < TOLERANCE),
                    It.Is<Data.UnitOfMeasure>(y => y == dataUnitOfMeasure)))
                .Returns(entryId);

            // The command as returned from the parser
            var parsedCommand = new Logic.Parser.ParsedCommand
            {
                Type = Logic.Parser.ParsedPhraseType.Add,
                Data = new Logic.ParsedEntry
                {
                    Name = entryName,
                    Quantity = entryQuantity,
                    Unit = Logic.UnitOfMeasure.Kg
                }
            };

            var russianCulture = new CultureInfo("ru-RU");
            var userInput = new Logic.UserInput
            { Raw = "raw", Prepared = "prepared", Button = "button", CultureInfo = russianCulture };

            var parserMock = new Mock<Logic.IInputParserService>(MockBehavior.Strict);
            parserMock.Setup(x => x.ParseInput(
                    It.Is<Logic.UserInput>(y =>
                        y.Raw == userInput.Raw
                        && y.Prepared == userInput.Prepared
                        && y.Button == userInput.Button
                        && y.CultureInfo == userInput.CultureInfo)))
                .Returns(parsedCommand);

            var cacheMock = new Mock<Logic.Cache.IResultCache>(MockBehavior.Strict);
            // Returning the last successful operation result from the cache
            cacheMock.Setup(x => x.Get(
                    It.Is<string>(y => y == userId)))
                .Returns(new Logic.ProcessingResult());
            // Saving the newly produced successful result to the cache
            cacheMock.Setup(x => x.Set(
                It.Is<string>(y => y == userId),
                It.Is<Logic.ProcessingResult>(y =>
                    y.Type == Logic.ProcessingResultType.Added
                    && (y.Data as Logic.Entry).Name == entryName
                    && (y.Data as Logic.Entry).Quantity == entryQuantity
                    && (y.Data as Logic.Entry).UnitOfMeasure == Logic.UnitOfMeasure.Kg)));

            var sut = new Logic.InventoryDialogService(
                storageMock.Object,
                parserMock.Object,
                cacheMock.Object,
                null);

            var result = sut.ProcessInput(userId, userInput);

            // Checking the result
            var resultEntry = result.Data as Logic.Entry;
            Assert.Equal(Logic.ProcessingResultType.Added, result.Type);
            Assert.Equal(entryName, resultEntry.Name);
            Assert.Equal(entryQuantity, resultEntry.Quantity);
            Assert.Equal(Logic.UnitOfMeasure.Kg, resultEntry.UnitOfMeasure);

            // Making sure no unnecessary calls have been made
            parserMock.Verify(x => x.ParseInput(
                It.IsAny<Logic.UserInput>()), Times.Once);

            cacheMock.Verify(x => x.Get(
                It.IsAny<string>()), Times.Once);
            cacheMock.Verify(x => x.Set(
                It.IsAny<string>(),
                It.IsAny<Logic.ProcessingResult>()), Times.Once);

            //storageMock.Verify(x => x.ReadAllEntries(
            //    It.IsAny<string>()), Times.Once);
            storageMock.Verify(x => x.CreateEntry(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<double>(),
                It.IsAny<Data.UnitOfMeasure>()), Times.Once);
        }

        [Theory]
        [InlineData(
            "яблоки", "e8973c01-03f0-44c9-a226-ab79507cd6e8", 15.5f, Data.UnitOfMeasure.Kg,
            Logic.ProcessingResultType.Added, "груши", 3, Logic.UnitOfMeasure.Unit,
            Logic.Parser.ParsedPhraseType.Add, "яблоки", 12.511d, Logic.UnitOfMeasure.Kg,
            28.011d,
            Logic.ProcessingResultType.Added, "яблоки", 12.511d, Logic.UnitOfMeasure.Kg)]
        [InlineData(
            "яблоки", "e8973c01-03f0-44c9-a226-ab79507cd6e8", 15.5f, Data.UnitOfMeasure.Kg,
            Logic.ProcessingResultType.Added, "груши", 3, Logic.UnitOfMeasure.Unit,
            Logic.Parser.ParsedPhraseType.Delete, "яблоки", 12.2d, Logic.UnitOfMeasure.Kg,
            3.3d,
            Logic.ProcessingResultType.Deleted, "яблоки", 12.2d, Logic.UnitOfMeasure.Kg)]
        [InlineData(
            "груши", "e8973c01-03f0-44c9-a226-ab79507cd6e8", 15.5f, Data.UnitOfMeasure.Kg,
            Logic.ProcessingResultType.Added, "груши", 3, Logic.UnitOfMeasure.Kg,
            Logic.Parser.ParsedPhraseType.More, null, 12.2d, null,
            27.7d,
            Logic.ProcessingResultType.Added, "груши", 12.2d, Logic.UnitOfMeasure.Kg)]
        [InlineData(
            "груши", "e8973c01-03f0-44c9-a226-ab79507cd6e8", 15.5f, Data.UnitOfMeasure.Kg,
            Logic.ProcessingResultType.Deleted, "груши", 3, Logic.UnitOfMeasure.Kg,
            Logic.Parser.ParsedPhraseType.More, null, 12.2d, null,
            3.3d,
            Logic.ProcessingResultType.Deleted, "груши", 12.2d, Logic.UnitOfMeasure.Kg)]
        [InlineData(
            "груши", "e8973c01-03f0-44c9-a226-ab79507cd6e8", 15.5f, Data.UnitOfMeasure.Kg,
            Logic.ProcessingResultType.Deleted, "груши", 3, Logic.UnitOfMeasure.Kg,
            Logic.Parser.ParsedPhraseType.Cancel, null, null, null,
            18.5f,
            Logic.ProcessingResultType.DeleteCanceled, "груши", 3, Logic.UnitOfMeasure.Kg)]
        [InlineData(
            "груши", "e8973c01-03f0-44c9-a226-ab79507cd6e8", 15.5f, Data.UnitOfMeasure.Kg,
            Logic.ProcessingResultType.Added, "груши", 3, Logic.UnitOfMeasure.Kg,
            Logic.Parser.ParsedPhraseType.Cancel, null, null, null,
            12.5f,
            Logic.ProcessingResultType.AddCanceled, "груши", 3, Logic.UnitOfMeasure.Kg)]
        [InlineData(
            "яблоки", "e8973c01-03f0-44c9-a226-ab79507cd6e8", 15.5f, Data.UnitOfMeasure.Kg,
            Logic.ProcessingResultType.Added, "груши", 3, Logic.UnitOfMeasure.Unit,
            ParsedPhraseType.Multiply, "яблоки", 2d, Logic.UnitOfMeasure.Kg,
            31d,
            Logic.ProcessingResultType.Multiplied, "яблоки", 2d, Logic.UnitOfMeasure.Kg)]
        [InlineData(
            "яблоки", "e8973c01-03f0-44c9-a226-ab79507cd6e8", 30f, Data.UnitOfMeasure.Kg,
            Logic.ProcessingResultType.Added, "груши", 3, Logic.UnitOfMeasure.Unit,
            ParsedPhraseType.Division, "яблоки", 2d, Logic.UnitOfMeasure.Kg,
            15d,
            Logic.ProcessingResultType.Divided, "яблоки", 2d, Logic.UnitOfMeasure.Kg)]
        [InlineData(
            "груши", "e8973c01-03f0-44c9-a226-ab79507cd6e8", 15.5f, Data.UnitOfMeasure.Kg,
            Logic.ProcessingResultType.Multiplied, "груши", 5, Logic.UnitOfMeasure.Unit,
            ParsedPhraseType.Cancel, null,null,null,
            3.1d,
            Logic.ProcessingResultType.MultiplyCanceled, "груши", 5, Logic.UnitOfMeasure.Kg)]
        [InlineData(
            "груши", "e8973c01-03f0-44c9-a226-ab79507cd6e8", 5f, Data.UnitOfMeasure.Kg,
            Logic.ProcessingResultType.Divided, "груши", 5, Logic.UnitOfMeasure.Unit,
            ParsedPhraseType.Cancel, null, null, null,
            25d,
            Logic.ProcessingResultType.DivisionCanceled, "груши", 5, Logic.UnitOfMeasure.Kg)]
        public void ProcessEntryUpdateCommands(
            string storageEntryName,
            string storageEntryIdString,
            double storageEntryQuantity,
            Data.UnitOfMeasure storageEntryUnit,
            Logic.ProcessingResultType stateType,
            string stateName,
            double stateQuantity,
            Logic.UnitOfMeasure stateUnit,
            ParsedPhraseType entryType,
            string entryName,
            double? entryQuantity,
            Logic.UnitOfMeasure? entryUnit,
            double updateQuantity,
            Logic.ProcessingResultType resultType,
            string resultName,
            double resultQuantity,
            Logic.UnitOfMeasure resultUnit
        )
        {
            var userId = "user1";
            var storageEntryId = new Guid(storageEntryIdString);
            storageEntryName = WordNormalizer.Normalize(storageEntryName);
            entryName = WordNormalizer.Normalize(entryName);
            resultName = WordNormalizer.Normalize(resultName);
            stateName = WordNormalizer.Normalize(stateName);

            // These are the entries already in storage
            var entries = new[]
            {
                new Data.Entry()
                {
                    Id = storageEntryId,
                    UserId = userId,
                    Name = storageEntryName,
                    UnitOfMeasure = storageEntryUnit,
                    Quantity = storageEntryQuantity
                },
                new Data.Entry()
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Name = storageEntryName + "text",
                    UnitOfMeasure = storageEntryUnit,
                    Quantity = storageEntryQuantity + 1
                },
            };

            var storageMock = new Mock<Data.IUserDataStorage>(MockBehavior.Strict);

            // Returning entries from storage
            storageMock.Setup(x => x.ReadAllEntries(
                    It.Is<string>(y => y == userId)))
                .Returns(entries);
            // Accepting request for entry update
            storageMock.Setup(x => x.UpdateEntry(
                It.Is<Guid>(y => y == storageEntryId),
                It.Is<double>(y => Math.Abs(y - updateQuantity) < TOLERANCE)));

            var russianCulture = new CultureInfo("ru-RU");
            var userInput = new Logic.UserInput
            { Raw = "raw", Prepared = "prepared", Button = "button", CultureInfo = russianCulture };

            // The entry as recognized by the parser
            var parsedCommand = new ParsedCommand
            {
                Type = entryType,
                Data = new Logic.ParsedEntry
                {
                    Unit = entryUnit,
                    Name = entryName,
                    Quantity = entryQuantity
                }
            };

            // Returning a parsed user command
            var parserMock = new Mock<Logic.IInputParserService>(MockBehavior.Strict);
            parserMock.Setup(x => x.ParseInput(
                    It.Is<Logic.UserInput>(y =>
                        y.Raw == userInput.Raw
                        && y.Prepared == userInput.Prepared
                        && y.Button == userInput.Button
                        && y.CultureInfo == userInput.CultureInfo)))
                .Returns(parsedCommand);

            var cacheMock = new Mock<Logic.Cache.IResultCache>(MockBehavior.Strict);
            // Returning the result of last successful logical operation
            cacheMock.Setup(x => x.Get(
                    It.Is<string>(y => y == userId)))
                .Returns(new Logic.ProcessingResult(
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
                It.Is<Logic.ProcessingResult>(y =>
                    y.Type == resultType
                    && (y.Data as Logic.Entry).Name == resultName
                    && (y.Data as Logic.Entry).Quantity == resultQuantity
                    && (y.Data as Logic.Entry).UnitOfMeasure == resultUnit)));

            var sut = new Logic.InventoryDialogService(
                storageMock.Object,
                parserMock.Object,
                cacheMock.Object,
                null);

            var result = sut.ProcessInput(userId, userInput);

            // Checking the result
            var resultEntry = result.Data as Logic.Entry;
            Assert.Equal(resultType, result.Type);
            Assert.Equal(resultName, resultEntry.Name);
            Assert.Equal(resultQuantity, resultEntry.Quantity);
            Assert.Equal(resultUnit, resultEntry.UnitOfMeasure);

            // Making sure no unnecessary calls have been made
            parserMock.Verify(x => x.ParseInput(
                It.IsAny<Logic.UserInput>()), Times.Once);

            cacheMock.Verify(x => x.Get(
                It.IsAny<string>()), Times.Once);
            cacheMock.Verify(x => x.Set(
                It.IsAny<string>(),
                It.IsAny<Logic.ProcessingResult>()), Times.Once);

            storageMock.Verify(x => x.ReadAllEntries(
                It.IsAny<string>()), Times.Once);
            storageMock.Verify(x => x.UpdateEntry(
                It.IsAny<Guid>(),
                It.IsAny<double>()), Times.Once);
        }

        [Theory]
        [InlineData(
            "яблоки", "e8973c01-03f0-44c9-a226-ab79507cd6e8", 15.5f, Data.UnitOfMeasure.Kg,
            Logic.ProcessingResultType.Added, "груши", 3, Logic.UnitOfMeasure.Unit,
            Logic.Parser.ParsedPhraseType.Add, "яблоки", -12.511d, Logic.UnitOfMeasure.Kg,
            28.011d,
            Logic.ProcessingResultType.InvalidCount, "яблоки", 12.511d, Logic.UnitOfMeasure.Kg)]
        [InlineData(
            "яблоки", "e8973c01-03f0-44c9-a226-ab79507cd6e8", 15.5f, Data.UnitOfMeasure.Kg,
            Logic.ProcessingResultType.Added, "груши", 3, Logic.UnitOfMeasure.Unit,
            Logic.Parser.ParsedPhraseType.Delete, "яблоки", -12.2d, Logic.UnitOfMeasure.Kg,
            3.3d,
            Logic.ProcessingResultType.InvalidCount, "яблоки", 12.2d, Logic.UnitOfMeasure.Kg)]
        public void ProcessEntryUpdateCommandsWithInvalidCount(
            string storageEntryName,
            string storageEntryIdString,
            double storageEntryQuantity,
            Data.UnitOfMeasure storageEntryUnit,
            Logic.ProcessingResultType stateType,
            string stateName,
            double stateQuantity,
            Logic.UnitOfMeasure stateUnit,
            ParsedPhraseType entryType,
            string entryName,
            double? entryQuantity,
            Logic.UnitOfMeasure? entryUnit,
            double updateQuantity,
            Logic.ProcessingResultType resultType,
            string resultName,
            double resultQuantity,
            Logic.UnitOfMeasure resultUnit
        )
        {
            var userId = "user1";
            var storageEntryId = new Guid(storageEntryIdString);
            storageEntryName = WordNormalizer.Normalize(storageEntryName);
            entryName = WordNormalizer.Normalize(entryName);
            resultName = WordNormalizer.Normalize(resultName);
            stateName = WordNormalizer.Normalize(stateName);

            // These are the entries already in storage
            var entries = new[]
            {
                new Data.Entry()
                {
                    Id = storageEntryId,
                    UserId = userId,
                    Name = storageEntryName,
                    UnitOfMeasure = storageEntryUnit,
                    Quantity = storageEntryQuantity
                },
                new Data.Entry()
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Name = storageEntryName + "text",
                    UnitOfMeasure = storageEntryUnit,
                    Quantity = storageEntryQuantity + 1
                },
            };

            var storageMock = new Mock<Data.IUserDataStorage>(MockBehavior.Strict);

            // Returning entries from storage
            storageMock.Setup(x => x.ReadAllEntries(
                    It.Is<string>(y => y == userId)))
                .Returns(entries);
            // Accepting request for entry update
            storageMock.Setup(x => x.UpdateEntry(
                It.Is<Guid>(y => y == storageEntryId),
                It.Is<double>(y => Math.Abs(y - updateQuantity) < TOLERANCE)));

            var russianCulture = new CultureInfo("ru-RU");
            var userInput = new Logic.UserInput
            { Raw = "raw", Prepared = "prepared", Button = "button", CultureInfo = russianCulture };

            // The entry as recognized by the parser
            var parsedCommand = new ParsedCommand
            {
                Type = entryType,
                Data = new Logic.ParsedEntry
                {
                    Unit = entryUnit,
                    Name = entryName,
                    Quantity = entryQuantity
                }
            };

            // Returning a parsed user command
            var parserMock = new Mock<Logic.IInputParserService>(MockBehavior.Strict);
            parserMock.Setup(x => x.ParseInput(
                    It.Is<Logic.UserInput>(y =>
                        y.Raw == userInput.Raw
                        && y.Prepared == userInput.Prepared
                        && y.Button == userInput.Button
                        && y.CultureInfo == userInput.CultureInfo)))
                .Returns(parsedCommand);

            var cacheMock = new Mock<Logic.Cache.IResultCache>(MockBehavior.Strict);
            // Returning the result of last successful logical operation
            cacheMock.Setup(x => x.Get(
                    It.Is<string>(y => y == userId)))
                .Returns(new Logic.ProcessingResult(
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
                It.Is<Logic.ProcessingResult>(y =>
                    y.Type == resultType)));

            var sut = new Logic.InventoryDialogService(
                storageMock.Object,
                parserMock.Object,
                cacheMock.Object,
                null);

            var result = sut.ProcessInput(userId, userInput);

            // Checking the result
            var resultEntry = result.Data as Logic.Entry;
            Assert.Equal(resultType, result.Type);

            // Making sure no unnecessary calls have been made
            parserMock.Verify(x => x.ParseInput(
                It.IsAny<Logic.UserInput>()), Times.Once);

            cacheMock.Verify(x => x.Get(
                It.IsAny<string>()), Times.Once);
            cacheMock.Verify(x => x.Set(
                It.IsAny<string>(),
                It.IsAny<Logic.ProcessingResult>()), Times.Once);

        }

        [Theory]
        [InlineData(Logic.ProcessingResultType.GreetingRequested, Logic.Parser.ParsedPhraseType.Hello,
            Logic.ProcessingResultType.GreetingRequested)]
        [InlineData(Logic.ProcessingResultType.GreetingRequested, Logic.Parser.ParsedPhraseType.Help,
            Logic.ProcessingResultType.HelpRequested)]
        [InlineData(Logic.ProcessingResultType.GreetingRequested, Logic.Parser.ParsedPhraseType.Exit,
            Logic.ProcessingResultType.ExitRequested)]
        [InlineData(Logic.ProcessingResultType.ClearRequested, Logic.Parser.ParsedPhraseType.Decline,
            Logic.ProcessingResultType.Declined)]
        [InlineData(Logic.ProcessingResultType.GreetingRequested, Logic.Parser.ParsedPhraseType.Clear,
            Logic.ProcessingResultType.ClearRequested)]
        public void ProcessSimpleCommands(
            Logic.ProcessingResultType stateType,
            Logic.Parser.ParsedPhraseType entryType,
            Logic.ProcessingResultType resultType)
        {
            var userId = "user1";
            var russianCulture = new CultureInfo("ru-RU");
            var userInput = new Logic.UserInput
            { Raw = "raw", Prepared = "prepared", Button = "button", CultureInfo = russianCulture };

            // The entry as recognized by the parser
            var parsedCommand = new Logic.Parser.ParsedCommand
            {
                Type = entryType
            };

            // Returning a parsed user command
            var parserMock = new Mock<Logic.IInputParserService>(MockBehavior.Strict);
            parserMock.Setup(x => x.ParseInput(
                    It.Is<Logic.UserInput>(y =>
                        y.Raw == userInput.Raw
                        && y.Prepared == userInput.Prepared
                        && y.Button == userInput.Button
                        && y.CultureInfo == userInput.CultureInfo)))
                .Returns(parsedCommand);

            var cacheMock = new Mock<Logic.Cache.IResultCache>(MockBehavior.Strict);
            // Returning the result of last successful logical operation
            cacheMock.Setup(x => x.Get(
                    It.Is<string>(y => y == userId)))
                .Returns(new Logic.ProcessingResult(stateType));
            // Accepting the result of the current completed logical operation
            cacheMock.Setup(x => x.Set(
                It.Is<string>(y => y == userId),
                It.Is<Logic.ProcessingResult>(y => y.Type == resultType)));

            var sut = new Logic.InventoryDialogService(
                null,
                parserMock.Object,
                cacheMock.Object,
                null);

            var result = sut.ProcessInput(userId, userInput);

            // Checking the result
            Assert.Equal(resultType, result.Type);
            Assert.Null(result.Data);

            // Making sure no unnecessary calls have been made
            parserMock.Verify(x => x.ParseInput(
                It.IsAny<Logic.UserInput>()), Times.Once);

            cacheMock.Verify(x => x.Get(
                It.IsAny<string>()), Times.Once);
            cacheMock.Verify(x => x.Set(
                It.IsAny<string>(),
                It.IsAny<Logic.ProcessingResult>()), Times.Once);
        }

        [Fact]
        public void ProcessReadList()
        {
            var userId = "user1";

            var entries = new[]
            {
                new Data.Entry()
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Name = "груши",
                    UnitOfMeasure = Data.UnitOfMeasure.Kg,
                    Quantity = 15.2f
                },
                new Data.Entry()
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Name = "яблоки",
                    UnitOfMeasure = Data.UnitOfMeasure.Unit,
                    Quantity = 12
                },
                new Data.Entry()
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Name = "яблоки",
                    UnitOfMeasure = Data.UnitOfMeasure.Kg,
                    Quantity = 12.5f
                },
            };

            var resultEntries = new[]
            {
                new Logic.Entry()
                {
                    Name = "груши",
                    UnitOfMeasure = Logic.UnitOfMeasure.Kg,
                    Quantity = 15.2f
                },
                new Logic.Entry()
                {
                    Name = "яблоки",
                    UnitOfMeasure = Logic.UnitOfMeasure.Unit,
                    Quantity = 12
                },
                new Logic.Entry()
                {
                    Name = "яблоки",
                    UnitOfMeasure = Logic.UnitOfMeasure.Kg,
                    Quantity = 12.5f
                },
            };

            var storageMock = new Mock<Data.IUserDataStorage>(MockBehavior.Strict);

            // Returning entries from storage
            storageMock.Setup(x => x.ReadAllEntries(
                    It.Is<string>(y => y == userId)))
                .Returns(entries);

            var russianCulture = new CultureInfo("ru-RU");
            var userInput = new Logic.UserInput
            { Raw = "raw", Prepared = "prepared", Button = "button", CultureInfo = russianCulture };

            // The entry as recognized by the parser
            var parsedCommand = new Logic.Parser.ParsedCommand
            {
                Type = Logic.Parser.ParsedPhraseType.ReadList
            };

            // Returning a parsed user command
            var parserMock = new Mock<Logic.IInputParserService>(MockBehavior.Strict);
            parserMock.Setup(x => x.ParseInput(
                    It.Is<Logic.UserInput>(y =>
                        y.Raw == userInput.Raw
                        && y.Prepared == userInput.Prepared
                        && y.Button == userInput.Button
                        && y.CultureInfo == userInput.CultureInfo)))
                .Returns(parsedCommand);

            var cacheMock = new Mock<Logic.Cache.IResultCache>(MockBehavior.Strict);
            // Returning the result of last successful logical operation
            cacheMock.Setup(x => x.Get(
                    It.Is<string>(y => y == userId)))
                .Returns(new Logic.ProcessingResult(Logic.ProcessingResultType.HelpRequested));
            // Accepting the result of the current completed logical operation
            cacheMock.Setup(x => x.Set(
                It.Is<string>(y => y == userId),
                It.Is<Logic.ProcessingResult>(y =>
                    y.Type == Logic.ProcessingResultType.ListRead
                    && ((Logic.Entry[])y.Data).Length == entries.Length
                    && ((Logic.Entry[])y.Data).Zip(resultEntries, (z1, z2) =>
                       z1.Name == z2.Name
                       && z1.Quantity == z2.Quantity
                       && z1.UnitOfMeasure == z2.UnitOfMeasure).All(z => z)
                )));

            var sut = new Logic.InventoryDialogService(
                storageMock.Object,
                parserMock.Object,
                cacheMock.Object,
                null);

            var result = sut.ProcessInput(userId, userInput);

            // Checking the result
            Assert.Equal(Logic.ProcessingResultType.ListRead, result.Type);
            Assert.Equal(resultEntries.Length, ((Logic.Entry[])result.Data).Length);
            Assert.All(resultEntries.Zip((Logic.Entry[])result.Data, (expected, actual) => (expected, actual)),
                x =>
                {
                    Assert.Equal(x.expected.Name, x.actual.Name);
                    Assert.Equal(x.expected.Quantity, x.actual.Quantity);
                    Assert.Equal(x.expected.UnitOfMeasure, x.actual.UnitOfMeasure);
                });

            // Making sure no unnecessary calls have been made
            parserMock.Verify(x => x.ParseInput(
                It.IsAny<Logic.UserInput>()), Times.Once);

            cacheMock.Verify(x => x.Get(
                It.IsAny<string>()), Times.Once);
            cacheMock.Verify(x => x.Set(
                It.IsAny<string>(),
                It.IsAny<Logic.ProcessingResult>()), Times.Once);

            storageMock.Verify(x => x.ReadAllEntries(
                It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void ProcessReadItem()
        {
            var userId = "user1";

            var entries = new[]
            {
                new Data.Entry()
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Name = "груши",
                    UnitOfMeasure = Data.UnitOfMeasure.Kg,
                    Quantity = 15.2f
                },
                new Data.Entry()
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Name = "яблоки",
                    UnitOfMeasure = Data.UnitOfMeasure.Unit,
                    Quantity = 12
                },
                new Data.Entry()
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Name = "яблоки",
                    UnitOfMeasure = Data.UnitOfMeasure.Kg,
                    Quantity = 12.5f
                },
            };

            var resultEntries =
                new Logic.Entry()
                {
                    Name = "груш",
                    UnitOfMeasure = Logic.UnitOfMeasure.Kg,
                    Quantity = 15.2f
                };


            var storageMock = new Mock<Data.IUserDataStorage>(MockBehavior.Strict);

            // Returning entries from storage
            storageMock.Setup(x => x.ReadAllEntries(
                    It.Is<string>(y => y == userId)))
                .Returns(entries);

            var russianCulture = new CultureInfo("ru-RU");
            var userInput = new Logic.UserInput
            { Raw = "raw", Prepared = "prepared", Button = "button", CultureInfo = russianCulture };

            // The entry as recognized by the parser
            var parsedCommand = new ParsedCommand
            {
                Type = ParsedPhraseType.ReadItem,
                Data = new Logic.ParsedEntry
                {
                    Name = resultEntries.Name,
                }
            };

            // Returning a parsed user command
            var parserMock = new Mock<Logic.IInputParserService>(MockBehavior.Strict);
            parserMock.Setup(x => x.ParseInput(
                    It.Is<Logic.UserInput>(y =>
                        y.Raw == userInput.Raw
                        && y.Prepared == userInput.Prepared
                        && y.Button == userInput.Button
                        && y.CultureInfo == userInput.CultureInfo)))
                .Returns(parsedCommand);

            var cacheMock = new Mock<Logic.Cache.IResultCache>(MockBehavior.Strict);
            // Returning the result of last successful logical operation
            cacheMock.Setup(x => x.Get(
                    It.Is<string>(y => y == userId)))
                .Returns(new Logic.ProcessingResult(Logic.ProcessingResultType.HelpRequested));
            // Accepting the result of the current completed logical operation
            cacheMock.Setup(x => x.Set(
                It.Is<string>(y => y == userId),
                It.Is<Logic.ProcessingResult>(y =>
                    y.Type == Logic.ProcessingResultType.ItemRead
                    && ((Logic.Entry[])y.Data).Length == 1
                    )));

            var sut = new Logic.InventoryDialogService(
                storageMock.Object,
                parserMock.Object,
                cacheMock.Object,
                null);

            var result = sut.ProcessInput(userId, userInput);

            // Checking the result
            Assert.Equal(Logic.ProcessingResultType.ItemRead, result.Type);
            Assert.Equal(1, ((Logic.Entry[])result.Data).Length);
            Assert.All(new List<Logic.Entry>() { resultEntries }.Zip((Logic.Entry[])result.Data, (expected, actual) => (expected, actual)),
                x =>
                {
                    Assert.Equal(x.expected.Name, x.actual.Name);
                    Assert.Equal(x.expected.Quantity, x.actual.Quantity);
                    Assert.Equal(x.expected.UnitOfMeasure, x.actual.UnitOfMeasure);
                });

            // Making sure no unnecessary calls have been made
            parserMock.Verify(x => x.ParseInput(
                It.IsAny<Logic.UserInput>()), Times.Once);

            cacheMock.Verify(x => x.Get(
                It.IsAny<string>()), Times.Once);
            cacheMock.Verify(x => x.Set(
                It.IsAny<string>(),
                It.IsAny<Logic.ProcessingResult>()), Times.Once);

            storageMock.Verify(x => x.ReadAllEntries(
                It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void ProcessAcceptClearList()
        {
            var userId = "user1";

            var storageMock = new Mock<Data.IUserDataStorage>(MockBehavior.Strict);

            // Deleting all the user's entries from storage
            storageMock.Setup(x => x.DeleteAllEntries(
                It.Is<string>(y => y == userId)));

            var russianCulture = new CultureInfo("ru-RU");
            var userInput = new Logic.UserInput
            { Raw = "raw", Prepared = "prepared", Button = "button", CultureInfo = russianCulture };

            // The entry as recognized by the parser
            var parsedCommand = new Logic.Parser.ParsedCommand
            {
                Type = Logic.Parser.ParsedPhraseType.Accept
            };

            // Returning a parsed user command
            var parserMock = new Mock<Logic.IInputParserService>(MockBehavior.Strict);
            parserMock.Setup(x => x.ParseInput(
                    It.Is<Logic.UserInput>(y =>
                        y.Raw == userInput.Raw
                        && y.Prepared == userInput.Prepared
                        && y.Button == userInput.Button
                        && y.CultureInfo == userInput.CultureInfo)))
                .Returns(parsedCommand);

            var cacheMock = new Mock<Logic.Cache.IResultCache>(MockBehavior.Strict);
            // Returning the result of last successful logical operation
            cacheMock.Setup(x => x.Get(
                    It.Is<string>(y => y == userId)))
                .Returns(new Logic.ProcessingResult(Logic.ProcessingResultType.ClearRequested));
            // Accepting the result of the current completed logical operation
            cacheMock.Setup(x => x.Set(
                It.Is<string>(y => y == userId),
                It.Is<Logic.ProcessingResult>(y => y.Type == Logic.ProcessingResultType.Cleared)));

            var sut = new Logic.InventoryDialogService(
                storageMock.Object,
                parserMock.Object,
                cacheMock.Object,
                null);

            var result = sut.ProcessInput(userId, userInput);

            // Checking the result
            Assert.Equal(Logic.ProcessingResultType.Cleared, result.Type);

            // Making sure no unnecessary calls have been made
            parserMock.Verify(x => x.ParseInput(
                It.IsAny<Logic.UserInput>()), Times.Once);

            cacheMock.Verify(x => x.Get(
                It.IsAny<string>()), Times.Once);
            cacheMock.Verify(x => x.Set(
                It.IsAny<string>(),
                It.IsAny<Logic.ProcessingResult>()), Times.Once);

            storageMock.Verify(x => x.DeleteAllEntries(
                It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void ProcessRequestSendListWithoutEmailSpecified()
        {
            var userId = "user1";

            var storageMock = new Mock<Data.IUserDataStorage>(MockBehavior.Strict);

            // No email stored for user
            storageMock.Setup(x => x.ReadUserMail(
                    It.Is<string>(y => y == userId)))
                .Returns(string.Empty);

            var russianCulture = new CultureInfo("ru-RU");
            var userInput = new Logic.UserInput
            { Raw = "raw", Prepared = "prepared", Button = "button", CultureInfo = russianCulture };
            // The entry as recognized by the parser
            var parsedCommand = new Logic.Parser.ParsedCommand
            {
                Type = Logic.Parser.ParsedPhraseType.SendMail
            };

            // Returning a parsed user command
            var parserMock = new Mock<Logic.IInputParserService>(MockBehavior.Strict);
            parserMock.Setup(x => x.ParseInput(
                    It.Is<Logic.UserInput>(y =>
                        y.Raw == userInput.Raw
                        && y.Prepared == userInput.Prepared
                        && y.Button == userInput.Button
                        && y.CultureInfo == userInput.CultureInfo)))
                .Returns(parsedCommand);

            var cacheMock = new Mock<Logic.Cache.IResultCache>(MockBehavior.Strict);
            // Returning the result of last successful logical operation
            cacheMock.Setup(x => x.Get(
                    It.Is<string>(y => y == userId)))
                .Returns(new Logic.ProcessingResult(Logic.ProcessingResultType.HelpRequested));
            // Accepting the result of the current completed logical operation
            cacheMock.Setup(x => x.Set(
                It.Is<string>(y => y == userId),
                It.Is<Logic.ProcessingResult>(y => y.Type == Logic.ProcessingResultType.RequestedMail)));

            var sut = new Logic.InventoryDialogService(
                storageMock.Object,
                parserMock.Object,
                cacheMock.Object,
                null);

            var result = sut.ProcessInput(userId, userInput);

            // Checking the result
            Assert.Equal(Logic.ProcessingResultType.RequestedMail, result.Type);

            // Making sure no unnecessary calls have been made
            parserMock.Verify(x => x.ParseInput(
                It.IsAny<Logic.UserInput>()), Times.Once);

            cacheMock.Verify(x => x.Get(
                It.IsAny<string>()), Times.Once);
            cacheMock.Verify(x => x.Set(
                It.IsAny<string>(),
                It.IsAny<Logic.ProcessingResult>()), Times.Once);

            storageMock.Verify(x => x.ReadUserMail(
                It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void ProcessRequestSendListWithEmailSpecified()
        {
            var emailAddress = "some.mail@test.org";
            var userId = "user1";
            var entries = new[]
            {
                new Data.Entry()
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Name = "груши",
                    UnitOfMeasure = Data.UnitOfMeasure.Kg,
                    Quantity = 15.2f
                },
                new Data.Entry()
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Name = "яблоки",
                    UnitOfMeasure = Data.UnitOfMeasure.Unit,
                    Quantity = 12
                },
                new Data.Entry()
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Name = "яблоки",
                    UnitOfMeasure = Data.UnitOfMeasure.Kg,
                    Quantity = 12.5f
                },
            };

            var resultEntries = new[]
            {
                new Logic.Entry()
                {
                    Name = "груши",
                    UnitOfMeasure = Logic.UnitOfMeasure.Kg,
                    Quantity = 15.2f
                },
                new Logic.Entry()
                {
                    Name = "яблоки",
                    UnitOfMeasure = Logic.UnitOfMeasure.Unit,
                    Quantity = 12
                },
                new Logic.Entry()
                {
                    Name = "яблоки",
                    UnitOfMeasure = Logic.UnitOfMeasure.Kg,
                    Quantity = 12.5f
                },
            };

            var storageMock = new Mock<Data.IUserDataStorage>(MockBehavior.Strict);

            // Saving the user's email
            storageMock.Setup(x => x.SetUserMail(
                It.Is<string>(y => y == userId),
                It.Is<string>(y => y == emailAddress)));
            // Returning entries from storage
            storageMock.Setup(x => x.ReadAllEntries(
                    It.Is<string>(y => y == userId)))
                .Returns(entries);

            var russianCulture = new CultureInfo("ru-RU");
            var userInput = new Logic.UserInput
            { Raw = "raw", Prepared = "prepared", Button = "button", CultureInfo = russianCulture };

            // The entry as recognized by the parser
            var parsedCommand = new Logic.Parser.ParsedCommand
            {
                Type = Logic.Parser.ParsedPhraseType.SendMail,
                Data = emailAddress
            };

            // Returning a parsed user command
            var parserMock = new Mock<Logic.IInputParserService>(MockBehavior.Strict);
            parserMock.Setup(x => x.ParseInput(
                    It.Is<Logic.UserInput>(y =>
                        y.Raw == userInput.Raw
                        && y.Prepared == userInput.Prepared
                        && y.Button == userInput.Button
                        && y.CultureInfo == userInput.CultureInfo)))
                .Returns(parsedCommand);

            var cacheMock = new Mock<Logic.Cache.IResultCache>(MockBehavior.Strict);
            // Returning the result of last successful logical operation
            cacheMock.Setup(x => x.Get(
                    It.Is<string>(y => y == userId)))
                .Returns(new Logic.ProcessingResult(Logic.ProcessingResultType.HelpRequested));
            // Accepting the result of the current completed logical operation
            cacheMock.Setup(x => x.Set(
                It.Is<string>(y => y == userId),
                It.Is<Logic.ProcessingResult>(y =>
                    y.Type == Logic.ProcessingResultType.MailSent
                    && y.Data.ToString() == emailAddress)));

            var emailerMock = new Mock<Logic.Email.IInventoryEmailService>(MockBehavior.Strict);
            // Send an email with all the user's entries
            emailerMock.Setup(x => x.SendListAsync(
                It.Is<string>(y => y == emailAddress),
                It.Is<Logic.Entry[]>(y => y.Zip(resultEntries, (z1, z2) =>
                    z1.Name == z2.Name
                    && z1.Quantity == z2.Quantity
                    && z1.UnitOfMeasure == z2.UnitOfMeasure).All(z => z))));

            var sut = new Logic.InventoryDialogService(
                storageMock.Object,
                parserMock.Object,
                cacheMock.Object,
                emailerMock.Object);

            var result = sut.ProcessInput(userId, userInput);

            // Checking the result
            Assert.Equal(Logic.ProcessingResultType.MailSent, result.Type);
            Assert.Equal(emailAddress, result.Data);

            // Making sure no unnecessary calls have been made
            parserMock.Verify(x => x.ParseInput(
                It.IsAny<Logic.UserInput>()), Times.Once);

            cacheMock.Verify(x => x.Get(
                It.IsAny<string>()), Times.Once);
            cacheMock.Verify(x => x.Set(
                It.IsAny<string>(),
                It.IsAny<Logic.ProcessingResult>()), Times.Once);

            storageMock.Verify(x => x.SetUserMail(
                It.IsAny<string>(),
                It.IsAny<string>()), Times.Once);
            storageMock.Verify(x => x.ReadAllEntries(
                It.IsAny<string>()), Times.Once);

            emailerMock.Verify(x => x.SendListAsync(
                It.IsAny<string>(),
                It.IsAny<Logic.Entry[]>()), Times.Once);
        }

        [Theory]
        [InlineData(Logic.ProcessingResultType.HelpRequested, Logic.Parser.ParsedPhraseType.SendMail,
            Logic.ProcessingResultType.MailSent)]
        [InlineData(Logic.ProcessingResultType.MailAdded, Logic.Parser.ParsedPhraseType.SendMail,
            Logic.ProcessingResultType.MailSent)]
        public void ProcessSendListWithEmailStored(Logic.ProcessingResultType stateType,
            Logic.Parser.ParsedPhraseType entryType, Logic.ProcessingResultType resultType)
        {
            var emailAddress = "some.mail@test.org";
            var userId = "user1";
            var entries = new[]
            {
                new Data.Entry()
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Name = "груши",
                    UnitOfMeasure = Data.UnitOfMeasure.Kg,
                    Quantity = 15.2f
                },
                new Data.Entry()
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Name = "яблоки",
                    UnitOfMeasure = Data.UnitOfMeasure.Unit,
                    Quantity = 12
                },
                new Data.Entry()
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Name = "яблоки",
                    UnitOfMeasure = Data.UnitOfMeasure.Kg,
                    Quantity = 12.5f
                },
            };

            var resultEntries = new[]
            {
                new Logic.Entry()
                {
                    Name = "груши",
                    UnitOfMeasure = Logic.UnitOfMeasure.Kg,
                    Quantity = 15.2f
                },
                new Logic.Entry()
                {
                    Name = "яблоки",
                    UnitOfMeasure = Logic.UnitOfMeasure.Unit,
                    Quantity = 12
                },
                new Logic.Entry()
                {
                    Name = "яблоки",
                    UnitOfMeasure = Logic.UnitOfMeasure.Kg,
                    Quantity = 12.5f
                },
            };

            var storageMock = new Mock<Data.IUserDataStorage>(MockBehavior.Strict);

            storageMock.Setup(x => x.ReadUserMail(
                    It.Is<string>(y => y == userId)))
                .Returns(emailAddress);
            // Returning entries from the storage
            storageMock.Setup(x => x.ReadAllEntries(
                    It.Is<string>(y => y == userId)))
                .Returns(entries);

            var russianCulture = new CultureInfo("ru-RU");
            var userInput = new Logic.UserInput
            { Raw = "raw", Prepared = "prepared", Button = "button", CultureInfo = russianCulture };
            // The entry as recognized by the parser
            var parsedCommand = new Logic.Parser.ParsedCommand
            {
                Type = entryType
            };

            // Returning a parsed user command
            var parserMock = new Mock<Logic.IInputParserService>(MockBehavior.Strict);
            parserMock.Setup(x => x.ParseInput(
                    It.Is<Logic.UserInput>(y =>
                        y.Raw == userInput.Raw
                        && y.Prepared == userInput.Prepared
                        && y.Button == userInput.Button
                        && y.CultureInfo == userInput.CultureInfo)))
                .Returns(parsedCommand);

            var cacheMock = new Mock<Logic.Cache.IResultCache>(MockBehavior.Strict);
            // Returning the result of last successful logical operation
            cacheMock.Setup(x => x.Get(
                    It.Is<string>(y => y == userId)))
                .Returns(new Logic.ProcessingResult(stateType));
            // Accepting the result of the current completed logical operation
            cacheMock.Setup(x => x.Set(
                It.Is<string>(y => y == userId),
                It.Is<Logic.ProcessingResult>(y =>
                    y.Type == resultType)));

            var emailerMock = new Mock<Logic.Email.IInventoryEmailService>(MockBehavior.Strict);
            // Send an email with all the user's entries
            emailerMock.Setup(x => x.SendListAsync(
                It.Is<string>(y => y == emailAddress),
                It.Is<Logic.Entry[]>(y => y.Zip(resultEntries, (z1, z2) =>
                    z1.Name == z2.Name
                    && z1.Quantity == z2.Quantity
                    && z1.UnitOfMeasure == z2.UnitOfMeasure).All(z => z))));

            var sut = new Logic.InventoryDialogService(
                storageMock.Object,
                parserMock.Object,
                cacheMock.Object,
                emailerMock.Object);

            var result = sut.ProcessInput(userId, userInput);

            // Checking the result
            Assert.Equal(resultType, result.Type);
            Assert.Equal(emailAddress, result.Data);

            // Making sure no unnecessary calls have been made
            parserMock.Verify(x => x.ParseInput(
                It.IsAny<Logic.UserInput>()), Times.Once);

            cacheMock.Verify(x => x.Get(
                It.IsAny<string>()), Times.Once);
            cacheMock.Verify(x => x.Set(
                It.IsAny<string>(),
                It.IsAny<Logic.ProcessingResult>()), Times.Once);

            storageMock.Verify(x => x.ReadUserMail(
                It.IsAny<string>()), Times.Once);
            storageMock.Verify(x => x.ReadAllEntries(
                It.IsAny<string>()), Times.Once);

            emailerMock.Verify(x => x.SendListAsync(
                It.IsAny<string>(),
                It.IsAny<Logic.Entry[]>()), Times.Once);
        }

        [Fact]
        public void ProcessAddMail()
        {
            var emailAddress = "some.mail@test.org";
            var userId = "user1";

            var storageMock = new Mock<Data.IUserDataStorage>(MockBehavior.Strict);

            // A new email address is stored
            storageMock.Setup(x => x.SetUserMail(
                It.Is<string>(y => y == userId),
                It.Is<string>(y => y == emailAddress)));

            var russianCulture = new CultureInfo("ru-RU");
            var userInput = new Logic.UserInput
            { Raw = "raw", Prepared = "prepared", Button = "button", CultureInfo = russianCulture };
            // The entry as recognized by the parser
            var parsedCommand = new Logic.Parser.ParsedCommand
            {
                Type = Logic.Parser.ParsedPhraseType.Mail,
                Data = emailAddress
            };

            // Returning a parsed user command
            var parserMock = new Mock<Logic.IInputParserService>(MockBehavior.Strict);
            parserMock.Setup(x => x.ParseInput(
                    It.Is<Logic.UserInput>(y =>
                        y.Raw == userInput.Raw
                        && y.Prepared == userInput.Prepared
                        && y.Button == userInput.Button
                        && y.CultureInfo == userInput.CultureInfo)))
                .Returns(parsedCommand);

            var cacheMock = new Mock<Logic.Cache.IResultCache>(MockBehavior.Strict);
            // Returning the result of last successful logical operation
            cacheMock.Setup(x => x.Get(
                    It.Is<string>(y => y == userId)))
                .Returns(new Logic.ProcessingResult(Logic.ProcessingResultType.RequestedMail));
            // Accepting the result of the current completed logical operation
            cacheMock.Setup(x => x.Set(
                It.Is<string>(y => y == userId),
                It.Is<Logic.ProcessingResult>(y =>
                    y.Type == Logic.ProcessingResultType.MailAdded
                    && y.Data.ToString() == emailAddress)));

            var sut = new Logic.InventoryDialogService(
                storageMock.Object,
                parserMock.Object,
                cacheMock.Object,
                null);

            var result = sut.ProcessInput(userId, userInput);

            // Checking the result
            Assert.Equal(Logic.ProcessingResultType.MailAdded, result.Type);
            Assert.Equal(emailAddress, result.Data);

            // Making sure no unnecessary calls have been made
            parserMock.Verify(x => x.ParseInput(
                It.IsAny<Logic.UserInput>()), Times.Once);

            cacheMock.Verify(x => x.Get(
                It.IsAny<string>()), Times.Once);
            cacheMock.Verify(x => x.Set(
                It.IsAny<string>(),
                It.IsAny<Logic.ProcessingResult>()), Times.Once);

            storageMock.Verify(x => x.SetUserMail(
                It.IsAny<string>(),
                It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void ProcessDeleteMail()
        {
            var emailAddress = "some.mail@test.org";
            var userId = "user1";

            var storageMock = new Mock<Data.IUserDataStorage>(MockBehavior.Strict);

            // The deleted email address is returned as a result
            storageMock.Setup(x => x.ReadUserMail(
                    It.Is<string>(y => y == userId)))
                .Returns(emailAddress);
            storageMock.Setup(x => x.DeleteUserMail(
                It.Is<string>(y => y == userId)));

            var russianCulture = new CultureInfo("ru-RU");
            var userInput = new Logic.UserInput
            { Raw = "raw", Prepared = "prepared", Button = "button", CultureInfo = russianCulture };

            // The entry as recognized by the parser
            var parsedCommand = new Logic.Parser.ParsedCommand
            {
                Type = Logic.Parser.ParsedPhraseType.DeleteMail
            };

            // Returning a parsed user command
            var parserMock = new Mock<Logic.IInputParserService>(MockBehavior.Strict);
            parserMock.Setup(x => x.ParseInput(
                    It.Is<Logic.UserInput>(y =>
                        y.Raw == userInput.Raw
                        && y.Prepared == userInput.Prepared
                        && y.Button == userInput.Button
                        && y.CultureInfo == userInput.CultureInfo)))
                .Returns(parsedCommand);

            var cacheMock = new Mock<Logic.Cache.IResultCache>(MockBehavior.Strict);
            // Returning the result of last successful logical operation
            cacheMock.Setup(x => x.Get(
                    It.Is<string>(y => y == userId)))
                .Returns(new Logic.ProcessingResult(Logic.ProcessingResultType.HelpRequested));
            // Accepting the result of the current completed logical operation
            cacheMock.Setup(x => x.Set(
                It.Is<string>(y => y == userId),
                It.Is<Logic.ProcessingResult>(y =>
                    y.Type == Logic.ProcessingResultType.MailDeleted
                    && y.Data.ToString() == emailAddress)));

            var sut = new Logic.InventoryDialogService(
                storageMock.Object,
                parserMock.Object,
                cacheMock.Object,
                null);

            var result = sut.ProcessInput(userId, userInput);

            // Checking the result
            Assert.Equal(Logic.ProcessingResultType.MailDeleted, result.Type);
            Assert.Equal(emailAddress, result.Data);

            // Making sure no unnecessary calls have been made
            parserMock.Verify(x => x.ParseInput(
                It.IsAny<Logic.UserInput>()), Times.Once);

            cacheMock.Verify(x => x.Get(
                It.IsAny<string>()), Times.Once);
            cacheMock.Verify(x => x.Set(
                It.IsAny<string>(),
                It.IsAny<Logic.ProcessingResult>()), Times.Once);

            storageMock.Verify(x => x.ReadUserMail(
                It.IsAny<string>()), Times.Once);
            storageMock.Verify(x => x.DeleteUserMail(
                It.IsAny<string>()), Times.Once);
        }
    }
}
