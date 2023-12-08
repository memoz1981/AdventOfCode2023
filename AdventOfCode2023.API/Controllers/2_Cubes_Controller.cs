using Common.Services;
using Microsoft.AspNetCore.Mvc;

namespace AdventOfCode2023.API.Controllers
{
    [Route("day2")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class _2_Cubes_Controller : ControllerBase
    {
        [HttpGet("exercise1")]
        public IActionResult Exercise1()
        {
            var lineReader = new StringLineReader();
            int maxBlue = 14;
            int maxRed = 12;
            int maxGreen = 13;
            int sum = 0; 
            foreach (var line in lineReader.ReadLines("data2.txt"))
            {
                var indexSemicolon = line.IndexOf(':');
                var dataStartIndex = indexSemicolon + 2;

                var attemptsPerGame =
                    line
                    .Substring(dataStartIndex, line.Length - dataStartIndex)
                    .Split(';');
                
                int gameNumber = int.Parse(line.Substring(5, indexSemicolon - 5));

                var colorDictionary = GetMaximumNumbersForSingleGame(attemptsPerGame);

                if (colorDictionary["blue"] <= maxBlue && colorDictionary["red"] <= maxRed
                    && colorDictionary["green"] <= maxGreen)
                    sum += gameNumber;
            }

            return Ok(sum);
        }

        [HttpGet("exercise2")]
        public IActionResult Exercise2()
        {
            var lineReader = new StringLineReader();
            int sum = 0;
            foreach (var line in lineReader.ReadLines("data2.txt"))
            {
                var indexSemicolon = line.IndexOf(':');
                var dataStartIndex = indexSemicolon + 2;

                var attemptsPerGame =
                    line
                    .Substring(dataStartIndex, line.Length - dataStartIndex)
                    .Split(';');

                var colorDictionary = GetMaximumNumbersForSingleGame(attemptsPerGame);

                sum += colorDictionary["blue"] * colorDictionary["red"] * colorDictionary["green"];
            }

            return Ok(sum);
        }

        private Dictionary<string, int> GetMaximumNumbersForSingleGame(string[] attempts)
        {
            var colorDictionary = new Dictionary<string, int>()
            {
                { "blue", 0}, { "red", 0}, { "green", 0}
            };

            foreach (var attempt in attempts)
            {
                var colorCodes = attempt.Split(',').Select(st => st.Trim());

                foreach (var item in colorCodes)
                {
                    var splittedItem = item.Split(' '); 
                    var num = int.Parse(splittedItem.First());
                    var color = splittedItem.Last().Trim();
                    colorDictionary[color] = Math.Max(colorDictionary[color], num);
                }

            }

            return colorDictionary; 
        }
    }
}
