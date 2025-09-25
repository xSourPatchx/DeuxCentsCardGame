// using DeuxCentsCardGame.Config;
using DeuxCentsCardGame.Events;
using DeuxCentsCardGame.Interfaces;
using DeuxCentsCardGame.Models;
// using DeuxCentsCardGame.Managers;


namespace DeuxCentsCardGame.Managers
{
    public class ScoringManager : IScoringManager
    {
        private readonly GameEventManager _eventManager;
        private readonly List<Player> _players;
        private readonly ITeamManager _teamManager;
        private readonly IGameConfig _gameConfig;

        public int TeamOneRoundPoints { get; private set; }
        public int TeamTwoRoundPoints { get; private set; }
        public int TeamOneTotalPoints { get; private set; }
        public int TeamTwoTotalPoints { get; private set; }

        public ScoringManager(GameEventManager eventManager, List<Player> players, 
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
            int teamRoundPoints = team == Team.TeamOne ? TeamOneRoundPoints : TeamTwoRoundPoints;
            int teamTotalPoints = team == Team.TeamOne ? TeamOneTotalPoints : TeamTwoTotalPoints;

            var (player1Index, player2Index) = _teamManager.GetTeamPlayerIndices(team);

            bool teamCannotScore = teamTotalPoints >= _gameConfig.CannotScoreThreshold &&
                                    !_players[player1Index].HasBet &&
                                    !_players[player2Index].HasBet;

            int awardedPoints = CalculateAwardedPoints(teamRoundPoints, teamCannotScore, isBidWinner, winningBid);

            string teamName = _teamManager.GetTeamName(team);
            bool madeBid = isBidWinner && teamRoundPoints >= winningBid;
            _eventManager.RaiseTeamScoring(teamName, teamRoundPoints, winningBid, madeBid, teamCannotScore, awardedPoints);

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