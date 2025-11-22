namespace DeuxCentsCardGame.Interfaces.Gameplay
{
    public interface IScoringLogic
    {
        bool DetermineIfTeamCannotScore(int teamTotalPoints, bool player1HasBet, bool player2HasBet);
        int CalculateAwardedPoints(int teamRoundPoints, bool teamCannotScore, bool isBidWinner, int winningBid);
        bool DetermineBidSuccess(bool isBidWinner, int teamRoundPoints, int winningBid);
        bool IsGameOver(int teamOneTotalPoints, int teamTwoTotalPoints);
    }
}