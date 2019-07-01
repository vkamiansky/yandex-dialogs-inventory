using System;

namespace AliceInventory.Logic
{
    public class ProcessingCommand
    {
        public ProcessingCommandType Type { get; set; }
        public object Data { get; set; }
    }
}
