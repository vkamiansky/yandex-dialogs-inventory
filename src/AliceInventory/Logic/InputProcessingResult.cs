using System;

namespace AliceInventory.Logic
{
    public enum InputProcessingResult
    {
        GreetingRequested,
        Declined,
        Added,
        AddCanceled,
        Deleted,
        DeleteCanceled,
        ClearRequested,
        Cleared,
        ListRead,
        MailSent,
        RequestedMail,
        MailAdded,
        MailDeleted,
        HelpRequested,
        Error,
        ExitRequested,
    }
}
