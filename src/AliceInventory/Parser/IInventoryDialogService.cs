using System;

namespace ConsoleApp
{
    public interface IInventoryDialogService
    {
        ChatResponse ProcessInput(string input);
    }
}
