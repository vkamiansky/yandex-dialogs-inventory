using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace AliceInventory.Logic.Parser
{
    public class CommandTemplateWithSingleEntry : CommandTemplate
    {
        private int nameGroupId;
        private int countGroupId;
        private int unitGroupId;

        public CommandTemplateWithSingleEntry(InputProcessingCommand command, string expression) : base(command, expression)
        {
            var groups = this.Regex.GetGroupNames();

            nameGroupId = Array.IndexOf(groups, RegexHelper.EntryNameGroupName);
            countGroupId = Array.IndexOf(groups, RegexHelper.EntryCountGroupName);
            unitGroupId= Array.IndexOf(groups, RegexHelper.EntryUnitGroupName);
        }

        protected override object GetObject(Match match, CultureInfo cultureInfo)
        {
            var groups = match.Groups;
            
            var name = nameGroupId > -1 ? groups[nameGroupId].Value : "noname";
            var count = countGroupId > -1 ? Convert.ToDouble(groups[RegexHelper.EntryCountGroupName].Value, cultureInfo) : 1;
            var unit = unitGroupId > -1 ? RegexHelper.ParseUnitOfMeasure(groups[RegexHelper.EntryUnitGroupName].Value) : UnitOfMeasure.Unit;

            return new SingleEntry()
            {
                Name = name,
                Count = count,
                Unit = unit,
            };
        }
    }
}
