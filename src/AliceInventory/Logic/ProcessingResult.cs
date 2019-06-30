using System;
using System.Globalization;

namespace AliceInventory.Logic
{
    public class ProcessingResult
    {
        public ProcessingResultType Type { get; set; }
        public object Data { get; set; }
        public CultureInfo CultureInfo { get; set; }

        public ProcessingResult() { }

        public ProcessingResult(ProcessingResultType type)
        {
            Type = type;
        }

        public ProcessingResult(ProcessingResultType type, object data)
        {
            Type = type;
            Data = data;
        }
    }
}
