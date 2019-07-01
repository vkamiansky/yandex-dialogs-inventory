using System;
using System.Globalization;

namespace AliceInventory.Logic
{
    public class ProcessingResult
    {
        public ProcessingResultType Type { get; set; }
        public object Data { get; set; }
        public Error Error { get; set; }

        public Exception Exception { get; set; }
        public CultureInfo CultureInfo { get; private set; }

        public ProcessingResult()
        { }
        public ProcessingResult(ProcessingResultType type)
        { Type = type; }
        public ProcessingResult(ProcessingResultType type, object data)
        { Type = type; Data = data; }
        public ProcessingResult(Exception exception)
        { Type = ProcessingResultType.Exception; Exception = exception; }
        public ProcessingResult(Error error)
        { Type = ProcessingResultType.Error; Error = error; }
    }
}
