using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdventOfCode2023.API.Controllers
{
    [Route("day18")]
    [ApiController]
    public class _18_Day18_Controller : ControllerBase
    {
        [HttpGet("exercise1")]
        public IActionResult Exercise1()
        {
            var lines = System.IO.File.ReadAllLines("data18.txt");

            foreach (var line in lines)
            {
                
            }

            return Ok();
        }
    }
}
