using System;

namespace AliceInventory.Logic
{
    public class ProcessingCommand
    {
        public InputProcessingCommand Command { get; set; }
        public object Data { get; set; }
    }
}
