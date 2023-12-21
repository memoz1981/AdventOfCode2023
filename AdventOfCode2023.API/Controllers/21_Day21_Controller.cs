using Microsoft.AspNetCore.Mvc;

namespace AdventOfCode2023.API.Controllers
{
    [Route("day21")]
    [ApiController]
    public class _21_Day21_Controller : ControllerBase
    {
        [HttpGet("exercise1")]
        public IActionResult Exercise1()
        {
            var lines = System.IO.File.ReadAllLines("data21.txt");

            foreach (var line in lines)
            {
                
            }
            
            return Ok(); 
        }
    }
}
