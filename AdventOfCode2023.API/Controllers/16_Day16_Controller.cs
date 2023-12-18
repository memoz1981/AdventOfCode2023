using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdventOfCode2023.API.Controllers
{
    [Route("day16")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class _16_Day16_Controller : ControllerBase
    {
              
        [HttpGet("exercise1")]
        public IActionResult Exercise1()
        {
            var lines = System.IO.File.ReadAllLines("data16.txt");

            var array = ReadLines(lines, out var height, out var width);

            var intArray = new int[height, width];

            MoveBeams(array, intArray, height, width);

            var result = GetResult(intArray, height, width); 

            return Ok(result);
        }

        [HttpGet("exercise2")]
        public IActionResult Exercise2()
        {
            var lines = System.IO.File.ReadAllLines("data16.txt");

            var array = ReadLines(lines, out var height, out var width);

            List<int> results = new();

            for (int j = 0; j < width; j++)
            {
                var intArray = new int[height, width];
                MoveBeams(array, intArray, height, width, BeamDirection.Down, 0, j);
                var result = GetResult(intArray, height, width);
                results.Add(result); 
            }

            for (int j = 0; j < width; j++)
            {
                var intArray = new int[height, width];
                MoveBeams(array, intArray, height, width, BeamDirection.Up, height-1, j);
                var result = GetResult(intArray, height, width);
                results.Add(result);
            }

            for (int i = 0; i < height; i++)
            {
                var intArray = new int[height, width];
                MoveBeams(array, intArray, height, width, BeamDirection.Right, i, 0);
                var result = GetResult(intArray, height, width);
                results.Add(result);
            }

            for (int i = 0; i < height; i++)
            {
                var intArray = new int[height, width];
                MoveBeams(array, intArray, height, width, BeamDirection.Left, i, width - 1);
                var result = GetResult(intArray, height, width);
                results.Add(result);
            }

            var resultMax = results.OrderByDescending(m => m).First(); 

            return Ok(resultMax);
        }

        public static int GetResult(int[,] intArray, int height, int width)
        {
            int result = 0; 
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                {
                    if (intArray[i, j] > 0)
                        result++; 
                }
            return result; 
        }

        public static void MoveBeams(char[,] array, int[,] intArray, int height, int width,
            BeamDirection nextDirection = BeamDirection.Right, int rowIndex = 0, int colIndex = 0,
            HashSet<(int rowIndex, int colIndex, BeamDirection direction)> previousDirections = null)
        {
            previousDirections = previousDirections ?? new HashSet<(int rowIndex, int colIndex, BeamDirection direction)>();

            if (previousDirections.Contains((rowIndex, colIndex, nextDirection)))
                return;

            previousDirections.Add((rowIndex, colIndex, nextDirection)); 
            
            intArray[rowIndex, colIndex]++;

            var nextDirections = GetNextDirections(array[rowIndex, colIndex], nextDirection);

            foreach (var direction in nextDirections)
            {
                if (direction == BeamDirection.Up && rowIndex == 0)
                    continue;
                else if (direction == BeamDirection.Down && rowIndex == height - 1)
                    continue;
                else if (direction == BeamDirection.Left && colIndex == 0)
                    continue;
                else if (direction == BeamDirection.Right && colIndex == width - 1)
                    continue;

                var increments = GetIncrements(direction);

                MoveBeams(array, intArray, height, width, direction, rowIndex + increments.rowIncrement,
                    colIndex + increments.colIncrement, previousDirections); 
            }
        }

        private static List<BeamDirection> GetNextDirections(char c, 
            BeamDirection directionFrom)
        {
            return (c, directionFrom) switch
            {
                ('.', BeamDirection direction) => new() { (direction) },
                
                ('|', BeamDirection.Up) => new() { BeamDirection.Up },
                ('|', BeamDirection.Down) => new() { BeamDirection.Down },
                ('|', _) => new() { BeamDirection.Up, BeamDirection.Down },
                
                ('-', BeamDirection.Left) => new() { BeamDirection.Left },
                ('-', BeamDirection.Right) => new() { BeamDirection.Right },
                ('-', _) => new() { BeamDirection.Left, BeamDirection.Right },

                ('/', BeamDirection.Left) => new() { BeamDirection.Down },
                ('/', BeamDirection.Right) => new() { BeamDirection.Up },
                ('/', BeamDirection.Up) => new() { BeamDirection.Right },
                ('/', BeamDirection.Down) => new() { BeamDirection.Left },

                ('\\', BeamDirection.Left) => new() { BeamDirection.Up },
                ('\\', BeamDirection.Right) => new() { BeamDirection.Down },
                ('\\', BeamDirection.Up) => new() { BeamDirection.Left },
                ('\\', BeamDirection.Down) => new() { BeamDirection.Right },

                _ => throw new ArgumentException()
            };
        }

        private static (int rowIncrement, int colIncrement) GetIncrements(BeamDirection direction)
        {
            return direction switch
            {
                BeamDirection.Up =>     (-1,0),
                BeamDirection.Right =>  (0, 1),
                BeamDirection.Down =>   (1, 0),
                BeamDirection.Left =>   (0, -1),
                _ => throw new ArgumentException()
            };
        }

        private char[,] ReadLines(string[] lines, out int height, out int width)
        {
            height = lines.Length;
            width = lines[0].Length;
            var array = new char[height, width]; 
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                for (int j = 0; j < line.Length; j++)
                {
                    array[i,j] = line[j];
                }
            }

            return array; 
        }
    }

    public enum BeamDirection { Right, Left, Up, Down }
}
