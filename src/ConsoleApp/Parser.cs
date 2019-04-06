namespace ConsoleApp {
    public class Parser {

        public Unit DefaultUnit = new Unit() {
            CommonName = "штука",
            ShortName = "шт.",
            AlternativeNames = new string[0],
        };

        public ParserResponse TryParse(string input) {
            return new ParserResponse {
                ItemString = "Какой-то предмет",
                ItemCount = 1,
                Unit =  DefaultUnit,
            };
        }
    }
}