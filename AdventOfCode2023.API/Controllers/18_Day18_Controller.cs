using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace AdventOfCode2023.API.Controllers
{
    [Route("day18")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class _18_Day18_Controller : ControllerBase
    {
        [HttpGet("exercise1")]
        public IActionResult Exercise1()
        {
            var lines = System.IO.File.ReadAllLines("data18.txt");

            var dictionary = ReadLines(lines, out var minRow, out var maxRow, 
                out var minCol, out var maxCol, out var _);

            var dictionaryNormalized = Normalize(dictionary, minRow, minCol);

            var height = maxRow - minRow + 1;
            var width = maxCol - minCol + 1;

            var array = ReturnArray(dictionaryNormalized, height , width);

            var arrayStr = FillNew(array, dictionaryNormalized, height, width); 

            return Ok(arrayStr.sum);
        }

        [HttpGet("exercise2")]
        public IActionResult Exercise2()
        {
            var lines = System.IO.File.ReadAllLines("data18.txt");

            var dictionary = ReadLinesNew(lines, out var minRow, out var maxRow,
                out var minCol, out var maxCol, out var _, out var distanceUp, 
                out var distanceLeft);

            int height = (dictionary.Select(m => m.RowIndex).Max() + 1);
            int width = (dictionary.Select(m => m.ColIndex).Max() + 1);

            var array = Fill(dictionary, height, width);

            var result = FillNewCave(array, dictionary, height, width, distanceUp, distanceLeft); 

            return Ok(result);
        }

        private long[,] Fill(List<CaveLine> caveLine, int height, int width)
        {
            var builder = new StringBuilder();
            var array = new long[height, width];

            for(int i = 0; i < caveLine.Count; i++)
            {
                var current = caveLine[i];
                var next = caveLine[(i + 1) % caveLine.Count];

                if (current.RowIndex == next.RowIndex) // same row
                {
                    var min = Math.Min(current.ColIndex, next.ColIndex);
                    var max = Math.Max(current.ColIndex, next.ColIndex);

                    for (int j = min; j <= max; j++)
                    {
                        array[current.RowIndex, j] = 1;
                    }
                }
                else if (current.ColIndex == next.ColIndex) // same col
                {
                    var min = Math.Min(current.RowIndex, next.RowIndex);
                    var max = Math.Max(current.RowIndex, next.RowIndex);

                    for (int k = min; k <= max; k++)
                    {
                        array[k, current.ColIndex] = 1;
                    }
                }
                else
                    throw new ArgumentException();
            }

            
            return array;
        }

        private long FillNewCave(long[,] array, List<CaveLine> list, int height, int width, long[] distanceUp, 
            long[] distanceLeft)
        {
            var caveDictionary = list.ToDictionary(m => (m.RowIndex, m.ColIndex), m => m);
            var caveRowDictionary = 
                list.Select(m => new { rowIndex = m.RowIndex, distance = m.DistanceUp})
                .OrderBy(m => m.rowIndex).Distinct().ToList();

            var caveColDictionary =
                list.Select(m => new { colIndex = m.ColIndex, distance = m.DistanceLeft })
                .OrderBy(m => m.colIndex).Distinct().ToList();

            var rowHeightDictionary = new Dictionary<int, long>();

            for(int i=2; i<=caveRowDictionary.Last().rowIndex; i = i + 2)
            {
                var rowIndexBelow = caveRowDictionary[i/2].rowIndex; //1 for 0, 3 for 2 etc. 
                rowHeightDictionary[i-1] = caveRowDictionary[i/2].distance - caveRowDictionary[i/2-1].distance;
            }

            var colWidthDictionary = new Dictionary<int, long>();

            for (int i = 2; i <= caveColDictionary.Last().colIndex; i = i + 2)
            {
                colWidthDictionary[i - 1] = caveColDictionary[i / 2].distance - caveColDictionary[i / 2 - 1].distance;
            }
            var sum = list.Sum(m => Math.Abs(m.Length)); 
            long result = 0;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (array[i, j] == 1)
                    {
                        if (caveDictionary.ContainsKey((i, j)))
                            result += Math.Abs(caveDictionary[(i, j)].Length);
                        else
                        {

                        }
                        continue;
                    }
                    var included = TraverseNew(list, i, j);
                    if (included )
                    {
                        var rowsHeight = i % 2 == 1 ? rowHeightDictionary[i] - 1 : 1;
                        var colWidth = j % 2 == 1 ? colWidthDictionary[j] - 1 : 1;
                        result += rowsHeight * colWidth;
                    }
                    else
                    { 
                    }
                    
                }

            }
            return result;
        }

        private bool TraverseNew(List<CaveLine> list, int row, int col)
        {
            int countExitedAbove = 0;
            int listCount = list.Count;

            for (int i = 0; i < listCount; i++)
            {
                var current = list[i];
                var next = list[(i + 1) % listCount];

                if (current.RowIndex == next.RowIndex && current.RowIndex < row) // same row - horizontalLine
                {
                    var min = Math.Min(current.ColIndex, next.ColIndex);
                    var max = Math.Max(current.ColIndex, next.ColIndex);

                    if (max >= col && min < col)
                        countExitedAbove++;
                }
            }

            return countExitedAbove % 2 == 1;
        }

        private (int[,] resultArray, int sum) FillNew(int[,] array, List<(int row, int col, string color)> list, int height, int width)
        {
            var resultArray = new int[height, width];
            var result = 0; 
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (array[i, j] == 1)
                    {
                        resultArray[i, j] = 1;
                        result += 1; 
                        continue; 
                    }
                    resultArray[i, j] = Traverse(list, i, j);
                    result += resultArray[i, j]; 
                }
                
            }
            return (resultArray, result); 
        }

        private string Fill(int[,] array, int height, int width)
        {
            var builder = new StringBuilder();
            for (int i = 0; i < height; i++)
            {
                int countRow = 0;
                List<int> ones = new();
                for (int j = 0; j < width; j++)
                {
                    if (array[i, j] == 1)
                    {
                        ones.Add(j);
                        countRow++; 
                    }
                }
                builder.AppendLine($"Row {i}: count: {countRow}, cols with ones: {string.Join(',', ones)}");
            }
            return builder.ToString();
        }

       

        private string ReadToString(int[,] array, int height, int width)
        {
            var builder = new StringBuilder();
            for (int i = 0; i < height; i++)
            {
                builder.AppendLine(); 
                for (int j = 0; j < width; j++)
                {
                    if (array[i, j] == 1)
                        builder.Append('#');
                    else
                        builder.Append('.'); 
                }
            }
            return builder.ToString(); 
        }

        

        private int[,] ReturnArray(List<(int row, int col, string color)> list, int height, int width)
        {
            int[,] array = new int[height, width];
            for (int i = 0; i < list.Count; i++)
            {
                var current = list[i];
                var next = list[(i + 1) % list.Count];

                if (current.row == next.row) // same row
                {
                    var min = Math.Min(current.col, next.col);
                    var max = Math.Max(current.col, next.col);

                    for (int j = min; j <= max; j++)
                    {
                        array[current.row, j] = 1;
                    }
                }
                else if (current.col == next.col) // same col
                {
                    var min = Math.Min(current.row, next.row);
                    var max = Math.Max(current.row, next.row);

                    for (int k = min; k <= max; k++)
                    {
                        array[k, current.col] = 1;
                    }
                }
                else
                    throw new ArgumentException(); 
            }
            return array; 
        }
        
        private List<(int row, int col, string color)> Normalize(List<(int row, int col, string color)> dictionary, int minRow, int minCol)
        {
            var first = dictionary.First();
            var last = dictionary.Last(); 
            
            
            List<(int row, int col, string color)> dictionaryNormalized = new(); 
            foreach (var element in dictionary)
            {
                dictionaryNormalized.Add((element.row - minRow, element.col - minCol, element.color));
            }

            return dictionaryNormalized;
        }

        private List<(int row, int col, string color)> ReadLines(string[] lines, out int minRow, out int maxRow, 
            out int minCol, out int maxCol, out int height)
        {
            height = lines.Length; 
            var dictionary = new List<(int row, int col, string color)>();
            int currentRow = 0;
            int currentCol = 0; 
            minRow = 0;
            maxRow = 0;
            minCol = 0;
            maxCol = 0; 
            foreach (var line in lines)
            {
                var splitted = line.Split(' ').Select(m => m.Trim()).ToArray();

                var length = int.Parse(splitted[1]);

                var direction = char.Parse(splitted[0]);

                var color = splitted[2].Substring(1, 7);

                var nextCoordinate = GetIncrements(direction, length);

                currentRow += nextCoordinate.row;
                currentCol += nextCoordinate.col;

                dictionary.Add((currentRow, currentCol, color));

                minRow = Math.Min(minRow, currentRow);
                maxRow = Math.Max(maxRow, currentRow);
                minCol = Math.Min(minCol, currentCol);
                maxCol = Math.Max(maxCol, currentCol); 
            }

            return dictionary;
        }

        private List<CaveLine> ReadLinesNew(string[] lines, out int minRow, out int maxRow,
            out int minCol, out int maxCol, out int height, out long[] distinctDistanceUp, 
            out long[] distinctDistanceLeft)
        {
            height = lines.Length;
            var dictionary = new List<CaveLine>();
            int currentRow = 0;
            int currentCol = 0;
            minRow = 0;
            maxRow = 0;
            minCol = 0;
            maxCol = 0;
            
            CaveLine first = new CaveLine(0, '0');
            CaveLine current = first;
            CaveLine next = null; 
            foreach (var line in lines)
            {
                var splitted = line.Split(' ').Select(m => m.Trim()).ToArray();

                var color = (splitted[2])[7];

                var lengthHex = splitted[2].Substring(2, 5); 

                var length = int.Parse(lengthHex, System.Globalization.NumberStyles.HexNumber);

                next = new CaveLine(length, color, current);

                dictionary.Add(next);

                current = next; 

                minRow = Math.Min(minRow, currentRow);
                maxRow = Math.Max(maxRow, currentRow);
                minCol = Math.Min(minCol, currentCol);
                maxCol = Math.Max(maxCol, currentCol);
            }

            distinctDistanceUp = dictionary.Select(m => m.DistanceUp).Distinct().OrderBy(m => m).ToArray();
            distinctDistanceLeft = dictionary.Select(m => m.DistanceLeft).Distinct().OrderBy(m => m).ToArray();

            var heightToRowIndexDictionary = distinctDistanceUp.Select((row, idx) => new { distance = row, index = idx })
                .ToDictionary(m => m.distance, m => m.index);

            var widthToColIndexDictionary = distinctDistanceLeft.Select((col, idx) => new { distance = col, index = idx })
                .ToDictionary(m => m.distance, m => m.index);

            foreach (var el in dictionary)
            {
                var rowIndex = heightToRowIndexDictionary[el.DistanceUp];
                var colIndex = widthToColIndexDictionary[el.DistanceLeft];

                el.RowIndex = rowIndex * 2;
                el.ColIndex = colIndex * 2; 
            }

            var last = dictionary.Last();

            return dictionary;
        }

        private int Traverse(List<(int row, int col, string color)> list, int row, int col)
        {
            int countExitedAbove = 0;
            int listCount = list.Count; 

            for(int i=0; i<listCount;i++)
            {
                var current = list[i];
                var next = list[(i +1) % listCount];

                if (current.row == next.row && current.row < row) // same row - horizontalLine
                {
                    var min = Math.Min(current.col, next.col);
                    var max = Math.Max(current.col, next.col);

                    if(max>=col && min<col)
                        countExitedAbove++;
                }
            }

            return countExitedAbove % 2; 
        }

        private (int row, int col) GetIncrements(char direction, int length)
        {
            return direction switch
            {
                'L' => (0, -length),
                'R' => (0, length),
                'U' => (-length, 0),
                'D' => (length, 0),
                _ => throw new ArgumentException()
            };
        }
    }

    public class CaveLine
    {
        public CaveLine(int length, char direction, CaveLine previousLine = null)
        {
            (Length, LineType) = SetData(length, direction);
            SetPreviousLine(previousLine);

            Previous = previousLine;

            if (previousLine != null)
                previousLine.Next = this; 
        }

        private (int length, CaveLineType lineType) SetData(int length, char direction)
        {
            return direction switch
            {
                '2' => (-length, CaveLineType.Horizontal), //L
                '0' => (length, CaveLineType.Horizontal), //R
                '3' => (-length, CaveLineType.Vertical), //U
                '1' => (length, CaveLineType.Vertical), //D
                _ => throw new ArgumentException()
            };
        }

        public int RowIndex { get; set; } = -1;

        public int ColIndex { get; set; } = -1; 

        public int Length { get; set; }

        public long DistanceUp { get; set; }

        public long DistanceLeft { get; set; }

        private void SetPreviousLine(CaveLine previousLine)
        {
            if (previousLine == null)
            {
                DistanceLeft = 0;
                DistanceUp = 0;
            }
            else
            {
                DistanceLeft = previousLine.DistanceLeft;
                DistanceUp = previousLine.DistanceUp;
            }

            if (LineType == CaveLineType.Horizontal)
                DistanceLeft += Length;

            if (LineType == CaveLineType.Vertical)
                DistanceUp += Length;
        }

        public void SetFollowingTo(CaveLine line)
        {
            if (line.LineType == LineType)
                throw new ArgumentException();

            DistanceLeft = line.DistanceLeft;
            DistanceUp = line.DistanceUp; 

            if (LineType == CaveLineType.Horizontal)
                DistanceLeft += Length;

            if (LineType == CaveLineType.Vertical)
                DistanceUp += Length;
        }

        public CaveLineType LineType { get; set; }

        public CaveLine Previous { get; set; }
        public CaveLine Next { get; set; }
    }

    public enum CaveLineType { Horizontal, Vertical }
}
