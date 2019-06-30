using System;
using System.Globalization;
using Xunit;
using Moq;
using AliceInventory;
using AliceInventory.Data;
using AliceInventory.Logic;
using AliceInventory.Logic.Parser;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using UnitOfMeasure = AliceInventory.Data.UnitOfMeasure;

namespace AliceInventory.UnitTests
{
    public class InventoryDialogServiceTests
    {
        private const float Tolerance = 0.0001f;
        private const string userId = "userId1";
        private const string addInputText = "объект1 123.123 кг";
        private const string delteInputText = "удали объект1 123.123 кг";
        private const string cancelInputText = "отмена";
        private const string entryName = "объект1";
        private const double entryCount = 123.123d;
        private const Logic.UnitOfMeasure entryUnit = Logic.UnitOfMeasure.Kg;
        private static readonly ParsedSingleEntry parsedEntry = new ParsedSingleEntry()
        {
            Name = entryName,
            Count = entryCount,
            Unit = entryUnit
        };
        private static readonly SingleEntry entry = new SingleEntry()
        {
            Name = entryName,
            Count = entryCount,
            Unit = entryUnit
        };
        private static readonly ParsedCommand addParsedCommand = new ParsedCommand()
        {
            Type = ParsedCommandType.Add,
            Data = parsedEntry
        };
        private static readonly ParsedCommand deleteParsedCommand = new ParsedCommand()
        {
            Type = ParsedCommandType.Delete,
            Data = parsedEntry
        };
        private static readonly ParsedCommand cancelParsedCommand = new ParsedCommand()
        {
            Type = ParsedCommandType.Cancel
        };

        private static readonly ProcessingCommand addProcessingCommand= new ProcessingCommand()
        {
            Type = ProcessingCommandType.Add,
            Data = entry
        };
        private static readonly ProcessingCommand deleteProcessingCommand = new ProcessingCommand()
        {
            Type = ProcessingCommandType.Delete,
            Data = entry
        };

        [Fact]
        public void ProcessInputAddTest()
        {
            var storageMock = new Mock<Data.IUserDataStorage>(MockBehavior.Strict);
            storageMock.Setup(x => x.AddEntry(
                    It.Is<string>(y => y == userId),
                    It.Is<string>(y => y == entry.Name),
                    It.Is<double>(y => Math.Abs(y - entry.Count) < Tolerance),
                    It.Is<Data.UnitOfMeasure>(y => y == entry.Unit.ToData())))
                .Returns(true);

            var parserMock = new Mock<IInputParserService>();
            parserMock.Setup(x => x.ParseInput(
                    It.Is<string>(y => y == addInputText),
                    It.IsAny<CultureInfo>()))
                .Returns(addParsedCommand);

            var commandCacheMock = new Mock<ICommandCache>(MockBehavior.Strict);
            commandCacheMock.Setup(x => x.Get(
                    It.Is<string>(y => y == userId)))
                .Returns(new ProcessingCommand());
            commandCacheMock.Setup(x => x.Set(
                It.Is<string>(y => y == userId),
                It.Is<ProcessingCommand>(command =>
                    command.Type == ProcessingCommandType.Add &&
                    command.Data is SingleEntry &&
                    ((SingleEntry) command.Data).Name == entry.Name &&
                    Math.Abs(((SingleEntry) command.Data).Count - entry.Count) < Tolerance &&
                    ((SingleEntry) command.Data).Unit == entry.Unit)));

            var emailMock = new Mock<Logic.Email.IInventoryEmailService>(MockBehavior.Strict);

            var sut = new InventoryDialogService(
                storageMock.Object,
                parserMock.Object,
                commandCacheMock.Object,
                emailMock.Object);

            var result = sut.ProcessInput(userId, addInputText, CultureInfo.CurrentCulture);
            Assert.Equal(ProcessingResultType.Added, result.Type);

            var resultEntry = result.Data as SingleEntry;
            Assert.NotNull(resultEntry);
            Assert.Equal(entry.Name, resultEntry.Name);
            Assert.Equal(entry.Count, resultEntry.Count);
            Assert.Equal(entry.Unit, resultEntry.Unit);

            storageMock.Verify(x =>
                x.AddEntry(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<double>(),
                    It.IsAny<Data.UnitOfMeasure>()), Times.Once);

            parserMock.Verify(x =>
                x.ParseInput(It.IsAny<string>(), It.IsAny<CultureInfo>()), Times.Once);

            commandCacheMock.Verify(x =>
                x.Set(
                    It.IsAny<string>(),
                    It.IsAny<Logic.ProcessingCommand>()), Times.Once);
        }

        [Fact]
        public void ProcessInputDeleteTest()
        {
            var storageMock = new Mock<Data.IUserDataStorage>(MockBehavior.Strict);
            storageMock.Setup(x => x.DeleteEntry(
                    It.Is<string>(y => y == userId),
                    It.Is<string>(y => y == entry.Name),
                    It.Is<double>(y => Math.Abs(y - entry.Count) < Tolerance),
                    It.Is<Data.UnitOfMeasure>(y => y == entry.Unit.ToData())))
                .Returns(true);

            var parserMock = new Mock<IInputParserService>();
            parserMock.Setup(x => x.ParseInput(
                    It.Is<string>(y => y == delteInputText),
                    It.IsAny<CultureInfo>()))
                .Returns(deleteParsedCommand);

            var commandCacheMock = new Mock<ICommandCache>(MockBehavior.Strict);
            commandCacheMock.Setup(x => x.Get(
                    It.Is<string>(y => y == userId)))
                .Returns(new ProcessingCommand());
            commandCacheMock.Setup(x => x.Set(
                It.Is<string>(y => y == userId),
                It.Is<ProcessingCommand>(command =>
                    command.Type == ProcessingCommandType.Delete &&
                    command.Data is SingleEntry &&
                    ((SingleEntry)command.Data).Name == entry.Name &&
                    Math.Abs(((SingleEntry)command.Data).Count - entry.Count) < Tolerance &&
                    ((SingleEntry)command.Data).Unit == entry.Unit)));

            var emailMock = new Mock<Logic.Email.IInventoryEmailService>(MockBehavior.Strict);

            var sut = new Logic.InventoryDialogService(
                storageMock.Object,
                parserMock.Object,
                commandCacheMock.Object,
                emailMock.Object);

            var result = sut.ProcessInput(userId, delteInputText, CultureInfo.CurrentCulture);
            Assert.Equal(ProcessingResultType.Deleted, result.Type);

            var resultEntry = result.Data as SingleEntry;
            Assert.NotNull(resultEntry);
            Assert.Equal(entry.Name, resultEntry.Name);
            Assert.Equal(entry.Count, resultEntry.Count);
            Assert.Equal(entry.Unit, resultEntry.Unit);

            storageMock.Verify(x =>
                x.DeleteEntry(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<double>(),
                    It.IsAny<Data.UnitOfMeasure>()), Times.Once);

            parserMock.Verify(x =>
                x.ParseInput(It.IsAny<string>(), It.IsAny<CultureInfo>()), Times.Once);

            commandCacheMock.Verify(x =>
                x.Set(
                    It.IsAny<string>(),
                    It.IsAny<Logic.ProcessingCommand>()), Times.Once);
        }

        [Fact]
        public void ProcessInputAddCancelTest()
        {
            var storageMock = new Mock<Data.IUserDataStorage>(MockBehavior.Strict);
            storageMock.Setup(x => x.DeleteEntry(
                    It.Is<string>(y => y == userId),
                    It.Is<string>(y => y == entry.Name),
                    It.Is<double>(y => Math.Abs(y - entry.Count) < Tolerance),
                    It.Is<Data.UnitOfMeasure>(y => y == entry.Unit.ToData())))
                .Returns(true);

            var parserMock = new Mock<Logic.IInputParserService>();
            parserMock.Setup(x => x.ParseInput(
                    It.Is<string>(y => y == cancelInputText),
                    It.IsAny<CultureInfo>()))
                .Returns(cancelParsedCommand);

            var commandCacheMock = new Mock<ICommandCache>(MockBehavior.Strict);
            commandCacheMock.Setup(x =>
                x.Get(It.Is<string>(y => y == userId))).Returns(addProcessingCommand);
            commandCacheMock.Setup(x =>
                x.Set(It.Is<string>(y => y == userId),
                    It.Is<ProcessingCommand>(command =>
                        command.Type == ProcessingCommandType.Delete &&
                        command.Data is SingleEntry &&
                        ((SingleEntry)command.Data).Name == entry.Name &&
                        Math.Abs(((SingleEntry)command.Data).Count - entry.Count) < Tolerance &&
                        ((SingleEntry)command.Data).Unit == entry.Unit)));

            var emailMock = new Mock<Logic.Email.IInventoryEmailService>(MockBehavior.Strict);

            var sut = new InventoryDialogService(
                storageMock.Object,
                parserMock.Object,
                commandCacheMock.Object,
                emailMock.Object);

            var result = sut.ProcessInput(userId, cancelInputText, CultureInfo.CurrentCulture);
            Assert.Equal(ProcessingResultType.AddCanceled, result.Type);

            var resultEntry = result.Data as SingleEntry;
            Assert.NotNull(resultEntry);
            Assert.Equal(entry.Name, resultEntry.Name);
            Assert.Equal(entry.Count, resultEntry.Count);
            Assert.Equal(entry.Unit, resultEntry.Unit);

            storageMock.Verify(x =>
                x.DeleteEntry(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<double>(),
                    It.IsAny<Data.UnitOfMeasure>()), Times.Once);

            parserMock.Verify(x =>
                x.ParseInput(It.IsAny<string>(), It.IsAny<CultureInfo>()), Times.Once);

            commandCacheMock.Verify(x =>
                x.Set(
                    It.IsAny<string>(),
                    It.IsAny<Logic.ProcessingCommand>()), Times.Once);
        }

        [Fact]
        public void ProcessInputDeleteCancelTest()
        {
            var storageMock = new Mock<Data.IUserDataStorage>(MockBehavior.Strict);
            storageMock.Setup(x => x.AddEntry(
                    It.Is<string>(y => y == userId),
                    It.Is<string>(y => y == entry.Name),
                    It.Is<double>(y => Math.Abs(y - entry.Count) < Tolerance),
                    It.Is<Data.UnitOfMeasure>(y => y == entry.Unit.ToData())))
                .Returns(true);

            var parserMock = new Mock<IInputParserService>();
            parserMock.Setup(x => x.ParseInput(
                    It.Is<string>(y => y == cancelInputText),
                    It.IsAny<CultureInfo>()))
                .Returns(cancelParsedCommand);

            var commandCacheMock = new Mock<Logic.ICommandCache>(MockBehavior.Strict);
            commandCacheMock.Setup(x =>
                x.Get(It.Is<string>(y => y == userId))).Returns(deleteProcessingCommand);
            commandCacheMock.Setup(x =>
                x.Set(It.Is<string>(y => y == userId),
                    It.Is<ProcessingCommand>(command =>
                        command.Type == ProcessingCommandType.Add &&
                        command.Data is SingleEntry &&
                        ((SingleEntry)command.Data).Name == entry.Name &&
                        Math.Abs(((SingleEntry)command.Data).Count - entry.Count) < Tolerance &&
                        ((SingleEntry)command.Data).Unit == entry.Unit)));

            var emailMock = new Mock<Logic.Email.IInventoryEmailService>(MockBehavior.Strict);

            var sut = new InventoryDialogService(
                storageMock.Object,
                parserMock.Object,
                commandCacheMock.Object,
                emailMock.Object);

            var result = sut.ProcessInput(userId, cancelInputText, CultureInfo.CurrentCulture);
            Assert.Equal(ProcessingResultType.DeleteCanceled, result.Type);

            var resultEntry = result.Data as SingleEntry;
            Assert.NotNull(resultEntry);
            Assert.Equal(entry.Name, resultEntry.Name);
            Assert.Equal(entry.Count, resultEntry.Count);
            Assert.Equal(entry.Unit, resultEntry.Unit);

            storageMock.Verify(x =>
                x.AddEntry(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<double>(),
                    It.IsAny<Data.UnitOfMeasure>()), Times.Once);

            parserMock.Verify(x =>
                x.ParseInput(It.IsAny<string>(), It.IsAny<CultureInfo>()), Times.Once);

            commandCacheMock.Verify(x =>
                x.Set(
                    It.IsAny<string>(),
                    It.IsAny<Logic.ProcessingCommand>()), Times.Once);
        }

        [Theory]
        [InlineData("ещё 5", null, 5, null,
            entryName, 5, entryUnit)]
        [InlineData("ещё яблоки", "яблоки", null, null,
            "яблоки", 1, Logic.UnitOfMeasure.Unit)]
        [InlineData("ещё кг", null, null, Logic.UnitOfMeasure.Kg,
            entryName, 1, Logic.UnitOfMeasure.Kg)]
        [InlineData("ещё 5 яблоки", "яблоки", 5, null,
            "яблоки", 5, Logic.UnitOfMeasure.Unit)]
        [InlineData("ещё 5 литров", null, 5, Logic.UnitOfMeasure.L,
            entryName, 5, Logic.UnitOfMeasure.L)]
        [InlineData("ещё 5 литров яблоки", "яблоки", 5, Logic.UnitOfMeasure.L,
            "яблоки", 5, Logic.UnitOfMeasure.L)]
        public void ProcessInputMoreAddTest(
            string input, string parsedName, double? parsedCount, Logic.UnitOfMeasure? parsedUnit,
            string currentName, double currentCount, Logic.UnitOfMeasure currentUnit)
        {
            var parsedCommand = new ParsedCommand()
            {
                Type = ParsedCommandType.More,
                Data = new ParsedSingleEntry()
                {
                    Name = parsedName,
                    Count = parsedCount,
                    Unit = parsedUnit
                }
            };
            var parserMock = new Mock<IInputParserService>();
            parserMock.Setup(x => x.ParseInput(
                    It.Is<string>(y => y == input),
                    It.IsAny<CultureInfo>()))
                .Returns(parsedCommand);


            var storageMock = new Mock<IUserDataStorage>(MockBehavior.Strict);
            storageMock.Setup(x => x.AddEntry(
                    It.Is<string>(y => y == userId),
                    It.Is<string>(y => y == currentName),
                    It.Is<double>(y => Math.Abs(y - currentCount) < Tolerance),
                    It.Is<Data.UnitOfMeasure>(y => y == currentUnit.ToData())))
                .Returns(true);

            var commandCacheMock = new Mock<Logic.ICommandCache>(MockBehavior.Strict);
            commandCacheMock.Setup(x =>
                x.Get(It.Is<string>(y => y == userId))).Returns(addProcessingCommand);
            commandCacheMock.Setup(x =>
                x.Set(It.Is<string>(y => y == userId),
                    It.Is<ProcessingCommand>(command =>
                        command.Type == ProcessingCommandType.Add &&
                        command.Data is SingleEntry &&
                        ((SingleEntry)command.Data).Name == currentName &&
                        Math.Abs(((SingleEntry)command.Data).Count - currentCount) < Tolerance &&
                        ((SingleEntry)command.Data).Unit == currentUnit)));

            var emailMock = new Mock<Logic.Email.IInventoryEmailService>(MockBehavior.Strict);

            var sut = new InventoryDialogService(
                storageMock.Object,
                parserMock.Object,
                commandCacheMock.Object,
                emailMock.Object);

            var result = sut.ProcessInput(userId, input, CultureInfo.CurrentCulture);
            Assert.Equal(ProcessingResultType.Added, result.Type);

            var resultEntry = result.Data as SingleEntry;
            Assert.NotNull(resultEntry);
            Assert.Equal(currentName, resultEntry.Name);
            Assert.Equal(currentCount, resultEntry.Count);
            Assert.Equal(currentUnit, resultEntry.Unit);

            storageMock.Verify(x =>
                x.AddEntry(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<double>(),
                    It.IsAny<Data.UnitOfMeasure>()), Times.Once);

            parserMock.Verify(x =>
                x.ParseInput(It.IsAny<string>(), It.IsAny<CultureInfo>()), Times.Once);

            commandCacheMock.Verify(x =>
                x.Set(
                    It.IsAny<string>(),
                    It.IsAny<Logic.ProcessingCommand>()), Times.Once);
        }

        [Theory]
        [InlineData("ещё 5", null, 5, null,
            entryName, 5, entryUnit)]
        [InlineData("ещё яблоки", "яблоки", null, null,
            "яблоки", 1, Logic.UnitOfMeasure.Unit)]
        [InlineData("ещё кг", null, null, Logic.UnitOfMeasure.Kg,
            entryName, 1, Logic.UnitOfMeasure.Kg)]
        [InlineData("ещё 5 яблоки", "яблоки", 5, null,
            "яблоки", 5, Logic.UnitOfMeasure.Unit)]
        [InlineData("ещё 5 литров", null, 5, Logic.UnitOfMeasure.L,
            entryName, 5, Logic.UnitOfMeasure.L)]
        [InlineData("ещё 5 литров яблоки", "яблоки", 5, Logic.UnitOfMeasure.L,
            "яблоки", 5, Logic.UnitOfMeasure.L)]
        public void ProcessInputMoreDeleteTest(
            string input, string parsedName, double? parsedCount, Logic.UnitOfMeasure? parsedUnit,
            string currentName, double currentCount, Logic.UnitOfMeasure currentUnit)
        {
            var parsedCommand = new ParsedCommand()
            {
                Type = ParsedCommandType.More,
                Data = new ParsedSingleEntry()
                {
                    Name = parsedName,
                    Count = parsedCount,
                    Unit = parsedUnit
                }
            };
            var parserMock = new Mock<IInputParserService>();
            parserMock.Setup(x => x.ParseInput(
                    It.Is<string>(y => y == input),
                    It.IsAny<CultureInfo>()))
                .Returns(parsedCommand);


            var storageMock = new Mock<IUserDataStorage>(MockBehavior.Strict);
            storageMock.Setup(x => x.DeleteEntry(
                    It.Is<string>(y => y == userId),
                    It.Is<string>(y => y == currentName),
                    It.Is<double>(y => Math.Abs(y - currentCount) < Tolerance),
                    It.Is<Data.UnitOfMeasure>(y => y == currentUnit.ToData())))
                .Returns(true);

            var commandCacheMock = new Mock<Logic.ICommandCache>(MockBehavior.Strict);
            commandCacheMock.Setup(x =>
                x.Get(It.Is<string>(y => y == userId))).Returns(deleteProcessingCommand);
            commandCacheMock.Setup(x =>
                x.Set(It.Is<string>(y => y == userId),
                    It.Is<ProcessingCommand>(command =>
                        command.Type == ProcessingCommandType.Delete &&
                        command.Data is SingleEntry &&
                        ((SingleEntry)command.Data).Name == currentName &&
                        Math.Abs(((SingleEntry)command.Data).Count - currentCount) < Tolerance &&
                        ((SingleEntry)command.Data).Unit == currentUnit)));

            var emailMock = new Mock<Logic.Email.IInventoryEmailService>(MockBehavior.Strict);

            var sut = new InventoryDialogService(
                storageMock.Object,
                parserMock.Object,
                commandCacheMock.Object,
                emailMock.Object);

            var result = sut.ProcessInput(userId, input, CultureInfo.CurrentCulture);
            Assert.Equal(ProcessingResultType.Deleted, result.Type);

            var resultEntry = result.Data as SingleEntry;
            Assert.NotNull(resultEntry);
            Assert.Equal(currentName, resultEntry.Name);
            Assert.Equal(currentCount, resultEntry.Count);
            Assert.Equal(currentUnit, resultEntry.Unit);

            storageMock.Verify(x =>
                x.DeleteEntry(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<double>(),
                    It.IsAny<Data.UnitOfMeasure>()), Times.Once);

            parserMock.Verify(x =>
                x.ParseInput(It.IsAny<string>(), It.IsAny<CultureInfo>()), Times.Once);

            commandCacheMock.Verify(x =>
                x.Set(
                    It.IsAny<string>(),
                    It.IsAny<Logic.ProcessingCommand>()), Times.Once);
        }
    }
}
