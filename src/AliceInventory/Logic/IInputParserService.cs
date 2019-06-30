using System;
using System.Globalization;
using AliceInventory.Logic.Parser;

namespace AliceInventory.Logic
{
    public interface IInputParserService
    {
        ParsedCommand ParseInput(string input, CultureInfo culture);
    }
}
