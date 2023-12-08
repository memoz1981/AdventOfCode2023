using Common.Services;
using Day5_Seeds;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdventOfCode2023.API.Controllers
{
    [Route("day6")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class _6_Day6_Controller : ControllerBase
    {
        private const string dataFile = "data6.txt"; 
        [HttpGet("exercise1")]
        public IActionResult Exercise1()
        {
            var moves = new List<Move>();
            moves.Add(new Move(46, 358));
            moves.Add(new Move(68, 1054));
            moves.Add(new Move(98, 1807));
            moves.Add(new Move(66, 1080));
            int result = 1;
            foreach (var move in moves)
            {
                int numWays = 0;

                for (int i = 0; i <= move.TimeTotal; i++)
                {
                    var distanceTravelled = i * (move.TimeTotal - i); 
                    if (distanceTravelled > move.DistanceTotal)
                        numWays++;
                }
                result = result * numWays;
            }
            return Ok(result);
        }

        [HttpGet("exercise2")]
        public IActionResult Exercise2()
        {
            var moves = new List<Move>();
            moves.Add(new Move(46689866, 358105418071080));
            int result = 1;
            foreach (var move in moves)
            {
                int numWays = 0;

                for (int i = 0; i <= move.TimeTotal; i++)
                {
                    var distanceTravelled = i * (move.TimeTotal - i);
                    if (distanceTravelled > move.DistanceTotal)
                        numWays++;
                }
                result = result * numWays;
            }
            return Ok(result);
        }

        public class Move
        {
            public Move(long timeTotal, long distanceTotal)
            {
                TimeTotal = timeTotal;
                DistanceTotal = distanceTotal;
            }

            public long TimeTotal { get; set; }

            public long DistanceTotal { get; set; }
        }
    }
}
