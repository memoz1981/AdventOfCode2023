using Day1_ExtractNumbersFromText;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace AdventOfCode2023.API.Controllers
{
    [Route("day1")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class _1_ExtractNumbersFromText_Controller : ControllerBase
    {
        [HttpGet("exercise1")]
        public IActionResult Exercise1()
        {
            var numberExtractor = new DigitsOnlyNumberExtractor();
            var result = GetTotal(numberExtractor); 
            return Ok(result);
        }

        [HttpGet("exercise2")]
        public IActionResult Exercise2()
        {
            var numberExtractor = new DigitsAndTextNumberExtractor();
            var result = GetTotal(numberExtractor);
            return Ok(result);
        }

        private int GetTotal(NumberExtractor numberExtractor)
        {
            var fileName = "data.txt";
            int result = 0;

            using (var fileStream = System.IO.File.OpenRead(fileName))
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true))
            {
                String line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    var number = numberExtractor.ExtractNumber(line);
                    result += number;
                }
            }

            return result; 
        }
    }
}
