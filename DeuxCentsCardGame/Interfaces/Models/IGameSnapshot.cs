using DeuxCentsCardGame.GameStates;
using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Interfaces.Models
{
    public interface IGameSnapshot
    {
        GameState CurrentState { get; set; }
        GameState PreviousState { get; set; }
        int RoundNumber { get; set; }
        int TrickNumber { get; set; }
        int DealerIndex { get; set; }
        CardSuit? TrumpSuit { get; set; }
        bool IsPaused { get; set; }
        DateTime Timestamp { get; set; }
        List<PlayerSnapshot> Players { get; set; }
        ScoreSnapshot Scores { get; set; }
        BettingSnapshot Betting { get; set; }
        TrickSnapshot? CurrentTrick { get; set; }
        string SnapshotId { get; set; }
        string? Description { get; set; }
        int Version { get; set; }


            byte[] Serialize();
            string SerializeToJson();
            GameSnapshot Clone();
            bool IsValid();
    }
}