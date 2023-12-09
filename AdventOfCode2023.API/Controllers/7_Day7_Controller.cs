using Common.Services;
using Day7_CamelCards;
using Microsoft.AspNetCore.Mvc;

namespace AdventOfCode2023.API.Controllers
{
    [Route("day7")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class _7_Day7_Controller : ControllerBase
    {
        [HttpGet("exercise1")]
        public IActionResult Exercise1()
        {
            var lineReader = new StringLineReader();
            var lines = lineReader.ReadLines("data7.txt");
            var hands = new List<Hand>();
            foreach (var line in lines)
            {
                hands.Add(new Hand(line)); 
            }
            var handsOrdered = hands.OrderBy(h => h).ToList();
            long result = 0; 
            for (int i = 0; i < handsOrdered.Count; i++)
            {
                result += handsOrdered[i].BidAmount * (i + 1);
            }
            return Ok(result);
        }

        [HttpGet("exercise2")]
        public IActionResult Exercise2()
        {
            var lineReader = new StringLineReader();
            var lines = lineReader.ReadLines("data7.txt");
            var hands = new List<HandWithJoker>();
            foreach (var line in lines)
            {
                hands.Add(new HandWithJoker(line));
            }
            var handsOrdered = hands.OrderBy(h => h).ToList();
            long result = 0;
            for (int i = 0; i < handsOrdered.Count; i++)
            {
                result += handsOrdered[i].BidAmount * (i + 1);
            }
            return Ok(result);
        }
    }
}
