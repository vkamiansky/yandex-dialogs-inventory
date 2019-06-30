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

        public CommandTemplateWithSingleEntry(ParsedCommandType commandType, params string[] regexParts) : base(commandType, regexParts)
        {
            var groups = this.Regex.GetGroupNames();

            _nameGroupId = Array.IndexOf(groups, RegexHelper.EntryNameGroupName);
            _countGroupId = Array.IndexOf(groups, RegexHelper.EntryCountGroupName);
            _unitGroupId= Array.IndexOf(groups, RegexHelper.EntryUnitGroupName);
        }

        protected override object GetObject(Match match, CultureInfo cultureInfo)
        {
            var groups = match.Groups;
            var entry = new ParsedSingleEntry();

            if (_nameGroupId > -1) entry.Name = groups[_nameGroupId].Value;
            if (_countGroupId > -1) entry.Count = Convert.ToDouble(groups[_countGroupId].Value, cultureInfo);
            if (_unitGroupId > -1) entry.Unit = RegexHelper.ParseUnitOfMeasure(groups[_unitGroupId].Value);

            return entry;
        }
    }
}
