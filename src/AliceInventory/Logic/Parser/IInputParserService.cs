using AliceInventory.Logic.Parser;

namespace AliceInventory.Logic
{
    public interface IInputParserService
    {
        ParsedCommand ParseInput(UserInput input);
    }
}
