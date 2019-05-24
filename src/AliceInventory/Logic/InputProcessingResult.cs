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
        DeleteEntryFromList,
        DeleteEntryFromListCanceled,
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
