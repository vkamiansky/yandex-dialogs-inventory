using System;
using System.Globalization;
using Xunit;
using AliceInventory.Logic;

namespace AliceInventory.UnitTests
{
    public class InventoryParserServiceTests
    {
        private readonly CultureInfo _defaultCulture = new CultureInfo("ru-RU");
        private readonly InputParserService _parser;
        public InventoryParserServiceTests()
        {
            _parser = new InputParserService();
        }

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
            ProcessingCommand parsedCommand = _parser.ParseInput(input, _defaultCulture);
            
            Assert.Equal(InputProcessingCommand.SayHello, parsedCommand.Command);
            Assert.Null(parsedCommand.Data);
        }

        [Theory]
        [InlineData("добавь яблок 1 килограмм", "яблок", 1, UnitOfMeasure.Kg)]
        [InlineData("яблоки 2 килограмма", "яблоки", 2, UnitOfMeasure.Kg)]
        [InlineData("Прибавь 3 единицы яблок", "яблок", 3, UnitOfMeasure.Unit)]
        [InlineData("4 литра яблок", "яблок", 4, UnitOfMeasure.L)]
        [InlineData("Плюс 5 яблок", "яблок", 5, UnitOfMeasure.Unit)]
        [InlineData("6 яблок", "яблок", 6, UnitOfMeasure.Unit)]
        [InlineData("засунь килограмм яблок", "яблок", 1, UnitOfMeasure.Kg)]
        [InlineData("Килограмм яблок", "яблок", 1, UnitOfMeasure.Kg)]
        [InlineData("закинь яблоко", "яблоко", 1, UnitOfMeasure.Unit)]
        public void AddCommandParsing(string input, string entryName, double entryCount, UnitOfMeasure entryUnitOfMeasure)
        {
            ProcessingCommand parsedCommand = _parser.ParseInput(input, _defaultCulture);

            Assert.Equal(InputProcessingCommand.Add, parsedCommand.Command);

            var data = parsedCommand.Data as Entry;

            Assert.Equal(entryName, data?.Name);
            Assert.Equal(entryCount, data?.Count);
            Assert.Equal(entryUnitOfMeasure, data?.Unit);
        }

        [Theory]
        [InlineData("добавь 5 ак47", "ак47", 5, UnitOfMeasure.Unit)]
        [InlineData("добавь ак-47 3 штуки", "ак-47", 3, UnitOfMeasure.Unit)]
        [InlineData("ак 47 3,5 килограмма", "ак 47", 3.5, UnitOfMeasure.Kg)]
        [InlineData("добавь молоко 1 литр 5 штук", "молоко 1 литр", 5, UnitOfMeasure.Unit)]
        [InlineData("ну давай добавим 5 кг яблок", "яблок", 1, UnitOfMeasure.Kg)]
        [InlineData("ещё 4 яблока", "яблока", 4, UnitOfMeasure.Unit)]
        public void SpecificAddCommandParsing(string input, string entryName, double entryCount, UnitOfMeasure entryUnitOfMeasure)
        {
            ProcessingCommand parsedCommand = _parser.ParseInput(input, _defaultCulture);

            Assert.Equal(InputProcessingCommand.Add, parsedCommand.Command);

            var data = parsedCommand.Data as Entry;

            Assert.Equal(entryName, data?.Name);
            Assert.Equal(entryCount, data?.Count);
            Assert.Equal(entryUnitOfMeasure, data?.Unit);
        }

        [Theory]
        [InlineData("удали яблок 1 килограмм", "яблок", 1, UnitOfMeasure.Kg)]
        [InlineData("убери 2 килограмм яблок", "яблок", 2, UnitOfMeasure.Kg)]
        [InlineData("вытащи 3 яблока", "яблока", 3, UnitOfMeasure.Unit)]
        [InlineData("сотри килограмм яблок", "яблок", 1, UnitOfMeasure.Kg)]
        [InlineData("высуни яблоко", "яблоко", 1, UnitOfMeasure.Unit)]
        public void DeleteCommandParsing(string input, string entryName, double entryCount, UnitOfMeasure entryUnitOfMeasure)
        {
            ProcessingCommand parsedCommand = _parser.ParseInput(input, _defaultCulture);

            Assert.Equal(InputProcessingCommand.Delete, parsedCommand.Command);

            var data = parsedCommand.Data as Entry;

            Assert.Equal(entryName, data?.Name);
            Assert.Equal(entryCount, data?.Count);
            Assert.Equal(entryUnitOfMeasure, data?.Unit);
        }

        [Theory]
        [InlineData("покажи", InputProcessingCommand.ReadList)]
        [InlineData("покажи всё", InputProcessingCommand.ReadList)]
        [InlineData("покажи инвентарь", InputProcessingCommand.ReadList)]
        [InlineData("покажи рюкзак", InputProcessingCommand.ReadList)]
        [InlineData("расскажи всё", InputProcessingCommand.ReadList)]
        [InlineData("продемонстрируй рюкзак", InputProcessingCommand.ReadList)]
        [InlineData("да", InputProcessingCommand.Accept)]
        [InlineData("конечно", InputProcessingCommand.Accept)]
        [InlineData("несомненно", InputProcessingCommand.Accept)]
        [InlineData("точно", InputProcessingCommand.Accept)]
        [InlineData("именно", InputProcessingCommand.Accept)]
        [InlineData("верно", InputProcessingCommand.Accept)]
        [InlineData("хочу", InputProcessingCommand.Accept)]
        [InlineData("давай", InputProcessingCommand.Accept)]
        [InlineData("ну давай", InputProcessingCommand.Accept)]
        [InlineData("нет", InputProcessingCommand.Decline)]
        [InlineData("не надо", InputProcessingCommand.Decline)]
        [InlineData("отмена", InputProcessingCommand.Cancel)]
        [InlineData("отмени", InputProcessingCommand.Cancel)]
        [InlineData("отменяй", InputProcessingCommand.Cancel)]
        [InlineData("отменяю", InputProcessingCommand.Cancel)]
        [InlineData("отменить", InputProcessingCommand.Cancel)]
        [InlineData("очисти", InputProcessingCommand.Clear)]
        [InlineData("очисти рюкзак", InputProcessingCommand.Clear)]
        [InlineData("очисти инвентарь", InputProcessingCommand.Clear)]
        [InlineData("очисть список", InputProcessingCommand.Clear)]
        [InlineData("очистить всё", InputProcessingCommand.Clear)]
        [InlineData("вычисти", InputProcessingCommand.Clear)]
        [InlineData("помоги", InputProcessingCommand.RequestHelp)]
        [InlineData("помогите", InputProcessingCommand.RequestHelp)]
        [InlineData("помощь", InputProcessingCommand.RequestHelp)]
        [InlineData("хелп", InputProcessingCommand.RequestHelp)]
        [InlineData("спасай", InputProcessingCommand.RequestHelp)]
        [InlineData("спасайте", InputProcessingCommand.RequestHelp)]
        [InlineData("выручай", InputProcessingCommand.RequestHelp)]
        [InlineData("выручайте", InputProcessingCommand.RequestHelp)]
        [InlineData("что ты умеешь?", InputProcessingCommand.RequestHelp)]
        [InlineData("что ты можешь", InputProcessingCommand.RequestHelp)]
        [InlineData("выход", InputProcessingCommand.RequestExit)]
        [InlineData("пока", InputProcessingCommand.RequestExit)]
        [InlineData("хватит", InputProcessingCommand.RequestExit)]
        [InlineData("прощай", InputProcessingCommand.RequestExit)]
        [InlineData("погода спб", InputProcessingCommand.SayUnknownCommand)]
        [InlineData("что делаешь", InputProcessingCommand.SayUnknownCommand)]
        [InlineData("люблю тесты", InputProcessingCommand.SayUnknownCommand)]
        [InlineData("добавь", InputProcessingCommand.SayUnknownCommand)]
        [InlineData("удали", InputProcessingCommand.SayUnknownCommand)]
        [InlineData("отправляй somemail@yaru", InputProcessingCommand.SayUnknownCommand)]
        public void AnotherCommandParsing(string input, InputProcessingCommand command)
        {
            ProcessingCommand parsedCommand = _parser.ParseInput(input, _defaultCulture);

            Assert.Equal(command, parsedCommand.Command);
            Assert.Null(parsedCommand.Data);
        }

        [Theory]
        [InlineData("Отправь на somemail@ya.ru", "somemail@ya.ru")]
        [InlineData("вышли на some.mai-l@ya.ru", "some.mai-l@ya.ru")]
        [InlineData("пошли somem333ail@ya.ru", "somem333ail@ya.ru")]
        [InlineData("послать на somem333ail@ya.ru", "somem333ail@ya.ru")]
        public void MailSentParsingTest(string input, string expectedEmail)
        {
            var parsedCommand = _parser.ParseInput(input, _defaultCulture);

            Assert.Equal(InputProcessingCommand.SendMail, parsedCommand.Command);

            var email = parsedCommand.Data as string;

            Assert.Equal(expectedEmail, email);
        }

        [Theory]
        [InlineData("добавь предмет 0,1 кг", "ru-RU", 0.1)]
        [InlineData("добавь предмет -1,1 кг", "ru-RU", -1.1)]
        [InlineData("добавь предмет 0.1 кг", "en-US", 0.1)]
        [InlineData("добавь предмет -1.1 кг", "en-US", -1.1)]
        public void CommandParsingCultureTest(string input, string culture, double entryCount)
        {
            var parsedCommand = _parser.ParseInput(input, new CultureInfo(culture));
            var data = parsedCommand.Data as Entry;

            Assert.Equal(entryCount, data?.Count);
        }
    }
}
