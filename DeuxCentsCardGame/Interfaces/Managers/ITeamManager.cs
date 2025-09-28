using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Interfaces.Managers
{
    public enum Team { TeamOne, TeamTwo }

    public interface ITeamManager
    {
        Team GetPlayerTeam(int playerIndex);
        bool IsPlayerOnTeamOne(int playerIndex);
        bool IsPlayerOnTeamTwo(int playerIndex);
        (int player1, int player2) GetTeamPlayerIndices(Team team);
        List<Player> GetTeamPlayers(Team team, List<Player> allPlayers);
        string GetTeamName(Team team);
        string GetTeamName(int playerIndex);
        bool HasTeamPlacedBet(Team team, List<Player> allPlayers);  
    }
}
