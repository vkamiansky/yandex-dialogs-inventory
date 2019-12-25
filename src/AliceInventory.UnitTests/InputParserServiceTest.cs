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
        public void HelloPreparedParsing(string prepared)
        {
            var input = new UserInput
            {
                Prepared = prepared,
                CultureInfo = _defaultCulture
            };
            ParsedCommand parsedCommand = _parser.ParseInput(input);

            Assert.Equal(ParsedPhraseType.Hello, parsedCommand.Type);
            Assert.Null(parsedCommand.Data);
        }

        [Theory]
        [InlineData("добавь яблок 1 килограмм", "яблоко", 1, UnitOfMeasure.Kg)]
        [InlineData("добавь ещё яблок 1 килограмм", "яблоко", 1, UnitOfMeasure.Kg)]
        [InlineData("добавь яблок ещё 1 килограмм", "яблоко", 1, UnitOfMeasure.Kg)]
        [InlineData("яблоки 2 килограмма", "яблоко", 2, UnitOfMeasure.Kg)]
        [InlineData("Прибавь 3 единицы яблок", "яблоко", 3, UnitOfMeasure.Unit)]
        [InlineData("Прибавь 3 шт яблок", "яблоко", 3, UnitOfMeasure.Unit)]
        [InlineData("4 литра яблок", "яблоко", 4, UnitOfMeasure.L)]
        [InlineData("Плюс 5 яблок", "яблоко", 5, null)]
        [InlineData("6 яблок", "яблоко", 6, null)]
        [InlineData("Килограмм яблок", "яблоко", null, UnitOfMeasure.Kg)]
        [InlineData("закинь яблоко", "яблоко", null, null)]
        public void AddPreparedParsing(string prepared, string entryName, double? entryQuantity, UnitOfMeasure? entryUnitOfMeasure)
        {
            var input = new UserInput
            {
                Prepared = prepared,
                CultureInfo = _defaultCulture
            };
            ParsedCommand parsedCommand = _parser.ParseInput(input);

            Assert.Equal(ParsedPhraseType.Add, parsedCommand.Type);

            var data = parsedCommand.Data as ParsedEntry;

            Assert.Equal(entryName, data?.Name);
            Assert.Equal(entryQuantity, data?.Quantity);
            Assert.Equal(entryUnitOfMeasure, data?.Unit);
        }

        [Theory]
        [InlineData("умножь яблоки на 5", "яблоко", 5)]
        [InlineData("увеличь яблоки в 5 раз", "яблоко", 5)]
        public void MultiplyPreparedParsing(string prepared, string entryName, double? entryQuantity)
        {
            var input = new UserInput
            {
                Prepared = prepared,
                CultureInfo = _defaultCulture
            };
            ParsedCommand parsedCommand = _parser.ParseInput(input);

            Assert.Equal(ParsedPhraseType.Multiply, parsedCommand.Type);

            var data = parsedCommand.Data as ParsedEntry;

            Assert.Equal(entryName, data?.Name);
            Assert.Equal(entryQuantity, data?.Quantity);
        }

        [Theory]
        [InlineData("уменьши яблоки в 5 раз", "яблоко", 5)]
        [InlineData("раздели яблоки на 5", "яблоко", 5)]
        public void DivisionPreparedParsing(string prepared, string entryName, double? entryQuantity)
        {
            var input = new UserInput
            {
                Prepared = prepared,
                CultureInfo = _defaultCulture
            };
            ParsedCommand parsedCommand = _parser.ParseInput(input);

            Assert.Equal(ParsedPhraseType.Division, parsedCommand.Type);

            var data = parsedCommand.Data as ParsedEntry;

            Assert.Equal(entryName, data?.Name);
            Assert.Equal(entryQuantity, data?.Quantity);
        }

        [Theory]
        [InlineData("сколько яблок", "яблоко")]
        [InlineData("покажи сколько яблок", "яблоко")]
        [InlineData("выведи яблоки", "яблоко")]
        public void ReadItemPreparedParsing(string prepared, string entryName)
        {
            var input = new UserInput
            {
                Prepared = prepared,
                CultureInfo = _defaultCulture
            };
            ParsedCommand parsedCommand = _parser.ParseInput(input);

            Assert.Equal(ParsedPhraseType.ReadItem, parsedCommand.Type);

            var data = parsedCommand.Data as ParsedEntry;

            Assert.Equal(entryName, data?.Name);
        }

        [Theory]
        [InlineData("добавь 5 ак47", "ак47", 5, null)]
        [InlineData("добавь ак 47 3 штуки", "ак 47", 3, UnitOfMeasure.Unit)]
        [InlineData("ак 47 3,5 килограмма", "ак 47", 3.5, UnitOfMeasure.Kg)]
        [InlineData("добавь молоко 1 литр 5 штук", "молоко 1 литр", 5, UnitOfMeasure.Unit)]
        [InlineData("ну давай добавим 5 кг яблок", "яблоко", 5, UnitOfMeasure.Kg)]
        [InlineData("добавь яблок 4", "яблоко", 4, null)]
        public void SpecificAddPreparedParsing(string prepared, string entryName, double? entryQuantity, UnitOfMeasure? entryUnitOfMeasure)
        {
            var input = new UserInput
            {
                Prepared = prepared,
                CultureInfo = _defaultCulture
            };
            ParsedCommand parsedCommand = _parser.ParseInput(input);

            Assert.Equal(ParsedPhraseType.Add, parsedCommand.Type);

            var data = parsedCommand.Data as ParsedEntry;

            Assert.Equal(entryName, data?.Name);
            Assert.Equal(entryQuantity, data?.Quantity);
            Assert.Equal(entryUnitOfMeasure, data?.Unit);
        }

        [Theory]
        [InlineData("ещё 5 ак47", "ак47", 5, null)]
        [InlineData("еще ак 47 3 штуки", "ак 47", 3, UnitOfMeasure.Unit)]
        [InlineData("ещё ак 47 3,5 килограмма", "ак 47", 3.5, UnitOfMeasure.Kg)]
        [InlineData("ещё молоко 1 литр 5 штук", "молоко 1 литр", 5, UnitOfMeasure.Unit)]
        [InlineData("ну ещё 5 кг яблок", "яблоко", 5, UnitOfMeasure.Kg)]
        [InlineData("ещё яблок 4", "яблоко", 4, null)]
        public void MorePreparedParsing(string prepared, string entryName, double? entryQuantity, UnitOfMeasure? entryUnitOfMeasure)
        {
            var input = new UserInput
            {
                Prepared = prepared,
                CultureInfo = _defaultCulture
            };
            ParsedCommand parsedCommand = _parser.ParseInput(input);

            Assert.Equal(ParsedPhraseType.More, parsedCommand.Type);

            var data = parsedCommand.Data as ParsedEntry;

            Assert.Equal(entryName, data?.Name);
            Assert.Equal(entryQuantity, data?.Quantity);
            Assert.Equal(entryUnitOfMeasure, data?.Unit);
        }

        [Theory]
        [InlineData("удали яблок 1 килограмм", "яблоко", 1, UnitOfMeasure.Kg)]
        [InlineData("ещё удали яблок 1 килограмм", "яблоко", 1, UnitOfMeasure.Kg)]
        [InlineData("убери 2 килограмм яблок", "яблоко", 2, UnitOfMeasure.Kg)]
        [InlineData("убери ка 3 яблока", "яблоко", 3, null)]
        [InlineData("убери ещё 3 яблока", "яблоко", 3, null)]
        [InlineData("убери яблок 5", "яблоко", 5, null)]
        [InlineData("убери молоко", "молоко", null, null)]
        [InlineData("давай убери яблок 3 штуки", "яблоко", 3, UnitOfMeasure.Unit)]
        [InlineData("сотри килограмм яблок", "яблоко", null, UnitOfMeasure.Kg)]
        public void DeletePreparedParsing(string prepared, string entryName, double? entryQuantity, UnitOfMeasure? entryUnitOfMeasure)
        {
            var input = new UserInput
            {
                Prepared = prepared,
                CultureInfo = _defaultCulture
            };
            ParsedCommand parsedCommand = _parser.ParseInput(input);

            Assert.Equal(ParsedPhraseType.Delete, parsedCommand.Type);

            var data = parsedCommand.Data as ParsedEntry;

            Assert.Equal(entryName, data?.Name);
            Assert.Equal(entryQuantity, data?.Quantity);
            Assert.Equal(entryUnitOfMeasure, data?.Unit);
        }

        [Theory]
        [InlineData("покажи", ParsedPhraseType.ReadList)]
        [InlineData("список", ParsedPhraseType.ReadList)]
        [InlineData("учёт", ParsedPhraseType.ReadList)]
        [InlineData("итого", ParsedPhraseType.ReadList)]
        [InlineData("ещё покажи", ParsedPhraseType.ReadList)]
        [InlineData("покажи всё", ParsedPhraseType.ReadList)]
        [InlineData("покажи список", ParsedPhraseType.ReadList)]
        [InlineData("покажи отчёт", ParsedPhraseType.ReadList)]
        [InlineData("покажи опись", ParsedPhraseType.ReadList)]
        [InlineData("выведи список", ParsedPhraseType.ReadList)]
        [InlineData("прочитай список", ParsedPhraseType.ReadList)]
        [InlineData("зачитай список", ParsedPhraseType.ReadList)]
        [InlineData("читай список", ParsedPhraseType.ReadList)]
        [InlineData("что в списке", ParsedPhraseType.ReadList)]
        [InlineData("да", ParsedPhraseType.Accept)]
        [InlineData("конечно", ParsedPhraseType.Accept)]
        [InlineData("конечно давай", ParsedPhraseType.Accept)]
        [InlineData("несомненно", ParsedPhraseType.Accept)]
        [InlineData("точно", ParsedPhraseType.Accept)]
        [InlineData("именно", ParsedPhraseType.Accept)]
        [InlineData("верно", ParsedPhraseType.Accept)]
        [InlineData("хочу", ParsedPhraseType.Accept)]
        [InlineData("давай", ParsedPhraseType.Accept)]
        [InlineData("ну давай", ParsedPhraseType.Accept)]
        [InlineData("нет", ParsedPhraseType.Decline)]
        [InlineData("не надо", ParsedPhraseType.Decline)]
        [InlineData("отмена", ParsedPhraseType.Cancel)]
        [InlineData("отмени", ParsedPhraseType.Cancel)]
        [InlineData("отменяй", ParsedPhraseType.Cancel)]
        [InlineData("отменяю", ParsedPhraseType.Cancel)]
        [InlineData("отменить", ParsedPhraseType.Cancel)]
        [InlineData("очисти", ParsedPhraseType.Clear)]
        [InlineData("очисти список", ParsedPhraseType.Clear)]
        [InlineData("очисть список", ParsedPhraseType.Clear)]
        [InlineData("очисть отчёт", ParsedPhraseType.Clear)]
        [InlineData("очисть опись", ParsedPhraseType.Clear)]
        [InlineData("очисть учёт", ParsedPhraseType.Clear)]
        [InlineData("очистить всё", ParsedPhraseType.Clear)]
        [InlineData("вычисти", ParsedPhraseType.Clear)]
        [InlineData("помоги", ParsedPhraseType.Help)]
        [InlineData("помогите", ParsedPhraseType.Help)]
        [InlineData("помощь", ParsedPhraseType.Help)]
        [InlineData("хелп", ParsedPhraseType.Help)]
        [InlineData("спасай", ParsedPhraseType.Help)]
        [InlineData("спасайте", ParsedPhraseType.Help)]
        [InlineData("выручай", ParsedPhraseType.Help)]
        [InlineData("выручайте", ParsedPhraseType.Help)]
        [InlineData("что ты умеешь", ParsedPhraseType.Help)]
        [InlineData("что ты можешь", ParsedPhraseType.Help)]
        [InlineData("выход", ParsedPhraseType.Exit)]
        [InlineData("пока", ParsedPhraseType.Exit)]
        [InlineData("хватит", ParsedPhraseType.Exit)]
        [InlineData("прощай", ParsedPhraseType.Exit)]
        [InlineData("давай прощай", ParsedPhraseType.Exit)]
        [InlineData("погода спб", ParsedPhraseType.UnknownCommand)]
        [InlineData("что делаешь", ParsedPhraseType.UnknownCommand)]
        [InlineData("люблю тесты", ParsedPhraseType.UnknownCommand)]
        [InlineData("добавь", ParsedPhraseType.UnknownCommand)]
        [InlineData("удали", ParsedPhraseType.UnknownCommand)]
        [InlineData("отправляй somemail ya ru", ParsedPhraseType.UnknownCommand)]
        public void MiscPreparedParsing(string prepared, ParsedPhraseType phraseType)
        {
            var input = new UserInput
            {
                Prepared = prepared,
                CultureInfo = _defaultCulture
            };
            ParsedCommand parsedCommand = _parser.ParseInput(input);

            Assert.Equal(phraseType, parsedCommand.Type);
            Assert.Null(parsedCommand.Data);
        }

        [Theory]
        [InlineData("да", ParsedPhraseType.Accept)]
        [InlineData("нет", ParsedPhraseType.Decline)]
        [InlineData("отмена", ParsedPhraseType.Cancel)]
        [InlineData("показать всё", ParsedPhraseType.ReadList)]
        [InlineData("закончить", ParsedPhraseType.Exit)]
        [InlineData("пока", ParsedPhraseType.Exit)]
        [InlineData("что ты умеешь", ParsedPhraseType.Help)]
        public void MiscButtonParsing(string button, ParsedPhraseType phraseType)
        {
            var input = new UserInput
            {
                Button = button,
                CultureInfo = _defaultCulture
            };
            ParsedCommand parsedCommand = _parser.ParseInput(input);

            Assert.Equal(phraseType, parsedCommand.Type);
            Assert.Null(parsedCommand.Data);
        }

        [Theory]
        [InlineData("Отправь на somemail@ya.ru", ParsedPhraseType.SendMail, "somemail@ya.ru")]
        [InlineData("вышли на some.mai-l@ya.ru", ParsedPhraseType.SendMail, "some.mai-l@ya.ru")]
        [InlineData("somem333ail@ya.ru", ParsedPhraseType.Mail, "somem333ail@ya.ru")]
        [InlineData("отправь список на somemail@yahoo.com", ParsedPhraseType.SendMail, "somemail@yahoo.com")]
        [InlineData("отправь список somemail@yahoo.com", ParsedPhraseType.SendMail, "somemail@yahoo.com")]
        [InlineData("отправь somemail@yahoo.com", ParsedPhraseType.SendMail, "somemail@yahoo.com")]
        [InlineData("перешли somemail@yahoo.com", ParsedPhraseType.SendMail, "somemail@yahoo.com")]
        public void SendMailRawParsingTest(string raw, ParsedPhraseType phraseType, string expectedEmail)
        {
            var input = new UserInput
            {
                Raw = raw,
                CultureInfo = _defaultCulture
            };
            var parsedCommand = _parser.ParseInput(input);

            Assert.Equal(phraseType, parsedCommand.Type);

            var email = parsedCommand.Data as string;

            Assert.Equal(expectedEmail, email);
        }

        [Theory]
        [InlineData("Удали мыло", ParsedPhraseType.DeleteMail, null)]
        [InlineData("Отправь на почту", ParsedPhraseType.SendMail, null)]
        [InlineData("Вышли на почту", ParsedPhraseType.SendMail, null)]
        [InlineData("отправь отчет на мою почту", ParsedPhraseType.SendMail, null)]
        [InlineData("перешли на мою почту", ParsedPhraseType.SendMail, null)]
        public void SendMailPreparedParsingTest(string prepared, ParsedPhraseType phraseType, string expectedEmail)
        {
            var input = new UserInput
            {
                Prepared = prepared,
                CultureInfo = _defaultCulture
            };
            var parsedCommand = _parser.ParseInput(input);

            Assert.Equal(phraseType, parsedCommand.Type);

            var email = parsedCommand.Data as string;

            Assert.Equal(expectedEmail, email);
        }

        [Theory]
        [InlineData("добавь предмет 0,1 кг", "ru-RU", 0.1)]
        [InlineData("добавь предмет -1,1 кг", "ru-RU", -1.1)]
        [InlineData("добавь предмет 0.1 кг", "en-US", 0.1)]
        [InlineData("добавь предмет -1.1 кг", "en-US", -1.1)]
        public void CommandParsingCultureTest(string prepared, string culture, double entryQuantity)
        {
            var input = new UserInput
            {
                Prepared = prepared,
                CultureInfo = new CultureInfo(culture)
            };
            var parsedCommand = _parser.ParseInput(input);
            var data = parsedCommand.Data as ParsedEntry;

            Assert.Equal(entryQuantity, data?.Quantity);
        }
    }
}
