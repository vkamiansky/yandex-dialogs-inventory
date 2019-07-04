using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AliceInventory.Logic.Parser
{
    public enum ParsedPhraseType
    {
        Hello,
        Accept,
        Decline,
        Cancel,
        Add,
        Delete,
        More,
        Clear,
        ReadList,
        SendMail,
        Mail,
        DeleteMail,
        Help,
        Exit,
        UnknownCommand
    }
}
