namespace Day7_CamelCards
{
    public class Hand : IComparable<Hand>
    {
        public Hand(string line)
        {
            Cards = line.Split(' ').First().Trim().ToCharArray();
            BidAmount = long.Parse(line.Split(' ').Last().Trim());
        }

        public Hand(string cards, string bidAmount)
        {
            Cards = cards.Trim().ToCharArray();
            BidAmount = long.Parse(bidAmount.Trim());
        }

        public char[] Cards { get; set; }

        public long BidAmount { get; set; }

        public virtual Type GetCardType()
        {
            var distinct = Cards.Distinct().ToList();

            if (distinct.Count == 1)
                return Type.FiveOfAKind;

            if (distinct.Count == 5)
                return Type.HighCard;

            if (distinct.Count == 4)
                return Type.OnePair;

            if (distinct.Count == 3)
            {
                var count1 = Cards.Count(m => m == distinct[0]);
                var count2 = Cards.Count(m => m == distinct[1]);
                var count3 = Cards.Count(m => m == distinct[2]);

                if (Math.Max(count3, Math.Max(count1, count2)) == 2)
                    return Type.TwoPair;

                return Type.ThreeOfAKind;
            }

            if (distinct.Count == 2)
            {
                var count1 = Cards.Count(m => m == distinct.First());
                var count2 = Cards.Count(m => m == distinct.Last());

                if (Math.Max(count1, count2) == 4)
                    return Type.FourOfAKind;

                return Type.FullHouse;
            }
            throw new ArgumentException();
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
