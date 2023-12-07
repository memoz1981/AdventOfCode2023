namespace Day7_CamelCards
{
    public class HandWithJoker : Hand
    {
        public HandWithJoker(string line) : base(line) => _values['J'] = 1;

        public HandWithJoker(char[] cards, long bidAmount) : base(cards, bidAmount) 
             => _values['J'] = 1;

        public override Type GetCardType()
        {
            var distinct = Cards.Distinct().ToList();

            if (distinct.Count == 1)
                return Type.FiveOfAKind;

            if (!distinct.Any(m => m == 'J'))
            {
                return base.GetCardType();
            }

            var typeMax = Type.HighCard;

            for (int i = 0; i < distinct.Count; i++)
            {
                if (distinct[i] == 'J')
                    distinct[i] = 'Z';
            }

            foreach (var ch in distinct)
            {
                var list = new char[5];
                for (int j = 0; j < 5; j++)
                {
                    if (Cards[j] == 'J')
                        list[j] = ch;
                    else
                        list[j] = Cards[j];
                }

                var cardsNew = new HandWithJoker(list, 1);
                var type = cardsNew.GetCardType();

                typeMax = (int)typeMax > (int)type ? typeMax : type;
            }
            return typeMax;
        }
    }
}
