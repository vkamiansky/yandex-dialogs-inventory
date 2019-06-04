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
        RequestHelp,
        RequestExit,
        SayUnknownCommand
    }
}
