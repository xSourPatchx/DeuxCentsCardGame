using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Services
{
    // Provides utility methods for common card collection operations.
    // Used for filtering, sorting, and analyzing collections of cards.
    public class CardCollectionHelper
    {
        // Filters cards by a specific suit.
        public List<Card> FilterBySuit(List<Card> cards, CardSuit suit)
        {
            if (cards == null || cards.Count == 0)
                return new List<Card>();

            return cards.Where(card => card.CardSuit == suit).ToList();
        }

        // Filters cards to only return trump cards.
        public List<Card> FilterByTrump(List<Card> cards, CardSuit? trumpSuit)
        {
            if (cards == null || cards.Count == 0 || !trumpSuit.HasValue)
                return new List<Card>();

            return cards.Where(card => card.IsTrump(trumpSuit)).ToList();
        }

        // Filters cards to only return non-trump cards.
        public List<Card> FilterByNonTrump(List<Card> cards, CardSuit? trumpSuit)
        {
            if (cards == null || cards.Count == 0)
                return new List<Card>();

            if (!trumpSuit.HasValue)
                return new List<Card>(cards);

            return cards.Where(card => !card.IsTrump(trumpSuit)).ToList();
        }

        // Sorts cards by suit, then by face value within each suit.
        // Suit order: Clubs, Diamonds, Hearts, Spades
        public List<Card> SortBySuit(List<Card> cards)
        {
            if (cards == null || cards.Count == 0)
                return new List<Card>();

            return cards
                .OrderBy(card => card.CardSuit)
                .ThenBy(card => card.CardFaceValue)
                .ToList();
        }

        // Sorts cards by face value only (ignores suit).
        public List<Card> SortByValue(List<Card> cards, bool descending = false)
        {
            if (cards == null || cards.Count == 0)
                return new List<Card>();

            return descending
                ? cards.OrderByDescending(card => card.CardFaceValue).ToList()
                : cards.OrderBy(card => card.CardFaceValue).ToList();
        }

        // Sorts cards by point value.
        public List<Card> SortByPointValue(List<Card> cards, bool descending = false)
        {
            if (cards == null || cards.Count == 0)
                return new List<Card>();

            return descending
                ? cards.OrderByDescending(card => card.CardPointValue)
                    .ThenByDescending(card => card.CardFaceValue)
                    .ToList()
                : cards.OrderBy(card => card.CardPointValue)
                    .ThenBy(card => card.CardFaceValue)
                    .ToList();
        }

        // Gets the highest card by face value.
        public Card? GetHighestCard(List<Card> cards)
        {
            if (cards == null || cards.Count == 0)
                return null;

            return cards.OrderByDescending(card => card.CardFaceValue).First();
        }

        // Gets the lowest card by face value.
        public Card? GetLowestCard(List<Card> cards)
        {
            if (cards == null || cards.Count == 0)
                return null;

            return cards.OrderBy(card => card.CardFaceValue).First();
        }

        // Gets the highest card from a specific suit.
        public Card? GetHighestCardOfSuit(List<Card> cards, CardSuit suit)
        {
            var suitCards = FilterBySuit(cards, suit);
            return GetHighestCard(suitCards);
        }

        // Gets the lowest card from a specific suit.
        public Card? GetLowestCardOfSuit(List<Card> cards, CardSuit suit)
        {
            var suitCards = FilterBySuit(cards, suit);
            return GetLowestCard(suitCards);
        }

        // Groups cards by suit.
        public Dictionary<CardSuit, List<Card>> GroupBySuit(List<Card> cards)
        {
            if (cards == null || cards.Count == 0)
                return new Dictionary<CardSuit, List<Card>>();

            return cards
                .GroupBy(card => card.CardSuit)
                .ToDictionary(group => group.Key, group => group.ToList());
        }

        // Counts cards by suit.
        public Dictionary<CardSuit, int> CountBySuit(List<Card> cards)
        {
            if (cards == null || cards.Count == 0)
                return new Dictionary<CardSuit, int>();

            return cards
                .GroupBy(card => card.CardSuit)
                .ToDictionary(group => group.Key, group => group.Count());
        }

        // Gets cards with point values (5s, 10s, Aces).
        public List<Card> GetPointCards(List<Card> cards)
        {
            if (cards == null || cards.Count == 0)
                return new List<Card>();

            return cards.Where(card => card.CardPointValue > 0).ToList();
        }

        // Gets cards without point values.
        public List<Card> GetNonPointCards(List<Card> cards)
        {
            if (cards == null || cards.Count == 0)
                return new List<Card>();

            return cards.Where(card => card.CardPointValue == 0).ToList();
        }

        // Gets high-value cards (face value >= threshold).
        public List<Card> GetHighValueCards(List<Card> cards, int threshold = 8)
        {
            if (cards == null || cards.Count == 0)
                return new List<Card>();

            return cards.Where(card => card.CardFaceValue >= threshold).ToList();
        }

        // Gets low-value cards (face value <= threshold).
        public List<Card> GetLowValueCards(List<Card> cards, int threshold = 3)
        {
            if (cards == null || cards.Count == 0)
                return new List<Card>();

            return cards.Where(card => card.CardFaceValue <= threshold).ToList();
        }

        // Calculates total point value of a card collection.
        public int CalculateTotalPoints(List<Card> cards)
        {
            if (cards == null || cards.Count == 0)
                return 0;

            return cards.Sum(card => card.CardPointValue);
        }

        // Calculates total face value of a card collection.
        public int CalculateTotalFaceValue(List<Card> cards)
        {
            if (cards == null || cards.Count == 0)
                return 0;

            return cards.Sum(card => card.CardFaceValue);
        }

        // Checks if collection contains any cards of specified suit.
        public bool HasSuit(List<Card> cards, CardSuit suit)
        {
            if (cards == null || cards.Count == 0)
                return false;

            return cards.Any(card => card.CardSuit == suit);
        }

        // Checks if collection contains any trump cards.
        public bool HasTrump(List<Card> cards, CardSuit? trumpSuit)
        {
            if (cards == null || cards.Count == 0 || !trumpSuit.HasValue)
                return false;

            return cards.Any(card => card.IsTrump(trumpSuit));
        }

        // Gets a random card from the collection.
        public Card? GetRandomCard(List<Card> cards, Random random)
        {
            if (cards == null || cards.Count == 0)
                return null;

            if (random == null)
                throw new ArgumentNullException(nameof(random));

            int index = random.Next(0, cards.Count);
            return cards[index];
        }

        // Gets a middle-value card from the collection (useful for AI play).
        public Card? GetMiddleCard(List<Card> cards)
        {
            if (cards == null || cards.Count == 0)
                return null;

            var sorted = SortByValue(cards);
            return sorted[sorted.Count / 2];
        }

        // Creates a deep copy of a card list.
        public List<Card> Clone(List<Card> cards)
        {
            if (cards == null)
                return new List<Card>();

            return new List<Card>(cards);
        }

        // Gets playable cards based on leading suit rules.
        // If no leading suit, all cards are playable.
        // If leading suit exists, must follow suit if possible.

        public List<Card> GetPlayableCards(List<Card> hand, CardSuit? leadingSuit)
        {
            if (hand == null || hand.Count == 0)
                return new List<Card>();

            // If no leading suit, all cards are playable
            if (!leadingSuit.HasValue)
                return new List<Card>(hand);

            // Must follow suit if possible
            var cardsOfLeadingSuit = FilterBySuit(hand, leadingSuit.Value);
            
            return cardsOfLeadingSuit.Any() ? cardsOfLeadingSuit : new List<Card>(hand);
        }
    }
}