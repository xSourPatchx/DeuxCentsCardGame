using DeuxCentsCardGame.Gameplay;
using DeuxCentsCardGame.Interfaces.Services;
using DeuxCentsCardGame.Models;
using DeuxCentsCardGame.Services;

namespace DeuxCentsCardGame.AI
{
    // Medium AI - Uses basic strategy with some randomness
    public class MediumAIPlayer : BaseAIPlayer
    {   
        public MediumAIPlayer(
            IRandomService randomService, 
            ICardUtility cardUtility, 
            HandEvaluator handEvaluator,
            CardCollectionHelper cardHelper,
            TrickAnalyzer trickAnalyzer) 
            : base(randomService, cardUtility, handEvaluator, trickAnalyzer, cardHelper, AIDifficulty.Medium)
        {
        }

        public override int DecideBet(List<Card> hand, int minBet, int maxBet, int betIncrement,
                                     int currentHighestBid, List<int> takenBids)
        {
            int handStrength = CalculateHandStrength(hand);

            // Calculate threshold (average hand would be ~80-90 points)
            int passThreshold = 70;

            if (handStrength < passThreshold)
                return -1; // Pass on weak hands

            // Bet based on hand strength
            int targetBet;
            if (handStrength < 90)
                targetBet = minBet;
            else if (handStrength < 110)
                targetBet = minBet + (betIncrement * 2);
            else
                targetBet = maxBet - betIncrement;

            // Find closest available bet
            var availableBets = new List<int>();
            for (int bet = minBet; bet <= maxBet; bet += betIncrement)
            {
                if (!takenBids.Contains(bet) && bet > currentHighestBid)
                    availableBets.Add(bet);
            }

            if (availableBets.Count == 0)
                return -1;

            // Get closest bet to target
            return availableBets.OrderBy(b => Math.Abs(b - targetBet)).First();
        }

        public override CardSuit SelectTrumpSuit(List<Card> hand)
        {    
            // Select suit we have most cards in
            var suitCounts = _cardHelper.CountBySuit(hand);
            return suitCounts.OrderByDescending(kvp => kvp.Value).First().Key;
        }

        public override int ChooseCard(List<Card> hand, CardSuit? leadingSuit, CardSuit? trumpSuit,
                                      List<(Card card, Player player)> cardsPlayedInTrick)
        {
            var playableCards = GetPlayableCards(hand, leadingSuit);

            Card selectedCard;
            
        if (cardsPlayedInTrick.Count == 0)
        {
            // Leading - play medium strength card
            selectedCard = _cardHelper.GetMiddleCard(playableCards)!;
        }
        else
        {
            var currentWinningCard = _trickAnalyzer.GetCurrentWinningCard(
                cardsPlayedInTrick, trumpSuit, leadingSuit);
            
            // Try to win with lowest card possible
            var winningCards = _trickAnalyzer.GetWinningCards(
                playableCards, currentWinningCard, trumpSuit, leadingSuit);

            if (winningCards.Any())
            {
                selectedCard = winningCards.First();
            }
            else
            {
                // Can't win - play lowest card
                selectedCard = _cardHelper.GetLowestCard(playableCards)!;
            }
        }

        return hand.IndexOf(selectedCard);
        }
    }
}