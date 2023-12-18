using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdventOfCode2023.API.Controllers;

[Route("day17new")]
[ApiController]
public class _17_Day17_New_Controller : ControllerBase
{
    [HttpGet("exercise1")]
    public IActionResult Exercise1()
    {
        var lines = System.IO.File.ReadAllLines("data17.txt");

        var array = ReadLines(lines, out var height, out var width);

        var resultsArray = new Cell[height, width];

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
        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            for (int j = 0; j < line.Length; j++)
            {
                array[i, j] = new Cell(int.Parse(line[j].ToString()), i, j);
            }
        }

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
    public int HeatLoss { get; set; }

    public int Row { get; set; }

    public int Column { get; set; }

    private int _totalHeatLossToDestination = 0;

    private string _lastThreeDirections = "";

    public Cell RightNeighbour { get; private set; } = null;

    public Cell LeftNeighbour { get; private set; } = null;

    public Cell UpperNeighbour { get; private set; } = null;

    public Cell LowerNeighbour { get; private set; } = null;

    public void AssignRight(Cell cell)
    {
        RightNeighbour = cell;
        cell.LeftNeighbour = this; 
    }

    public void AssignLeft(Cell cell)
    {
        LeftNeighbour = cell;
        cell.RightNeighbour = this;
    }

    public void AssignUp(Cell cell)
    {
        UpperNeighbour = cell;
        cell.LowerNeighbour = this;
    }

    public void AssignDown(Cell cell)
    {
        LowerNeighbour = cell;
        cell.UpperNeighbour = this;
    }

    public int GetTotalHeatLoss() => _totalHeatLossToDestination; 





}
