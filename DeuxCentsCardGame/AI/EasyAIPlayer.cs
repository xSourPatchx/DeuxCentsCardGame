using DeuxCentsCardGame.Interfaces.Services;
using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.AI
{
    // Easy AI - Makes mostly random decisions with minimal strategy
    public class MediumAIPlayer : BaseAIPlayer
    {
        public MediumAIPlayer(IRandomService randomService)
            : base(randomService, AIDifficulty.Easy)
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
            var suits = Deck.GetCardSuits();
            return suits[_randomService.Next(0, suits.Length)];
        }

        public override int ChooseCard(List<Card> hand, CardSuit? leadingSuit, CardSuit? trumpSuit,
                                      List<(Card card, Player player)> cardsPlayedInTrick)
        {
            var playableCards = GetPlayableCards(hand, leadingSuit);

            // Play random valid card
            var randomCard = playableCards[_randomService.Next(0, playableCards.Count)];
            return hand.IndexOf(randomCard);
        }  
    }
}