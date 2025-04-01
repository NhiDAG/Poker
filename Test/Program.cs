using System.ComponentModel;
using System.Diagnostics;

namespace Test
{
    internal class Program
    {
        private static Dictionary<char, int> CardValue = new Dictionary<char, int>
        {  {'2', 2}, {'3', 3}, {'4', 4}, {'5', 5}, {'6', 6}, {'7', 7}, {'8', 8},
            {'9', 9}, {'T', 10}, {'J', 11}, {'Q', 12}, {'K', 13}, {'A', 14}
        };

        private static HashSet<string> GenerateDeck()
        {
            var deck = new HashSet<string>();
            char[] suits = { 'h', 'd', 'c', 's' };

            foreach (char r in CardValue.Keys) {
                foreach (char c in suits) {
                    deck.Add($"{r}{c}");
                }
            }
            return deck;
        }

        private static bool IsLowStraight(List<char> ranks)
        {
            var lowStraight = new List<char> { '2', '3', '4', '5', 'A' };
            return ranks.SequenceEqual(lowStraight);
        }

        private static readonly HashSet<string> FullDeck = GenerateDeck();

        private static string DetermineHandRank(string hand)
        {
            string[] cards = hand.Split(' ');

            if(cards.Distinct().Count() !=5 || !cards.All(c => FullDeck.Contains(c))){
                return "Invalid Hand";
            }

            var ranks = cards.Select(c => c[0]).OrderBy(r => CardValue[r]).ToList();
            var suits = cards.Select(c => c[1]).ToList();

            var rankCount = ranks.GroupBy(r => r).ToDictionary(g => g.Key, g => g.Count());

            bool isFlush = suits.Distinct().Count() == 1;
            bool isStraigth = ranks.Distinct().Count() == 5 && (CardValue[ranks.Max()] - CardValue[ranks.Min()]) == 4 || IsLowStraight(ranks);

            if (isFlush && isStraigth && CardValue[ranks.Max()] == 14) return "Royal Flush";
            else if (isFlush && isStraigth) return "Straight Flush";
            else if (ranks.Distinct().Count() == 2) return "Four of a Kind";
            else if (rankCount.Values.Contains(3) && rankCount.Values.Contains(2)) return "Full House";
            else if (isFlush) return "Flush";
            else if (isStraigth) return "Straigth";
            else if (rankCount.Values.Contains(3)) return "Three of a Kind";
            else if (rankCount.Values.Count(v => v == 2) == 2) return "Two pairs";
            else if (rankCount.Values.Contains(2)) return "One pair";
            else
            return $"High Card: {ranks.Max()}";

        }

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

            foreach (var h in hands) {
                Console.WriteLine($"{h} => {DetermineHandRank(h)}");
            }
        }
    }
}
