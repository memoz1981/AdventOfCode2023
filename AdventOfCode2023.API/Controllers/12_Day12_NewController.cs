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

        private List<string> ReturnSplitted(string symbols)
        {
            return symbols.Split('.').Where(m => m != "").ToList(); 
        }

        private bool CanNumberBeUsedInString(string symbol, int number, int startIndex, out int nextIndex)
        {
            nextIndex = startIndex + number; 
            if (startIndex + number > symbol.Length)
                return false;

            if (startIndex + number  == symbol.Length)
            {
                return true; 
            }

            if (symbol[nextIndex] == '#')
                return false;

            return true; 
        }
    }
}
