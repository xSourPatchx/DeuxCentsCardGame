using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Events.EventArgs
{
    public class TrickPointsAwardedEventArgs : System.EventArgs
    {
        public Player Player { get; }
        public int TrickPoints { get; }
        public string TeamName { get; }

        public TrickPointsAwardedEventArgs(Player player, int trickPoints, string teamName)
        {
            Player = player;
            TrickPoints = trickPoints;
            TeamName = teamName;
        }
    }
}