using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Events.EventArgs
{
    public class BettingCompletedEventArgs : System.EventArgs
    {
        public Player WinningBidder { get; }
        public int WinningBid { get; }
        public Dictionary<Player, int> AllBids { get; }

        public BettingCompletedEventArgs(Player winningBidder, int winningBid, Dictionary<Player, int> allBids)
        {
            WinningBidder = winningBidder;
            WinningBid = winningBid;
            AllBids = new Dictionary<Player, int>(allBids);
        }
    }
}