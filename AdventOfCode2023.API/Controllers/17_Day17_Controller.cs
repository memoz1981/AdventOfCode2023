using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdventOfCode2023.API.Controllers;
[Route("day17")]
[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
public class _17_Day17_Controller : ControllerBase
{
    [HttpGet("exercise1")]
    public IActionResult Exercise1()
    {
        var lines = System.IO.File.ReadAllLines("data17.txt");

        var array = ReadLines(lines, out var height, out var width);

        var resultsArray = new int[height, width];

        var res = FindMinDistance(array, height, width);

        return Ok(res);
    }

    private static int FindMinDistance(int[,] array, int height, int width,
        HashSet<string> traversed = null,
        int startRow = 0, int startCol = 0,
        List<BeamDirection> lastThreeDirections = null,
        BeamDirection nextDirection = BeamDirection.Right)
    {
        if (lastThreeDirections == null)
        {
            var a = 1; 
        }
        
        lastThreeDirections = lastThreeDirections ?? new List<BeamDirection>();

        traversed = traversed ?? new HashSet<string>();

        if (startRow == height - 1 && startCol == width - 1)
            return array[startCol, startRow];

        traversed.Add(nextDirection.ReturnDirectionKey(lastThreeDirections, startRow, startCol));
        var results = new List<int>(); 
        
        var directions = GetNextDirections(lastThreeDirections, nextDirection, startRow, startCol, height, width);

        if (directions.Count == 0)
        {
            var b = 1; 
        }

        foreach (var direction in directions)
        {
            var increments = direction.next.GetIncrements();
            int rowNext = startRow + increments.rowIncrement;
            int colNext = startCol + increments.colIncrement;

            if (direction.lastThree == null)
            {
                var a = 1;
            }

            var nextKey = direction.next.ReturnDirectionKey(direction.lastThree, rowNext, colNext);
            if (traversed.Contains(nextKey))
                continue;

            var result = FindMinDistance(array, height, width, traversed, startRow + increments.rowIncrement,
                startCol + increments.colIncrement, direction.lastThree, direction.next);
            
            results.Add(result);
        }

        var resultsMin = results.OrderBy(m => m).FirstOrDefault();

        if (!results.Any())
        {
            return int.MaxValue; 
        }

        return resultsMin + array[startRow, startCol]; 
    }

    //private static void FindMinHeatLoss(int[,] array, int[,] resultsArray, int height, int width,
    //    HashSet<(int row, int col)> traversed = null,
    //    int startRow = 0, int startCol = 0,
    //    List<BeamDirection> lastThreeDirections = null,
    //    BeamDirection nextDirection = BeamDirection.Right)
    //{
    //    lastThreeDirections = lastThreeDirections ?? new List<BeamDirection>();
    //    traversed = traversed ?? new HashSet<(int col, int row)>();
    //    if (traversed.Contains((startRow, startCol)))
    //        return;

    //    if (startRow == height - 1 && startCol == width - 1)
    //        return;

    //    traversed.Add((startRow, startCol));

    //    resultsArray[startRow, startCol] += array[startRow, startCol];

    //    var directions = GetNextDirections(lastThreeDirections, nextDirection, startRow, startCol, height, width);

    //    foreach (var direction in directions)
    //    {
    //        var increments = direction.GetIncrements();

    //        var nextLastThreeDirections = lastThreeDirections.Select(m => m).ToList();
    //        if (lastThreeDirections.Count < 3)
    //        {
    //            nextLastThreeDirections.Add(direction);
    //        }
    //        else
    //        {
    //            nextLastThreeDirections = new List<BeamDirection> { lastThreeDirections[1], lastThreeDirections[2], nextDirection };
    //        }


    //        FindMinHeatLoss(array, resultsArray, height, width, traversed, startRow + increments.rowIncrement,
    //            startCol + increments.colIncrement, nextLastThreeDirections, direction); 
    //    }



    //}

    public static List<(BeamDirection next, List<BeamDirection> lastThree)> GetNextDirections(List<BeamDirection> lastThreeDirections,
        BeamDirection nextDirection, int row, int col, int height, int width)
    {
        var allDirections = new List<BeamDirection>
        { BeamDirection.Up, BeamDirection.Down,
            BeamDirection.Right, BeamDirection.Left};

        var potentialDirections = allDirections
            .Where(dir =>
            (dir != nextDirection.GetReverse())
            && (row > 0 || dir != BeamDirection.Up)
            && (row < height - 1 || dir != BeamDirection.Down)
            && (col > 0 || dir != BeamDirection.Left)
            && (col < width - 1 || dir != BeamDirection.Right)
            )
            .ToList();

        var count = lastThreeDirections.Count;
        var list = new List<BeamDirection>();

        if (count == 0)
        {
            //no filtering
        }
        else if (count == 1)
        {
            //no filtering
            list.Add(lastThreeDirections.Last());
        }
        else if (count == 2)
        {
            if (nextDirection == lastThreeDirections[0] && nextDirection == lastThreeDirections[1])
                potentialDirections = potentialDirections.Where(dir => dir != nextDirection).ToList();

            list.Add(lastThreeDirections[0]);
            list.Add(lastThreeDirections[1]);

        }
        else if (count == 3)
        {
            if (nextDirection == lastThreeDirections[2] && nextDirection == lastThreeDirections[1])
                potentialDirections = potentialDirections.Where(dir => dir != nextDirection).ToList();
            list.Add(lastThreeDirections[1]);
            list.Add(lastThreeDirections[2]);
        }
        else
        {
            throw new ArgumentException(); 
        }

        list.Add(nextDirection);

        var result = new List<(BeamDirection, List<BeamDirection> directions)>(); 

        foreach (var item in potentialDirections)
        {
            result.Add((item, list)); 
        }

        if (!result.Any())
        {
            var a = 1; 
        }

        return result; 
    }

    private int[,] ReadLines(string[] lines, out int height, out int width)
    {
        height = lines.Length;
        width = lines[0].Length;
        var array = new int[height, width];
        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            for (int j = 0; j < line.Length; j++)
            {
                array[i, j] = int.Parse(line[j].ToString());
            }
        }

        return array;
    }
}

public static class DirectionExtensions
{
    public static BeamDirection GetReverse(this BeamDirection direction)
    {
        return direction switch
        {
            BeamDirection.Up => BeamDirection.Down,
            BeamDirection.Down => BeamDirection.Up,
            BeamDirection.Right => BeamDirection.Left,
            BeamDirection.Left => BeamDirection.Right,

            _ => throw new ArgumentException()
        };
    }

    public static (int rowIncrement, int colIncrement) GetIncrements(this BeamDirection direction)
    {
        return direction switch
        {
            BeamDirection.Up => (-1, 0),
            BeamDirection.Down => (1, 0),
            BeamDirection.Left => (0, -1),
            BeamDirection.Right => (0, 1),

            _ => throw new ArgumentException()
        }; 
    }

    public static char ReturnDirectionChar(this BeamDirection direction)
    {
        return direction switch
        {
            BeamDirection.Up => 'U',
            BeamDirection.Down => 'D',
            BeamDirection.Right => 'R',
            BeamDirection.Left => 'L',
            _ => throw new ArgumentException()
        };
    }

    public static string ReturnDirectionKey(this BeamDirection direction, List<BeamDirection> lastThree, int row, int col)
    {
        var lastThreeList = lastThree.Select(m => m.ReturnDirectionChar()).ToList();

        lastThreeList.Add(direction.ReturnDirectionChar());
        return $"{new string(lastThreeList.ToArray())} - {row} - {col}";
    }
}


