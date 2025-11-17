using DeuxCentsCardGame.Interfaces.Services;
using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.AI
{
    // Hard AI - Uses advanced strategy and optimal play
    public class HardAIPlayer : BaseAIPlayer
    {
        public HardAIPlayer(IRandomService randomService, ICardUtility cardUtility) : base(randomService, cardUtility, AIDifficulty.Hard)
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
                if (!takenBids.Contains(bet) && bet >= currentHighestBid + betIncrement)
                    availableBets.Add(bet);
            }

            if (availableBets.Count == 0)
                return -1;

            // Get closest bet to target that beats current high bid
            return availableBets.OrderBy(b => Math.Abs(b - targetBet)).First();
        }

        public override CardSuit SelectTrumpSuit(List<Card> hand)
        {
            // Select suit with best combination of count and strength
            var suitScores = hand.GroupBy(card => card.CardSuit)
                .Select(g => new
            {
                Suit = g.Key,
                Count = g.Count(),
                Strength = g.Sum(c => c.CardFaceValue),
                HighCards = g.Count(c => c.CardFaceValue >= 8)
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
            strength += hand.Sum(c => c.CardPointValue + c.CardFaceValue);
            
            // Bonus for high cards
            strength += hand.Count(c => c.CardFaceValue >= 8) * 3;
            
            // Bonus for having multiple cards in same suit
            var suitCounts = GetSuitCounts(hand);
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
            var nonTrumpCards = playableCards.Where(c => !c.IsTrump(trumpSuit)).ToList();
            
            if (nonTrumpCards.Any())
            {
                // Lead with mid-high card from our strongest suit
                return nonTrumpCards.OrderByDescending(c => c.CardFaceValue)
                                .Skip(1)
                                .FirstOrDefault() ?? nonTrumpCards.First();
            }
            
            // If only trump, lead with lowest trump
            return playableCards.OrderBy(c => c.CardFaceValue).First();
        }

        private Card ChooseLastPlayerCard(List<Card> playableCards, 
                                        List<(Card card, Player player)> cardsPlayedInTrick,
                                        CardSuit? trumpSuit, CardSuit? leadingSuit)
        {
            var currentWinningCard = GetCurrentWinningCard(cardsPlayedInTrick, trumpSuit, leadingSuit);
            
            // Calculate if winning is worth it
            int trickValue = cardsPlayedInTrick.Sum(t => t.card.CardPointValue);
            
            // Try to win if trick has value
            if (trickValue >= 10)
            {
                var winningCards = playableCards
                    .Where(c => c.WinsAgainst(currentWinningCard, trumpSuit, leadingSuit))
                    .OrderBy(c => c.CardFaceValue)
                    .ToList();
                
                if (winningCards.Any())
                    return winningCards.First();
            }
            
            // Otherwise dump lowest card
            return playableCards.OrderBy(c => c.CardPointValue)
                            .ThenBy(c => c.CardFaceValue)
                            .First();
        }

        private Card ChooseMiddlePositionCard(List<Card> playableCards,
                                            List<(Card card, Player player)> cardsPlayedInTrick,
                                            CardSuit? trumpSuit, CardSuit? leadingSuit)
        {
            var currentWinningCard = GetCurrentWinningCard(cardsPlayedInTrick, trumpSuit, leadingSuit);
            int trickValue = cardsPlayedInTrick.Sum(t => t.card.CardPointValue);
            
            // If partner is winning, play low
            bool partnerWinning = IsPartnerWinning(cardsPlayedInTrick, trumpSuit, leadingSuit);
            if (partnerWinning)
            {
                return playableCards.OrderBy(c => c.CardFaceValue).First();
            }
            
            // If trick is valuable, try to win
            if (trickValue >= 10)
            {
                var winningCards = playableCards
                    .Where(c => c.WinsAgainst(currentWinningCard, trumpSuit, leadingSuit))
                    .OrderBy(c => c.CardFaceValue)
                    .ToList();
                
                if (winningCards.Any())
                    return winningCards.First();
            }
            
            // Play medium card
            return playableCards.OrderBy(c => c.CardFaceValue)
                            .ElementAt(playableCards.Count / 2);
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

        private bool IsPartnerWinning(List<(Card card, Player player)> cardsPlayed,
                                    CardSuit? trumpSuit, CardSuit? leadingSuit)
        {
            // Note: This is a simplified check. In actual implementation,
            // you'd need to pass team information to determine if partner is winning
            // For now, returns false as placeholder
            return false;
        }
    }
}