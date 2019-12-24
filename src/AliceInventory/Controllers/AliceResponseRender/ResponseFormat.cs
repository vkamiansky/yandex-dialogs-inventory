namespace AliceInventory.Controllers.AliceResponseRender
{
    public enum ResponseFormat
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
        ItemRead,
        EmptyListRead,
        EmptyItemRead,
        MailSent,
        MailRequest,
        MailAdded,
        MailDeleted,
        MailIsEmpty,
        HelpRequested,
        Error,
        ExitRequested,
        EntryNotFound,
        NotEnoughEntryToDelete
    }
}
