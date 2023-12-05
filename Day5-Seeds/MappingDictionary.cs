namespace Day5_Seeds
{
    public class MappingDictionary
    {
        public List<Mapping> SourceMaps { get; }

        public MappingDictionary(List<Mapping> sourceMaps)
        {
            SourceMaps = sourceMaps;
        }

        public MappingDictionary Cascade(MappingDictionary destination)
        {
            var result = new List<Mapping>();
            foreach (var map in SourceMaps.OrderBy(m => m.DestinationStart))
            {
                var matchesForSrcMap = destination
                    .SourceMaps
                    .Where(dest => dest.MatchesSource(map))
                    .OrderBy(dest => dest.SourceStart);

                if (!matchesForSrcMap.Any())
                    result.Add(map);
                else
                {
                    var maps = ReturnMappings(map, matchesForSrcMap);
                    result.AddRange(maps);
                }
            }
            return new MappingDictionary(result); 
        }

        private List<Mapping> ReturnMappings(Mapping map, IOrderedEnumerable<Mapping> dest)
        {
            var start = map.DestinationStart;
            var result = new List<Mapping>();

            foreach (var item in dest)
            {
                if (start > map.DestinationEnd)
                    break; 
                var srcStart = item.SourceStart;

                if (start < srcStart)
                {
                    result.Add(new Mapping(map.DestinationStart, map.SourceStart, srcStart - start));
                }

                var destStart = Math.Max(start, srcStart);
                var destEnd = Math.Min(map.DestinationEnd, item.SourceEnd);

                result.Add(new Mapping(item.DestinationForSource(destStart), destStart, destEnd - destStart + 1));

                start = destEnd + 1; 
            }

            if (map.DestinationEnd > start)
                result.Add(new Mapping(start, map.SourceForDestination(start), map.DestinationEnd - start + 1));

            return result; 
        }
    }
}
