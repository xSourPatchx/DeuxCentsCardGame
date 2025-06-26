using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Events.EventArgs
{
    public class TeamScoringEventArgs : System.EventArgs
    {
        public string TeamName { get; }
        public int RoundPoints { get; }
        public int WinningBid { get; }
        public bool MadeBid { get; }
        public bool CannotScore { get; }
        public int AwardedPoints { get; }

        public TeamScoringEventArgs(string teamName, int roundPoints, int winningBid, bool madeBid, bool cannotScore, int awardedPoints)
        {
            TeamName = teamName;
            RoundPoints = roundPoints;
            WinningBid = winningBid;
            MadeBid = madeBid;
            CannotScore = cannotScore;
            AwardedPoints = awardedPoints;
        }
    }
}