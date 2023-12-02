using Common.Services;
using Microsoft.AspNetCore.Mvc;

namespace AdventOfCode2023.API.Controllers
{
    [Route("day2")]
    [ApiController]
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

                var numbersForLine = GetMaximumNumbersForSingleGame(attemptsPerGame);

                if (numbersForLine.blue <= maxBlue && numbersForLine.red <= maxRed
                    && numbersForLine.green <= maxGreen)
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

                var numbersForLine = GetMaximumNumbersForSingleGame(attemptsPerGame);

                sum += numbersForLine.blue * numbersForLine.red * numbersForLine.green;
            }

            return Ok(sum);
        }

        private (int blue, int red, int green) GetMaximumNumbersForSingleGame(string[] attempts)
        {
            var numBlueMax = 0;
            var numGreenMax = 0;
            var numRedMax = 0;

            foreach (var attempt in attempts)
            {
                var splitByColor = attempt.Split(',');
                var numBlue = 0;
                var numGreen = 0;
                var numRed = 0;

                foreach (var color in splitByColor)
                {
                    numBlue = GetNumberFromColor(color, "blue");
                    numRed = GetNumberFromColor(color, "red");
                    numGreen = GetNumberFromColor(color, "green");

                    numBlueMax = Math.Max(numBlueMax, numBlue);
                    numRedMax = Math.Max(numRedMax, numRed);
                    numGreenMax = Math.Max(numGreenMax, numGreen);
                }
            }
            return (numBlueMax, numRedMax, numGreenMax);
        }

        private int GetNumberFromColor(string str, string color)
        {
            var index = str.IndexOf(color);
            if (index == -1)
            {
                return 0;
            }
            var numColor = str.Substring(0, index);

            return int.Parse(numColor);
        }
    }
}
