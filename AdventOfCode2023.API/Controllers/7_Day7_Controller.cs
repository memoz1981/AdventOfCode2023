using Common.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdventOfCode2023.API.Controllers
{
    [Route("day7")]
    [ApiController]
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
                var el = line.Split(' ');
                var hand = new Hand(el.First(), el.Last());
                var type = hand.GetCardType(); 
                hands.Add(hand);
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

            foreach (var line in lines)
            {

            }

            return Ok();
        }
    }

    public class Game
    {
        public Hand Hand { get; set; }
        public long Rank { get; set; }
    }

    public class Hand : IComparable<Hand>
    {
        public Hand(string cards, string bidAmount)
        {
            Cards = cards.Trim().ToCharArray();
            BidAmount = long.Parse(bidAmount.Trim()); 
        }
        public char[] Cards { get; set; }

        public long BidAmount { get; set; }

        public Type GetCardType()
        {
            var distinct = Cards.Distinct().ToList();

            if (distinct.Count == 1)
                return Type.FiveOfAKind;

            if (!distinct.Any(m => m == 'J'))
            {
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

            var typeMax = Type.HighCard;
            var distinctNew = new char[distinct.Count]; 

            for (int i = 0; i < distinct.Count; i++)
            {
                if (distinct[i] == 'J')
                    distinctNew[i] = 'Z';
                else
                    distinctNew[i] = distinct[i];
            }

            foreach (var ch in distinctNew)
            {
                var list = new char[5]; 
                for (int j = 0; j < 5; j++)
                {
                    if (Cards[j] == 'J')
                        list[j] = ch;
                    else
                        list[j] = Cards[j];
                }
                
                var cardsNew = new Hand(new string(list), "1");
                var type = cardsNew.GetCardType();

                typeMax = (int)typeMax > (int)type ? typeMax : type;
            }
            return typeMax; 
            

            
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


        private Dictionary<char, int> _values = new()
        {
            { 'A', 20 },
            { 'K', 19 },
            { 'Q', 18 },
            { 'J', 1 },
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

    public enum Type
    {
        FiveOfAKind = 10, FourOfAKind = 9, FullHouse = 8, ThreeOfAKind = 7, TwoPair = 6, OnePair = 5, HighCard = 0
    }
}
