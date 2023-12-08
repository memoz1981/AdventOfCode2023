using Common.Services;
using Microsoft.AspNetCore.Mvc;

namespace AdventOfCode2023.API.Controllers
{
    [Route("day3")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class _3_Day3_Controller : ControllerBase
    {
        [HttpGet("exercise1")]
        public IActionResult Exercise1()
        {
            var numbers = FindNumbers("data3.txt");
            var lineReader = new StringLineReader(); 
            var charArray = lineReader.ReadLines("data3.txt")
                .Select(x => x.ToCharArray())
                .ToArray();
            var result = 0; 

            foreach (var line in numbers)
            {
                foreach (var str in line.Value)
                {
                    var numberInline = str.GetNumber();
                    if (NeedToBeIncluded(charArray, str, line.Key))
                    {
                        result += str.GetNumber();
                    }
                }

            }

            return Ok(result);
        }

        private bool NeedToBeIncluded(char[][] charArray, Numbers str, int key)
        {
            int colStart = Math.Max(0, str.Start - 1);
            int colEnd = Math.Min(str.End + 1, charArray.GetLength(0)-1);
            int rowStart = Math.Max(0, key - 1);
            int rowEnd = Math.Min(key + 1, charArray[key].Length-1);

            for (int i = rowStart; i <= rowEnd; i++)
                for (int j = colStart; j <= colEnd; j++)
                {
                    var @char = charArray[i][j]; 
                    if ((@char < 48 || @char > 57) && @char != '.')
                        return true; 


                }
            return false;
                    
        }

        [HttpGet("exercise2")]
        public IActionResult Exercise2()
        {
            var numbers = FindNumbers("data3.txt");
            var lineReader = new StringLineReader();
            var charArray = lineReader.ReadLines("data3.txt")
                .Select(x => x.ToCharArray())
                .ToArray();
            var result = 0;

            var numbersAsList = new List<Numbers>(); 
            foreach (var line in numbers)
            {
                foreach (var str in line.Value)
                {
                    var numberInline = str.GetNumber();
                    if (NeedToBeIncluded(charArray, str, line.Key))
                    {
                        result += str.GetNumber();
                        numbersAsList.Add(str);
                    }
                }

            }

            var stars = FindStars("data3.txt");

            var resultNew = ReturnAddition(stars, numbersAsList, result);
            return Ok(resultNew);
        }

        private int ReturnAddition(Dictionary<int, int[]> stars, List<Numbers> numbersAsList, int result)
        {
            result = 0;
            foreach (var key in stars.Keys)
            {
                var starLine = stars[key];
                int num1 = 0;
                int num2 = 0; 
                foreach (int index in starLine)
                {
                    var adjacentNumbers = numbersAsList
                        .Where(
                        (m => ((m.Start >= index - 1 && m.Start <= index + 1)
                        || (m.End >= index - 1 && m.End <= index + 1))
                        && m.Row >= key - 1 && m.Row <= key + 1)).ToList();


                    if (adjacentNumbers.Count == 2)
                    {
                        num1 = adjacentNumbers.First().GetNumber(); 
                        num2 = adjacentNumbers.Last().GetNumber();
                        result = result + num1 * num2; 
                    }
                        
                }
                
            }
            return result;
        }

        private Dictionary<int, int[]> FindStars(string fileName)
        {
            var lineReader = new StringLineReader();
            var row = 0; 
            var results = new Dictionary<int, int[]>();
            foreach (var line in lineReader.ReadLines("data3.txt"))
            {
                var indexes = new List<int>();
                var array = line.ToCharArray();
                for (int i = 0; i < array.Length; i++)
                {
                    if (array[i] == '*')
                        indexes.Add(i);
                }
                results[row++] = indexes.ToArray();
            }
            return results;
        }

        private Dictionary<int, List<Numbers>> FindNumbers(string fileName)
        {
            var lineReader = new StringLineReader();
            var lines = lineReader.ReadLines("data3.txt").ToArray();
            char[][] array = lines.Select(m => m.ToCharArray()).ToArray();
            var result = new Dictionary<int, List<Numbers>>();

            for (int i = 0; i < array.Length; i++)
            {
                var row = array[i];
                var list = new List<Numbers>();
                bool numStarted = false;
                var number = new Numbers();
                for (int j = 0; j < row.Length; j++)
                {
                    if (row[j] >= 48 && row[j] <= 57)
                    {
                        if (!numStarted)
                        {
                            number = new Numbers();
                            numStarted = true;
                            number.Start = j;
                        }
                        number.End = j;
                        number.NumChar.Add(row[j]);
                        number.Row = i; 

                        if (j == row.Length - 1)
                        {
                            list.Add(number);
                        }
                    }
                    else if (numStarted)
                    {
                        numStarted = false;
                        list.Add(number);
                    }
                }
                result[i] = list;
            }
            return result;
        }





    }
    public class Numbers
    {
        public List<char> NumChar = new();
        public int GetNumber()
        {
            var numStr = NumChar.ToArray();
            return int.Parse(numStr); 
        }

        public int Start { get; set; }

        public int End { get; set; }

        public int Row { get; set; }
    }
}
