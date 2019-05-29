using System;
using System.Globalization;

namespace AliceInventory.Logic
{
    public class ProcessingResult
    {
        public InputProcessingResult Result { get; set; }
        public object Data { get; set; }
        public CultureInfo CultureInfo { get; set; }
    }
}
