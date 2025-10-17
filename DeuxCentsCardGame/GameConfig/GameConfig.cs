using DeuxCentsCardGame.Constants;
using DeuxCentsCardGame.Interfaces.GameConfig;

namespace DeuxCentsCardGame.GameConfig
{
    public class GameConfig : IGameConfig
    {
        // Game configuration - using constants as defaults
        public int TeamOnePlayer1 { get; set; } = GameConstants.TEAM_ONE_PLAYER_1;
        public int TeamOnePlayer2 { get; set; } = GameConstants.TEAM_ONE_PLAYER_2;
        public int TeamTwoPlayer1 { get; set; } = GameConstants.TEAM_TWO_PLAYER_1;
        public int TeamTwoPlayer2 { get; set; } = GameConstants.TEAM_TWO_PLAYER_2;
        public int TotalPlayers { get; set; } = GameConstants.TOTAL_PLAYERS;
        public int PlayersPerTeam { get; set; } = GameConstants.PLAYERS_PER_TEAM;
        public int WinningScore { get; set; } = GameConstants.WINNING_SCORE;
        public int CannotScoreThreshold { get; set; } = GameConstants.CANNOT_SCORE_THRESHOLD;

        // Betting configuration
        public int MinimumBet { get; set; } = GameConstants.MINIMUM_BET;
        public int MaximumBet { get; set; } = GameConstants.MAXIMUM_BET;
        public int BetIncrement { get; set; } = GameConstants.BET_INCREMENT;
        public int MinimumPlayersToPass { get; set; } = GameConstants.MINIMUM_PLAYERS_TO_PASS;
        public int PassedBidValue { get; set; } = GameConstants.PASSED_BID_VALUE;

        public static IGameConfig CreateDefault()
        {
            return new GameConfig();
        }
    }
}