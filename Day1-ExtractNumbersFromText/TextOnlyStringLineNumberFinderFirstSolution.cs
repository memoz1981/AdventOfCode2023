namespace Day1_ExtractNumbersFromText
{
    public class TextOnlyStringLineNumberFinderFirstSolution : IStringLineNumberFinder
    {
        public StringNumberFinderResult GetNumbers(string stringLine)
        {
            var firstIndexes = new int[10];
            firstIndexes[1] = stringLine.IndexOf("one");
            firstIndexes[2] = stringLine.IndexOf("two");
            firstIndexes[3] = stringLine.IndexOf("three");
            firstIndexes[4] = stringLine.IndexOf("four");
            firstIndexes[5] = stringLine.IndexOf("five");
            firstIndexes[6] = stringLine.IndexOf("six");
            firstIndexes[7] = stringLine.IndexOf("seven");
            firstIndexes[8] = stringLine.IndexOf("eight");
            firstIndexes[9] = stringLine.IndexOf("nine");

            var lastIndexes = new int[10];
            lastIndexes[1] = stringLine.LastIndexOf("one");
            lastIndexes[2] = stringLine.LastIndexOf("two");
            lastIndexes[3] = stringLine.LastIndexOf("three");
            lastIndexes[4] = stringLine.LastIndexOf("four");
            lastIndexes[5] = stringLine.LastIndexOf("five");
            lastIndexes[6] = stringLine.LastIndexOf("six");
            lastIndexes[7] = stringLine.LastIndexOf("seven");
            lastIndexes[8] = stringLine.LastIndexOf("eight");
            lastIndexes[9] = stringLine.LastIndexOf("nine");

            var indexFirst = (index: int.MaxValue, value: 0);
            for (int i = 1; i < 10; i++)
            {
                if (firstIndexes[i] != -1 && firstIndexes[i] < indexFirst.index)
                    indexFirst = (firstIndexes[i], i);
            }

            var indexLast = (index: -1, value: 0);
            for (int i = 1; i < 10; i++)
            {
                if (lastIndexes[i] > indexLast.index)
                    indexLast = (lastIndexes[i], i);
            }

            return new StringNumberFinderResult(
                FirstIndex: indexFirst.index,
                FirstValue: indexFirst.value,
                LastIndex: indexLast.index,
                LastValue: indexLast.value
                );
        }
    }
}
