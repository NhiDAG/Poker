namespace Poker
{
    public enum PokerHand
    {
        RoyalFlush,
        Flush,
        StraightFlush,
        Straight,
        ThreeOfAKind,
        FourOfAKind,
        FullHouse,
        TwoPairs,
        OnePair,
    }

    class CardCollector {

        public static readonly Dictionary<char, int> CardValue = new Dictionary<char, int>
        {
            {'2', 2}, {'3', 3}, {'4', 4}, {'5', 5}, {'6', 6}, {'7', 7}, {'8', 8}, {'9', 9},
            {'T', 10},{'J', 11}, {'Q', 12}, {'K', 13}, {'A', 14}
        };

        public static readonly char[] suits = { 'h', 'd', 'c', 's' };

        public static readonly HashSet<string> FullDeck = CardValue.Keys.SelectMany(rank => suits.Select(suit => $"{rank}{suit}")).ToHashSet();
    }

    class HandEvaluator
    {
        private readonly List<char> ranks;
        private readonly List<char> suits;
        private readonly Dictionary<char, int> rankCount;

        public HandEvaluator(List<char> ranks, List<char> suits)
        {
            this.ranks = ranks;
            this.suits = suits;
            this.rankCount = ranks.GroupBy(r => r).ToDictionary(g => g.Key, g => g.Count());
        }
        private bool IsFlush() => suits.Distinct().Count() == 1;

        private bool IsStraight()
        {
            var values = ranks.Select(r => CardCollector.CardValue[r]).OrderBy(v => v).ToList();
            return values.Distinct().Count() == 5 && values.Last() - values.First() == 4;
        }
        private bool IsRoyalFlush() => IsFlush() && IsStraight() && ranks.Contains('A') && ranks.Contains('K');
        private bool IsStraightFlush() => IsFlush() && IsStraight();
        private bool IsFourOfAKind() => rankCount.Values.Contains(4);
        private bool IsFullHouse() => rankCount.Values.Contains(3) && rankCount.Values.Contains(2);
        private bool IsThreeOfAKind() => rankCount.Values.Contains(3);
        private bool IsTwoPairs() => rankCount.Values.Count(v => v == 2) == 2;
        private bool IsOnePair() => rankCount.Values.Contains(2);

        public string EvaluatedHand()
        {

            return IsRoyalFlush() ? PokerHand.RoyalFlush.ToString() :
                   IsStraightFlush() ? PokerHand.StraightFlush.ToString() :
                   IsFourOfAKind() ? PokerHand.FourOfAKind.ToString() :
                   IsFullHouse() ? PokerHand.FullHouse.ToString() :
                   IsFlush() ? PokerHand.Flush.ToString() :
                   IsStraight() ? PokerHand.Straight.ToString() :
                   IsThreeOfAKind() ? PokerHand.ThreeOfAKind.ToString() :
                   IsTwoPairs() ? PokerHand.TwoPairs.ToString() :
                   IsOnePair() ? PokerHand.OnePair.ToString() :
                   $"High Card: {ranks.OrderByDescending(r => CardCollector.CardValue[r]).First()}";
        }

    }

    class HandValidator 
    {
        public static List<string> ConvertToValidCards(string hand)
            => hand.Replace("10", "T").Split(' ').ToList();

        public static bool IsValidHand(string hand)
            => ConvertToValidCards(hand).Distinct().Count() == 5 && ConvertToValidCards(hand).All(c => CardCollector.FullDeck.Contains(c));

    }

    class Program
    {
        static void Main(string[] args)
        {
            string[] hands =
            {
                "Ah Kh Qh Jh Th",
                "4s 5s 6s 7s 8s",
                "9h 9s 9d 9c 2h",
                "2h 2d 2c 4s 4h",
                "2h 5d 8d Jd Kd",
                "4h 5s 6d 7c 8h",
                "Th Ts Td 4c 2h",
                "4h 4s 7d 7c 2h",
                "6h 6s 3d 8c 2h",
                "Ah Kd 10d 5c 2h",
                "6h 6h 6d 6s 6c",
                "4d 4d 5c 2h 8s"
            };

            foreach (var hand in hands)
            {

                if (HandValidator.IsValidHand(hand))
                {
                    var ranks = HandValidator.ConvertToValidCards(hand).Select(c => c[0]).ToList();
                    var suits = HandValidator.ConvertToValidCards(hand).Select(c => c[1]).ToList();
                    var result = new HandEvaluator(ranks, suits).EvaluatedHand();
                    Console.WriteLine($"{hand} => {result}");
                }
                else
                {
                    Console.WriteLine($"{hand} => Invalid Hand");
                }
            }
        }
    }
}
