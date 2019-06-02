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
        RequestMail,
        MailAdded,
        MailDeleted,
        HelpRequested,
        Error,
        ExitRequested,
    }
}
