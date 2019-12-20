namespace AliceInventory.Logic
{
    public interface IInventoryDialogService
    {
        ProcessingResult ProcessInput(string userId, UserInput input);
    }
}
