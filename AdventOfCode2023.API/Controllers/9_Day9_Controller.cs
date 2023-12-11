using Common.Services;
using Microsoft.AspNetCore.Mvc;

namespace AdventOfCode2023.API.Controllers
{
    [Route("day9")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class _9_Day9_Controller : ControllerBase
    {
        [HttpGet("exercise1")]
        public IActionResult Exercise1()
        {
            var lineReader = new StringLineReader();
            var lines = lineReader.ReadLines("data9.txt");

            long result = 0; 
            foreach (var line in lines)
            {
                var numbers = line.Split(' ').Select(x => long.Parse(x)).ToList(); 
                result += FindExtraPolatedLast(numbers);
            }

            return Ok(result);
        }

        [HttpGet("exercise2")]
        public IActionResult Exercise2()
        {
            var lineReader = new StringLineReader();
            var lines = lineReader.ReadLines("data9.txt");

            long result = 0;
            foreach (var line in lines)
            {
                var numbers = line.Split(' ').Select(x => long.Parse(x)).ToList();
                result += FindExtraPolatedFirst(numbers);
            }

            return Ok(result);
        }

        private static long FindExtraPolatedLast(List<long> numbers)
        {
            if (numbers.All(m => m == 0))
                return 0;

            var numNext = FindNextRow(numbers);

            return FindExtraPolatedLast(numNext) + numbers[numbers.Count - 1];
        }

        private static long FindExtraPolatedFirst(List<long> numbers)
        {
            if (numbers.All(m => m == 0))
                return 0;

            var numNext = FindNextRow(numbers);

            return numbers[0] - FindExtraPolatedFirst(numNext);
        }

        private static List<long> FindNextRow(List<long> numbers)
        {
            var numNext = new List<long>();

            for (int i = 0; i < numbers.Count - 1; i++)
            {
                numNext.Add(numbers[i + 1] - numbers[i]);
            }

            return numNext;
        }
    }
}
