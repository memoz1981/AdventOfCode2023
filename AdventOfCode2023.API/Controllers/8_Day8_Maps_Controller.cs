using Common.Services;
using Microsoft.AspNetCore.Mvc;

namespace AdventOfCode2023.API.Controllers
{
    [Route("day8new")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class _8_Day8_Maps_Controller : ControllerBase
    {

        [HttpGet("exercise1")]
        public IActionResult Exercise1()
        {
            var lineReader = new StringLineReader();
            var lines = lineReader.ReadLines("data8.txt");
            var instructionList = new List<char>();

            var dictionary = new Dictionary<string, (string left, string right)>();
            ReadInput(lines, instructionList, dictionary);

            var instructionArray = instructionList.ToArray();
            var instructionLength = instructionArray.Length;
            var iteration = GetRecurringLength("AAA", current => current == "ZZZ",
                instructionArray, dictionary);

            return Ok(iteration);
        }

        private static void ReadInput(List<string> lines, List<char> instructionList, Dictionary<string, (string left, string right)> dictionary)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                if (i < 2)
                {
                    instructionList.AddRange(lines[i].Trim());
                }
                else
                {
                    var splitted = lines[i].Split('=');

                    var name = splitted[0].Trim();

                    var children = splitted[1].Trim();

                    var left = children.Split(',').First().Trim().Substring(1, 3);
                    var right = children.Split(',').Last().Trim().Substring(0, 3);

                    dictionary[name] = (left, right);
                }
            }
        }

        [HttpGet("exercise2")]
        public IActionResult Exercise2()
        {
            var lineReader = new StringLineReader();
            var lines = lineReader.ReadLines("data8.txt");
            var instructionList = new List<char>();

            var dictionary = new Dictionary<string, (string left, string right)>();
            ReadInput(lines, instructionList, dictionary);

            var instructionArray = instructionList.ToArray();
            var instructionLength = instructionArray.Length;

            var endingInA = dictionary.Where(m => m.Key.EndsWith("A"));

            var numIterations = new List<long>();

            foreach (var element in endingInA)
            {
                var iteration = GetRecurringLength(element.Key, current => current.EndsWith("Z"),
                    instructionArray, dictionary);
                numIterations.Add(iteration);
            }

            var result = LeastCommonMultiplierFinder.Calculate(numIterations.ToArray());

            return Ok(result);
        }

        private long GetRecurringLength(string current, Func<string, bool> target, 
            char[] instructionArray, Dictionary<string, (string left, string right)> dictionary)
        {
            var instructionLength = instructionArray.Length;
            long iteration = 0; 
            while (true)
            {
                var direction = instructionArray[iteration % instructionLength];

                if (direction == 'L')
                    current = dictionary[current].left;
                else if (direction == 'R')
                    current = dictionary[current].right;
                else
                    throw new ArgumentException();
                iteration++;
                if (target(current))
                    return iteration; 
            }
        }
    }
}
