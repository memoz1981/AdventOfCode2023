namespace Day1_ExtractNumbersFromText
{
    public class DigitsAndTextNumberExtractor : NumberExtractor
    {
        public override HashSet<IStringLineNumberFinder> ReturnFinders()
            => new() 
            { 
                new NumbersOnlyStringLineNumberFinder(), 
                new TextOnlyStringLineNumberFinder() 
            };
    }
}
