using Common.Services;
using Microsoft.AspNetCore.Mvc;

namespace AdventOfCode2023.API.Controllers
{
    [Route("day4")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class _4_Day4_Controller : ControllerBase
    {
        [HttpGet("exercise1")]
        public IActionResult Exercise1()
        {
            var lineReader = new StringLineReader();
            var lines = lineReader.ReadLines("data4.txt");
            var result = 0;

            foreach (var line in lines)
            {
                var matches = FindMatches(line); 

                result += (int)Math.Pow((double)2, (double)(matches - 1));
            }

            return Ok(result);
        }

        [HttpGet("exercise2")]
        public IActionResult Exercise2()
        {
            var lineReader = new StringLineReader();
            var lines = lineReader.ReadLines("data4.txt");

            var matchArray = lines.Select(l => FindMatches(l)).ToArray();
            var result = FindCardNumbers(matchArray); 

            return Ok(result);
        }

        [NonAction]
        private static long FindCardNumbers(int[] matchArray)
        {
            int[] cardCount = new int[matchArray.Length];
            for(int i=0; i <= matchArray.Length - 1; i++)
            {
                cardCount[i]++; 

                for (int j = i + 1; j <= Math.Min(i + matchArray[i], matchArray.Length-1); j++)
                {
                    cardCount[j] = cardCount[j] + cardCount[i];
                }
            }

            return cardCount.Sum(m => m); 
        }

        [NonAction]
        private static int FindMatches(string line)
        {
            var winNumStartIndex = line.IndexOf(':') + 2;
            var winNumEnd = line.IndexOf('|') - 1;

            var numStart = winNumEnd + 2;

            var winNumbers = line
                .Substring(winNumStartIndex, winNumEnd - winNumStartIndex)
                .Trim()
                .Split(' ')
                .Where(m => int.TryParse(m, out var _))
                .Select(m => int.Parse(m))
                .OrderBy(m => m).ToHashSet();

            var numbers = line
                .Substring(numStart, line.Length - numStart)
                .Trim()
                .Split(' ')
                .Where(m => int.TryParse(m, out var _))
                .Select(m => int.Parse(m))
                .OrderBy(m => m).ToList();
            int matches = 0;
            foreach (var num in numbers)
            {
                if (winNumbers.Contains(num))
                    matches++;
            }

            return matches; 
        }
    }
}
