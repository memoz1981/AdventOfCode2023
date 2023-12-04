using Common.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdventOfCode2023.API.Controllers
{
    [Route("day4")]
    [ApiController]
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
                int match = 0;
                foreach (var num in numbers)
                {
                    if (winNumbers.Contains(num))
                        match++;
                }

                result += (int)Math.Pow((double)2, (double)(match - 1));
            }

            return Ok(result);
        }

        [HttpGet("exercise2")]
        public IActionResult Exercise2()
        {
            var lineReader = new StringLineReader();
            var lines = lineReader.ReadLines("data4.txt");
            long result = 0;
            var index = 0;

            var matchArray = lines.Select(l => FindMatches(l)).ToArray();
            result = FindCardNumbers(matchArray, 0, matchArray.Length - 1); 

            return Ok(result);
        }

        [NonAction]
        private static long FindCardNumbers(int[] matchArray, int startIndex, int endIndex, long result = 0)
        {
            int sum = 0; 
            int[] finalArray = new int[matchArray.Length];
            for(int i=startIndex; i <= endIndex; i++)
            {
                var matches = matchArray[i];
                finalArray[i]++; 

                for (int j = i + 1; j <= Math.Min(i + matches, matchArray.Length-1); j++)
                {
                    finalArray[j] = finalArray[j] + finalArray[i];
                }
            }

            return finalArray.Sum(m => m); 
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
            int match = 0;
            foreach (var num in numbers)
            {
                if (winNumbers.Contains(num))
                    match++;
            }

            return match; 
        }


    }
}
