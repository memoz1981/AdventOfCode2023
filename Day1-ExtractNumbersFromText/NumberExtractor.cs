namespace Day1_ExtractNumbersFromText
{
    public abstract class NumberExtractor
    {
        private HashSet<IStringLineNumberFinder> _numberFinders;

        public NumberExtractor() => _numberFinders = ReturnFinders();
        public int ExtractNumber(string stringLine)
        {
            var finderResult = _numberFinders
                .Select(finder => finder.GetNumbers(stringLine));

            return CalculateInteger(finderResult); 
        }

        public abstract HashSet<IStringLineNumberFinder> ReturnFinders();

        private int CalculateInteger(IEnumerable<StringNumberFinderResult> results)
        {
            var firstValue = results
                .OrderBy(res => res.FirstIndex)
                .First()
                .FirstValue;

            var lastValue = results
                .OrderByDescending(res => res.LastIndex)
                .First()
                .LastValue;

            return firstValue * 10 + lastValue;
        }
    }
}
