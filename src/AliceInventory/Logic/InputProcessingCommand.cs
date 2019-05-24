using System;

namespace AliceInventory.Logic
{
    public enum InputProcessingCommand
    {
        RequestGreeting,
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
    }
}
