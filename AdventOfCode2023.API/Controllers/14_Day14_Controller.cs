using Common.Services;
using Microsoft.AspNetCore.Mvc;

namespace AdventOfCode2023.API.Controllers
{
    [Route("day14")]
    [ApiController]
    public class _14_Day14_Controller : ControllerBase
    {
        [HttpGet("exercise1")]
        public IActionResult Exercise1()
        {
            var lineReader = new StringLineReader();
            var lines = lineReader.ReadLines("data14.txt");

            int result = 0; 
            foreach (var line in lines)
            {
               
            }

            return Ok(result);
        }
    }
}
