// Recommendation: Break this into smaller classes:
// - DealingService
// - TrumpSelectionService
// - ScoringService

// Other Folder called "Managers"
// - TeamManager

// 1. Scoring logic should be separate
public class ScoringCalculator
{   
    public int CalculateTeamScore(int roundPoints, int totalPoints, int winningBid, 
                                 bool isBidWinner, bool teamHasBet)
    {
        bool cannotScore = totalPoints >= CANNOT_SCORE_THRESHOLD && !teamHasBet;
        
        if (cannotScore)
            return 0;
            
        if (isBidWinner)
            return roundPoints >= winningBid ? roundPoints : -winningBid;
            
        return roundPoints;
    }
}

// 2. Team management logic
public class TeamManager
{
    public bool IsPlayerOnTeamOne(int playerIndex)
    {
        return playerIndex % 2 == 0;
    }
    
    public (int player1, int player2) GetTeamPlayerIndices(bool isTeamOne)
    {
        return isTeamOne ? (TEAM_ONE_PLAYER_1, TEAM_ONE_PLAYER_2) : (TEAM_TWO_PLAYER_1, TEAM_TWO_PLAYER_2);
    }
    
    public bool TeamHasBet(List<Player> players, bool isTeamOne)
    {
        var (player1Index, player2Index) = GetTeamPlayerIndices(isTeamOne);
        return players[player1Index].HasBet || players[player2Index].HasBet;
    }
}

// Main orchestrator - will become MonoBehaviour in Unity
public class GameController 
{
    // Game flow control
    // Round management  
    // Coordination between services
}