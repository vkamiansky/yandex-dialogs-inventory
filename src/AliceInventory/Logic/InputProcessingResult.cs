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
        DeleteEntry,
        DeleteEntryCanceled,
        EntryNotFoundError,
        ClearRequested,
        Cleared,
        ListRead,
        ListEmpty,
        MailSent,
        MailSendError,
        Error,
        Parting
    }
}
