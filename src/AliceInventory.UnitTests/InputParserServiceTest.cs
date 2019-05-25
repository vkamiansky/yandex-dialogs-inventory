using System;
using System.Globalization;
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
        [InlineData("добавь молоко 0.5 литра", InputProcessingCommand.Add, "молоко", 0.5, UnitOfMeasure.L)]
        [InlineData("удали яблоко 1 килограмм", InputProcessingCommand.Delete, "яблоко", 1, UnitOfMeasure.Kg)]
        public void CommandParsingNotNullEntryTest(
            string input,
            InputProcessingCommand command,
            string entryName,
            double entryCount,
            UnitOfMeasure entryUnitOfMeasure)
        {
            InputParserService sut = new InputParserService();
            ProcessingCommand parsedCommand = sut.ParseInput(input, CultureInfo.CurrentCulture);

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
            ProcessingCommand parsedCommand = sut.ParseInput(input, CultureInfo.CurrentCulture);

            Assert.Equal(command, parsedCommand.Command);
            Assert.Null(parsedCommand.Data);
        }

        [Theory]
        [InlineData("добавь предмет ,1 кг", 0.1)]
        [InlineData("добавь предмет 0,1 кг", 0.1)]
        [InlineData("добавь предмет 1,1 кг", 1.1)]
        public void CommandParsingCultureRu(
            string input,
            double entryCount)
        {
            InputParserService sut = new InputParserService();
            CultureInfo clientCulture = new CultureInfo("ru-RU");
            ProcessingCommand parsedCommand = sut.ParseInput(input, clientCulture);

            var data = parsedCommand.Data as Entry;

            Assert.Equal(entryCount, data?.Count);
        }

        [Theory]
        [InlineData("добавь предмет .1 кг", 0.1)]
        [InlineData("добавь предмет 0.1 кг", 0.1)]
        [InlineData("добавь предмет 1.1 кг", 1.1)]
        public void CommandParsingCultureEng(
            string input,
            double entryCount)
        {
            InputParserService sut = new InputParserService();
            CultureInfo clientCulture = new CultureInfo("en-US");
            ProcessingCommand parsedCommand = sut.ParseInput(input, clientCulture);

            var data = parsedCommand.Data as Entry;

            Assert.Equal(entryCount, data?.Count);
        }
    }
}
