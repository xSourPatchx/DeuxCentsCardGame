using DeuxCentsCardGame.Interfaces;
using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Managers
{
    public enum Team { TeamOne, TeamTwo }

    public class TeamManager : ITeamManager
    {
        private readonly IGameConfig _gameConfig;

        public TeamManager(IGameConfig gameConfig)
        {
            _gameConfig = gameConfig ?? throw new ArgumentNullException(nameof(gameConfig));
        }

        public Team GetPlayerTeam(int playerIndex)
        {
            return IsPlayerOnTeamOne(playerIndex) ? Team.TeamOne : Team.TeamTwo;
        }

        public bool IsPlayerOnTeamOne(int playerIndex)
        {
            return playerIndex % 2 == 0;
        }

        public bool IsPlayerOnTeamTwo(int playerIndex)
        {
            return playerIndex % 2 == 1;
        }

        public (int player1, int player2) GetTeamPlayerIndices(Team team)
        {
            return team switch
            {
                Team.TeamOne => (_gameConfig.TeamOnePlayer1, _gameConfig.TeamOnePlayer2),
                Team.TeamTwo => (_gameConfig.TeamTwoPlayer1, _gameConfig.TeamTwoPlayer2),
                _ => throw new ArgumentException($"Invalid team: {team}")
            };
        }

        public List<Player> GetTeamPlayers(Team team, List<Player> allPlayers)
        {
            var (player1Index, player2Index) = GetTeamPlayerIndices(team);
            return new List<Player> { allPlayers[player1Index], allPlayers[player2Index] };
        }

        public string GetTeamName(Team team)
        {
            return team switch
            {
                Team.TeamOne => "Team One",
                Team.TeamTwo => "Team Two",
                _ => throw new ArgumentException($"Invalid team: {team}")
            };
        }

        public string GetTeamName(int playerIndex)
        {
            return GetTeamName(GetPlayerTeam(playerIndex));
        }

        public bool HasTeamPlacedBet(Team team, List<Player> allPlayers)
        {
            var teamPlayers = GetTeamPlayers(team, allPlayers);
            return teamPlayers.Any(player => player.HasBet);
        }
    }
}