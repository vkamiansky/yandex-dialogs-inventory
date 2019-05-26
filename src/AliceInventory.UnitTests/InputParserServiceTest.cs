using System;
using Xunit;
using AliceInventory.Logic;

namespace AliceInventory.UnitTests
{
    public class InventoryParserServiceTests
    {
        [Theory]
        [InlineData("добавь яблоко 1 килограмм", InputProcessingCommand.Add, "яблоко", 1, UnitOfMeasure.Kg)]
        [InlineData("добавь арбуз 3 штуки", InputProcessingCommand.Add, "арбуз", 3, UnitOfMeasure.Unit)]
        [InlineData("добавь арбуз 1 штуки", InputProcessingCommand.Add, "арбуз", 1, UnitOfMeasure.Unit)]
        [InlineData("добавь молоко 0,5 литра", InputProcessingCommand.Add, "молоко", 0.5, UnitOfMeasure.L)]
        [InlineData("удали яблоко 1 килограмм", InputProcessingCommand.Delete, "яблоко", 1, UnitOfMeasure.Kg)]
        public void CommandParsingNotNullEntryTest(
            string input,
            InputProcessingCommand command,
            string entryName,
            double entryCount,
            UnitOfMeasure entryUnitOfMeasure)
        {
            InputParserService sut = new InputParserService();
            ProcessingCommand parsedCommand = sut.ParseInput(input);

            Assert.Equal(command, parsedCommand.Command);

            var data = parsedCommand.Data as Entry;

            Assert.Equal(entryName, data?.Name);
            Assert.Equal(entryCount, data?.Count);
            Assert.Equal(entryUnitOfMeasure, data?.Unit);
        }

        [Theory]
        [InlineData("покажи", InputProcessingCommand.ReadList)]
        [InlineData("покажи всё", InputProcessingCommand.ReadList)]
        [InlineData("очисти", InputProcessingCommand.Clear)]
        [InlineData("очисти инвентарь", InputProcessingCommand.Clear)]
        [InlineData("погода спб", InputProcessingCommand.SayUnknownCommand)]
        public void CommandParsingNullEntryTest(
            string input,
            InputProcessingCommand command)
        {
            InputParserService sut = new InputParserService();
            ProcessingCommand parsedCommand = sut.ParseInput(input);

            Assert.Equal(command, parsedCommand.Command);
            Assert.Null(parsedCommand.Data);
        }
    }
}
