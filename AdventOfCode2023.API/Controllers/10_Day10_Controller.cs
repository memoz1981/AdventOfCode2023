using Common.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace AdventOfCode2023.API.Controllers
{
    [Route("day10")]
    [ApiController]
    public class _10_Day10_Controller : ControllerBase
    {
        [HttpGet("exercise1")]
        public IActionResult Exercise1()
        {
            var lineReader = new StringLineReader();
            var lines = lineReader.ReadLines("data10.txt");

            var height = lines.Count;
            var width = lines[0].ToCharArray().Length;

            var array = new char[height, width];
            var sIndex = (-1, -1); 
            for(int i=0; i<height; i++)
            {
                var line = lines[i].ToCharArray();
                for (int j = 0; j < width; j++)
                {
                    array[i, j] = line[j];
                    if (line[j] == 'S')
                        sIndex = (i, j); 
                }
            }

            var list = FindPipes(array, sIndex.Item1 + 1, sIndex.Item2, 'S', height, width);

            return Ok(list.Count/2);
        }

        [HttpGet("exercise2")]
        public IActionResult Exercise2()
        {
            var lineReader = new StringLineReader();
            var lines = lineReader.ReadLines("data10.txt");

            var height = lines.Count;
            var width = lines[0].ToCharArray().Length;

            var array = new char[height, width];
            var sIndex = (-1, -1);
            for (int i = 0; i < height; i++)
            {
                var line = lines[i].ToCharArray();
                for (int j = 0; j < width; j++)
                {
                    array[i, j] = line[j];
                    if (line[j] == 'S')
                        sIndex = (i, j);
                }
            }

            var containArray = FindContained(array, sIndex.Item1 + 1, sIndex.Item2, 'S', height, width);
            var result = FindNumElement(array, containArray, height, width);

            //var resultstr = FindContainedString(array, sIndex.Item1 + 1, sIndex.Item2, 'S', height, width);




            return Ok(result);
        }

        private int FindEx2Result(char[,] array, char[,] directionArray, int height, int width)
        {
            var result = 0;
            var numHoritontalLinesPerColumn = new int[width];
            var horizontalDirections = new HashSet<char>() { 'W', 'E' };
            var verticalDirections = new HashSet<char>() { 'N', 'S' };
            var numForColumn = new int[width];

            for (int i = 0; i < height; i++)
            {
                var numForRow = 0; 
                for (int j = 0; j < width; j++)
                {
                    if (horizontalDirections.Contains(directionArray[i, j]))
                        numForColumn[j]++;
                    else if (verticalDirections.Contains(directionArray[i, j]))
                        numForRow++;
                    else
                    {
                        var horizontalOk = numForColumn[j] % 2 == 1;
                        var verticalOK = numForRow % 2 == 1;

                        if (horizontalOk && verticalOK)
                            result++; 
                    }

                    
                }

            }

            return result;
        }



        private int FindNumElement(char[,] array, int[,] containArray, int height, int width)
        {
            var result = 0; 
            var startedForColForPreviousRows = new bool[width];
            var startElements = new HashSet<char>() { 'L', 'F'};
            var stopElements = new HashSet<char>() { 'J', '7' };

            for (int i = 0; i < height; i++)
            {
                var startedForRow = false;
                for (int j = 0; j < width; j++)
                {
                    if (array[i, j] == 'S')
                        array[i, j] = 'F';
                    if (containArray[i, j] == 1)
                    {
                        if (startElements.Contains(array[i, j]))
                        {
                            startedForRow = !startedForRow;
                            startedForColForPreviousRows[j] = !startedForColForPreviousRows[j];

                        }
                        if (stopElements.Contains(array[i, j]))
                        {
                            startedForRow = !startedForRow;
                        }
                        else if (array[i, j] == '|')
                        {
                            startedForRow = !startedForRow;
                        }
                        else if (array[i, j] == '-')
                        {
                            startedForColForPreviousRows[j] = !startedForColForPreviousRows[j];

                        }
                    }
                    else
                    {
                        var previousRowOk = startedForColForPreviousRows[j];
                        

                        if (containArray[i, j] == 0 && startedForColForPreviousRows[j] && startedForRow)
                            result++; 
                    }
                }
               
            }

            return result; 
        }

        private List<char> FindPipes(char[,] array, int startHeightIndex, int startWidthIndex, 
            char previousDirection, int height, int width)
        {
            Dictionary<char, List<Direction>> directionsFromTo = new()
            {
                { '|', new() { new Direction('N','N', -1,0), new Direction('S', 'S', 1, 0) } },
                { '-', new() { new Direction('E', 'E', 0, 1), new Direction('W', 'W', 0, -1) } },
                { 'L', new() { new Direction('S', 'E', 0, 1), new Direction('W', 'N', -1, 0) } },
                { 'J', new() { new Direction('S', 'W', 0, -1), new Direction('E', 'N', -1, 0) } },
                { '7', new() { new Direction('N', 'W', 0, -1), new Direction('E', 'S', 1, 0) } },
                { 'F', new() { new Direction('N', 'E', 0, 1), new Direction('W', 'S', 1, 0) } },
                { '.', new() { } },
            };

            List<char> pipes = new();
            var element = array[startHeightIndex, startWidthIndex];

            while (true)
            {
                pipes.Add(element);

                if (element == 'S')
                {
                    return pipes;
                }

                if (element == '.')
                    return new List<char>(); 

                if (!directionsFromTo.TryGetValue(element, out var directionsForCh))
                {
                    return new List<char>();
                }

                var direction = directionsForCh.FirstOrDefault(m => m.PreviousDirection == previousDirection);

                if (direction == null)
                    return new List<char>();

                startHeightIndex += direction.incrementHeight;
                startWidthIndex += direction.incrementWidth;
                previousDirection = direction.NextDirection; 

                if (startHeightIndex >= height)
                    return new List<char>();

                if (startWidthIndex >= width)
                    return new List<char>(); 
                element = array[startHeightIndex, startWidthIndex];
            }
        }

        private int[,] FindContained(char[,] array, int startHeightIndex, int startWidthIndex,
            char previousDirection, int height, int width)
        {
            Dictionary<char, List<Direction>> directionsFromTo = new()
            {
                { '|', new() { new Direction('N', 'N', -1, 0), new Direction('S', 'S', 1, 0) } },
                { '-', new() { new Direction('E', 'E', 0, 1), new Direction('W', 'W', 0, -1) } },
                { 'L', new() { new Direction('S', 'E', 0, 1), new Direction('W', 'N', -1, 0) } },
                { 'J', new() { new Direction('S', 'W', 0, -1), new Direction('E', 'N', -1, 0) } },
                { '7', new() { new Direction('N', 'W', 0, -1), new Direction('E', 'S', 1, 0) } },
                { 'F', new() { new Direction('N', 'E', 0, 1), new Direction('W', 'S', 1, 0) } },
                { '.', new() { } },
            };

            var contained = new int[height, width]; 
            var element = array[startHeightIndex, startWidthIndex];

            while (true)
            {
                contained[startHeightIndex, startWidthIndex] = 1;
                if (element == 'S')
                {
                    return contained;
                }

                if (element == '.')
                    throw new ArgumentException(); 

                if (!directionsFromTo.TryGetValue(element, out var directionsForCh))
                {
                    throw new ArgumentException();
                }

                var direction = directionsForCh.FirstOrDefault(m => m.PreviousDirection == previousDirection);

                if (direction == null)
                    throw new ArgumentException();

                startHeightIndex += direction.incrementHeight;
                startWidthIndex += direction.incrementWidth;
                previousDirection = direction.NextDirection;

                if (startHeightIndex >= height)
                    throw new ArgumentException();

                if (startWidthIndex >= width)
                    throw new ArgumentException();
                element = array[startHeightIndex, startWidthIndex];
            }
        }

        private char[,] FindContainedString(char[,] array, int startHeightIndex, int startWidthIndex,
            char previousDirection, int height, int width)
        {
            Dictionary<char, List<Direction>> directionsFromTo = new()
            {
                { '|', new() { new Direction('N', 'N', -1, 0), new Direction('S', 'S', 1, 0) } },
                { '-', new() { new Direction('E', 'E', 0, 1), new Direction('W', 'W', 0, -1) } },
                { 'L', new() { new Direction('S', 'E', 0, 1), new Direction('W', 'N', -1, 0) } },
                { 'J', new() { new Direction('S', 'W', 0, -1), new Direction('E', 'N', -1, 0) } },
                { '7', new() { new Direction('N', 'W', 0, -1), new Direction('E', 'S', 1, 0) } },
                { 'F', new() { new Direction('N', 'E', 0, 1), new Direction('W', 'S', 1, 0) } },
                { '.', new() { } },
            };

            var contained = new char[height, width];
            var element = array[startHeightIndex, startWidthIndex];

            while (true)
            {
                contained[startHeightIndex, startWidthIndex] = previousDirection;
                if (element == 'S')
                {
                    return contained; 
                }

                if (element == '.')
                    throw new ArgumentException();

                if (!directionsFromTo.TryGetValue(element, out var directionsForCh))
                {
                    throw new ArgumentException();
                }

                var direction = directionsForCh.FirstOrDefault(m => m.PreviousDirection == previousDirection);

                if (direction == null)
                    throw new ArgumentException();

                startHeightIndex += direction.incrementHeight;
                startWidthIndex += direction.incrementWidth;
                previousDirection = direction.NextDirection;

                if (startHeightIndex >= height)
                    throw new ArgumentException();

                if (startWidthIndex >= width)
                    throw new ArgumentException();
                element = array[startHeightIndex, startWidthIndex];
            }
        }
    }

    public record Direction(char PreviousDirection, char NextDirection, 
        int incrementHeight, int incrementWidth); 

}
