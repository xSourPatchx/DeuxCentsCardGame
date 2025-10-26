using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Events.EventArgs
{
    public class RoundEndedEventArgs : System.EventArgs
    {
        public int RoundNumber { get; }
        public int TeamOneRoundPoints { get; }
        public int TeamTwoRoundPoints { get; }
        public Player WinningBidder { get; }
        public int WinningBid { get; }

        public RoundEndedEventArgs(int roundNumber, int teamOneRoundPoints, int teamTwoRoundPoints, Player winningBidder, int winningBid)
        {
            RoundNumber = roundNumber;
            TeamOneRoundPoints = teamOneRoundPoints;
            TeamTwoRoundPoints = teamTwoRoundPoints;
            WinningBidder = winningBidder;
            WinningBid = winningBid;
        }
    }
}