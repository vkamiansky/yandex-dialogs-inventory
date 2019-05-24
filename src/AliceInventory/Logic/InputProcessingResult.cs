using System;

namespace AliceInventory.Logic
{
    public enum InputProcessingResult
    {
        GreetingRequestDetected,
        Added,
        AddCanceled,
        Deleted,
        ClearRequested,
        Cleared,
        ListRead,
        MailSent,
        HelpRequestDetected,
        Error,
        PartingRequestDetected
    }
}
