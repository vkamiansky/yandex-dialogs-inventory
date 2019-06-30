using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AliceInventory.Logic.Parser
{
    public enum ParsedCommandType
    {
        SayHello,
        Accept,
        Decline,
        Cancel,
        Add,
        Delete,
        More,
        Clear,
        ReadList,
        SendMail,
        SendMailTo,
        AddMail,
        DeleteMail,
        RequestHelp,
        RequestExit,
        SayUnknownCommand
    }
}
