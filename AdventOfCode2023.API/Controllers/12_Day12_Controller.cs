using Common.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace AdventOfCode2023.API.Controllers
{
    [Route("day12")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class _12_Day12_Controller : ControllerBase
    {
        [HttpGet("exercise1")]
        public IActionResult Exercise1()
        {
            var lineReader = new StringLineReader();
            var lines = lineReader.ReadLines("data12.txt");

            int result = 0; 
            foreach (var line in lines)
            {
                var symbols = line.Split(' ').First().Trim();
                var numbers = (line.Split(' ').Last().Trim()).Split(',').Select(m => int.Parse(m)).ToList();
                var combinations = GetAllCombinationStrings(symbols);

                var count = combinations.Count(c => CombinationFits(c, numbers));

                result += count; 
            }

            return Ok(result);
        }

        [HttpGet("exercise2")]
        public IActionResult Exercise2()
        {
            var lineReader = new StringLineReader();
            var lines = lineReader.ReadLines("data12.txt");

            long result = 0;
            var builder = new StringBuilder();
            int index = 0; 
            foreach (var line in lines)
            {
                var symbols = line.Split(' ').First().Trim();
                bool dotStarted = true;
                //List<char> symbolNew = new();
                //foreach (char c in symbols)
                //{
                //    if (c == '.')
                //    {
                //        if (!dotStarted)
                //            symbolNew.Add(c);
                //        dotStarted = true;

                //    }
                //    else
                //    {
                //        symbolNew.Add(c);
                //        dotStarted = false; 
                //    }

                //}

                //symbols = new string(symbols.ToArray()); 

                var symbolsMutated = $"{symbols}?{symbols}?{symbols}?{symbols}?{symbols}";

                var numbersAll = new List<int>();
                var numbers = (line.Split(' ').Last().Trim()).Split(',').Select(m => int.Parse(m)).ToList();

                for (int i = 0; i < 5; i++)
                    numbersAll.AddRange(numbers);

                //var numberString = BuildStringFromNumbers(numbersAll);

                //builder.AppendLine($"Line index : {index}");
                //builder.AppendLine($"Line characters : {symbolsMutated}");
                //builder.AppendLine($"Line numbers string : {numberString}");
                //builder.AppendLine($"Line chararacter Length : {symbolsMutated.Length}");
                //builder.AppendLine($"Line numbers length : {numberString.Length}");
                //builder.AppendLine($"Characters splitted by dot : {symbolsMutated.Split('.').Count()}");
                //builder.AppendLine($"Numbers count : {numbersAll.Count}");
                //builder.AppendLine("");
                //builder.AppendLine("***");
                //builder.AppendLine("");

                var comb = GetAllCombinationStrings(symbolsMutated);

                var count = comb.Count(c => CombinationFits(c, numbersAll));

                result += count;
                index++;


            }

            return Ok(result);
        }

        private string BuildStringFromNumbers(List<int> numbers)
        {
            var builder = new StringBuilder(); 
            for(int i=0; i<numbers.Count;i++)
            {
                for(int j = 0;j<numbers[i];j++)
                    builder.Append("#");

                if(i<numbers.Count - 1)
                    builder.Append(".");
            }

            return builder.ToString();
        }





        private HashSet<(int, int, int, int, int)> ReturnPotentialElements(int sum, int count,
            Dictionary<int, List<int>> counts, Dictionary<int, List<int>> sums)
        {
            var distinctCount = new HashSet<(int, int, int, int, int)>(); 
            for (int i = 0; i < counts.Count; i++)
                for (int j = 0; j < counts.Count; j++)
                    for (int k = 0; k < counts.Count; k++)
                        for (int l = 0; l < counts.Count; l++)
                            for (int m = 0; m < counts.Count; m++)
                            {
                                if (counts.ElementAt(i).Key + counts.ElementAt(j).Key + counts.ElementAt(k).Key
                                    + counts.ElementAt(l).Key + counts.ElementAt(m).Key == count)
                                {
                                    distinctCount.Add((i, j, k, l, m)); 
                                }
                            }

            var result = new HashSet<(int, int, int, int, int)>(); 
            foreach (var element in distinctCount)
            {
                var itemsi = counts.ElementAt(element.Item1).Value;
                var itemsj = counts.ElementAt(element.Item2).Value;
                var itemsk = counts.ElementAt(element.Item3).Value;
                var itemsl = counts.ElementAt(element.Item4).Value;
                var itemsm = counts.ElementAt(element.Item5).Value;

                var potentials = ReturnPotentials(itemsi, itemsj, itemsk, itemsl, itemsm);
                foreach(var potential in potentials)
                    result.Add(potential); 
            }

            var distinctSum = new HashSet<(int, int, int, int, int)>();
            for (int i = 0; i < sums.Count; i++)
                for (int j = 0; j < sums.Count; j++)
                    for (int k = 0; k < sums.Count; k++)
                        for (int l = 0; l < sums.Count; l++)
                            for (int m = 0; m < sums.Count; m++)
                            {
                                if (sums.ElementAt(i).Key + sums.ElementAt(j).Key + sums.ElementAt(k).Key
                                    + sums.ElementAt(l).Key + sums.ElementAt(m).Key == sum)
                                {
                                    distinctSum.Add((i, j, k, l, m));
                                }
                            }

            var resultNew = new HashSet<(int, int, int, int, int)>(); 
            foreach (var element in distinctSum)
            {
                var itemsi = sums.ElementAt(element.Item1).Value;
                var itemsj = sums.ElementAt(element.Item2).Value;
                var itemsk = sums.ElementAt(element.Item3).Value;
                var itemsl = sums.ElementAt(element.Item4).Value;
                var itemsm = sums.ElementAt(element.Item5).Value;

                var potentials = ReturnPotentials(itemsi, itemsj, itemsk, itemsl, itemsm);
                foreach (var item in potentials)
                {
                    if(result.Contains(item))
                        resultNew.Add(item);
                }
                    
            }
            return resultNew; 
        }

        private HashSet<(int, int, int, int, int)> ReturnPotentials(List<int> itemsi, List<int> itemsj, 
            List<int> itemsk, List<int> itemsl, List<int> itemsm)
        {
            var result = new HashSet<(int, int, int, int, int)>();
            for (int i = 0; i < itemsi.Count; i++)
                for (int j = 0; j < itemsj.Count; j++)
                    for (int k = 0; k < itemsk.Count; k++)
                        for (int l = 0; l < itemsl.Count; l++)
                            for (int m = 0; m < itemsm.Count; m++)
                            {
                                result.Add((i, j, k, l, m));
                            }

            return result; 
        }

        private List<string> GetRepeatCombinations(List<string> combinations)
        {
            var mutatedList = new List<string>();

            foreach (var comb in combinations)
            {
                mutatedList.Add($"{comb}.{comb}.{comb}.{comb}.{comb}");
                mutatedList.Add($"{comb}.{comb}.{comb}.{comb}#{comb}");
                mutatedList.Add($"{comb}.{comb}.{comb}#{comb}.{comb}");
                mutatedList.Add($"{comb}.{comb}.{comb}#{comb}#{comb}");
                mutatedList.Add($"{comb}.{comb}#{comb}.{comb}.{comb}");
                mutatedList.Add($"{comb}.{comb}#{comb}.{comb}#{comb}");
                mutatedList.Add($"{comb}.{comb}#{comb}#{comb}.{comb}");
                mutatedList.Add($"{comb}.{comb}#{comb}#{comb}#{comb}");
                mutatedList.Add($"{comb}#{comb}.{comb}.{comb}.{comb}");
                mutatedList.Add($"{comb}#{comb}.{comb}.{comb}#{comb}");
                mutatedList.Add($"{comb}#{comb}.{comb}#{comb}.{comb}");
                mutatedList.Add($"{comb}#{comb}.{comb}#{comb}#{comb}");
                mutatedList.Add($"{comb}#{comb}#{comb}.{comb}.{comb}");
                mutatedList.Add($"{comb}#{comb}#{comb}.{comb}#{comb}");
                mutatedList.Add($"{comb}#{comb}#{comb}#{comb}.{comb}");
                mutatedList.Add($"{comb}#{comb}#{comb}#{comb}#{comb}");
            }

            return mutatedList; 
        }

        private bool CombinationFits(string combination, List<int> numbers)
        {
            var deducted = ReadToArray(combination);

            if (deducted.Count != numbers.Count)
                return false;

            for (int i = 0; i < numbers.Count; i++)
            {
                if (deducted[i] != numbers[i])
                    return false; 
            }
            return true; 
        }

        private (List<int>, List<string>) ReturnForLine(string line)
        {
            var elements = (line.Split(' ').Last().Trim()).Split(',').Select(m => int.Parse(m)).ToList();
            var springs = line.Split(' ').First().Trim();
            var strings = springs.Split('.').Where(m => m != "").ToList();

            return (elements, strings); 
        }

        private static List<string> GetAllCombinationStrings(string line, int startIndex = 0)
        {
            var result = new List<string>();

            if (startIndex == line.Length - 1)
            {
                if (line[startIndex] == '?')
                {
                    result.Add(".");
                    result.Add("#");

                }
                else
                    result.Add($"{line[startIndex].ToString()}");

                return result; 
            }

            var resultReturned = GetAllCombinationStrings(line, startIndex + 1);

            foreach (var res in resultReturned)
            {
                if (line[startIndex] == '?')
                {
                    result.Add($".{res}");
                    result.Add($"#{res}");
                }
                else
                {
                    result.Add($"{line[startIndex].ToString()}{res}");
                }
            }

            return result; 
        }

        private List<int> ReadToArray(string line)
        {
            var regionStarted = false;
            int regionLength = 0;
            List<int> result = new(); 
            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] == '.')
                {
                    if (regionStarted)
                    {
                        result.Add(regionLength);
                        regionStarted = false;
                        regionLength = 0; 
                    }
                    else
                    {
                        //do nothing
                    }
                }
                else
                {
                    if (regionStarted)
                    {
                        regionLength++; 
                    }
                    else
                    {
                        regionLength++;
                        regionStarted = true; 
                    }
                }
            }

            if(regionStarted)
                result.Add(regionLength);

            return result; 
        }


    }
    public record Contiguous(int Length, int StartIndex, int EndIndex)
    {
        public bool ContainsIndex(int index)
        {
            return index >= StartIndex && index < EndIndex;
        }
    }
}
