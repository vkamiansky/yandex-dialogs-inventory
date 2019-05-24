using System;

namespace AliceInventory.Logic
{
    public enum InputProcessingResult
    {
        ShowGreeting,
        Added,
        AddCanceled,
        Deleted,
        DeleteCanceled,
        EntryNotFoundError,
        ClearRequested,
        Cleared,
        ListRead,
        ListEmpty,
        MailSent,
        ShowHelp,
        Error,
        ShowParting
    }
}
