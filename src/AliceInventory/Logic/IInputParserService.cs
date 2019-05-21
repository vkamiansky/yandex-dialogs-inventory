using System;

namespace AliceInventory.Logic
{
    public interface IInputParserService
    {
        ProcessingCommand ParseInput(string input);
    }
}
