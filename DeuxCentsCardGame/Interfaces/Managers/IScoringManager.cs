namespace DeuxCentsCardGame.Interfaces.Managers
{
    public interface IScoringManager
    {
        int TeamOneRoundPoints { get; }
        int TeamTwoRoundPoints { get; }
        int TeamOneTotalPoints { get; }
        int TeamTwoTotalPoints { get; }
        void ResetRoundPoints();
        void AwardTrickPoints(int trickWinnerIndex, int trickPoints);
        void ScoreRound(int winningBidIndex, int winningBid);
        bool IsGameOver();
        void RaiseGameOverEvent();
    }
}
