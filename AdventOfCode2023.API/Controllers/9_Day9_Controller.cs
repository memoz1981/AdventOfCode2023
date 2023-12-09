using Common.Services;
using Microsoft.AspNetCore.Mvc;

namespace AdventOfCode2023.API.Controllers
{
    [Route("day9")]
    [ApiController]
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
                long[] numbers = line.Split(' ').Select(x => long.Parse(x)).ToArray(); 
                result += FindExtraPolatedValue(numbers);
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
                long[] numbers = line.Split(' ').Select(x => long.Parse(x)).ToArray();
                result += FindExtraPolatedValueNew(numbers);
            }

            return Ok(result);
        }

        private static long FindExtraPolatedValue(long[] numbers)
        {
            if (numbers.All(m => m == 0))
                return 0;

            var numNext = new List<long>();
            for (int i = 0; i < numbers.Length-1; i++)
            {
                numNext.Add(numbers[i + 1] - numbers[i]);
            }
            var numArray = numNext.ToArray();

            return FindExtraPolatedValue(numArray) + numbers[numbers.Length - 1];
        }

        private static long FindExtraPolatedValueNew(long[] numbers)
        {
            if (numbers.All(m => m == 0))
                return 0;

            var numNext = new List<long>();
            for (int i = 0; i < numbers.Length - 1; i++)
            {
                numNext.Add(numbers[i + 1] - numbers[i]);
            }
            var numArray = numNext.ToArray();

            return numbers[0] - FindExtraPolatedValueNew(numArray);
        }
    }
}
