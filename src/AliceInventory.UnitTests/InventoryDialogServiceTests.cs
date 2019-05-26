using System;
using Xunit;
using Moq;
using AliceInventory;

namespace AliceInventory.UnitTests
{
    public class InventoryDialogServiceTests
    {
        [Fact]
        public void ProcessInputAddTest()
        {
            var input1 = "объект1 123.123кг";
            var userId1 = "userId1";
            var logicEntryStub1 = new Logic.Entry
            {
                Unit = Logic.UnitOfMeasure.Kg,
                Name = "объект1",
                Count = 123.123d
            };
            var processingCommand1 = new Logic.ProcessingCommand
            {
                Command = Logic.InputProcessingCommand.Add,
                Data = logicEntryStub1
            };

            var storageMock = new Mock<Data.IInventoryStorage>(MockBehavior.Strict);
            storageMock.Setup(x => x.Add(
                It.Is<string>(y => y == userId1),
                It.Is<Data.Entry>(y =>
                    Logic.Extensions.ToLogic(y.Unit) == logicEntryStub1.Unit
                    && y.Name == logicEntryStub1.Name
                    && y.Count == logicEntryStub1.Count)))
                .Returns(true);

            var parserMock = new Mock<Logic.IInputParserService>();
            parserMock.Setup(x => x.ParseInput(
                    It.Is<string>(y => y == input1)))
                .Returns(processingCommand1);

            var commandCacheMock = new Mock<Logic.ICommandCache>(MockBehavior.Strict);
            commandCacheMock.Setup(x => x.Set(
                It.Is<string>(y => y == userId1),
                It.Is<Logic.ProcessingCommand>(y =>
                    y.Command == processingCommand1.Command
                    && y.Data == processingCommand1.Data)));

            var sut = new Logic.InventoryDialogService(
                storageMock.Object,
                parserMock.Object,
                commandCacheMock.Object);

            var result = sut.ProcessInput(userId1, input1);

            Assert.Equal(Logic.InputProcessingResult.Added, result.Result);
            Assert.Equal(logicEntryStub1, result.Data);

            storageMock.Verify(x =>
                x.Add(
                    It.IsAny<string>(),
                    It.IsAny<Data.Entry>()), Times.Once);

            parserMock.Verify(x =>
                x.ParseInput(It.IsAny<string>()), Times.Once);

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
            var logicEntryStub = new Logic.Entry
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

            var storageMock = new Mock<Data.IInventoryStorage>(MockBehavior.Strict);
            storageMock.Setup(x => x.Delete(
                It.Is<string>(y => y == userId),
                It.Is<Data.Entry>(y =>
                    Logic.Extensions.ToLogic(y.Unit) == logicEntryStub.Unit
                    && y.Name == logicEntryStub.Name
                    && y.Count == logicEntryStub.Count)))
                .Returns(true);

            var parserMock = new Mock<Logic.IInputParserService>();
            parserMock.Setup(x => x.ParseInput(
                    It.Is<string>(y => y == input)))
                .Returns(processingCommand);

            var commandCacheMock = new Mock<Logic.ICommandCache>(MockBehavior.Strict);
            commandCacheMock.Setup(x => x.Set(
                It.Is<string>(y => y == userId),
                It.Is<Logic.ProcessingCommand>(y =>
                    y.Command == processingCommand.Command
                    && y.Data == processingCommand.Data)));

            var sut = new Logic.InventoryDialogService(
                storageMock.Object,
                parserMock.Object,
                commandCacheMock.Object);

            var result = sut.ProcessInput(userId, input);

            Assert.Equal(Logic.InputProcessingResult.Deleted, result.Result);
            Assert.Equal(logicEntryStub, result.Data);

            storageMock.Verify(x =>
                x.Delete(
                    It.IsAny<string>(),
                    It.IsAny<Data.Entry>()), Times.Once);

            parserMock.Verify(x =>
                x.ParseInput(It.IsAny<string>()), Times.Once);

            commandCacheMock.Verify(x =>
                x.Set(
                    It.IsAny<string>(),
                    It.IsAny<Logic.ProcessingCommand>()), Times.Once);
        }
    }
}
