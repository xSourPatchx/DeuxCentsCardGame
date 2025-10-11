using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Interfaces.Events
{
    public interface IGameEventManager
    {
        void RaiseBettingRoundStarted(string message);
        string RaiseBetInput(Player player, int minBet, int maxBet, int increment);
        void RaiseBettingAction(Player player, int bid, bool hasPassed, bool hasBet);
        void RaiseInvalidBet(string message);
        void RaiseBettingCompleted(Player winningBidder, int winningBid, Dictionary<Player, int> allBids);
    }
}