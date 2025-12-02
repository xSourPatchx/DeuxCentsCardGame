using DeuxCentsCardGame.Gameplay;
using DeuxCentsCardGame.Interfaces.Services;
using DeuxCentsCardGame.Models;
using DeuxCentsCardGame.Services;

namespace DeuxCentsCardGame.AI
{
    // Hard AI - Uses advanced strategy and optimal play
    public class HardAIPlayer : BaseAIPlayer
    {
        public HardAIPlayer(
            IRandomService randomService, 
            ICardUtility cardUtility, 
            HandEvaluator handEvaluator, 
            CardCollectionHelper cardHelper, 
            TrickAnalyzer trickAnalyzer) 
            : base(randomService, cardUtility, handEvaluator, trickAnalyzer, cardHelper, AIDifficulty.Hard)
        {
        }

        public override int DecideBet(List<Card> hand, int minBet, int maxBet, int betIncrement, 
                                    int currentHighestBid, List<int> takenBids)
        {
            int handStrength = CalculateAdvancedHandStrength(hand);
            
            // More sophisticated thresholds
            if (handStrength < 75)
                return -1; // Pass on weak hands

            // Calculate optimal bet based on hand strength
            int targetBet;
            if (handStrength < 85)
                targetBet = minBet;
            else if (handStrength < 95)
                targetBet = minBet + betIncrement;
            else if (handStrength < 105)
                targetBet = minBet + (betIncrement * 3);
            else if (handStrength < 115)
                targetBet = maxBet - (betIncrement * 2);
            else
                targetBet = maxBet;

            // Only bet if we're confident
            if (currentHighestBid > targetBet)
                return -1; // Pass if someone already bet higher than our target

            // Find best available bet
            var availableBets = new List<int>();
            for (int bet = minBet; bet <= maxBet; bet += betIncrement)
            {
                if (!takenBids.Contains(bet) && bet > currentHighestBid)
                    availableBets.Add(bet);
            }

            if (availableBets.Count == 0)
                return -1;

            // Get closest bet to target that beats current high bid
            return availableBets.OrderBy(b => Math.Abs(b - targetBet)).First();
        }

        public override CardSuit SelectTrumpSuit(List<Card> hand)
        {
            // Group cards by suit
            var suitGroups = _cardHelper.GroupBySuit(hand);

            // Select suit with best combination of count and strength
            var suitScores = suitGroups.Select(g => new
            {
                Suit = g.Key,
                Count = g.Value.Count,
                Strength = _cardHelper.CalculateTotalFaceValue(g.Value),
                HighCards = _cardHelper.GetHighValueCards(g.Value, 8).Count
            })
            .Select(s => new
            {
                s.Suit,
                Score = (s.Count * 10) + s.Strength + (s.HighCards * 5)
            })
            .OrderByDescending(s => s.Score)
            .First();

            return suitScores.Suit;
        }

        public override int ChooseCard(List<Card> hand, CardSuit? leadingSuit, CardSuit? trumpSuit, 
                                    List<(Card card, Player player)> cardsPlayedInTrick)
        {
            var playableCards = GetPlayableCards(hand, leadingSuit);

            Card selectedCard;

            if (cardsPlayedInTrick.Count == 0)
            {
                // Leading - strategic choice
                selectedCard = ChooseLeadingCard(playableCards, trumpSuit);
            }
            else if (cardsPlayedInTrick.Count == 3)
            {
                // Last player - optimal play
                selectedCard = ChooseLastPlayerCard(playableCards, cardsPlayedInTrick, 
                                                trumpSuit, leadingSuit);
            }
            else
            {
                // Middle position - balanced strategy
                selectedCard = ChooseMiddlePositionCard(playableCards, cardsPlayedInTrick, 
                                                    trumpSuit, leadingSuit);
            }

            return hand.IndexOf(selectedCard);
        }

        private int CalculateAdvancedHandStrength(List<Card> hand)
        {
            int strength = 0;
            
            // Base strength from point and face values
            strength += _cardHelper.CalculateTotalPoints(hand);
            strength += _cardHelper.CalculateTotalFaceValue(hand);
            
            // Bonus for high cards using helper
            strength += _cardHelper.GetHighValueCards(hand, 8).Count * 3;
            
            // Bonus for having multiple cards in same suit
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

        private Card ChooseLeadingCard(List<Card> playableCards, CardSuit? trumpSuit)
        {
            // Lead with a strong non-trump card to test opponents
            var nonTrumpCards = _cardHelper.FilterByNonTrump(playableCards, trumpSuit);
            
            if (nonTrumpCards.Any())
            {
                // Lead with mid-high card from our strongest suit
                var sorted = _cardHelper.SortByValue(nonTrumpCards, descending: true);
                return sorted.Skip(1).FirstOrDefault() ?? sorted.First();
            }
            
            // If only trump, lead with lowest trump
            return _cardHelper.GetLowestCard(playableCards)!;
        }

        private Card ChooseLastPlayerCard(List<Card> playableCards, 
                                        List<(Card card, Player player)> cardsPlayedInTrick,
                                        CardSuit? trumpSuit, CardSuit? leadingSuit)
        {
            var currentWinningCard = _trickAnalyzer.GetCurrentWinningCard(
                cardsPlayedInTrick, trumpSuit, leadingSuit);
            
            // Try to win if trick has value
            if (_trickAnalyzer.IsTrickValuable(cardsPlayedInTrick, 10))
            {
                var winningCards = _trickAnalyzer.GetWinningCards(
                    playableCards, currentWinningCard, trumpSuit, leadingSuit);
                
                if (winningCards.Any())
                    return winningCards.First();
            }
            
            // Otherwise dump lowest card
            var lowestPoint = _cardHelper.SortByPointValue(playableCards).First();
            return lowestPoint;
        }

        private Card ChooseMiddlePositionCard(List<Card> playableCards,
                                            List<(Card card, Player player)> cardsPlayedInTrick,
                                            CardSuit? trumpSuit, CardSuit? leadingSuit)
        {
            var currentWinningCard = _trickAnalyzer.GetCurrentWinningCard(
                cardsPlayedInTrick, trumpSuit, leadingSuit);
            
            // If trick is valuable, try to win
            if (_trickAnalyzer.IsTrickValuable(cardsPlayedInTrick, 10))
            {
                var winningCards = _trickAnalyzer.GetWinningCards(
                    playableCards, currentWinningCard, trumpSuit, leadingSuit);
                
                if (winningCards.Any())
                    return winningCards.First();
            }
            
            // Play medium card
            return _cardHelper.GetMiddleCard(playableCards)!;
        }
    }
}