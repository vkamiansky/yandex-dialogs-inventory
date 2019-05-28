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
        HelpRequested,
        Error,
        ExitRequested,
    }
}
