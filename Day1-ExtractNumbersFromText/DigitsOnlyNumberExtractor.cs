namespace Day1_ExtractNumbersFromText
{
    public class DigitsOnlyNumberExtractor : NumberExtractor
    {
        public override HashSet<IStringLineNumberFinder> ReturnFinders()
            => new() { new NumbersOnlyStringLineNumberFinder() };
    }
}
