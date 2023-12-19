using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdventOfCode2023.API.Controllers
{
    [Route("day19")]
    [ApiController]
    public class _19_Day19_Controller : ControllerBase
    {
        [HttpGet("exercise1")]
        public IActionResult Exercise1()
        {
            var lines = System.IO.File.ReadAllLines("data19.txt").ToArray();

            int result = 0;


            return Ok(result);
        }
    }
}
