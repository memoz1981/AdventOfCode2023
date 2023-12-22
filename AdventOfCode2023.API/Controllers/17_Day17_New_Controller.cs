using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdventOfCode2023.API.Controllers;

[Route("day17new")]
[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
public class _17_Day17_New_Controller : ControllerBase
{
    [HttpGet("exercise1")]
    public IActionResult Exercise1()
    {
        var lines = System.IO.File.ReadAllLines("data17.txt");

        var array = ReadLines(lines, out var height, out var width);


        FindArrayLengths(array, height); 

        return Ok();
    }

    private static void FindArrayLengths(Cell[,] array, int height, int degree = 0)
    {
        if (degree == height - 1)
        {
            array[height - 1, height - 1] =  new Cell(array[height-1, height-1].HeatLoss, height-1, height-1);
            return; 
        }

        FindArrayLengths(array, height, degree + 1);
        var currentRow = degree; //0 => 0, height -1 => height - 1

        // for a degree - index starts at degree and ends in height -1 
        // for example for 0 it starts at 0 and ends in height -1
        // for height -1 => it starts at height -1 and ends in height -1 
        // same for the column indexes

        for (int j = degree + 1; j <= height-1; j++)
        {
            var heatLoss = array[currentRow, j].HeatLoss + array[currentRow + 1, j].HeatLoss;
            array[currentRow, j] = new Cell(heatLoss, currentRow, j);
        }
        var colDegreeMin = Math.Min(array[currentRow, degree + 1].HeatLoss, array[currentRow + 1, degree].HeatLoss);
        var heatLossNew = array[degree, degree].HeatLoss + array[degree + 1, degree + 1].HeatLoss + colDegreeMin;
        array[degree, degree] = new Cell(heatLossNew, degree, degree);

        //cols
        for (int i = degree + 1; i <= height - 1; i++)
        {
            var heatLoss = array[i, degree].HeatLoss + array[i, degree + 1].HeatLoss;
            array[i, degree] = new Cell(heatLoss, i, degree);
        }

    }

    private static Cell FindMinResultForFirstRowCell(int[,] array, Cell[,] resultsArray, int colIndex, 
        int height, int degree)
    {
        // find results for rows between 1 to height-1
        var rowCount = height - degree;
        var currentRow = degree; 
        int minValue = int.MaxValue; 
        for (int j = 0; j < rowCount; j++)
        {
            var heatLoss = FindMinDistanceToNextRowCols(array, currentRow, colIndex, j);
            minValue = Math.Min(heatLoss, minValue); 
        }

        return new Cell(minValue, degree, colIndex); 
    }

    public static int FindMinDistanceToNextRowCols(int[,] array, int rowIndex, int colIndex, int nextColIndex)
    {
        (int upperRow, int lowerRow) = (array[rowIndex, colIndex], array[rowIndex + 1, colIndex]);
        if (nextColIndex == colIndex)
        {
            return array[rowIndex, colIndex];
        }

        else if (nextColIndex > colIndex)
        {
            
            for (int i = colIndex; i <nextColIndex; i++)
            {
                upperRow += Math.Min(array[rowIndex, colIndex + 1],
                    array[rowIndex + 1, colIndex] + array[rowIndex + 1, colIndex + 1]);
                lowerRow += Math.Min(array[rowIndex, colIndex + 1], array[rowIndex + 1, colIndex]);
                lowerRow += array[rowIndex + 1, colIndex + 1]; 
            }

            
        }
        
        for (int i = colIndex - 1; i >= nextColIndex; i--)
        {
            upperRow += Math.Min(array[rowIndex, colIndex - 1],
                array[rowIndex + 1, colIndex] + array[rowIndex + 1, colIndex - 1]);
            lowerRow += Math.Min(array[rowIndex, colIndex - 1], array[rowIndex + 1, colIndex]);
            lowerRow += array[rowIndex + 1, colIndex - 1];
        }

        return lowerRow;

    }

    private Cell[,] ReadLines(string[] lines, out int height, out int width)
    {
        height = lines.Length;
        width = lines[0].Length;
        var array = new Cell[height, width];
        var rowSums = new int[height];
        var colSums = new int[width]; 
        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            for (int j = 0; j < line.Length; j++)
            {
                array[i, j] = new Cell(int.Parse(line[j].ToString()), i, j);
                rowSums[i] += int.Parse(line[j].ToString());
                colSums[j] += int.Parse(line[j].ToString());
            }
        }

        var distinctRows = rowSums.Distinct().OrderBy(m=>m).ToList();
        var distinctCols = colSums.Distinct().OrderBy(m => m).ToList(); 


        return array;
    }
}

public class Cell
{
    public Cell(int heatLoss, int row, int column)
    {
        HeatLoss = heatLoss; 
        Row = row;
        Column = column; 
    }

    public int Degree { get => Math.Max(Row, Column); }
    public int HeatLoss { get; set; }

    public int Row { get; set; }

    public int Column { get; set; }

    private int _totalHeatLossToDestination = 0;

    private string _lastThreeDirections = "";

    private Dictionary<char, Cell> _neighbours = new();
    
    public Cell RightNeighbour { get => _neighbours.ContainsKey('R') ? _neighbours['R'] : null; }

    public Cell LeftNeighbour { get => _neighbours.ContainsKey('L') ? _neighbours['L'] : null; }

    public Cell UpperNeighbour { get => _neighbours.ContainsKey('U') ? _neighbours['U'] : null; }

    public Cell LowerNeighbour { get => _neighbours.ContainsKey('D') ? _neighbours['D'] : null; }

    public List<Cell> NeighboursOfSameOrLowerDegree 
    { get => _neighbours.Select(m => m.Value).Where(m => m.Degree >= Degree).ToList(); }

    public void AssignRight(Cell cell)
    {
        _neighbours['R'] = cell;
        cell.AssignLeft(this); 
    }

    public void AssignLeft(Cell cell)
    {
        _neighbours['L'] = cell;
        cell.AssignRight(this);
    }

    public void AssignUp(Cell cell)
    {
        _neighbours['U'] = cell;
        cell.AssignDown(this);
    }

    public void AssignDown(Cell cell)
    {
        _neighbours['D'] = cell;
        cell.AssignUp(this);
    }

    public List<(List<char> path,int heatLoss)>  GetAllPathsToLowerOrSameDegree(int height, int stopAtDegree, HashSet<(int row, int column)> traversed = null)
    {
        if (!NeighboursOfSameOrLowerDegree.Any())
            return new() { ( Array.Empty<char>().ToList(), 3 ) };
        
        traversed = traversed ?? new HashSet<(int row,int column)>();
        //path = path ?? new List<char>(); 

        var traversedCopy = traversed.Select(m => m).ToHashSet();
        //var pathCopy = path.Select(m => m).ToList();

        traversedCopy.Add((Row, Column)); 

        foreach (var neighbour in _neighbours)
        {
            if (traversed.Contains((Row, Column)))
            {
                continue; 
            }
            if (neighbour.Value.Degree == Degree - 1)
            {
                //continue - go only below right
                continue; 
            }
            else if (neighbour.Value.Degree == Degree)
            {
                var pathsOfNeighbour = neighbour.Value.GetAllPathsToLowerOrSameDegree(height, Degree + 1, traversedCopy);

                foreach (var path in pathsOfNeighbour)
                {

                }
            }
            else if (neighbour.Value.Degree == Degree + 1)
            {
                foreach (var item in neighbour.Value.HeatLossMap)
                {
                    string key = item.Key;

                    if (key.Length >= 2)
                    {
                        key = key.Substring(0, 2);  
                    }

                    var keyChar = new List<char>();
                    keyChar.Add(neighbour.Key);
                    keyChar.AddRange(key.ToCharArray());

                    if (keyChar.Count == 3 && keyChar.Distinct().Count() == 1)
                        continue;
                    else
                    {
                        
                    }

                    key = $"{neighbour.Key.ToString()}{key}";
                }

            }


            
        }
        return null;
    }

    public Dictionary<string, int> HeatLossMap { get; set; } = new(); 

    public int GetTotalHeatLoss() => _totalHeatLossToDestination;

    public int GetDistanceToLowerRowElement()
    {
        return 0; 
    }
}
