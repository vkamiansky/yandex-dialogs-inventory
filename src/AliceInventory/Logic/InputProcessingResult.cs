using System;

namespace AliceInventory.Logic
{
    public enum InputProcessingResult
    {
        GreetingRequestDetected,
        Added,
        AddCanceled,
        Deleted,
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
