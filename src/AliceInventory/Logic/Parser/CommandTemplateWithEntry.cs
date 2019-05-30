using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace AliceInventory.Logic.Parser
{
    public class CommandTemplateWithEntry : CommandTemplate
    {
        public CommandTemplateWithEntry(InputProcessingCommand command, string expression) : base(command, expression)
        { }

        protected override object GetObject(Match match, CultureInfo cultureInfo)
        {
            var groups = match.Groups;

            string name = groups.ContainsName(RegexHelper.EntryNameGroupName)
                ? groups[RegexHelper.EntryNameGroupName].Value
                : "noname";
            double count = groups.ContainsName(RegexHelper.EntryCountGroupName)
                ? Convert.ToDouble(groups[RegexHelper.EntryCountGroupName].Value, cultureInfo)
                : 1;
            UnitOfMeasure unit = groups.ContainsName(RegexHelper.EntryUnitGroupName)
                ? RegexHelper.ParseUnitOfMeasure(groups[RegexHelper.EntryUnitGroupName].Value)
                : UnitOfMeasure.Unit;

            return new Entry()
            {
                Name = name,
                Count = count,
                Unit = unit,
            };
        }
    }
}
