using Microsoft.AspNetCore.Mvc;

namespace AdventOfCode2023.API.Controllers;
[Route("day22")]
[ApiController]
public class _22_Day22_Controller : ControllerBase
{
    [HttpGet("exercise1")]
    public IActionResult Exercise1()
    {
        var lines = System.IO.File.ReadAllLines("data22.txt");

        var bricks = lines
            .Select(l => new BrickLine(l))
            .OrderBy(m => m.GetLowestInitialLevel())
            .ThenBy(m => m.X)
            .ThenBy(m => m.Y)
            .ToHashSet();

        var brickCount = bricks.Count; 

        var occupied = new Dictionary<(int x, int y, int z), BrickLine>();

        foreach (var brick in bricks)
            brick.Fall(occupied);

        var result = bricks.Where(m => m.Sources.Count == 1).Select(m => m.Sources.First()).Distinct().Count(); 

        
        return Ok(result);
    }

    [HttpGet("exercise2")]
    public IActionResult Exercise2()
    {
        var lines = System.IO.File.ReadAllLines("data22.txt");

        var bricks = lines
            .Select(l => new BrickLine(l))
            .OrderBy(m => m.GetLowestInitialLevel())
            .ThenBy(m => m.X)
            .ThenBy(m => m.Y)
            .ToHashSet();

        var brickCount = bricks.Count;

        var occupied = new Dictionary<(int x, int y, int z), BrickLine>();

        foreach (var brick in bricks)
            brick.Fall(occupied);

        foreach (var brick in bricks)
            brick.Destinations = brick.Destinations.Distinct().ToList();

        var brickFallList = new Dictionary<BrickLine, List<BrickLine>>();

        foreach (var brick in bricks)
        {
            var brickSourcesByLevel = brick.AllSources.GroupBy(m => m.FinalZ.zEnd)
                .ToDictionary(m => m.Key, m => m.ToList()).Where(m => m.Value.Count == 1).ToList();

            foreach (var item in brickSourcesByLevel)
            {
                foreach (var element in item.Value)
                {
                    if (!brickFallList.ContainsKey(element))
                    {
                        brickFallList.Add(element, new() { brick });
                    }
                    else
                    {
                        brickFallList[element].Add(brick);
                    }
                }
            }
        }

        int result = 0;

        foreach (var element in brickFallList)
        {
            var bricksToFall = element.Value.Distinct().ToList();

            result += bricksToFall.Count; 
        }

        return Ok(result);
    }

    [HttpGet("exercise2new")]
    public IActionResult Exercise2New()
    {
        var lines = System.IO.File.ReadAllLines("data22.txt");

        var bricks = lines
            .Select(l => new BrickLine(l))
            .OrderBy(m => m.GetLowestInitialLevel())
            .ThenBy(m => m.X)
            .ThenBy(m => m.Y)
            .ToHashSet();

        var brickCount = bricks.Count;

        var occupied = new Dictionary<(int x, int y, int z), BrickLine>();

        foreach (var brick in bricks)
            brick.Fall(occupied);

        foreach (var brick in bricks)
            brick.Destinations = brick.Destinations.Distinct().ToList();

        var bricksToCauseFall = bricks.Where(m => m.Sources.Count == 1).Select(m => m.Sources.First()).Distinct().ToList();

        int result = 0;

        foreach (var element in bricksToCauseFall)
        {
            var bricksToFall = bricks.Where(m => m.SingleSources.Contains(element)).Distinct().ToList();

            result += bricksToFall.Count;
        }

        return Ok(result);
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

        HasFallen = false;
        Name = Guid.NewGuid(); 
        name = line; 

        PopulateOccupied(); 

        Orientation = (X.xStart != X.xEnd) ? BrichLineOrientation.X : 
            (Y.yStart != Y.yEnd) ? BrichLineOrientation.Y 
            : BrichLineOrientation.Z;

        Sources = new();
        Destinations = new();
        AllSources = new();
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
        if (name.Contains("3,4,4~3,4,4"))
        {
            var a = 1;
        }

        while (!occupied.Keys.Any(m => Occupied.Any(n => (n.x == m.x && n.y == m.y && n.z-1 == m.z))) && !Occupied.Any(m => m.z-1<1))
        {
            FinalZ = (FinalZ.zStart - 1, FinalZ.zEnd - 1);
            PopulateOccupied(); 
        }

        var supports = occupied.Where(m => Occupied.Any(n => (n.x == m.Key.x && n.y == m.Key.y && n.z - 1 == m.Key.z))).ToList();

        AddSources(supports.Select(m => m.Value).Distinct().ToList());

        foreach (var value in Occupied)
            occupied.Add(value, this);
    }

    public List<BrickLine> Sources { get; set; }
    public List<BrickLine> Destinations { get; set; }

    public List<BrickLine> AllSources { get; set; }

    public List<BrickLine> SingleSources { get; set; }

    public void AddSources(List<BrickLine> supports)
    {
        if (!supports.Any())
            return;

        AllSources.AddRange(supports);

        AllSources.AddRange(supports.SelectMany(m => m.AllSources).Distinct());

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

        foreach (var source in AllSources)
            source.Destinations.Add(this); 

        Sources.AddRange(supports);
    }

    public bool CanRemove(Dictionary<(int x, int y, int z), Guid> occupied)
    {
        return false; 
    }

    public int GetLowestInitialLevel() => Math.Min(InitialZ.zStart, InitialZ.zEnd);

    public HashSet<(int x, int y, int z)> Occupied { get; set; }

    public Guid Name { get; set; }

    public BrichLineOrientation Orientation { get; set; }
    
    public (int xStart, int xEnd) X { get; set; }

    public (int yStart, int yEnd) Y { get; set; }

    public (int zStart, int zEnd) InitialZ { get; set; }

    public (int zStart, int zEnd) FinalZ { get; set; }

    public bool HasFallen { get; set; }
}

public enum BrichLineOrientation { X, Y, Z}
