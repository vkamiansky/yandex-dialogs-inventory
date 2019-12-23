using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AliceInventory.Logic.Parser
{
    public static class RegexHelper
    {
        public const string EntryNameGroupName = "g1";
        public const string EntryQuantityGroupName = "g2";
        public const string EntryUnitGroupName = "g3";
        public const string EmailGroupName = "g4";
        public const string AddWord =
            @"(?:(?:за|в|над?|п(?:о|ри)|до)?(?:плюс|с(?:оедин)|ки|лож|мест|бав)(?:н)?(?:л|им?|ь)?(?:ай|яй|ть|те|\sка)?)";
        public const string DeleteWord =
            @"(?:(?:вы|у)?(?:дал|бер|брос|бра|тащ|сотр|изъя|ничтож|стер|стир|таск)(?:ива|ова|ева|ыва)?(?:ай|у|е|и|й|ь)?(?:ть)?(?:те|\sка)?)";
        public const string ClearWord =
            @"(?:(?:о|вы)чист(?:и|(?:ит)?ь))";
        public const string HelloPattern =
            @"(?:привет(?:ик|ствую)?|здравствуй(?:те)?|хай|хеллоу)";
        public const string AcceptWord =
            @"(?:(?:ну\s)?да(?:вай)?|конечно|несомненно|точно|именно|верно|хочу|подтвер(?:дить|ждаю))";
        public const string DeclineWord =
            @"(?:не(?:т|\sнадо)?|от(?:вали|стань))";
        public const string CancelWord =
            @"(?:отмен(?:а|и(?:ть)?|я(?:й|ю)))";
        public const string MoreWord =
            @"(?:ещ(?:е|ё))";        
        public const string HelpWord =
            @"(?:помо(?:ги(?:те)?|щь)|хелп|спас(?:и|а(?:й(?:те)?))|выручай(?:те)?|что ты (?:уме|мож)ешь\??)";
        public const string ReadWord =
            @"(?:пока(?:жи|зать)|выведи|(?:за|про)?читай)";
        public const string SendWord =
            @"(?:(?:отправ(?:ь(?:те)?|ить|ляй)|вы(?:шли|слать)|(?:пере|по)(?:шли|слать)|ски(?:дывай|нь))(?:\sка)?)";
        public const string ExitWord =
            @"(?:п(?:ока|рощай)|выход|хватит|закончить)";
        public const string ListWord =
            @"(?:вс(?:е|ё)|спис(?:ок|ке)|опись|(?:от|у)ч(?:е|ё)т|итог(?:е|о)?|результате?)";
        public const string MailWord =
            @"(?:(?:мо(?:я|й|ю|ё|е)\s)?(?:почт(?:а|у|ой)|(?:и|е)?мейл|e?mail|мыло))";

        /// <summary>
        /// CanBeIgnoreWordPattern - шаблон слов, которые в большинстве случаев могут быть проигнорированы.
        /// "Давай добавим 5 яблок" - слово "давай" можно проигнорировать,
        /// но если же у нас только слово "Давай", то это можно воспринять, как согласие.
        /// </summary>
        public const string CanBeIgnoreWordPattern = @"(?:ещ(?:е|ё)|конечно|давай)";

        /// <summary>
        /// IgnoreWord - слова, которые должны быть проигнорированы.
        /// </summary>
        private const string IgnoreWordPattern = @"(?:ну)";
        public static readonly Regex IgnoreWord = new Regex(IgnoreWordPattern);

        /// <summary>
        /// MultipleSpaces - шаблон нескольких пробелов подряд
        /// </summary>
        private const string MultipleSpacesPattern = @"(?:[ ]{2,})";
        public static readonly Regex MultipleSpaces = new Regex(MultipleSpacesPattern);

        public const string UnitPattern = @"шт(?:ук|ука|уки|уку|уковин)?|единиц(?:а|у|ы)?";
        public const string KgPattern = @"к(?:г|илограмм(?:а|ов)?)";
        public const string LiterPattern = @"л(?:итр(?:а|ов)?)?";
        public const string UnitOfMeasurePattern = UnitPattern + "|" + KgPattern + "|" + LiterPattern;
        public static readonly string EntryUnit = AddGroupName(EntryUnitGroupName, UnitOfMeasurePattern);
        public const string EntryNamePattern = @".{2}.+?";
        public static readonly string EntryName = AddGroupName(EntryNameGroupName, EntryNamePattern);
        public const string Number = @"-?\d+(?:(?:\.|,)\d+)?";
        public static readonly string EntryQuantity = AddGroupName(EntryQuantityGroupName, Number);
        public const string EmailPattern = @"[a-z0-9!#$%&'*+\=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+\=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?";
        public static readonly string Email = AddGroupName(EmailGroupName, EmailPattern);
        private static readonly Dictionary<UnitOfMeasure, string> UnitOfMeasureDictionary;

        static RegexHelper()
        {
            UnitOfMeasureDictionary = new Dictionary<UnitOfMeasure, string>
            {
                [UnitOfMeasure.Unit] = UnitPattern,
                [UnitOfMeasure.Kg] = KgPattern,
                [UnitOfMeasure.L] = LiterPattern
            };
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