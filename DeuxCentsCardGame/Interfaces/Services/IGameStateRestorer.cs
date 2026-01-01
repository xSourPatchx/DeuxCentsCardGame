using DeuxCentsCardGame.Controllers;
using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Services
{
    public interface IGameStateRestorer
    {
        bool RestoreGameState(GameSnapshot snapshot, GameController gameController);
        bool RestorePlayers(List<PlayerSnapshot> playerSnapshots);
        bool RestoreScores(ScoreSnapshot scoreSnapshot);
        bool RestoreBetting(BettingSnapshot bettingSnapshot);
    }
}