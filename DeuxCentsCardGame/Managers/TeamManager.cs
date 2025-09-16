using DeuxCentsCardGame.Config;
using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Managers
{
    public class TeamManager
    {
        public enum Team { TeamOne, TeamTwo }

        public static Team GetPlayerTeam(int playerIndex)
        {
            return IsPlayerOnTeamOne(playerIndex) ? Team.TeamOne : Team.TeamTwo;
        }

        public static bool IsPlayerOnTeamOne(int playerIndex)
        {
            return playerIndex % 2 == 0;
        }

        public static bool IsPlayerOnTeamTwo(int playerIndex)
        {
            return playerIndex % 2 == 1;
        }

        public static (int player1, int player2) GetTeamPlayerIndices(Team team)
        {
            return team switch
            {
                Team.TeamOne => (GameConfig.TeamOnePlayer1, GameConfig.TeamOnePlayer2),
                Team.TeamTwo => (GameConfig.TeamTwoPlayer1, GameConfig.TeamTwoPlayer2),
                _ => throw new ArgumentException($"Invalid team: {team}")
            };
        }

        public static List<Player> GetTeamPlayers(Team team, List<Player> allPlayers)
        {
            var (player1Index, player2Index) = GetTeamPlayerIndices(team);
            return new List<Player> { allPlayers[player1Index], allPlayers[player2Index] };
        }

        public static string GetTeamName(Team team)
        {
            return team switch
            {
                Team.TeamOne => "Team One",
                Team.TeamTwo => "Team Two",
                _ => throw new ArgumentException($"Invalid team: {team}")
            };
        }

        public static string GetTeamName(int playerIndex)
        {
            return GetTeamName(GetPlayerTeam(playerIndex));
        }

        public static bool HasTeamPlacedBet(Team team, List<Player> allPlayers)
        {
            var teamPlayers = GetTeamPlayers(team, allPlayers);
            return teamPlayers.Any(player => player.HasBet);
        }

    }
}