using Microsoft.AspNetCore.Mvc;

namespace AdventOfCode2023.API.Controllers
{
    [Route("day21")]
    [ApiController]
    public class _21_Day21_Controller : ControllerBase
    {
        [HttpGet("exercise1")]
        public IActionResult Exercise1(int stepCount)
        {
            var lines = System.IO.File.ReadAllLines("data21.txt").ToList();

            var array = ReadArray(lines, out var height, out var width, out var startRow, out var startCol);

            var result = ReturnForStep(array, startRow, startCol, stepCount, height, width);

            return Ok(result.Count(m => (m.rowIndex + m.colIndex) % 2 == stepCount % 2));
        }

        [HttpGet("exercise2")]
        public IActionResult Exercise2(int stepCount)
        {
            var lines = System.IO.File.ReadAllLines("data21.txt").ToList();

            var array = ReadArray(lines, out var height, out var width, out var startRow, out var startCol);

            var result = ReturnForStepEx2(array, startRow, startCol, stepCount, height, width);

            return Ok(result.Count(m => (m.rowIndex + m.colIndex + m.verticalSlice + m.horizontalSlice) % 2 == stepCount % 2));
        }

        private static bool[,] ReadArray(List<string> lines, out int height, out int width, out int startRow, out int startCol)
        {
            height = lines.Count;
            width = lines[0].Length;

            startRow = 0;
            startCol = 0;

            var array = new bool[height, width];

            for (int i = 0; i < height; i++)
            {
                var line = lines[i];
                for (int j = 0; j < width; j++)
                {
                    if (line[j] == '.')
                        array[i, j] = true;
                    else if (line[j] == '#')
                        array[i, j] = false;
                    else if (line[j] == 'S')
                    {
                        array[i, j] = true;
                        startRow = i;
                        startCol = j; 
                    }
                    else
                        throw new ArgumentException(); 
                }
            }

            return array; 
        }

        public static List<(int rowIndex, int colIndex)> ReturnForStep(bool[,] array, int startRow, 
            int startCol, int stepCount, int height, int width)
        {
            if (stepCount == 0)
                return new() { (startRow, startCol) };

            
            var innerStepList = ReturnForStep(array, startRow, startCol, stepCount - 1, height, width);

            var list = new List<(int rowIndex, int colIndex)>(); 

            foreach (var index in innerStepList)
            {
                int upperRowIndex = index.rowIndex - 1;
                int lowerRowIndex = index.rowIndex + 1;

                int rightColIndex = index.colIndex + 1; 
                int leftColIndex = index.colIndex - 1;

                if (upperRowIndex >= 0 && array[upperRowIndex, index.colIndex])
                    list.Add((upperRowIndex, index.colIndex));

                if (lowerRowIndex < height && array[lowerRowIndex, index.colIndex])
                    list.Add((lowerRowIndex, index.colIndex));

                if (rightColIndex < width && array[index.rowIndex, rightColIndex])
                    list.Add((index.rowIndex, rightColIndex));

                if (leftColIndex >=0 && array[index.rowIndex, leftColIndex])
                    list.Add((index.rowIndex, leftColIndex));
            }

            list = list.Distinct().ToList(); 

            return list; 
        }

        public static HashSet<(int rowIndex, int colIndex, int verticalSlice, int horizontalSlice)> ReturnForStepEx2(bool[,] array, int startRow,
           int startCol, int stepCount, int height, int width)
        {
            if (stepCount == 0)
                return new() { (startRow, startCol, 0, 0) };

            var innerStepList = ReturnForStepEx2(array, startRow, startCol, stepCount - 1, height, width);

            var list = new HashSet<(int rowIndex, int colIndex, int verticalSlice, int horizontalSlice)>();

            foreach (var index in innerStepList)
            {
                var upperRowIndex = GetNextSliceAndIndex(index.rowIndex, -1, height, index.verticalSlice);
                var lowerRowIndex = GetNextSliceAndIndex(index.rowIndex, 1, height, index.verticalSlice);

                var rightColIndex = GetNextSliceAndIndex(index.colIndex, 1, width, index.horizontalSlice);
                var leftColIndex = GetNextSliceAndIndex(index.colIndex, -1, width, index.horizontalSlice);

                if (array[upperRowIndex.nextIndex, index.colIndex] && !list.Contains((upperRowIndex.nextIndex, index.colIndex, upperRowIndex.nextSlice, index.horizontalSlice)))
                    list.Add((upperRowIndex.nextIndex, index.colIndex, upperRowIndex.nextSlice, index.horizontalSlice));

                if (array[lowerRowIndex.nextIndex, index.colIndex] && !list.Contains((lowerRowIndex.nextIndex, index.colIndex, lowerRowIndex.nextSlice, index.horizontalSlice)))
                    list.Add((lowerRowIndex.nextIndex, index.colIndex, lowerRowIndex.nextSlice, index.horizontalSlice));

                if (array[index.rowIndex, rightColIndex.nextIndex] && !list.Contains((index.rowIndex, rightColIndex.nextIndex, index.verticalSlice, rightColIndex.nextSlice)))
                    list.Add((index.rowIndex, rightColIndex.nextIndex, index.verticalSlice, rightColIndex.nextSlice));

                if (array[index.rowIndex, leftColIndex.nextIndex] && !list.Contains((index.rowIndex, leftColIndex.nextIndex, index.verticalSlice, leftColIndex.nextSlice)))
                    list.Add((index.rowIndex, leftColIndex.nextIndex, index.verticalSlice, leftColIndex.nextSlice));
            }

            return list;
        }

        private static (int nextIndex, int nextSlice) GetNextSliceAndIndex(int index, int increment, int length, int currentSlice)
        {
            var nextIndex = index + increment;
            if (nextIndex < length && nextIndex >= 0)
                return (index + increment, currentSlice);
            else if (nextIndex <= 0)
            {
                return ((nextIndex + length) % length, currentSlice-1);
            }
            else
            {
                return ((nextIndex - length) % length, currentSlice + 1);
            }
        }
    }
}
