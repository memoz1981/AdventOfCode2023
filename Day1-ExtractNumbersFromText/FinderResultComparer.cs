namespace Day1_ExtractNumbersFromText
{
    public class FinderResultComparer
    {
        public int CalculateInteger(IEnumerable<StringNumberFinderResult> results)
        {
            int firstValue = results.OrderBy(res => res.FirstIndex).First().FirstValue; 

            int lastValue = results.OrderByDescending(res => res.LastIndex).First().LastValue;

            return firstValue * 10 + lastValue; 
        }
    }
}
