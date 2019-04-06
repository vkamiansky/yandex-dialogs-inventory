using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Linq.Expressions;

namespace AliceInventory
{
    public static class StringParser
    {
        public static string GetReply(string input = "Hello!")
        {
                return GetNumbers(input);
        }       

        public static string GetNumbers(string input)
        {
             // Split on one or more non-digit characters.
            string[] numbers = Regex.Split(input, @"\D+");
            var ret=new List<string>();
            foreach (string value in numbers)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    int i = int.Parse(value);
                    ret.Add($"Number: {i}, ");
                }
            }
            var defaultAnswer="no numbers in string";
            var count=ret.Any() ? string.Concat(ret.ToArray()) :defaultAnswer;
            return $"{System.DateTime.Now.ToLongTimeString()} {count}";
        }
    }
}