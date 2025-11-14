using DeuxCentsCardGame.AI;
using DeuxCentsCardGame.Interfaces.AI;
using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Interfaces.Services
{
    public interface IAIService
    {
        // Gets the AI player instance for a specific difficulty
        IAIPlayer GetAIPlayer(AIDifficulty difficulty);

        // Makes an AI betting decision
        int MakeAIBettingDecision(Player player, AIDifficulty difficulty, int minBet, 
                                int maxBet, int betIncrement, int currentHighestBid, 
                                List<int> takenBids);

        // Makes an AI trump suit selection
        CardSuit MakeAITrumpSelection(Player player, AIDifficulty difficulty);

        // Makes an AI card selection
        int MakeAICardSelection(Player player, AIDifficulty difficulty, CardSuit? leadingSuit, 
                            CardSuit? trumpSuit, List<(Card card, Player player)> cardsPlayedInTrick);
    }
}