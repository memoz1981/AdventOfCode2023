namespace Common.Extensions
{
    public static class IntegerExtensions
    {
        public static string ConvertDigitToEnlishLowerCaseWord(this int i)
        {
            var englishDigitWords = new[] 
            { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };
            
            if (i < 0 || i > 9)
                throw new ArgumentException("Only for digits...");

            return englishDigitWords[i]; 
        }
    }
}
