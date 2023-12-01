using Common.Extensions;

namespace Day1_ExtractNumbersFromText
{
    public class TextOnlyStringLineNumberFinderFirstSolution : IStringLineNumberFinder
    {
        public StringNumberFinderResult GetNumbers(string stringLine)
        {
            var firstIndexes = new int[10];
            var lastIndexes = new int[10];
            for (int i = 1; i <= 9; i++)
            {
                firstIndexes[i] = stringLine.IndexOf(i.ConvertDigitToEnlishLowerCaseWord());
                lastIndexes[i] = stringLine.LastIndexOf(i.ConvertDigitToEnlishLowerCaseWord());
            }

            var indexFirst = (index: int.MaxValue, value: 0);
            var indexLast = (index: -1, value: 0);

            for (int i = 1; i <= 9; i++)
            {
                if (firstIndexes[i] != -1 && firstIndexes[i] < indexFirst.index)
                    indexFirst = (firstIndexes[i], i);

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
