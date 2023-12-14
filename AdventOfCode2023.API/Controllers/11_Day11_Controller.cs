using Common.Services;
using Microsoft.AspNetCore.Mvc;

namespace AdventOfCode2023.API.Controllers
{
    [Route("day11")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class _11_Day11_Controller : ControllerBase
    {
        [HttpGet("exercise1")]
        public IActionResult Exercise1()
        {
            var result = ReturnSum(2);

            return Ok(result);
        }

        [HttpGet("exercise2")]
        public IActionResult Exercise2()
        {
            var result = ReturnSum(1000000); 

            return Ok(result);
        }

        private long ReturnSum(int repeatCycle)
        {
            var lineReader = new StringLineReader();
            var lines = lineReader.ReadLines("data11.txt");

            var repeatRows = ReturnEmptyRowIndices(lines);
            var repeatCols = ReturnEmptyColumnIndices(lines);

            var coordinates = GetGalaxies(lines);

            return GetAllLengths(coordinates, repeatRows, repeatCols, repeatCycle);
        }

        private long GetAllLengths((int, int)[] coordinates, int[] repeatRows = null, int[] repeatCols = null, int repeatCycle = 2)
        {
            long result = 0;

            for (int i = 0; i < coordinates.Length; i++)
            {
                for (int j = i + 1; j < coordinates.Length; j++)
                    result += GetLength(coordinates[i], coordinates[j], repeatRows, repeatCols, repeatCycle);
            }

            return result; 
        }

        private long GetLength((int, int) startCoordinates, (int, int) stopCoordinates, int[] repeatRows = null, int[] repeatCols = null, int repeatCycle = 2)
        {
            var repeatRowsBetweenCoordinates = repeatRows?
                .Count(m => (m < stopCoordinates.Item1 && m > startCoordinates.Item1)
                || (m > stopCoordinates.Item1 && m < startCoordinates.Item1)) * (repeatCycle-1) ?? 0;

            var repeatColumnBetweenCoordinates = repeatCols?
                .Count(m => (m < stopCoordinates.Item2 && m > startCoordinates.Item2) ||
                m > stopCoordinates.Item2 && m < startCoordinates.Item2) * (repeatCycle-1) ?? 0;

            return Math.Abs(stopCoordinates.Item1 - startCoordinates.Item1) + 
                Math.Abs(stopCoordinates.Item2 - startCoordinates.Item2) + 
                repeatColumnBetweenCoordinates + repeatRowsBetweenCoordinates; 
        }

        private (int, int)[] GetGalaxies(List<string> lines)
        {
            var height = lines.Count; 
            var width = lines[0].Length;
            var list = new List<(int, int)>();
            for (int i = 0; i < height; i++)
            {
                var line = lines[i].ToCharArray(); 
                for (int j = 0; j < width; j++)
                {
                    if (line[j] == '#')
                        list.Add((i, j));
                }
            }
            return list.ToArray();
        }

        private int[] ReturnEmptyRowIndices(List<string> lines)
        {
            var list = new List<int>();
            for (int i = 0; i < lines.Count; i++)
            {
                var line = lines[i];
                if(line.ToCharArray().All(m => m=='.'))
                    list.Add(i);
            }

            return list.ToArray(); 
        }

        private int[] ReturnEmptyColumnIndices(List<string> lines)
        {
            var result = new List<int>();
            var list = lines.Select(line => line.ToCharArray().ToList()).ToList();
            for (int j = 0; j < lines[0].Length; j++)
            {
                bool isColEmpty = true;
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i][j] != '.')
                    {
                        isColEmpty = false;
                        break;
                    }
                }

                if(isColEmpty)
                    result.Add(j);
            }

            return result.ToArray();
        }
    }
}
