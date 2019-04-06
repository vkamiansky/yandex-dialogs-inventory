using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace AliceInventory
{
    public static class StringParser
    {
        public static string GetReply(string input = "Hello!")
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
            return ret.Any()? string.Concat(ret.ToArray()) : "no numbers in string";
        }       
    }
}