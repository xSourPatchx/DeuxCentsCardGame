using DeuxCentsCardGame.Interfaces.Gameplay;
using DeuxCentsCardGame.Interfaces.Services;
using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Gameplay
{
    // Evaluates card hands for strength, composition, and strategic value.
    // Used by AI players for decision-making and can provide hints to human players.
    public class HandEvaluator : IHandEvaluator
    {
        private readonly ICardCollectionHelper _cardHelper;

        public HandEvaluator(ICardCollectionHelper cardHelper)
        {
            _cardHelper = cardHelper ?? throw new ArgumentNullException(nameof(cardHelper));
        }

        // Calculates overall hand strength based on face values and point values.
        public int CalculateHandStrength(List<Card> hand)
        {
            if (hand == null || hand.Count == 0)
                return 0;

            return hand.Sum(card => card.CardPointValue + card.CardFaceValue);
        }

        // Calculates advanced hand strength with bonuses for high cards and suit distribution.
        public int CalculateAdvancedHandStrength(List<Card> hand)
        {
            if (hand == null || hand.Count == 0)
                return 0;

            int strength = 0;
            
            // Base strength from point and face values
            strength += hand.Sum(c => c.CardPointValue + c.CardFaceValue);
            
            // Bonus for high cards (8-10 face value)
            strength += CountHighCards(hand, 8) * 3;
            
            // Bonus for having multiple cards in same suit (good for trump)
            var suitCounts = _cardHelper.CountBySuit(hand);
            foreach (var count in suitCounts.Values)
            {
                if (count >= 4)
                    strength += 5;
                else if (count >= 3)
                    strength += 2;
            }
            
            return strength;
        }

        // Gets the count of cards in each suit.
        public Dictionary<CardSuit, int> GetSuitCounts(List<Card> hand)
        {
            return _cardHelper.CountBySuit(hand);
        }

        // Determines the strongest suit based on total face value.
        public CardSuit GetStrongestSuit(List<Card> hand)
        {
            if (hand == null || hand.Count == 0)
                throw new ArgumentException("Cannot determine strongest suit from empty hand", nameof(hand));

            var suitGroups = _cardHelper.GroupBySuit(hand);

            return suitGroups
                .OrderByDescending(g => _cardHelper.CalculateTotalFaceValue(g.Value))
                .First()
                .Key;
        }

        // Gets detailed suit strength analysis including count, face value sum, and high cards.
        public Dictionary<CardSuit, SuitStrength> GetSuitStrengths(List<Card> hand)
        {
            if (hand == null || hand.Count == 0)
                return new Dictionary<CardSuit, SuitStrength>();

            var suitGroups = _cardHelper.GroupBySuit(hand);
            
            return suitGroups.ToDictionary(
                g => g.Key,
                g => new SuitStrength
                {
                    Count = g.Value.Count,
                    TotalFaceValue = _cardHelper.CalculateTotalFaceValue(g.Value),
                    TotalPointValue = _cardHelper.CalculateTotalPoints(g.Value),
                    HighCardCount = _cardHelper.GetHighValueCards(g.Value, 8).Count,
                    HighestCard = _cardHelper.GetHighestCard(g.Value)?.CardFaceValue ?? 0
                });
        }

        // Counts cards with face value at or above threshold.
        public int CountHighCards(List<Card> hand, int threshold)
        {
            if (hand == null || hand.Count == 0)
                return 0;

            return hand.Count(card => card.CardFaceValue >= threshold);
        }

        // Checks if hand has no cards of specified suit (a void).
        public bool HasVoid(List<Card> hand, CardSuit suit)
        {
            if (hand == null || hand.Count == 0)
                return true;

            return !_cardHelper.HasSuit(hand, suit);
        }

        // Checks if hand has only one card of specified suit (a singleton).
        public bool HasSingleton(List<Card> hand, CardSuit suit)
        {
            if (hand == null || hand.Count == 0)
                return false;

            return _cardHelper.FilterBySuit(hand, suit).Count == 1;
        }

        // Gets all voids (suits with no cards) in the hand.
        public List<CardSuit> GetVoids(List<Card> hand)
        {
            if (hand == null || hand.Count == 0)
                return Enum.GetValues<CardSuit>().ToList();

            var suitCounts = _cardHelper.CountBySuit(hand);
            var allSuits = Enum.GetValues<CardSuit>();
            
            return allSuits.Where(suit => !suitCounts.ContainsKey(suit)).ToList();
        }

        // Evaluates if a suit would make a good trump choice.
        public int EvaluateTrumpPotential(List<Card> hand, CardSuit suit)
        {
            if (hand == null || hand.Count == 0)
                return 0;

            var cardsInSuit = _cardHelper.FilterBySuit(hand, suit);
            
            if (cardsInSuit.Count == 0)
                return 0;

            int score = 0;
            
            // More cards = better
            score += cardsInSuit.Count * 10;
            
            // High cards = better
            score += _cardHelper.CalculateTotalFaceValue(cardsInSuit);
            
            // High cards bonus
            score += _cardHelper.GetHighValueCards(cardsInSuit, 8).Count * 5;
            
            return score;
        }

        // Gets the best trump suit recommendation based on hand composition.
        public CardSuit RecommendTrumpSuit(List<Card> hand)
        {
            if (hand == null || hand.Count == 0)
                throw new ArgumentException("Cannot recommend trump from empty hand", nameof(hand));

            var suitScores = Enum.GetValues<CardSuit>()
                .Select(suit => new
                {
                    Suit = suit,
                    Score = EvaluateTrumpPotential(hand, suit)
                })
                .OrderByDescending(x => x.Score)
                .ToList();

            return suitScores.First().Suit;
        }
    }

    // Detailed strength analysis for a single suit.
    public class SuitStrength
    {
        public int Count { get; set; }
        public int TotalFaceValue { get; set; }
        public int TotalPointValue { get; set; }
        public int HighCardCount { get; set; }
        public int HighestCard { get; set; }
    }
}