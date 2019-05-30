using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AliceInventory.Logic.Parser
{
    public static class RegexHelper
    {
        public const string AddWord =
            @"(?:за|в|над?|п(?:о|ри)|до)?(?:с(?:оедин|ун)|пих|ки|лож|мест|бав)(?:н)?(?:л|и|ь)?(?:ай|яй|ть|те|ка)?";
        public const string DeleteWord =
            @"(?:вы|у)?(?:дал|бер|сун|брос|бра|тащ|сотр|изъя|ничтож|стер|стир|н|таск)(?:ива|ова|ева|ыва)?(?:ай|у|е|и|й|ь)?(?:ть)?(?:те|ка)?";
        public const string ClearWord =
            @"(?:о|вы)чист(?:и|(?:ит)?ь)(?:\s(?:вс(?:е|ё)|список|рюкзак|инвентарь))?";
        public const string HelloWord =
            @"привет(?:ик|ствую)?|здравствуй(?:те)?|хай|хеллоу";
        public const string AcceptWord =
            @"да(?:вай)?|конечно|несомненно|точно|именно|верно|хочу|подтвер(?:дить|ждаю)";
        public const string DeclineWord =
            @"не(?:т|\sнадо)?|от(?:вали|стань)";
        public const string CancelWord =
            @"отмен(?:а|и(?:ть)?|я(?:й|ю))";
        public const string HelpWord =
            @"помо(?:ги(?:те)?|щь)|хелп|спас(?:и|а(?:й(?:те)?))|выручай(?:те)?|что ты умеешь";
        public const string ReadListWord =
            @"(?:п(?:ока(?:жи|зать)|родемонстрируй)|расскажи)(?:\s(?:вс(?:е|ё)|список|рюкзак|инвентарь))?";
        public const string SendMailWord =
            @"отправ(?:ь(?:те)?|ить|ляй)|вы(?:шли|слать)|по(?:шли|слать)";
        public const string ExitWord =
            @"п(?:ока|рощай)|выход|хватит";
        
        public const string UnitWord = @"штук(?:а|и|у|овин)?|единиц(?:а|у|ы)?";
        public const string KgWord = @"к(?:г|илограмм(?:а|ов)?)";
        public const string LiterWord = @"литр(?:а|ов)?";
        private static Dictionary<UnitOfMeasure, string> unitOfMeasureDictionary;
        public const string UnitOfMeasureWord = UnitWord + "|" + KgWord + "|" + LiterWord;

        public static readonly string EntryNameWord = @"[а-яА-ЯёЁ\d]{3,}";
        public static readonly string Number = @"-?\d+(?:(?:\.|,)\d+)?";
        public static readonly string Email = @"";

        static RegexHelper()
        {
            unitOfMeasureDictionary = new Dictionary<UnitOfMeasure, string>();
            unitOfMeasureDictionary[UnitOfMeasure.Unit] = UnitWord;
            unitOfMeasureDictionary[UnitOfMeasure.Kg] = KgWord;
            unitOfMeasureDictionary[UnitOfMeasure.L] = LiterWord;
        }

        public static UnitOfMeasure ParseUnitOfMeasure(string unit)
        {
            foreach (var unitOfMeasure in unitOfMeasureDictionary)
            {
                if (Regex.IsMatch(unit, unitOfMeasure.Value))
                    return unitOfMeasure.Key;
            }

            return UnitOfMeasure.Unit;
        }
    }

}
