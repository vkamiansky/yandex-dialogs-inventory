using System;

namespace AliceInventory.Logic
{
    public enum InputProcessingResult
    {
        Added,
        AddCanceled,
        Deleted,
        Cleared,
        ListRead,
        MailSent,
        Error,
    }
}
