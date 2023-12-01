using Common.Extensions;

namespace Day1_ExtractNumbersFromText
{
    public class TextOnlyStringLineNumberFinder : IStringLineNumberFinder
    {
        public StringNumberFinderResult GetNumbers(string stringLine)
        {
            var indexFirst = (index: int.MaxValue, value: 0);
            var indexLast = (index: -1, value: 0);

            for (int i = 1; i <= 9; i++)
            {
                var word = i.ConvertDigitToEnlishLowerCaseWord(); 
                var firstIndexOfi = stringLine.IndexOf(word);
                var lastIndexOfi = stringLine.LastIndexOf(word);

                if (firstIndexOfi != -1 && firstIndexOfi < indexFirst.index)
                    indexFirst = (firstIndexOfi, i);

                if (lastIndexOfi > indexLast.index)
                    indexLast = (lastIndexOfi, i);
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
