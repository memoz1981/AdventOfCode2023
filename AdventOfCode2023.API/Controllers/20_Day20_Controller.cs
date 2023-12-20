using Microsoft.AspNetCore.Mvc;

namespace AdventOfCode2023.API.Controllers;

[Route("day20")]
[ApiController]
public class _20_Day20_Controller : ControllerBase
{
    [HttpGet("exercise1")]
    public IActionResult Exercise1()
    {
        var lines = System.IO.File.ReadAllLines("data20.txt");

        foreach (var line in lines)
        {

        }

        return Ok();
    }
}
