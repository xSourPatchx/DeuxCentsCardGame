using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Interfaces.AI
{
    public interface IAIPlayer
    {
        int DecideBet(List<Card> hand, int minBet, int maxBet, int betIncrement, int currentHighestBid, List<int> takenBids);

        CardSuit SelectTrumpSuit(List<Card> hand);
        int ChooseCard(List<Card> hand, CardSuit? leadingSuit, CardSuit? trumpSuit, List<(Card card, Player player)> cardsPlayedInTrick);
    }
}