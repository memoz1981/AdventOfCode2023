namespace Day1_ExtractNumbersFromText
{
    public class NumbersOnlyStringLineNumberFinder : IStringLineNumberFinder
    {
        public StringNumberFinderResult GetNumbers(string stringLine)
        {
            (int, int)? first = null;
            (int, int)? last = null;
            int index = 0;

            foreach (var ch in stringLine)
            {
                if (ch >= 48 && ch <= 57)
                {
                    first = first ?? (index, ch - 48);
                    last = (index, ch - 48);
                }
                index++;
            }

            if (first == null || last == null)
                throw new InvalidOperationException(); 

            return new StringNumberFinderResult(
                FirstIndex: first.Value.Item1, 
                FirstValue: first.Value.Item2,
                LastIndex: last.Value.Item1,
                LastValue: last.Value.Item2
                ); 
        }
    }
}
