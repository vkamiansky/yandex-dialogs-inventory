using System;
using System.Globalization;

namespace AliceInventory.Logic
{
    public interface IInputParserService
    {
        ProcessingCommand ParseInput(string input, CultureInfo culture);
    }
}
