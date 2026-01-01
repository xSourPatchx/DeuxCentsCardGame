using DeuxCentsCardGame.Interfaces.AI;
using DeuxCentsCardGame.Interfaces.Gameplay;
using DeuxCentsCardGame.Interfaces.Services;
using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.AI
{
    public abstract class BaseAIPlayer : IAIPlayer
    {
        protected readonly IRandomService _randomService;
        protected readonly ICardUtility _cardUtility;
        protected readonly IHandEvaluator _handEvaluator;
        protected readonly ITrickAnalyzer _trickAnalyzer;
        protected readonly ICardCollectionHelper _cardHelper;
        protected readonly AIDifficulty _difficulty;

        protected BaseAIPlayer(
            IRandomService randomService, 
            ICardUtility cardUtility, 
            IHandEvaluator handEvaluator,
            ITrickAnalyzer trickAnalyzer,
            ICardCollectionHelper cardHelper,
            AIDifficulty difficulty)
        {
            _randomService = randomService ?? throw new ArgumentNullException(nameof(randomService));
            _cardUtility = cardUtility ?? throw new ArgumentNullException(nameof(cardUtility));
            _handEvaluator = handEvaluator ?? throw new ArgumentNullException(nameof(handEvaluator));
            _trickAnalyzer = trickAnalyzer ?? throw new ArgumentNullException(nameof(trickAnalyzer));
            _cardHelper = cardHelper ?? throw new ArgumentNullException(nameof(cardHelper));
            _difficulty = difficulty;
        }

        public abstract int DecideBet(List<Card> hand, int minBet, int maxBet, int betIncrement, int currentHighestBid, List<int> takenBids);

        public abstract CardSuit SelectTrumpSuit(List<Card> hand);

        public abstract int ChooseCard(List<Card> hand, CardSuit? leadingSuit, CardSuit? trumpSuit, List<(Card card, Player player)> cardsPlayedInTrick);

        // Helper methods for all AI implementations
        protected int CalculateHandStrength(List<Card> hand)
        {
            return _handEvaluator.CalculateHandStrength(hand);
        }

        protected Dictionary<CardSuit, int> GetSuitCounts(List<Card> hand)
        {
            return _handEvaluator.GetSuitCounts(hand);
        }

        protected CardSuit GetStrongestSuit(List<Card> hand)
        {
            return _handEvaluator.GetStrongestSuit(hand);
        }

        protected List<Card> GetPlayableCards(List<Card> hand, CardSuit? leadingSuit)
        {         
            return _cardHelper.GetPlayableCards(hand, leadingSuit);
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