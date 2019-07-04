using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace AliceInventory.Logic.Parser
{
    public class CommandTemplateWithEntry : CommandTemplate
    {
        private readonly int _nameGroupId;
        private readonly int _quantityGroupId;
        private readonly int _unitGroupId;

        public CommandTemplateWithEntry(ParsedPhraseType phraseType, params string[] regexParts) : base(phraseType, regexParts)
        {
            var groups = this.Regex.GetGroupNames();

            _nameGroupId = Array.IndexOf(groups, RegexHelper.EntryNameGroupName);
            _quantityGroupId = Array.IndexOf(groups, RegexHelper.EntryQuantityGroupName);
            _unitGroupId= Array.IndexOf(groups, RegexHelper.EntryUnitGroupName);
        }

        protected override object GetObject(Match match, CultureInfo cultureInfo)
        {
            var groups = match.Groups;
            var entry = new ParsedEntry();

            if (_nameGroupId > -1) entry.Name = groups[_nameGroupId].Value;
            if (_quantityGroupId > -1) entry.Quantity = Convert.ToDouble(groups[_quantityGroupId].Value, cultureInfo);
            if (_unitGroupId > -1) entry.Unit = RegexHelper.ParseUnitOfMeasure(groups[_unitGroupId].Value);

            return entry;
        }
    }
}
