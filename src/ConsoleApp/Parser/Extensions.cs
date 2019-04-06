
    public static class Extensions
    {
        public static bool IsNumber(this string input)
        {
            return int.TryParse(input,out int i);
        }
        public static bool IsUnit(this string input)
        {
            //TODO: check!
            return string.Equals(input,"штука");
        }
    }