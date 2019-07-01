using System;

namespace AliceInventory.Logic
{
    public enum InputProcessingCommand
    {
        SayHello,
        Accept,
        Decline,
        Cancel,
        Add,
        Delete,
        Clear,
        ReadList,
        SendMail,
        SendMailTo,
        AddMail,
        DeleteMail,
        RequestHelp,
        RequestExit,
        SayUnknownCommand,
    }
}
