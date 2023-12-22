using Microsoft.AspNetCore.Mvc;

namespace AdventOfCode2023.API.Controllers;
[Route("day22")]
[ApiController]
public class _22_Day22_Controller : ControllerBase
{
    [HttpGet("exercise1")]
    public IActionResult Exercise1()
    {
        ReadLinesReturnBricksAndSingleSources(out var bricks, out var bricksToCauseFall);

        var result = bricks.Count - bricksToCauseFall.Count;

        return Ok(result);
    }

    [HttpGet("exercise2")]
    public IActionResult Exercise2()
    {
        ReadLinesReturnBricksAndSingleSources(out var bricks, out var bricksToCauseFall);

        int result = 0;

        foreach (var element in bricksToCauseFall)
        {
            var bricksToFall = bricks
                .Where(m => m.SingleSources.Contains(element))
                .Distinct()
                .ToList();

            result += bricksToFall.Count;
        }

        return Ok(result);
    }

    private static void ReadLinesReturnBricksAndSingleSources(out List<BrickLine> bricks, out List<BrickLine> bricksToCauseFall)
    {
        var lines = System.IO.File.ReadAllLines("data22.txt");

        bricks = lines
            .Select(l => new BrickLine(l))
            .OrderBy(m => m.GetLowestInitialLevel())
            .ThenBy(m => m.X)
            .ThenBy(m => m.Y)
            .ToList();

        var occupied = new Dictionary<(int x, int y, int z), BrickLine>();

        foreach (var brick in bricks)
            brick.Fall(occupied);

        bricksToCauseFall = bricks
            .SelectMany(m => m.SingleSources)
            .Distinct()
            .ToList();
    }
}

public class BrickLine
{
    public BrickLine(string line)
    {
        var positions = line.Trim().Split('~');
        var start = positions[0].Split(',').Select(m => int.Parse(m.Trim())).ToList();
        var end = positions[1].Split(',').Select(m => int.Parse(m.Trim())).ToList();

        X = (start[0], end[0]);
        Y = (start[1], end[1]);
        InitialZ = (start[2], end[2]);
        FinalZ = (start[2], end[2]);

        name = line; 

        PopulateOccupied(); 

        SingleSources = new(); 
    }

    private string name = "";

    public override string ToString()
    {
        return name;
    }

    private void PopulateOccupied()
    {
        Occupied = new();
        for (int i = X.xStart; i <= X.xEnd; i++)
        {
            for (int j = Y.yStart; j <= Y.yEnd; j++)
            {
                for (int k = FinalZ.zStart; k <= FinalZ.zEnd; k++)
                {
                    Occupied.Add((i, j, k));
                }
            }

        }
    }

    public void Fall(Dictionary<(int x, int y, int z), BrickLine> occupied)
    {
        while (!occupied.Keys.Any(m => Occupied.Any(n => (n.x == m.x && n.y == m.y && n.z-1 == m.z))) && !Occupied.Any(m => m.z-1 < 1))
        {
            FinalZ = (FinalZ.zStart - 1, FinalZ.zEnd - 1);
            PopulateOccupied(); 
        }

        //lower plane supports
        var supports = occupied
            .Where(m => Occupied.Any(n => (n.x == m.Key.x && n.y == m.Key.y && n.z - 1 == m.Key.z)))
            .ToList();

        AddSources(supports.Select(m => m.Value).Distinct().ToList());

        foreach (var value in Occupied)
            occupied.Add(value, this);
    }

    public List<BrickLine> SingleSources { get; set; }

    public void AddSources(List<BrickLine> supports)
    {
        if (!supports.Any())
            return;

        if (supports.Count == 1)
        {
            SingleSources.Add(supports.First()); 
        }

        var additionalSingle = new List<BrickLine>();

        foreach (var element in supports.First().SingleSources)
        {
            if (supports.Any(m => !m.SingleSources.Contains(element)))
            {
                continue; 
            }
            additionalSingle.Add(element); 
        }

        SingleSources.AddRange(additionalSingle.Distinct()); 
    }

    public int GetLowestInitialLevel() => Math.Min(InitialZ.zStart, InitialZ.zEnd);

    public HashSet<(int x, int y, int z)> Occupied { get; set; }

    public (int xStart, int xEnd) X { get; set; }

    public (int yStart, int yEnd) Y { get; set; }

    public (int zStart, int zEnd) InitialZ { get; set; }

    public (int zStart, int zEnd) FinalZ { get; set; }
}
