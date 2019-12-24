namespace AliceInventory.Logic
{
    public enum ProcessingResultType
    {
        GreetingRequested,
        Declined,
        InvalidCount,
        Added,
        AddCanceled,
        Deleted,
        DeleteCanceled,
        ClearRequested,
        Cleared,
        ListRead,
        ItemRead,
        MailSent,
        RequestedMail,
        MailAdded,
        MailDeleted,
        HelpRequested,
        Error,
        Exception,
        ExitRequested,
    }
}