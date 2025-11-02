using DeuxCentsCardGame.Interfaces.Events;
using DeuxCentsCardGame.Interfaces.GameConfig;
using DeuxCentsCardGame.Interfaces.Managers;
using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Managers
{
    public class ScoringManager : IScoringManager
    {
        private readonly IGameEventManager _eventManager;
        private readonly List<Player> _players;
        private readonly ITeamManager _teamManager;
        private readonly IGameConfig _gameConfig;

        public int TeamOneRoundPoints { get; private set; }
        public int TeamTwoRoundPoints { get; private set; }
        public int TeamOneTotalPoints { get; private set; }
        public int TeamTwoTotalPoints { get; private set; }

        public ScoringManager(IGameEventManager eventManager, List<Player> players, 
                            ITeamManager teamManager, IGameConfig gameConfig)
        {
            _eventManager = eventManager ?? throw new ArgumentNullException(nameof(eventManager));
            _players = players ?? throw new ArgumentNullException(nameof(players));
            _teamManager = teamManager ?? throw new ArgumentNullException(nameof(teamManager));
            _gameConfig = gameConfig ?? throw new ArgumentNullException(nameof(gameConfig));
        }

        public void ResetRoundPoints()
        {
            TeamOneRoundPoints = 0;
            TeamTwoRoundPoints = 0;
        }
    
        public void AwardTrickPoints(int trickWinnerIndex, int trickPoints)
        {
            bool isTeamOne = _teamManager.IsPlayerOnTeamOne(trickWinnerIndex);
            string teamName = _teamManager.GetTeamName(trickWinnerIndex);

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
            bool bidWinnerIsTeamOne = _teamManager.IsPlayerOnTeamOne(winningBidIndex);

            ScoreTeam(Team.TeamOne, bidWinnerIsTeamOne, winningBid);
            ScoreTeam(Team.TeamTwo, !bidWinnerIsTeamOne, winningBid);

            _eventManager.RaiseScoreUpdated(TeamOneRoundPoints, TeamTwoRoundPoints,
                                            TeamOneTotalPoints, TeamTwoTotalPoints,
                                            bidWinnerIsTeamOne, winningBid);
        }

        private void ScoreTeam(Team team, bool isBidWinner, int winningBid)
        {
            int teamRoundPoints = GetTeamRoundPoints(team);
            int teamTotalPoints = GetTeamTotalPoints(team);

            var (player1Index, player2Index) = _teamManager.GetTeamPlayerIndices(team);
            bool teamCannotScore = DetermineIfTeamCannotScore(teamTotalPoints, player1Index, player2Index);

            int awardedPoints = CalculateAwardedPoints(teamRoundPoints, teamCannotScore, isBidWinner, winningBid);

            RaiseTeamScoringEvent(team, teamRoundPoints, winningBid, isBidWinner, teamCannotScore, awardedPoints);

            AwardPointsToTeam(team, awardedPoints);
        }

        private int GetTeamRoundPoints(Team team)
        {
            return team == Team.TeamOne ? TeamOneRoundPoints : TeamTwoRoundPoints;
        }

        private int GetTeamTotalPoints(Team team)
        {
            return team == Team.TeamOne ? TeamOneTotalPoints : TeamTwoTotalPoints;
        }

        private bool DetermineIfTeamCannotScore(int teamTotalPoints, int player1Index, int player2Index)
        {
            return teamTotalPoints >= _gameConfig.CannotScoreThreshold &&
                                    !_players[player1Index].HasBet &&
                                    !_players[player2Index].HasBet;
        }

        private void RaiseTeamScoringEvent(Team team, int teamRoundPoints, int winningBid, 
                                          bool isBidWinner, bool teamCannotScore, int awardedPoints)
        {
            string teamName = _teamManager.GetTeamName(team);
            bool madeBid = isBidWinner && teamRoundPoints >= winningBid; 
            _eventManager.RaiseTeamScoring(teamName, teamRoundPoints, winningBid, madeBid, teamCannotScore, awardedPoints);
        }

        private void AwardPointsToTeam(Team team, int awardedPoints)
        {
            if (team == Team.TeamOne)
            {
                TeamOneTotalPoints += awardedPoints;
            }
            else
            {
                TeamTwoTotalPoints += awardedPoints;
            }
        }

        private int CalculateAwardedPoints(int teamRoundPoints, bool teamCannotScore, bool isBidWinner, int winningBid)
        {
            if (teamCannotScore)
            {
                return 0;
            }

            if (isBidWinner)
            {
                return CalculateBidWinnerPoints(teamRoundPoints, winningBid);
            }

            return teamRoundPoints;
        }
        
        private int CalculateBidWinnerPoints(int teamRoundPoints, int winningBid)
        {
            return teamRoundPoints >= winningBid ? teamRoundPoints : -winningBid;
        }

        public bool IsGameOver()
        {
            return TeamOneTotalPoints >= _gameConfig.WinningScore || 
                    TeamTwoTotalPoints >= _gameConfig.WinningScore;
        }

        public void RaiseGameOverEvent()
        {
            _eventManager.RaiseGameOver(TeamOneTotalPoints, TeamTwoTotalPoints);
        }
    }
}