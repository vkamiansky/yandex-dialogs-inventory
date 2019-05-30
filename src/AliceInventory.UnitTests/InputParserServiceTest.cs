using System;
using System.Globalization;
using Xunit;
using AliceInventory.Logic;

namespace AliceInventory.UnitTests
{
    public class InventoryParserServiceTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("привет")]
        [InlineData("приветствую")]
        [InlineData("здравствуй")]
        [InlineData("здравствуйте")]
        [InlineData("хай")]
        [InlineData("хеллоу")]
        public void CommandParsingSayHelloTest(string input)
        {
            InputParserService sut = new InputParserService();
            ProcessingCommand parsedCommand = sut.ParseInput(input, CultureInfo.InvariantCulture);

            Assert.Equal(InputProcessingCommand.SayHello, parsedCommand.Command);
            Assert.Null(parsedCommand.Data);
        }

        [Theory]
        [InlineData("добавь яблоко 1 килограмм", InputProcessingCommand.Add, "яблоко", 1, UnitOfMeasure.Kg)]
        [InlineData("добавь яблоко 1 единицу", InputProcessingCommand.Add, "яблоко", 1, UnitOfMeasure.Unit)]
        [InlineData("добавь яблоко 1 штуку", InputProcessingCommand.Add, "яблоко", 1, UnitOfMeasure.Unit)]
        [InlineData("добавь яблок штуки 2", InputProcessingCommand.Add, "яблок", 2, UnitOfMeasure.Unit)]
        [InlineData("добавь штуки яблок 2", InputProcessingCommand.Add, "яблок", 2, UnitOfMeasure.Unit)]
        [InlineData("добавь штуки 2 яблок", InputProcessingCommand.Add, "яблок", 2, UnitOfMeasure.Unit)]
        [InlineData("добавь 1 штуку яблок", InputProcessingCommand.Add, "яблок", 1, UnitOfMeasure.Unit)]
        [InlineData("добавь 1 яблок килограмм", InputProcessingCommand.Add, "яблок", 1, UnitOfMeasure.Kg)]
        [InlineData("добавь 1 яблоко", InputProcessingCommand.Add, "яблоко", 1, UnitOfMeasure.Unit)]
        [InlineData("добавь яблоко", InputProcessingCommand.Add, "яблоко", 1, UnitOfMeasure.Unit)]
        [InlineData("добавь килограмм яблок", InputProcessingCommand.Add, "яблок", 1, UnitOfMeasure.Kg)]
        [InlineData("добавь арбуз 3 штуки", InputProcessingCommand.Add, "арбуз", 3, UnitOfMeasure.Unit)]
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
        [InlineData("добавь предмет -2 штуки", InputProcessingCommand.SayIllegalArguments)]
        [InlineData("удали предмет 0 штук", InputProcessingCommand.SayIllegalArguments)]
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
        [InlineData("отправь на testmail@test.ru", "testmail@test.ru")]
        [InlineData("отправь на почту testmail@test.ru", "testmail@test.ru")]
        [InlineData("отправь test-mail@test.ru", "test-mail@test.ru")]
        [InlineData("отправь test.mail@test.ru", "test.mail@test.ru")]
        [InlineData("отправь test_mail@test.ru", "test_mail@test.ru")]
        [InlineData("отправь test.mail.test@test.ru", "test.mail.test@test.ru")]
        [InlineData("отправь testmail@test.test", "testmail@test.test")]
        [InlineData("отправь TestMail@test.test", "testmail@test.test")]
        [InlineData("отправь testmail@testdomain.test", "testmail@testdomain.test")]
        public void CommandParsingRightEmailTest(
            string input,
            string email)
        {
            InputParserService sut = new InputParserService();
            ProcessingCommand parsedCommand = sut.ParseInput(input, CultureInfo.InvariantCulture);

            Assert.NotNull(parsedCommand);
            Assert.NotNull(parsedCommand.Data);
            Assert.Equal(InputProcessingCommand.SendMail, parsedCommand.Command);
            Assert.Equal(email, parsedCommand.Data.ToString());
        }
        
        [Theory]
        [InlineData("отправь @test.ru")]
        [InlineData("отправь test")]
        [InlineData("отправь test@@test.ru")]
        [InlineData("отправь test@test@test.ru")]
        [InlineData("отправь test@test.")]
        [InlineData("отправь test@test.a")]
        public void CommandParsingWrongEmailTest(string input)
        {
            InputParserService sut = new InputParserService();
            ProcessingCommand parsedCommand = sut.ParseInput(input, CultureInfo.InvariantCulture);

            Assert.NotNull(parsedCommand);
            Assert.Null(parsedCommand.Data);
            Assert.Equal(InputProcessingCommand.SayIllegalArguments, parsedCommand.Command);
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
