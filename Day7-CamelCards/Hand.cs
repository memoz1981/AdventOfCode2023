namespace Day7_CamelCards
{
    public class Hand : IComparable<Hand>
    {
        public Hand(string line)
        {
            Cards = line.Split(' ').First().Trim().ToCharArray();
            BidAmount = long.Parse(line.Split(' ').Last().Trim());
        }

        public Hand(char[] cards, long bidAmount)
        {
            Cards = cards;
            BidAmount = bidAmount;
        }

        public char[] Cards { get; set; }

        public long BidAmount { get; set; }

        public virtual Type GetCardType()
        {
            var distinct = Cards.Distinct().ToList();

            var maxRepetition = Cards.GroupBy(x => x)
               .Select(x => new { Char = x.Key, Count = x.Count() })
               .OrderByDescending(x => x.Count)
               .First().Count;

            return (distinct.Count, maxRepetition) switch
            {
                (1, _) => Type.FiveOfAKind,
                (2, 4) => Type.FourOfAKind,
                (2, 3) => Type.FullHouse,
                (3, 3) => Type.ThreeOfAKind,
                (3, 2) => Type.TwoPair,
                (4, _) => Type.OnePair,
                (5, _) => Type.HighCard,

                _ => throw new ArgumentException()
            };
        }

        public int CompareTo(Hand other)
        {
            var typeThis = GetCardType();
            var typeOther = other.GetCardType();

            if ((int)typeThis > (int)typeOther)
                return 1;
            if ((int)typeThis < (int)typeOther)
                return -1;

            for (int i = 0; i < 5; i++)
            {
                if (_values[Cards[i]] > _values[other.Cards[i]])
                    return 1;
                if (_values[Cards[i]] < _values[other.Cards[i]])
                    return -1;
                else
                    continue;
            }
            return 0;
        }

        protected Dictionary<char, int> _values = new()
        {
            { 'A', 20 },
            { 'K', 19 },
            { 'Q', 18 },
            { 'J', 17 },
            { 'T', 16 },
            { '9', 15 },
            { '8', 14 },
            { '7', 13 },
            { '6', 12 },
            { '5', 11 },
            { '4', 10 },
            { '3', 9 },
            { '2', 8 }
        };
    }
}
