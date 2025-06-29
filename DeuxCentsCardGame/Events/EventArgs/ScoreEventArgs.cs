namespace DeuxCentsCardGame.Events.EventArgs
{    public class ScoreEventArgs : System.EventArgs
    {
        public int TeamOneRoundPoints { get; }
        public int TeamTwoRoundPoints { get; }
        public int TeamOneTotalPoints { get; }
        public int TeamTwoTotalPoints { get; }
        public bool IsBidWinnerTeamOne { get; }
        public int WinningBid { get; }

        public ScoreEventArgs(int teamOneRoundPoints, int teamTwoRoundPoints, int teamOneTotalPoints, int teamTwoTotalPoints, bool isBidWinnerTeamOne, int winningBid)
        {
            TeamOneRoundPoints = teamOneRoundPoints;
            TeamTwoRoundPoints = teamTwoRoundPoints;
            TeamOneTotalPoints = teamOneTotalPoints;
            TeamTwoTotalPoints = teamTwoTotalPoints;
            IsBidWinnerTeamOne = isBidWinnerTeamOne;
            WinningBid = winningBid;
        }
    }
}