using System;
using System.Globalization;

namespace AliceInventory.Logic
{
    public class ProcessingResult
    {
        public static implicit operator ProcessingResult(Error e)
        {
            return new ProcessingResult(e);
        }
        public static implicit operator ProcessingResult(Exception e)
        {
            return new ProcessingResult(e);
        }
        public static implicit operator ProcessingResult(ProcessingResultType type)
        {
            return new ProcessingResult(type);
        }
        public ProcessingResultType Type { get; }
        public object Data { get; }
        public Error Error { get; }
        public Exception Exception { get; }
        public CultureInfo CultureInfo { get; set; }
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
