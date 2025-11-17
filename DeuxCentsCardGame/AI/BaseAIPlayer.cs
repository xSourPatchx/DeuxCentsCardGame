using DeuxCentsCardGame.Interfaces.AI;
using DeuxCentsCardGame.Interfaces.Services;
using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.AI
{
    public abstract class BaseAIPlayer : IAIPlayer
    {
        protected readonly IRandomService _randomService;
        protected readonly ICardUtility _cardUtility;
        protected readonly AIDifficulty _difficulty;

        protected BaseAIPlayer(IRandomService randomService, ICardUtility cardUtility, AIDifficulty difficulty)
        {
            _randomService = randomService ?? throw new ArgumentNullException(nameof(randomService));
            _cardUtility = cardUtility ?? throw new ArgumentNullException(nameof(cardUtility));
            _difficulty = difficulty;
        }

        public abstract int DecideBet(List<Card> hand, int minBet, int maxBet, int betIncrement, int currentHighestBid, List<int> takenBids);

        public abstract CardSuit SelectTrumpSuit(List<Card> hand);

        public abstract int ChooseCard(List<Card> hand, CardSuit? leadingSuit, CardSuit? trumpSuit, List<(Card card, Player player)> cardsPlayedInTrick);

        // Helper methods for all AI implementations
        protected int CalculateHandStrength(List<Card> hand)
        {
            return hand.Sum(card => card.CardPointValue + card.CardFaceValue);
        }

        protected Dictionary<CardSuit, int> GetSuitCounts(List<Card> hand)
        {
            return hand.GroupBy(card => card.CardSuit)
                       .ToDictionary(g => g.Key, g => g.Count());
        }

        protected CardSuit GetStrongestSuit(List<Card> hand)
        {
            return hand.GroupBy(card => card.CardSuit)
                       .OrderByDescending(g => g.Sum(c => c.CardFaceValue))
                       .First()
                       .Key;
        }

        protected List<Card> GetPlayableCards(List<Card> hand, CardSuit? leadingSuit)
        {
            if (!leadingSuit.HasValue)
                return new List<Card>(hand);

            var cardsOfLeadingSuit = hand.Where(c => c.CardSuit == leadingSuit.Value).ToList();
            return cardsOfLeadingSuit.Any() ? cardsOfLeadingSuit : new List<Card>(hand);
        }

        protected int GetRandomValidBet(int minBet, int maxBet, int betIncrement, List<int> takenBids)
        {
            var availableBets = new List<int>();
            for (int bet = minBet; bet <= maxBet; bet += betIncrement)
            {
                if (!takenBids.Contains(bet))
                    availableBets.Add(bet);
            }

            if (availableBets.Count == 0)
                return -1; // Pass if no bets available

            return availableBets[_randomService.Next(0, availableBets.Count)];
        }
    }
}