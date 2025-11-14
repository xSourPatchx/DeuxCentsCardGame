using DeuxCentsCardGame.Interfaces.Services;
using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.AI
{
    // Medium AI - Uses basic strategy with some randomness
     public class MediumAIPlayer : BaseAIPlayer
    {
        public MediumAIPlayer(IRandomService randomService)
            : base(randomService, AIDifficulty.Medium)
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
                if (!takenBids.Contains(bet))
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
            var suitCounts = GetSuitCounts(hand);
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
            selectedCard = playableCards.OrderBy(c => c.CardFaceValue)
                                       .ElementAt(playableCards.Count / 2);
        }
        else
        {
            var currentWinningCard = GetCurrentWinningCard(cardsPlayedInTrick, trumpSuit, leadingSuit);
            
            // Try to win with lowest card possible
            var winningCards = playableCards
                .Where(c => c.WinsAgainst(currentWinningCard, trumpSuit, leadingSuit))
                .OrderBy(c => c.CardFaceValue)
                .ToList();

            if (winningCards.Any())
            {
                selectedCard = winningCards.First();
            }
            else
            {
                // Can't win - play lowest card
                selectedCard = playableCards.OrderBy(c => c.CardFaceValue).First();
            }
        }

        return hand.IndexOf(selectedCard);
    }

    private Card GetCurrentWinningCard(List<(Card card, Player player)> cardsPlayed, 
                                      CardSuit? trumpSuit, CardSuit? leadingSuit)
    {
        var winningCard = cardsPlayed[0].card;
        
        for (int i = 1; i < cardsPlayed.Count; i++)
        {
            if (cardsPlayed[i].card.WinsAgainst(winningCard, trumpSuit, leadingSuit))
            {
                winningCard = cardsPlayed[i].card;
            }
        }
        
        return winningCard;
        }  
    }
}