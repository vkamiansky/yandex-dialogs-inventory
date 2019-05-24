using System;

namespace AliceInventory.Logic
{
    public enum InputProcessingResult
    {
        GreetingRequestDetected,
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
        HelpRequestDetected,
        Error,
        PartingRequestDetected
    }
}
