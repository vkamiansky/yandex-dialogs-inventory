using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace AliceInventory.Logic.Parser
{
    public class CommandTemplateWithSingleEntry : CommandTemplate
    {
        private readonly int _nameGroupId;
        private readonly int _countGroupId;
        private readonly int _unitGroupId;

        public CommandTemplateWithSingleEntry(InputProcessingCommand command, string expression) : base(command, expression)
        {
            var groups = this.Regex.GetGroupNames();

            _nameGroupId = Array.IndexOf(groups, RegexHelper.EntryNameGroupName);
            _countGroupId = Array.IndexOf(groups, RegexHelper.EntryCountGroupName);
            _unitGroupId= Array.IndexOf(groups, RegexHelper.EntryUnitGroupName);
        }

        protected override object GetObject(Match match, CultureInfo cultureInfo)
        {
            var groups = match.Groups;
            
            var name = _nameGroupId > -1 ? groups[_nameGroupId].Value : "noname";
            var count = _countGroupId > -1 ? Convert.ToDouble(groups[RegexHelper.EntryCountGroupName].Value, cultureInfo) : 1;
            var unit = _unitGroupId > -1 ? RegexHelper.ParseUnitOfMeasure(groups[RegexHelper.EntryUnitGroupName].Value) : UnitOfMeasure.Unit;

            return new SingleEntry()
            {
                Name = name,
                Count = count,
                Unit = unit,
            };
        }
    }
}
