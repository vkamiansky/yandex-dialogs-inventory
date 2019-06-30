using System;
using System.Globalization;
using Xunit;
using AliceInventory.Logic;
using AliceInventory.Logic.Parser;

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
            ParsedCommand parsedCommand = _parser.ParseInput(input, _defaultCulture);
            
            Assert.Equal(ParsedCommandType.SayHello, parsedCommand.Type);
            Assert.Null(parsedCommand.Data);
        }

        [Theory]
        [InlineData("добавь яблок 1 килограмм", "яблок", 1, UnitOfMeasure.Kg)]
        [InlineData("добавь ещё яблок 1 килограмм", "яблок", 1, UnitOfMeasure.Kg)]
        [InlineData("добавь яблок ещё 1 килограмм", "яблок", 1, UnitOfMeasure.Kg)]
        [InlineData("яблоки 2 килограмма", "яблоки", 2, UnitOfMeasure.Kg)]
        [InlineData("Прибавь 3 единицы яблок", "яблок", 3, UnitOfMeasure.Unit)]
        [InlineData("Прибавь 3 шт яблок", "яблок", 3, UnitOfMeasure.Unit)]
        [InlineData("4 литра яблок", "яблок", 4, UnitOfMeasure.L)]
        [InlineData("Плюс 5 яблок", "яблок", 5, null)]
        [InlineData("6 яблок", "яблок", 6, null)]
        [InlineData("Килограмм яблок", "яблок", null, UnitOfMeasure.Kg)]
        [InlineData("закинь яблоко", "яблоко", null, null)]
        public void AddCommandParsing(string input, string entryName, double? entryCount, UnitOfMeasure? entryUnitOfMeasure)
        {
            ParsedCommand parsedCommand = _parser.ParseInput(input, _defaultCulture);

            Assert.Equal(ParsedCommandType.Add, parsedCommand.Type);

            var data = parsedCommand.Data as ParsedSingleEntry;

            Assert.Equal(entryName, data?.Name);
            Assert.Equal(entryCount, data?.Count);
            Assert.Equal(entryUnitOfMeasure, data?.Unit);
        }

        [Theory]
        [InlineData("добавь 5 ак47", "ак47", 5, null)]
        [InlineData("добавь ак-47 3 штуки", "ак-47", 3, UnitOfMeasure.Unit)]
        [InlineData("ак 47 3,5 килограмма", "ак 47", 3.5, UnitOfMeasure.Kg)]
        [InlineData("добавь молоко 1 литр 5 штук", "молоко 1 литр", 5, UnitOfMeasure.Unit)]
        [InlineData("ну давай добавим 5 кг яблок", "яблок", 5, UnitOfMeasure.Kg)]
        [InlineData("добавь яблок 4", "яблок", 4, null)]
        public void SpecificAddCommandParsing(string input, string entryName, double? entryCount, UnitOfMeasure? entryUnitOfMeasure)
        {
            ParsedCommand parsedCommand = _parser.ParseInput(input, _defaultCulture);

            Assert.Equal(ParsedCommandType.Add, parsedCommand.Type);

            var data = parsedCommand.Data as ParsedSingleEntry;

            Assert.Equal(entryName, data?.Name);
            Assert.Equal(entryCount, data?.Count);
            Assert.Equal(entryUnitOfMeasure, data?.Unit);
        }

        [Theory]
        [InlineData("ещё 5 ак47", "ак47", 5, null)]
        [InlineData("еще ак-47 3 штуки", "ак-47", 3, UnitOfMeasure.Unit)]
        [InlineData("ещё ак 47 3,5 килограмма", "ак 47", 3.5, UnitOfMeasure.Kg)]
        [InlineData("ещё молоко 1 литр 5 штук", "молоко 1 литр", 5, UnitOfMeasure.Unit)]
        [InlineData("ну ещё 5 кг яблок", "яблок", 5, UnitOfMeasure.Kg)]
        [InlineData("ещё яблок 4", "яблок", 4, null)]
        public void MoreCommandParsing(string input, string entryName, double? entryCount, UnitOfMeasure? entryUnitOfMeasure)
        {
            ParsedCommand parsedCommand = _parser.ParseInput(input, _defaultCulture);

            Assert.Equal(ParsedCommandType.More, parsedCommand.Type);

            var data = parsedCommand.Data as ParsedSingleEntry;

            Assert.Equal(entryName, data?.Name);
            Assert.Equal(entryCount, data?.Count);
            Assert.Equal(entryUnitOfMeasure, data?.Unit);
        }

        [Theory]
        [InlineData("удали яблок 1 килограмм", "яблок", 1, UnitOfMeasure.Kg)]
        [InlineData("ещё удали яблок 1 килограмм", "яблок", 1, UnitOfMeasure.Kg)]
        [InlineData("убери 2 килограмм яблок", "яблок", 2, UnitOfMeasure.Kg)]
        [InlineData("убери 3 яблока", "яблока", 3, null)]
        [InlineData("убери ещё 3 яблока", "яблока", 3, null)]
        [InlineData("убери яблок 5", "яблок", 5, null)]
        [InlineData("давай убери яблок 3 штуки", "яблок", 3, UnitOfMeasure.Unit)]
        [InlineData("сотри килограмм яблок", "яблок", null, UnitOfMeasure.Kg)]
        public void DeleteCommandParsing(string input, string entryName, double? entryCount, UnitOfMeasure? entryUnitOfMeasure)
        {
            ParsedCommand parsedCommand = _parser.ParseInput(input, _defaultCulture);

            Assert.Equal(ParsedCommandType.Delete, parsedCommand.Type);

            var data = parsedCommand.Data as ParsedSingleEntry;

            Assert.Equal(entryName, data?.Name);
            Assert.Equal(entryCount, data?.Count);
            Assert.Equal(entryUnitOfMeasure, data?.Unit);
        }

        [Theory]
        [InlineData("покажи", ParsedCommandType.ReadList)]
        [InlineData("ещё покажи", ParsedCommandType.ReadList)]
        [InlineData("покажи всё", ParsedCommandType.ReadList)]
        [InlineData("покажи список", ParsedCommandType.ReadList)]
        [InlineData("покажи отчёт", ParsedCommandType.ReadList)]
        [InlineData("покажи опись", ParsedCommandType.ReadList)]
        [InlineData("список", ParsedCommandType.ReadList)]
        [InlineData("выведи список", ParsedCommandType.ReadList)]
        [InlineData("итого", ParsedCommandType.ReadList)]
        [InlineData("прочитай список", ParsedCommandType.ReadList)]
        [InlineData("зачитай список", ParsedCommandType.ReadList)]
        [InlineData("читай список", ParsedCommandType.ReadList)]
        [InlineData("что в списке", ParsedCommandType.ReadList)]
        [InlineData("да", ParsedCommandType.Accept)]
        [InlineData("конечно", ParsedCommandType.Accept)]
        [InlineData("конечно давай", ParsedCommandType.Accept)]
        [InlineData("несомненно", ParsedCommandType.Accept)]
        [InlineData("точно", ParsedCommandType.Accept)]
        [InlineData("именно", ParsedCommandType.Accept)]
        [InlineData("верно", ParsedCommandType.Accept)]
        [InlineData("хочу", ParsedCommandType.Accept)]
        [InlineData("давай", ParsedCommandType.Accept)]
        [InlineData("ну давай", ParsedCommandType.Accept)]
        [InlineData("нет", ParsedCommandType.Decline)]
        [InlineData("не надо", ParsedCommandType.Decline)]
        [InlineData("отмена", ParsedCommandType.Cancel)]
        [InlineData("отмени", ParsedCommandType.Cancel)]
        [InlineData("отменяй", ParsedCommandType.Cancel)]
        [InlineData("отменяю", ParsedCommandType.Cancel)]
        [InlineData("отменить", ParsedCommandType.Cancel)]
        [InlineData("очисти", ParsedCommandType.Clear)]
        [InlineData("очисти список", ParsedCommandType.Clear)]
        [InlineData("очисть список", ParsedCommandType.Clear)]
        [InlineData("очисть отчёт", ParsedCommandType.Clear)]
        [InlineData("очисть опись", ParsedCommandType.Clear)]
        [InlineData("очисть учёт", ParsedCommandType.Clear)]
        [InlineData("очистить всё", ParsedCommandType.Clear)]
        [InlineData("вычисти", ParsedCommandType.Clear)]
        [InlineData("помоги", ParsedCommandType.RequestHelp)]
        [InlineData("помогите", ParsedCommandType.RequestHelp)]
        [InlineData("помощь", ParsedCommandType.RequestHelp)]
        [InlineData("хелп", ParsedCommandType.RequestHelp)]
        [InlineData("спасай", ParsedCommandType.RequestHelp)]
        [InlineData("спасайте", ParsedCommandType.RequestHelp)]
        [InlineData("выручай", ParsedCommandType.RequestHelp)]
        [InlineData("выручайте", ParsedCommandType.RequestHelp)]
        [InlineData("что ты умеешь", ParsedCommandType.RequestHelp)]
        [InlineData("что ты можешь", ParsedCommandType.RequestHelp)]
        [InlineData("выход", ParsedCommandType.RequestExit)]
        [InlineData("пока", ParsedCommandType.RequestExit)]
        [InlineData("хватит", ParsedCommandType.RequestExit)]
        [InlineData("прощай", ParsedCommandType.RequestExit)]
        [InlineData("давай прощай", ParsedCommandType.RequestExit)]
        [InlineData("погода спб", ParsedCommandType.SayUnknownCommand)]
        [InlineData("что делаешь", ParsedCommandType.SayUnknownCommand)]
        [InlineData("люблю тесты", ParsedCommandType.SayUnknownCommand)]
        [InlineData("добавь", ParsedCommandType.SayUnknownCommand)]
        [InlineData("удали", ParsedCommandType.SayUnknownCommand)]
        [InlineData("отправляй somemail@yaru", ParsedCommandType.SayUnknownCommand)]
        public void AnotherCommandParsing(string input, ParsedCommandType commandType)
        {
            ParsedCommand parsedCommand = _parser.ParseInput(input, _defaultCulture);

            Assert.Equal(commandType, parsedCommand.Type);
            Assert.Null(parsedCommand.Data);
        }

        [Theory]
        [InlineData("Отправь на somemail@ya.ru", ParsedCommandType.SendMailTo, "somemail@ya.ru")]
        [InlineData("вышли на some.mai-l@ya.ru", ParsedCommandType.SendMailTo, "some.mai-l@ya.ru")]
        [InlineData("somem333ail@ya.ru", ParsedCommandType.AddMail, "somem333ail@ya.ru")]
        [InlineData("Удали мыло", ParsedCommandType.DeleteMail, null)]
        [InlineData("Отправь на почту", ParsedCommandType.SendMail, null)]
        [InlineData("Вышли на почту", ParsedCommandType.SendMail, null)]
        public void MailSentParsingTest(string input, ParsedCommandType commandType, string expectedEmail)
        {
            var parsedCommand = _parser.ParseInput(input, _defaultCulture);

            Assert.Equal(commandType, parsedCommand.Type);

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
            var data = parsedCommand.Data as ParsedSingleEntry;

            Assert.Equal(entryCount, data?.Count);
        }
    }
}
