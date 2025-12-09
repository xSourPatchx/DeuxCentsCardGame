using DeuxCentsCardGame.Gameplay;
using DeuxCentsCardGame.Interfaces.Events;
using DeuxCentsCardGame.Interfaces.Managers;
using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Managers
{
public class ScoringManager : IScoringManager
{
private readonly IGameEventManager _eventManager;
private readonly List<Player> _players;
private readonly ITeamManager _teamManager;
private readonly ScoringLogic _scoringLogic;


    public int TeamOneRoundPoints { get; private set; }
    public int TeamTwoRoundPoints { get; private set; }
    public int TeamOneTotalPoints { get; private set; }
    public int TeamTwoTotalPoints { get; private set; }

    public ScoringManager(
        IGameEventManager eventManager,
        List<Player> players, 
        ITeamManager teamManager,
        ScoringLogic scoringLogic)
    {
        _eventManager = eventManager ?? throw new ArgumentNullException(nameof(eventManager));
        _players = players ?? throw new ArgumentNullException(nameof(players));
        _teamManager = teamManager ?? throw new ArgumentNullException(nameof(teamManager));
        _scoringLogic = scoringLogic ?? throw new ArgumentNullException(nameof(scoringLogic));
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

        _eventManager.RaiseScoreUpdated(
            TeamOneRoundPoints, TeamTwoRoundPoints,
            TeamOneTotalPoints, TeamTwoTotalPoints,
            bidWinnerIsTeamOne, winningBid);
    }

    private void ScoreTeam(Team team, bool isBidWinner, int winningBid)
    {
        int teamRoundPoints = GetTeamRoundPoints(team);
        int teamTotalPoints = GetTeamTotalPoints(team);

        var (player1Index, player2Index) = _teamManager.GetTeamPlayerIndices(team);
        bool teamCannotScore = _scoringLogic.DetermineIfTeamCannotScore(
            teamTotalPoints,
            _players[player1Index].HasBet,
            _players[player2Index].HasBet);

        int awardedPoints = _scoringLogic.CalculateAwardedPoints(
            teamRoundPoints,
            teamCannotScore,
            isBidWinner,
            winningBid);

        bool madeBid = _scoringLogic.DetermineBidSuccess(isBidWinner, teamRoundPoints, winningBid);

        RaiseTeamScoringEvent(team, teamRoundPoints, winningBid, madeBid, teamCannotScore, awardedPoints);

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

    private void RaiseTeamScoringEvent(
        Team team,
        int teamRoundPoints,
        int winningBid,
        bool madeBid,
        bool teamCannotScore,
        int awardedPoints)
    {
        string teamName = _teamManager.GetTeamName(team);
        _eventManager.RaiseTeamScoring(
            teamName,
            teamRoundPoints,
            winningBid,
            madeBid,
            teamCannotScore,
            awardedPoints);
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

    public bool IsGameOver()
    {
        return _scoringLogic.IsGameOver(TeamOneTotalPoints, TeamTwoTotalPoints);
    }

    public void RaiseGameOverEvent()
    {
        _eventManager.RaiseGameOver(TeamOneTotalPoints, TeamTwoTotalPoints);
    }
}
}