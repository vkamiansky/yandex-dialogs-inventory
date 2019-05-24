using System;

namespace AliceInventory.Logic
{
    public enum InputProcessingResult
    {
        GreetingRequested,
        Added,
        AddCanceled,
        Deleted,
        ClearRequested,
        Cleared,
        ListRead,
        MailSent,
        HelpRequested,
        Error,
        ExitRequested
    }
}
