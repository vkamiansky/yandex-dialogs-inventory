using System;

namespace AliceInventory.Logic
{
    public interface IInventoryDialogService
    {
        ProcessingResult ProcessInput(string userId, string input);
    }
}
