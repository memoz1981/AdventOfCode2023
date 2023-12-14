using Common.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace AdventOfCode2023.API.Controllers
{
    [Route("day10")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class _10_Day10_Controller : ControllerBase
    {
        Dictionary<char, List<Direction>> _directions;
        public _10_Day10_Controller()
        {
            _directions = BuildDirections();
        }

        private static Dictionary<char, List<Direction>> BuildDirections()
        {
            return new()
            {
                { '|', new() { new Direction('N', 'N', -1, 0), new Direction('S', 'S', 1, 0) } },
                { '-', new() { new Direction('E', 'E', 0, 1), new Direction('W', 'W', 0, -1) } },
                { 'L', new() { new Direction('S', 'E', 0, 1), new Direction('W', 'N', -1, 0) } },
                { 'J', new() { new Direction('S', 'W', 0, -1), new Direction('E', 'N', -1, 0) } },
                { '7', new() { new Direction('N', 'W', 0, -1), new Direction('E', 'S', 1, 0) } },
                { 'F', new() { new Direction('N', 'E', 0, 1), new Direction('W', 'S', 1, 0) } },
                { '.', new() { } },
            };
        }

        [HttpGet("exercise1")]
        public IActionResult Exercise1()
        {
            var lineReader = new StringLineReader();
            var lines = lineReader.ReadLines("data10.txt");

            var height = lines.Count;
            var width = lines[0].ToCharArray().Length;

            var array = new char[height, width];
            var sIndex = FindSIndexAndReadArray(lines, height, width, array);

            //seems that only possible option is to start at element south of 'S' (could also start at norht one - same)
            var list = FindPipes(array, sIndex.Item1 + 1, sIndex.Item2, 'S', height, width);

            return Ok(list.Count / 2);
        }

        private static (int, int) FindSIndexAndReadArray(List<string> lines, int height, int width, char[,] array)
        {
            (int, int) sIndex = (-1, -1);
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

            return sIndex;
        }

        private List<Path> FindPipes(char[,] array, int startHeightIndex, int startWidthIndex,
            char previousDirection, int height, int width)
        {
            //list to be returned
            List<Path> pipes = new();

            //start element
            var element = array[startHeightIndex, startWidthIndex];

            while (true)
            {
                //add all traversed to list
                pipes.Add(new Path(startHeightIndex, startWidthIndex, element));

                //if S reached - stop
                if (element == 'S')
                {
                    return pipes;
                }

                //if next element is empty - return empty list
                if (element == '.')
                    return new List<Path>();

                //if direction is not valid - return empty list
                if (!_directions.TryGetValue(element, out var directionsForCh))
                {
                    return new List<Path>();
                }

                //find direction
                var direction = directionsForCh.FirstOrDefault(m => m.PreviousDirection == previousDirection);

                //if no direction found - return empty list
                if (direction == null)
                    return new List<Path>();

                //find new coordinates
                startHeightIndex += direction.IncrementHeight;
                startWidthIndex += direction.IncrementWidth;
                previousDirection = direction.NextDirection;

                //validations
                if (startHeightIndex >= height)
                    return new List<Path>();

                if (startWidthIndex >= width)
                    return new List<Path>();

                //next element
                element = array[startHeightIndex, startWidthIndex];
            }
        }

        [HttpGet("exercise2")]
        public IActionResult Exercise2()
        {
            var lineReader = new StringLineReader();
            var lines = lineReader.ReadLines("data10.txt");

            var height = lines.Count;
            var width = lines[0].ToCharArray().Length;

            var array = new char[height, width];
            var sIndex = FindSIndexAndReadArray(lines, height, width, array);

            //seems that only possible option is to start at element south of 'S' (could also start at norht one - same)
            var pipes = FindPipes(array, sIndex.Item1 + 1, sIndex.Item2, 'S', height, width);

            var reordered = ReorderPath(pipes, 'F');

            if (reordered.Count == 0)
                return Ok(string.Join(';', reordered.Select(m => m.ToString()))); 

            var (vertical, horizontal) = ReturnRowLinesPerColumn(reordered, height, width);

            var builder = new StringBuilder();
            builder.AppendLine("Vertical lines: ");
            builder.AppendLine(string.Join('-', vertical.Select(m => m.ToString())));
            builder.AppendLine();
            builder.AppendLine();
            builder.AppendLine("Horizontal lines: ");
            builder.AppendLine(string.Join('-', horizontal.Select(m => m.ToString())));
            var pipesDictionary = reordered.GroupBy(m => m.RowIndex)
                .ToDictionary(m => m.Key, m => m.Select(n => n.ColumnIndex).ToHashSet());
            var result = 0;
            var builderNew = new StringBuilder();

            var candidates = new List<(int col, int row)>(); 
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    (int row, int column) coordinate= (i, j);
                    var coordinateIncluded = IsCandidate(coordinate, vertical, horizontal,
                        pipesDictionary);

                    if (coordinateIncluded!)
                        candidates.Add(coordinate); 
                }
            }

            foreach (var item in candidates)
            {
                if (IsElementNew((item.col, item.row), pipes))
                    result++; 
            }

            return Ok(result);
        }

        private bool IsElementNew((int row, int col) coordinate, List<Path> path)
        {
            //assume we start from upper left most corner and traverse to right
            var pipe = path[0];
            (int row, int col) lastCoordinateBeforeEntry = (pipe.RowIndex, pipe.ColumnIndex);
            int initialDifference = lastCoordinateBeforeEntry.col - coordinate.col; 
            int i = 0;
            int count = 0;
            while (initialDifference == 0)
            {
                i++;
                pipe = path[i];
                lastCoordinateBeforeEntry = (pipe.RowIndex, pipe.ColumnIndex);
                initialDifference = lastCoordinateBeforeEntry.col - coordinate.col;
            }
            for (int j = i; j < path.Count; j++)
            {
                pipe = path[j];

                var difference = pipe.ColumnIndex - coordinate.col;

                if (difference == 0)
                    continue; 

                if (Math.Sign(difference) != Math.Sign(initialDifference))
                {
                    lastCoordinateBeforeEntry = (pipe.RowIndex, pipe.ColumnIndex);
                    initialDifference = difference;
                    
                    if(pipe.RowIndex < coordinate.row)
                        count++;
                }
                else
                {
                    lastCoordinateBeforeEntry = (pipe.RowIndex, pipe.ColumnIndex);
                    initialDifference = difference;
                }
            }
            return count % 2 == 1;
        }

        private bool IsElement((int row, int col) coordinate, List<Path> path)
        {
            //assume we start from upper left most corner and traverse to right
            var pipe = path[0]; 
            int? initialDifference = pipe.RowIndex < coordinate.row ? 
                pipe.ColumnIndex - coordinate.col: null;
            int i = 0;
            int count = 0; 
            while (initialDifference ==null || Math.Abs(initialDifference.Value) >1 || initialDifference.Value == 0)
            {
                i++;
                pipe = path[i];
                initialDifference = pipe.RowIndex < coordinate.row ?
                pipe.ColumnIndex - coordinate.col : null;
            }
            for (int j = i; j < path.Count ; j++)
            {
                pipe = path[j];

                if (pipe.RowIndex >= coordinate.row)
                {
                    initialDifference = null;
                    continue;
                }

                int difference = pipe.ColumnIndex - coordinate.col;


                if (difference == 0)
                {
                    continue; 
                }

                if (initialDifference == null)
                    initialDifference = difference;

                if (Math.Sign(difference) != Math.Sign(initialDifference.Value))
                {
                    initialDifference = difference;
                    count++;
                }

                     
            }
            return count % 2 == 1; 
        }

        private bool IsCandidate((int row, int column) coordinate, 
            Line[] verticalLines,
            Line[] horizontalLines, Dictionary<int, HashSet<int>> pipesDictionary)
        {
            var verticalLineDic = verticalLines.ToDictionary(m => m.Index, m => m.GetLines());
            var horizontalLineDic = horizontalLines.ToDictionary(m => m.Index, m => m.GetLines());

            if (pipesDictionary.TryGetValue(coordinate.row, out var pipesDictionaryRow))
            {
                if (pipesDictionaryRow.Contains(coordinate.column))
                    return false;
            }

            //1. Above
            int verticalLineAboveCount = 0;
            List<(int start, int end)> verticalLinesAbove = new(); 
            if (verticalLineDic.ContainsKey(coordinate.column))
            {
                verticalLinesAbove = verticalLineDic[coordinate.column]
                .Where(line => line.end < coordinate.row).ToList();

                verticalLineAboveCount = verticalLinesAbove.Count;
            }
 
            var horizontalLinesAbove = horizontalLineDic
                .Where(row => row.Key < coordinate.row)
                .SelectMany(line => line.Value.Where(m => m.end >= coordinate.column
                && m.start <= coordinate.column)).ToList(); 

            var horizontalLinesAboveCount = horizontalLinesAbove.Count;

            var sumAbove = verticalLineAboveCount + horizontalLinesAboveCount;

            if (sumAbove == 0)
                return false;

            //2. Below
            int verticalLineBelowCount = 0;
            List<(int start, int end)> verticalLinesBelow = new();
            if (verticalLineDic.ContainsKey(coordinate.column))
            {
                verticalLinesBelow = verticalLineDic[coordinate.column]
                .Where(line => line.start > coordinate.row).ToList();

                verticalLineBelowCount = verticalLinesBelow.Count;
            }

            var horizontalLinesBelow = horizontalLineDic
                .Where(row => row.Key > coordinate.row)
                .SelectMany(line => line.Value.Where(m => m.end >= coordinate.column
                && m.start <= coordinate.column)).ToList();

            var horizontalLinesBelowCount = horizontalLinesBelow.Count;

            var sumBelow = verticalLineBelowCount + horizontalLinesBelowCount;

            if (sumBelow == 0)
                return false;

            //3. Left
            var verticalLinesLeft = verticalLineDic
                .Where(col => col.Key < coordinate.column)
                .SelectMany(line => line.Value.Where(m => m.end >= coordinate.row
                && m.start <= coordinate.row)).ToList();

            var verticalLineLeftCount = verticalLinesLeft.Count;

            int horizontalLinesLeftCount = 0;
            List<(int start, int end)> horizontalLinesLeft = new();
            if (horizontalLineDic.ContainsKey(coordinate.row))
            {
                horizontalLinesLeft = horizontalLineDic[coordinate.row]
                .Where(line => line.end < coordinate.column).ToList();

                horizontalLinesLeftCount = horizontalLinesLeft.Count;
            }

            var sumLeft = verticalLineLeftCount + horizontalLinesLeftCount;

            if (sumLeft == 0)
                return false;

            //4. Right
            var verticalLinesRight = verticalLineDic
                .Where(col => col.Key > coordinate.column)
                .SelectMany(line => line.Value.Where(m => m.end >= coordinate.row
                && m.start <= coordinate.row)).ToList();

            var verticalLineRightCount = verticalLinesRight.Count;

            int horizontalLinesRightCount = 0;
            List<(int start, int end)> horizontalLinesRight = new();
            if (horizontalLineDic.ContainsKey(coordinate.row))
            {
                horizontalLinesRight = horizontalLineDic[coordinate.row]
                .Where(line => line.start > coordinate.column).ToList();

                horizontalLinesRightCount = horizontalLinesRight.Count;
            }

            var sumRight = verticalLineRightCount + horizontalLinesRightCount;

            if (sumRight == 0 )
                return false;


            return true;
        }

        private (Line[] verticalLines, Line[] horizontalLines) 
            ReturnRowLinesPerColumn(List<Path> pipes, int height, int width)
        {
            var pipe = pipes[0];
            var colIndex = pipe.ColumnIndex;
            var rowIndex = pipe.RowIndex; 

            var verticalLines = new Line[width];
            var horizontalLines = new Line[height];
            for (int col = 0; col < width; col++)
            {
                verticalLines[col] = new Line(col, LineType.Column); 
            }

            for (int row = 0; row < height; row++)
            {
                horizontalLines[row] = new Line(row, LineType.Row);
            }

            verticalLines[colIndex].MoveToLine(rowIndex); 
            horizontalLines[rowIndex].MoveToLine(colIndex);

            for (int i = 1; i < pipes.Count; i++)
            {
                pipe = pipes[i];

                if (pipe.ColumnIndex != colIndex)
                {
                    verticalLines[colIndex].MoveToLine(pipe.RowIndex);
                    verticalLines[pipe.ColumnIndex].MoveToLine(pipe.RowIndex); 
                }

                if (pipe.RowIndex != rowIndex)
                {
                    horizontalLines[rowIndex].MoveToLine(pipe.ColumnIndex);
                    horizontalLines[pipe.RowIndex].MoveToLine(pipe.ColumnIndex);
                }

                colIndex = pipe.ColumnIndex;
                rowIndex = pipe.RowIndex; 
            }

            return (verticalLines, horizontalLines); 
        }

        private List<Path> ReorderPath(List<Path> pipes, char sReplace)
        {
            var upperLeftMost = pipes.First();

            //first find upper leftmost - it's optional just for better understanding
            int i = 0;
            for (; i < pipes.Count; i++)
            {
                var item = pipes[i];
                if (item.RowIndex < upperLeftMost.RowIndex)
                    upperLeftMost = item;
                else if (item.RowIndex == upperLeftMost.RowIndex && item.ColumnIndex < upperLeftMost.ColumnIndex)
                    upperLeftMost = item;
            }

            //then re-order list to start at upper left most - it's optional just for better understanding
            i = 0;
            for (; i < pipes.Count; i++)
            {
                var item = pipes[i];

                if (item.RowIndex == upperLeftMost.RowIndex
                    && item.ColumnIndex == upperLeftMost.ColumnIndex)
                    break;
            }

            //re-ordered list
            var list = new List<Path>(); 
            for (int j = 0; j < pipes.Count; j++)
            {
                var pipe = pipes[(j + i) % pipes.Count];

                if (pipe.Character == 'S')
                    pipe = pipe with { Character = sReplace }; 
                list.Add(pipe); 
            }

            return list; 
        }
    }

    //Record to be used to traverse
    public record Direction(char PreviousDirection, char NextDirection, 
        int IncrementHeight, int IncrementWidth);

    //new record defined to contain path data
    public record Path(int RowIndex, int ColumnIndex, char Character) 
    {
        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine($"row:{RowIndex}, column:{ColumnIndex}, char:{Character.ToString()}");
            return builder.ToString();  
        }
    }

    public class Line
    {
        public Line(int index, LineType lineType)
        {
            Index = index;
            LineType = lineType;
            _lines = new();
            _entered = false;
        }

        public List<(int start, int end)> GetLines() => _lines; 

        public int Index { get; set; }
        public LineType LineType { get; set; }

        private List<(int start, int end)> _lines; 

        private bool _entered;

        private (int start, int end) _currentLine; 

        public void MoveToLine(int fromRowOrColumn)
        {
            if (!_entered)
            {
                _entered = true;
                _currentLine.start = fromRowOrColumn;
            }
            else
            {
                _currentLine.end = fromRowOrColumn;
                if (_currentLine.start != _currentLine.end)
                {
                    if (_currentLine.start < _currentLine.end)
                        _lines.Add(_currentLine);
                    else
                    {
                        var start = _currentLine.start;
                        _currentLine.start = _currentLine.end;
                        _currentLine.end = start;
                        _lines.Add(_currentLine);
                    }

                }
                    
                _entered = false;
            }
        }

        public bool FindAbove(int rowColIndex)
        {
            return _lines.Any(l => (l.start >= rowColIndex && l.end <= rowColIndex)
            || (l.start <= rowColIndex && l.end >= rowColIndex)); 
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append($"line index: {Index}, "); 

            for (int i=0;i<_lines.Count;i++)
                builder.Append($"line {i}: from {_lines[i].start} to {_lines[i].end}.");
            return builder.ToString();
        }
    }

    public enum LineType
    {
        Column, Row
    }
}
