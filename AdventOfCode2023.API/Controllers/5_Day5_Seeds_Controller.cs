using Common.Services;
using Day5_Seeds;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdventOfCode2023.API.Controllers
{
    [Route("day5new")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class _5_Day5_Seeds_Controller : ControllerBase
    {
        [HttpGet("exercise1")]
        public IActionResult Exercise1()
        {
            var lineReader = new StringLineReader();
            var lines = lineReader.ReadLines("data5.txt");

            var seedLine = lines[0];
            var idxStart = seedLine.IndexOf(':') + 2;

            var numStr = seedLine.Substring(idxStart, seedLine.Length - idxStart);
            var seedNumbers = numStr.Split(' ').Select(m => long.Parse(m)).ToList();

            var seeds = BuildSeedMapForEx1(seedNumbers);

            var soils = BuildMapping("seed-to-soil map:", "soil-to-fertilizer map:", lines);
            var fertilizers = BuildMapping("soil-to-fertilizer map:", "fertilizer-to-water map:", lines);
            var water = BuildMapping("fertilizer-to-water map:", "water-to-light map:", lines);
            var light = BuildMapping("water-to-light map:", "light-to-temperature map:", lines);
            var temperature = BuildMapping("light-to-temperature map:", "temperature-to-humidity map:", lines);
            var humidity = BuildMapping("temperature-to-humidity map:", "location", lines);
            var location = BuildMapping("location", "new mapping", lines);

            long result = FindResult(seeds, soils, fertilizers, water, light, temperature, humidity, location);

            return Ok(result); 
        }

        private long FindResult(MappingDictionary seeds, MappingDictionary soils, MappingDictionary fertilizers, MappingDictionary water, MappingDictionary light, MappingDictionary temperature, MappingDictionary humidity, MappingDictionary location)
        {
            var locs = seeds
                .Cascade(soils)
                .Cascade(fertilizers)
                .Cascade(water)
                .Cascade(light)
                .Cascade(temperature)
                .Cascade(humidity)
                .Cascade(location);

            return locs.SourceMaps.OrderBy(m => m.DestinationStart).First().DestinationStart; 
        }

        private MappingDictionary BuildSeedMapForEx1(List<long> seedData)
        {
            var result = new List<Mapping>();

            foreach (var seed in seedData)
            {
                result.Add(new Mapping(seed, seed, 1));
            }
            return new MappingDictionary(result);
        }

        [HttpGet("exercise2")]
        public IActionResult Exercise2()
        {
            var lineReader = new StringLineReader();
            var lines = lineReader.ReadLines("data5.txt");

            var seedLine = lines[0];
            var idxStart = seedLine.IndexOf(':') + 2;

            var numStr = seedLine.Substring(idxStart, seedLine.Length - idxStart);
            var seedNumbers = numStr.Split(' ').Select(m => long.Parse(m)).ToList();

            var seeds = BuildSeedMapForEx2(seedNumbers);

            var soils = BuildMapping("seed-to-soil map:", "soil-to-fertilizer map:", lines);
            var fertilizers = BuildMapping("soil-to-fertilizer map:", "fertilizer-to-water map:", lines);
            var water = BuildMapping("fertilizer-to-water map:", "water-to-light map:", lines);
            var light = BuildMapping("water-to-light map:", "light-to-temperature map:", lines);
            var temperature = BuildMapping("light-to-temperature map:", "temperature-to-humidity map:", lines);
            var humidity = BuildMapping("temperature-to-humidity map:", "location", lines);
            var location = BuildMapping("location", "new mapping", lines);

            long result = FindResult(seeds, soils, fertilizers, water, light, temperature, humidity, location);

            return Ok(result);
        }

        private MappingDictionary BuildSeedMapForEx2(List<long> seedData)
        {
            var result = new List<Mapping>(); 
            for (int i = 0; i < seedData.Count; i = i + 2)
            {
                result.Add(new Mapping(seedData[i], seedData[i], seedData[i + 1]));
            }

            return new MappingDictionary(result);
        }

        private MappingDictionary BuildMapping(string mapName, string untilMapName, List<string> lines)
        {
            var result = new List<Mapping>();
            bool started = false;
            for (int i = 0; i < lines.Count; i++)
            {
                var line = lines[i];

                if (line.Contains(mapName))
                {
                    started = true;
                    continue;
                }

                if (line.Contains(untilMapName))
                    break;

                if (!started)
                    continue;

                var numbers = line.Split(' ').Where(m => long.TryParse(m, out _)).Select(m => long.Parse(m)).ToArray();

                if (numbers.Length != 3)
                    continue;

                result.Add(new Mapping(numbers[0], numbers[1], numbers[2]));
            }
            return new MappingDictionary(result);
        }
    }
}
