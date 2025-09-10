using DeuxCentsCardGame.Config;
using DeuxCentsCardGame.Events;
using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Managers
{
    public class ScoringManager
    {
        private readonly GameEventManager _eventManager;
        private readonly List<Player> _players;

        public int TeamOneRoundPoints { get; private set; }
        public int TeamTwoRoundPoints { get; private set; }
        public int TeamOneTotalPoints { get; private set; }
        public int TeamTwoTotalPoints { get; private set; }

        public ScoringManager(GameEventManager eventManager, List<Player> players)
        {
            _eventManager = eventManager;
            _players = players;
        }

        public void ResetRoundPoints()
        {
            TeamOneRoundPoints = 0;
            TeamTwoRoundPoints = 0;
        }
    
        public void AwardTrickPoints(int trickWinnerIndex, int trickPoints)
        {
            bool isTeamOne = IsPlayerOnTeamOne(trickWinnerIndex);
            string teamName = isTeamOne ? "Team One" : "Team Two";

            if (isTeamOne)
            {
                TeamOneRoundPoints += trickPoints;
            }
            else
            {
                TeamTwoRoundPoints += trickPoints;
            }

            _eventManager.RaiseTrickPointsAwarded(_players[trickWinnerIndex], trickPoints, teamName);
        }

        public void ScoreRound(int winningBidIndex, int winningBid)
        {
            bool bidWinnerIsTeamOne = IsPlayerOnTeamOne(winningBidIndex);

            ScoreTeam(bidWinnerIsTeamOne, true, winningBid);
            ScoreTeam(!bidWinnerIsTeamOne, false, winningBid);

            _eventManager.RaiseScoreUpdated(TeamOneRoundPoints, TeamTwoRoundPoints,
                                            TeamOneTotalPoints, TeamTwoTotalPoints,
                                            bidWinnerIsTeamOne, winningBid);
        }

        private void ScoreTeam(bool isTeamOne, bool isBidWinner, int winningBid)
        {
            int teamRoundPoints = isTeamOne ? TeamOneRoundPoints : TeamTwoRoundPoints;
            int teamTotalPoints = isTeamOne ? TeamOneTotalPoints : TeamTwoTotalPoints;

            var (player1Index, player2Index) = GetPlayerIndices(isTeamOne);

            bool teamCannotScore = teamTotalPoints >= GameConfig.CannotScoreThreshold &&
                                    !_players[player1Index].HasBet &&
                                    !_players[player2Index].HasBet;

            int awardedPoints = CalculateAwardedPoints(teamRoundPoints, teamCannotScore, isBidWinner, winningBid);

            string teamName = isTeamOne ? "Team One" : "Team Two";
            bool madeBid = isBidWinner && teamRoundPoints >= winningBid;
            _eventManager.RaiseTeamScoring(teamName, teamRoundPoints, winningBid, madeBid, teamCannotScore, awardedPoints);

            if (isTeamOne)
            {
                TeamOneTotalPoints += awardedPoints;
            }
            else
            {
                TeamTwoTotalPoints += awardedPoints;
            }
        }

        private (int player1, int player2) GetPlayerIndices(bool isTeamOne)
        {
            return isTeamOne ? (GameConfig.TeamOnePlayer1, GameConfig.TeamOnePlayer2)
                                : (GameConfig.TeamTwoPlayer1, GameConfig.TeamTwoPlayer2);
        }

        private int CalculateAwardedPoints(int teamRoundPoints, bool teamCannotScore, bool isBidWinner, int winningBid)
        {
            if (teamCannotScore)
            {
                return 0;
            }
            else if (isBidWinner)
            {
                return teamRoundPoints >= winningBid
                                        ? teamRoundPoints
                                        : -winningBid;
            }
            else
            {
                return teamRoundPoints;
            }
        }

        public bool IsPlayerOnTeamOne(int playerIndex)
        {
            return playerIndex % 2 == 0;
        }

        public bool IsGameOver()
        {
            return TeamOneTotalPoints >= GameConfig.WinningScore || 
                    TeamTwoTotalPoints >= GameConfig.WinningScore;
        }

        public void RaiseGameOverEvent()
        {
            _eventManager.RaiseGameOver(TeamOneTotalPoints, TeamTwoTotalPoints);
        }
    }
}