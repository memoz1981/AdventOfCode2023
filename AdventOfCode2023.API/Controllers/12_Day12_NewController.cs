using Common.Services;
using Microsoft.AspNetCore.Mvc;

namespace AdventOfCode2023.API.Controllers
{
    [Route("day12_new")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class _12_Day12_NewController : ControllerBase
    {
        [HttpGet("exercise2")]
        public IActionResult Exercise2()
        {
            var lineReader = new StringLineReader();
            var lines = lineReader.ReadLines("data12.txt");

            int result = 0;
            foreach (var line in lines)
            {
                var symbols = line.Split(' ').First().Trim();
                var numbers = (line.Split(' ').Last().Trim()).Split(',').Select(m => int.Parse(m)).ToArray();

                var str = RemoveUnnecessaryDotsAndReturnCharArray(symbols, out var indices); 
                

                
            }

            return Ok(result);
        }

        private string MutateString(string symbols, int[] numbers)
        {
            var array = RemoveUnnecessaryDotsAndReturnCharArray(symbols, out var indices);
            var dic = ReturnIndexByNumber(symbols);

            //first round to see if max and maxes are matching - if yes interpolate regions... 
            MutateByMaximums(array, dic, numbers, indices); 

            return new string(array);
        }

        private char[] RemoveUnnecessaryDotsAndReturnCharArray(string symbols, out List<int> dotIndexes)
        {
            int index = 0;
            dotIndexes = new(); 
            var list = new List<char>();

            while (symbols[index] == '.')
                index++;

            bool dotFound = false;

            for (int i = index, j=index; i < symbols.Length; i++)
            {
                if (symbols[i] != '.' || !dotFound)
                {
                    list.Add(symbols[i]);
                    dotFound = symbols[i] == '.';
                    if (dotFound)
                        dotIndexes.Add(j);
                    j++;
                    continue; 
                }
            }

            if (list.Last() == '.')
            {
                list.RemoveAt(list.Count - 1);
                dotIndexes.RemoveAt(dotIndexes.Count - 1); 
            }

            return list.ToArray();
        }

        private void MutateByMaximums(char[] array, Dictionary<int, List<int>> dic, int[] numbers, List<int> dotIndices)
        {
            var numbersSorted = numbers.Where(m => m != -1)
                .Select((num, idx) => new {idx = idx, num = num })
                .GroupBy(m => m.num)
                .ToDictionary(m => m.Key, m => m.Select(r => r.idx).ToList());
            
            var mutatedArray = array.Select(m => m).ToList();

            foreach (var num in numbersSorted.OrderByDescending(m => m.Key))
            {
                var indexes = num.Value;

                var dictionaryElements = 1; 
            }
            
            
            foreach (var element in dic.OrderByDescending(m => m.Key))
            {
                var indexesForHashes = element.Value;

                if (indexesForHashes.Count == 1)
                {
                    if (element.Key == numbersSorted.OrderByDescending(m => m.Key).First().Key)
                    {
                        var indexes = numbersSorted[element.Key];

                        if (indexes.Count == 1) //there is only one max index
                        {
                            var start = indexesForHashes.First();
                            var end = start + indexesForHashes.First() - 1;

                            numbersSorted.Remove(element.Key);

                            
                            continue;

                        }

                       
                    }




                }
                
                
            }
        }

        private Dictionary<int, List<int>> ReturnIndexByNumber(string line)
        {
            var dict = new Dictionary<int, List<int>>();
            bool hashFound = false;
            int hashIndex = -1; 
            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] == '#')
                {
                    if (!hashFound)
                    {
                        hashFound = true;
                        hashIndex = i;
                    }
                }
                else
                {
                    if (hashFound)
                    {
                        var count = i - hashIndex;
                        if (dict.TryGetValue(count, out var element))
                        {
                            element.Add(hashIndex);
                        }
                        else
                        {
                            dict[count] = new() { hashIndex };
                        }
                        hashIndex = -1;
                        hashFound = false; 
                    }
                }
            }
            return dict; 
        }

        private static bool CanNumberBeUsedInString(string symbol, int number, int startIndex, int endIndex, 
            out int nextIndex)
        {
            nextIndex = startIndex + number + 1; //we want to add the space (.)
            if (startIndex == endIndex || endIndex >= symbol.Length)
                return false; 
            
            if (startIndex + number > symbol.Length)
                return false;

            if (startIndex + number  == symbol.Length)
            {
                return true; 
            }

            if (symbol[startIndex + number] == '#')
                return false;

            return true; 
        }
    }
}
