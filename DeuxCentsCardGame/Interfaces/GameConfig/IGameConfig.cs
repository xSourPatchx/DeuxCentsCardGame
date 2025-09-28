namespace DeuxCentsCardGame.Interfaces.GameConfig
{
    public interface IGameConfig
    {
        int TeamOnePlayer1 { get; }
        int TeamOnePlayer2 { get; }
        int TeamTwoPlayer1 { get; }
        int TeamTwoPlayer2 { get; }
        int TotalPlayers { get; }
        int PlayersPerTeam { get; }
        int WinningScore { get; }
        int CannotScoreThreshold { get; }
        int MinimumBet { get; }
        int MaximumBet { get; }
        int BetIncrement { get; }
        int MinimumPlayersToPass { get; }
        int PassedBidValue { get; }
    }
}