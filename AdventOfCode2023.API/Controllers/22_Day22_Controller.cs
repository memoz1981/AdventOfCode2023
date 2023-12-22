using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdventOfCode2023.API.Controllers
{
    [Route("day22")]
    [ApiController]
    public class _22_Day22_Controller : ControllerBase
    {
        [HttpGet("exercise1")]
        public IActionResult Exercise1()
        {
            var lines = System.IO.File.ReadAllLines("data22.txt");

            foreach (var line in lines)
            { 
            }

            return Ok(); 
        }
    }
}
