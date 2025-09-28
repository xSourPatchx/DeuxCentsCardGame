using DeuxCentsCardGame.Interfaces.GameConfig;

namespace DeuxCentsCardGame.GameConfig
{
    public class GameConfig : IGameConfig
    {
        // Game configuration
        public int TeamOnePlayer1 { get; set; } = 0;
        public int TeamOnePlayer2 { get; set; } = 2;
        public int TeamTwoPlayer1 { get; set; } = 1;
        public int TeamTwoPlayer2 { get; set; } = 3;
        public int TotalPlayers { get; set; } = 4;
        public int PlayersPerTeam { get; set; } = 2;
        public int WinningScore { get; set; } = 200;
        public int CannotScoreThreshold { get; set; } = 100;

        // Betting configuration
        public int MinimumBet { get; set; } = 50;
        public int MaximumBet { get; set; } = 100;
        public int BetIncrement { get; set; } = 5;
        public int MinimumPlayersToPass { get; set; } = 3;
        public int PassedBidValue { get; set; } = -1; // not using this yet

        public static IGameConfig CreateDefault()
        {
            return new GameConfig();
        }
    }
}