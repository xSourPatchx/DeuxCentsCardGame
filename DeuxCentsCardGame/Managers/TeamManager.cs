namespace DeuxCentsCardGame.Managers
{
    public class TeamManager
    {
        public enum Team { TeamOne, TeamTwo }

        public static bool IsPlayerOnTeamOne(int playerIndex)
        {
            return playerIndex % 2 == 0;
        }

        public static bool IsPlayerOnTeamTwo(int playerIndex)
        {
            return playerIndex % 2 == 1;
        }
    }
}