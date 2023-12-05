namespace Day5_Seeds
{
    public class Mapping
    {
        public Mapping(long destStart, long srcStart, long length)
        {
            DestinationStart = destStart;
            SourceStart = srcStart;
            Length = length;
            DestinationEnd = DestinationStart + Length - 1;
            SourceEnd = SourceStart + Length - 1; 
        }
        public long DestinationStart { get; }

        public long SourceStart { get; }

        public long Length { get; }

        public long SourceEnd { get; }

        public long DestinationEnd { get; }

        public long DestinationForSource(long source)
        {
            return source - SourceStart + DestinationStart; 
        }

        public long SourceForDestination(long destination)
        {
            return destination - DestinationStart + SourceStart; 
        }

        public bool MatchesSource(Mapping source)
        {
            if (source.DestinationStart > SourceEnd)
                return false;

            if (source.DestinationEnd < SourceStart)
                return false;

            return true; 
        }
    }
}
