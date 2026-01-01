using DeuxCentsCardGame.Interfaces.Gameplay;
using DeuxCentsCardGame.Interfaces.Services;
using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.AI
{
    // Easy AI - Makes mostly random decisions with minimal strategy
    public class EasyAIPlayer : BaseAIPlayer
    {
        public EasyAIPlayer(
            IRandomService randomService, 
            ICardUtility cardUtility,
            IHandEvaluator handEvaluator,
            ITrickAnalyzer trickAnalyzer,
            ICardCollectionHelper cardHelper) 
            : base(randomService, cardUtility, handEvaluator, trickAnalyzer, cardHelper, AIDifficulty.Easy)
        { 
        }

        public override int DecideBet(List<Card> hand, int minBet, int maxBet, int betIncrement,
                                     int currentHighestBid, List<int> takenBids)
        {
            // 50% chance to pass immediately
            if (_randomService.Next(0, 2) == 0)
                return -1;

            // Otherwise make a random valid bet
            return GetRandomValidBet(minBet, maxBet, betIncrement, takenBids);
        }

        public override CardSuit SelectTrumpSuit(List<Card> hand)
        {
            // Select random suit
            var suits = _cardUtility.GetAllCardSuits();
            return suits[_randomService.Next(0, suits.Length)];
        }

        public override int ChooseCard(List<Card> hand, CardSuit? leadingSuit, CardSuit? trumpSuit,
                                      List<(Card card, Player player)> cardsPlayedInTrick)
        {
            var playableCards = GetPlayableCards(hand, leadingSuit);

            // Play random valid card
            var randomCard = _cardHelper.GetRandomCard(playableCards, new Random());
            return hand.IndexOf(randomCard!);
        }  
    }
}