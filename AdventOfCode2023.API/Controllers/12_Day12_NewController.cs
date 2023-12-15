using Common.Services;
using Microsoft.AspNetCore.Mvc;

namespace AdventOfCode2023.API.Controllers
{
    [Route("day12_new")]
    [ApiController]
    public class _12_Day12_NewController : ControllerBase
    {
        [HttpGet("exercise2")]
        public IActionResult Exercise2()
        {
            var lineReader = new StringLineReader();
            var lines = lineReader.ReadLines("data12.txt");

            foreach (var line in lines)
            {
                var splitted = line.Split(' ');
                var symbols = splitted.First().Trim();
                var numbers = splitted.Last().Trim()
                    .Split(',')
                    .Select(m => int.Parse(m))
                    .ToList();

                var splittedString = ReturnSplitted(symbols); 
            }

            return Ok(); 
        }

        private static Dictionary<int, int> 
            ReturnArrayIndexVsCombinationCountForSymbol(string symbol, int[] numbers, 
            int numberStartIndex = 0)
        {
            if (!CanNumberBeUsedInString(symbol, numbers[numberStartIndex], 0, symbol.Length - 1, out var nextIndex))
            {
                return null; 
            }

            

            return new(); 
        }

        private List<string> ReturnSplitted(string symbols)
        {
            return symbols.Split('.').Where(m => m != "").ToList(); 
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
