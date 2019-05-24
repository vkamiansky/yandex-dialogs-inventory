using System;

namespace AliceInventory.Logic
{
    public enum InputProcessingResult
    {
        Greeting,
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
        Error,
        Parting
    }
}
