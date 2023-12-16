using Common.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace AdventOfCode2023.API.Controllers
{
    [Route("day14")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class _14_Day14_Controller : ControllerBase
    {
        [HttpGet("exercise1")]
        public IActionResult Exercise1()
        {
            var lineReader = new StringLineReader();
            var lines = lineReader.ReadLines("data14.txt").ToArray();

            int result = 0;
            var array = TiltNorth(lines);

            var builder = new StringBuilder();

            result = ReturnResult(array);

            return Ok(result);
        }

        [HttpGet("exercise2")]
        public IActionResult Exercise2(int count)
        {
            var lineReader = new StringLineReader();
            var lines = lineReader.ReadLines("data14.txt").ToArray();

            int result = 0;
            var tiltResult = TiltCycles(lines, count);
            var array = tiltResult.Item1;
            var iteration = tiltResult.Item2; 

            var builder = new StringBuilder();

            builder.AppendLine($"Finished after {iteration}'th Iteration");
            builder.AppendLine(); 

            for (int i = 0; i < lines.Length; i++)
            {
                builder.AppendLine();
                for (int j = 0; j < lines[0].Length; j++)
                    builder.Append(array[i, j]);
            }

            result = ReturnResult(array);

            return Ok(result);
        }

        private (char[,], int) TiltCycles(string[] lines, int cycleCount = 1)
        {
            var data = ConvertToArray(lines);
            var array = data.Item1;
            var height = data.Item2;
            var width = data.Item3;
            int i = 1;

            var dictionaryByValue = new Dictionary<string, int>();
            var dictionaryByKey = new Dictionary<int, char[,]>();

            for (; i <= cycleCount; i++)
            {
                var arrayNew = TiltCycle(array, height, width);
                var arrayString = ConvertToString(arrayNew, height, width);

                if (!dictionaryByValue.TryGetValue(arrayString, out var cycleStartIndex))
                {
                    dictionaryByValue[arrayString] = i;
                    dictionaryByKey[i] = arrayNew; 
                    array = arrayNew; 
                    continue;
                }

                var cycleLength = i - cycleStartIndex;
                var leftElementCount = cycleCount - cycleStartIndex;
                var elementToFetchIndex = cycleStartIndex + leftElementCount % cycleLength;

                return (dictionaryByKey[elementToFetchIndex], i); 
            }
            return (array, i);
        }

        private string ConvertToString(char[,] array, int height, int width)
        {
            var builder = new StringBuilder();

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    builder.Append(array[i, j]);
                }
            }

            return builder.ToString();
        }

        private int ReturnResult(char[,] array)
        {
            int result = 0;
            int height = array.GetLength(0);
            int width = array.GetLength(1); 
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (array[i, j] == 'O')
                        result += (height-i);
                }
            }

            return result;
        }

        private char[,] TiltNorth(string[] lines)
        {
            var height = lines.Length;
            var width = lines[0].Length;

            var array = new char[height, width]; 
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];

                for (int j = 0; j < line.Length; j++)
                {
                    array[i,j] = line[j];
                }
            }

            for (int i = 1; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (array[i, j] != 'O')
                        continue;
                    int k = i; 

                    while (k>0)
                    {
                        if (array[k - 1, j] == '#' || array[k - 1, j] == 'O')
                            break;

                        array[k - 1, j] = 'O';
                        array[k, j] = '.';

                        k--;

                    }
                    
                }
            }
            return array; 
        }

        private (char[,],int, int)  ConvertToArray(string[] lines)
        {
            var height = lines.Length;
            var width = lines[0].Length;
            var array = new char[height, width];
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];

                for (int j = 0; j < line.Length; j++)
                {
                    array[i, j] = line[j];
                }
            }
            return (array, height, width); 
        }

        private char[,] TiltCycle(char[,] arrayOld, int height, int width)
        {
            //copy array
            var array = new char[height, width]; 
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    array[i,j] = arrayOld[i,j];
                }
            }
            
            //Tilt North

            for (int i = 1; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (array[i, j] != 'O')
                        continue;
                    int k = i;

                    while (k > 0)
                    {
                        if (array[k - 1, j] == '#' || array[k - 1, j] == 'O')
                            break;

                        array[k - 1, j] = 'O';
                        array[k, j] = '.';

                        k--;

                    }

                }
            }

            //Tilt West
            for (int j = 1; j < width; j++)
            {
                for (int i = 0; i < height; i++)
                {
                    if (array[i, j] != 'O')
                        continue;
                    int k = j;

                    while (k > 0)
                    {
                        if (array[i, k-1] == '#' || array[i, k-1] == 'O')
                            break;

                        array[i, k-1] = 'O';
                        array[i, k] = '.';

                        k--;

                    }

                }
            }

            //Tilt South
            for (int i = height-2; i >=0 ; i--)
            {
                for (int j = 0; j < width; j++)
                {
                    if (array[i, j] != 'O')
                        continue;
                    int k = i;

                    while (k < height-1)
                    {
                        if (array[k + 1, j] == '#' || array[k + 1, j] == 'O')
                            break;

                        array[k + 1, j] = 'O';
                        array[k, j] = '.';

                        k++;

                    }

                }
            }

            //Tilt East
            for (int j = width-2; j >=0; j--)
            {
                for (int i = 0; i < height; i++)
                {
                    if (array[i, j] != 'O')
                        continue;
                    int k = j;

                    while (k < width-1)
                    {
                        if (array[i, k + 1] == '#' || array[i, k + 1] == 'O')
                            break;

                        array[i, k + 1] = 'O';
                        array[i, k] = '.';

                        k++;

                    }

                }
            }


            return array;
        }


    }
}
