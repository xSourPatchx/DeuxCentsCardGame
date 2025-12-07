using DeuxCentsCardGame.AI;
using DeuxCentsCardGame.Interfaces.AI;
using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Interfaces.Services
{
    public interface IAIService
    {
        IAIPlayer GetAIPlayer(AIDifficulty difficulty);
        Task<int> MakeAIBettingDecision(Player player, AIDifficulty difficulty, int minBet, 
                                int maxBet, int betIncrement, int currentHighestBid, 
                                List<int> takenBids);
        Task<CardSuit> MakeAITrumpSelection(Player player, AIDifficulty difficulty);
        Task<int> MakeAICardSelection(Player player, AIDifficulty difficulty, CardSuit? leadingSuit, 
                            CardSuit? trumpSuit, List<(Card card, Player player)> cardsPlayedInTrick);
    }
}