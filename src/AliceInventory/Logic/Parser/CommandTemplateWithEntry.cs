using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AliceInventory.Logic.Parser
{
    public class CommandTemplateWithEntry : CommandTemplate
    {
        private int NameGroupId;
        private int CountGroupId;
        private int UnitGroupId;

        public CommandTemplateWithEntry(string expression, int nameGroupId,
            int countGroupId, int unitGroupId) : base(expression)
        {
            NameGroupId = nameGroupId;
            CountGroupId = countGroupId;
            UnitGroupId = unitGroupId;
        }

        protected override object GetObject(Match match, CultureInfo cultureInfo)
        {
            return new Entry()
            {
                Name = NameGroupId > -1 ? match.Groups[NameGroupId + 1].Value : "noname",
                Count = CountGroupId > -1 ? Convert.ToDouble(match.Groups[CountGroupId + 1].Value, cultureInfo) : 1,
                Unit = UnitGroupId > -1 ? RegexHelper.ParseUnitOfMeasure(match.Groups[UnitGroupId + 1].Value) : UnitOfMeasure.Unit,
            };
        }
    }
}
