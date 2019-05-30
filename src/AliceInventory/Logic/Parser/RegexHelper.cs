using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AliceInventory.Logic.Parser
{
    public static class RegexHelper
    {
        public const string EntryNameGroupName = "ename";
        public const string EntryCountGroupName = "enumber";
        public const string EntryUnitGroupName = "eunit";
        public const string EmailGroupName = "email";

        public const string AddPattern =
            @"(?:за|в|над?|п(?:о|ри)|до)?(?:плюс|с(?:оедин|ун)|пих|ки|лож|мест|бав)(?:н)?(?:л|и|ь)?(?:ай|яй|ть|те|ка)?";
        public const string DeletePattern =
            @"(?:вы|у)?(?:дал|бер|сун|брос|бра|тащ|сотр|изъя|ничтож|стер|стир|таск)(?:ива|ова|ева|ыва)?(?:ай|у|е|и|й|ь)?(?:ть)?(?:те|ка)?";
        public const string ClearPattern =
            @"(?:о|вы)чист(?:и|(?:ит)?ь)(?:\s(?:вс(?:е|ё)|список|рюкзак|инвентарь))?";
        public const string HelloPattern =
            @"привет(?:ик|ствую)?|здравствуй(?:те)?|хай|хеллоу";
        public const string AcceptPattern =
            @"да(?:вай)?|конечно|несомненно|точно|именно|верно|хочу|подтвер(?:дить|ждаю)";
        public const string DeclinePattern =
            @"не(?:т|\sнадо)?|от(?:вали|стань)";
        public const string CancelPattern =
            @"отмен(?:а|и(?:ть)?|я(?:й|ю))";
        public const string HelpPattern =
            @"помо(?:ги(?:те)?|щь)|хелп|спас(?:и|а(?:й(?:те)?))|выручай(?:те)?|что ты (?:уме|мож)ешь\??";
        public const string ReadListPattern =
            @"(?:п(?:ока(?:жи|зать)|родемонстрируй)|расскажи)(?:\s(?:вс(?:е|ё)|список|рюкзак|инвентарь))?";
        public const string SendMailOnPattern =
            @"(?:отправ(?:ь(?:те)?|ить|ляй)|вы(?:шли|слать)|по(?:шли|слать))(?:\sна)?";
        public const string ExitPattern =
            @"п(?:ока|рощай)|выход|хватит";
        
        public const string UnitPattern = @"штук(?:а|и|у|овин)?|единиц(?:а|у|ы)?";
        public const string KgPattern = @"к(?:г|илограмм(?:а|ов)?)";
        public const string LiterPattern = @"литр(?:а|ов)?";
        public const string UnitOfMeasurePattern = UnitPattern + "|" + KgPattern + "|" + LiterPattern;
        public static readonly string EntryUnit = AddGroupName(EntryUnitGroupName, UnitOfMeasurePattern);
        public const string EntryNamePattern = @"[а-яА-ЯёЁ\d\s]{3,}";
        public static readonly string EntryName = AddGroupName(EntryNameGroupName, EntryNamePattern);
        public const string Number = @"-?\d+(?:(?:\.|,)\d+)?";
        public static readonly string EntryCount = AddGroupName(EntryCountGroupName, Number);
        public const string EmailPattern = @"[a-z0-9!#$%&'*+\=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+\=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?";
        public static readonly string Email = AddGroupName(EmailGroupName, EmailPattern);
        
        private static readonly Dictionary<UnitOfMeasure, string> UnitOfMeasureDictionary;
        static RegexHelper()
        {
            UnitOfMeasureDictionary = new Dictionary<UnitOfMeasure, string>();
            UnitOfMeasureDictionary[UnitOfMeasure.Unit] = UnitPattern;
            UnitOfMeasureDictionary[UnitOfMeasure.Kg] = KgPattern;
            UnitOfMeasureDictionary[UnitOfMeasure.L] = LiterPattern;
        }
       
        private static string AddGroupName(string name, string pattern)
        {
            return $"(?<{name}>{pattern})";
        }

        public static UnitOfMeasure ParseUnitOfMeasure(string unit)
        {
            foreach (var unitOfMeasure in UnitOfMeasureDictionary)
            {
                if (Regex.IsMatch(unit, unitOfMeasure.Value))
                    return unitOfMeasure.Key;
            }

            return UnitOfMeasure.Unit;
        }
    }

}
