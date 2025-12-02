using DeuxCentsCardGame.GameStates;
using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Interfaces.Models
{  
    public interface IGameSnapshotBuilder
    {
        IGameSnapshotBuilder WithGameState(GameState current, GameState previous);
        IGameSnapshotBuilder WithRound(int roundNumber, int trickNumber);
        IGameSnapshotBuilder WithDealer(int dealerIndex);
        IGameSnapshotBuilder WithTrump(CardSuit? trumpSuit);
        IGameSnapshotBuilder WithPaused(bool isPaused);
        IGameSnapshotBuilder WithPlayers(List<Player> players);
        IGameSnapshotBuilder WithScores(ScoreSnapshot scores);
        IGameSnapshotBuilder WithBetting(BettingSnapshot betting);
        IGameSnapshotBuilder WithCurrentTrick(TrickSnapshot? trick);
        IGameSnapshotBuilder WithDescription(string description);
        GameSnapshot Build();
    }
}