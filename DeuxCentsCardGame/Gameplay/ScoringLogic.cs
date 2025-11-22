using DeuxCentsCardGame.Interfaces.Gameplay;
using DeuxCentsCardGame.Interfaces.GameConfig;

namespace DeuxCentsCardGame.Gameplay
{
    public class ScoringLogic : IScoringLogic
    {
        private readonly IGameConfig _gameConfig;

        public ScoringLogic(IGameConfig gameConfig)
        {
            _gameConfig = gameConfig ?? throw new ArgumentNullException(nameof(gameConfig));
        }

        public bool DetermineIfTeamCannotScore(int teamTotalPoints, bool player1HasBet, bool player2HasBet)
        {
            return teamTotalPoints >= _gameConfig.CannotScoreThreshold &&
                !player1HasBet &&
                !player2HasBet;
        }

        public int CalculateAwardedPoints(int teamRoundPoints, bool teamCannotScore, bool isBidWinner, int winningBid)
        {
            // Cannot score - no points awarded
            if (teamCannotScore)
            {
                return 0;
            }

            // Bid winner - check if bid was made
            if (isBidWinner)
            {
                return CalculateBidWinnerPoints(teamRoundPoints, winningBid);
            }

            // Non-bid winner - award round points
            return teamRoundPoints;
        }

        private int CalculateBidWinnerPoints(int teamRoundPoints, int winningBid)
        {
            return teamRoundPoints >= winningBid ? teamRoundPoints : -winningBid;
        }

        public bool DetermineBidSuccess(bool isBidWinner, int teamRoundPoints, int winningBid)
        {
            return isBidWinner && teamRoundPoints >= winningBid;
        }

        public bool IsGameOver(int teamOneTotalPoints, int teamTwoTotalPoints)
        {
            return teamOneTotalPoints >= _gameConfig.WinningScore || 
                teamTwoTotalPoints >= _gameConfig.WinningScore;
        }
    }
}