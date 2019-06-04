using System;
using System.Globalization;
using Xunit;
using Moq;
using AliceInventory;
using AliceInventory.Data;
using AliceInventory.Logic;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace AliceInventory.UnitTests
{
    public class InventoryDialogServiceTests
    {
        [Fact]
        public void ProcessInputAddTest()
        {
            var input = "объект1 123.123кг";
            var userId = "userId1";
            var logicEntryStub = new Logic.SingleEntry
            {
                Name = "объект1",
                Count = 123.123d,
                Unit = Logic.UnitOfMeasure.Kg
            };
            var processingCommand = new Logic.ProcessingCommand
            {
                Command = Logic.InputProcessingCommand.Add,
                Data = logicEntryStub
            };
            var entries = new Logic.Entry[]
            {
                new Logic.Entry("камни", 10, Logic.UnitOfMeasure.Unit)
            };

            var storageMock = new Mock<Data.IUserDataStorage>(MockBehavior.Strict);
            storageMock.Setup(x => x.AddEntry(
                    It.Is<string>(y => y == userId),
                    It.Is<string>(y => y == logicEntryStub.Name),
                    It.Is<double>(y => y == logicEntryStub.Count),
                    It.Is<Data.UnitOfMeasure>(y => y == logicEntryStub.Unit.ToData())))
                .Returns(true);

            var parserMock = new Mock<Logic.IInputParserService>();
            parserMock.Setup(x => x.ParseInput(
                    It.Is<string>(y => y == input),
                    It.IsAny<CultureInfo>()))
                .Returns(processingCommand);

            var commandCacheMock = new Mock<Logic.ICommandCache>(MockBehavior.Strict);
            commandCacheMock.Setup(x =>
                x.Set(It.Is<string>(y => y == userId),
                    It.Is<Logic.ProcessingCommand>(y =>
                        y.Command == processingCommand.Command
                        && y.Data == processingCommand.Data)));
            commandCacheMock.Setup(x =>
                x.Get(It.Is<string>(y => y == userId))).Returns(new ProcessingCommand());

            var emailMock = new Mock<Logic.Email.IInventoryEmailService>(MockBehavior.Strict);

            var sut = new Logic.InventoryDialogService(
                storageMock.Object,
                parserMock.Object,
                commandCacheMock.Object,
                emailMock.Object);

            var result = sut.ProcessInput(userId, input, CultureInfo.CurrentCulture);

            Assert.Equal(Logic.InputProcessingResult.Added, result.Result);
            Assert.Equal(logicEntryStub, result.Data);

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
            var input = "удалить объект1 123.123кг";
            var userId = "userId1";
            var logicEntryStub = new Logic.SingleEntry
            {
                Unit = Logic.UnitOfMeasure.Kg,
                Name = "объект1",
                Count = 123.123d
            };
            var processingCommand = new Logic.ProcessingCommand
            {
                Command = Logic.InputProcessingCommand.Delete,
                Data = logicEntryStub
            };
            var entries = new Logic.Entry[]
            {
                new Logic.Entry("камни", 10, Logic.UnitOfMeasure.Unit)
            };

            var storageMock = new Mock<Data.IUserDataStorage>(MockBehavior.Strict);
            storageMock.Setup(x => x.DeleteEntry(
                    It.Is<string>(y => y == userId),
                    It.Is<string>(y => y == logicEntryStub.Name),
                    It.Is<double>(y => y == logicEntryStub.Count),
                    It.Is<Data.UnitOfMeasure>(y => y == logicEntryStub.Unit.ToData())))
                .Returns(true);

            var parserMock = new Mock<Logic.IInputParserService>();
            parserMock.Setup(x => x.ParseInput(
                    It.Is<string>(y => y == input),
                    It.IsAny<CultureInfo>()))
                .Returns(processingCommand);

            var commandCacheMock = new Mock<Logic.ICommandCache>(MockBehavior.Strict);
            commandCacheMock.Setup(x =>
                x.Set(It.Is<string>(y => y == userId),
                    It.Is<Logic.ProcessingCommand>(y =>
                        y.Command == processingCommand.Command
                        && y.Data == processingCommand.Data)));
            commandCacheMock.Setup(x =>
                x.Get(It.Is<string>(y => y == userId))).Returns(new ProcessingCommand());

            var emailMock = new Mock<Logic.Email.IInventoryEmailService>(MockBehavior.Strict);

            var sut = new Logic.InventoryDialogService(
                storageMock.Object,
                parserMock.Object,
                commandCacheMock.Object,
                emailMock.Object);

            var result = sut.ProcessInput(userId, input, CultureInfo.CurrentCulture);

            Assert.Equal(Logic.InputProcessingResult.Deleted, result.Result);
            Assert.Equal(logicEntryStub, result.Data);

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
            var input = "отмена";
            var userId = "userId1";
            var parsedCommand = new Logic.ProcessingCommand
            {
                Command = Logic.InputProcessingCommand.Cancel
            };
            var cachedEntryStub = new Logic.SingleEntry
            {
                Unit = Logic.UnitOfMeasure.Kg,
                Name = "объект1",
                Count = 123.123d
            };
            var cachedAddCommand = new Logic.ProcessingCommand
            {
                Command = Logic.InputProcessingCommand.Add,
                Data = cachedEntryStub
            };

            var storageMock = new Mock<Data.IUserDataStorage>(MockBehavior.Strict);
            storageMock.Setup(x => x.DeleteEntry(
                    It.Is<string>(y => y == userId),
                    It.Is<string>(y => y == cachedEntryStub.Name),
                    It.Is<double>(y => y == cachedEntryStub.Count),
                    It.Is<Data.UnitOfMeasure>(y => y == cachedEntryStub.Unit.ToData())))
                .Returns(true);

            var parserMock = new Mock<Logic.IInputParserService>();
            parserMock.Setup(x => x.ParseInput(
                    It.Is<string>(y => y == input),
                    It.IsAny<CultureInfo>()))
                .Returns(parsedCommand);

            var commandCacheMock = new Mock<Logic.ICommandCache>(MockBehavior.Strict);
            commandCacheMock.Setup(x =>
                x.Set(It.Is<string>(y => y == userId),
                    It.Is<Logic.ProcessingCommand>(y =>
                        y.Command == parsedCommand.Command
                        && y.Data == parsedCommand.Data)));
            commandCacheMock.Setup(x =>
                x.Get(It.Is<string>(y => y == userId))).Returns(cachedAddCommand);

            var emailMock = new Mock<Logic.Email.IInventoryEmailService>(MockBehavior.Strict);

            var sut = new Logic.InventoryDialogService(
                storageMock.Object,
                parserMock.Object,
                commandCacheMock.Object,
                emailMock.Object);

            var result = sut.ProcessInput(userId, input, CultureInfo.CurrentCulture);

            Assert.Equal(InputProcessingResult.AddCanceled, result.Result);
            Assert.Equal(cachedEntryStub, result.Data);

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
            var input = "отмена";
            var userId = "userId1";
            var parsedCommand = new Logic.ProcessingCommand
            {
                Command = Logic.InputProcessingCommand.Cancel
            };
            var cachedEntryStub = new Logic.SingleEntry
            {
                Unit = Logic.UnitOfMeasure.Kg,
                Name = "объект1",
                Count = 123.123d
            };
            var cachedDeleteCommand = new Logic.ProcessingCommand
            {
                Command = Logic.InputProcessingCommand.Delete,
                Data = cachedEntryStub
            };

            var storageMock = new Mock<Data.IUserDataStorage>(MockBehavior.Strict);
            storageMock.Setup(x => x.AddEntry(
                    It.Is<string>(y => y == userId),
                    It.Is<string>(y => y == cachedEntryStub.Name),
                    It.Is<double>(y => y == cachedEntryStub.Count),
                    It.Is<Data.UnitOfMeasure>(y => y == cachedEntryStub.Unit.ToData())))
                .Returns(true);

            var parserMock = new Mock<Logic.IInputParserService>();
            parserMock.Setup(x => x.ParseInput(
                    It.Is<string>(y => y == input),
                    It.IsAny<CultureInfo>()))
                .Returns(parsedCommand);

            var commandCacheMock = new Mock<Logic.ICommandCache>(MockBehavior.Strict);
            commandCacheMock.Setup(x =>
                x.Set(It.Is<string>(y => y == userId),
                    It.Is<Logic.ProcessingCommand>(y =>
                        y.Command == parsedCommand.Command
                        && y.Data == parsedCommand.Data)));
            commandCacheMock.Setup(x =>
                x.Get(It.Is<string>(y => y == userId))).Returns(cachedDeleteCommand);

            var emailMock = new Mock<Logic.Email.IInventoryEmailService>(MockBehavior.Strict);

            var sut = new Logic.InventoryDialogService(
                storageMock.Object,
                parserMock.Object,
                commandCacheMock.Object,
                emailMock.Object);

            var result = sut.ProcessInput(userId, input, CultureInfo.CurrentCulture);

            Assert.Equal(InputProcessingResult.DeleteCanceled, result.Result);
            Assert.Equal(cachedEntryStub, result.Data);

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
    }
}
