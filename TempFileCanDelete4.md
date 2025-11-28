 Can you help me create NetworkSyncValidator (Located in DeuxCentsCardGame/Network/NetworkSyncValidator.cs) to future-proof for Unity Netcode. The purpose is to Validate game state consistency which is Essential for multiplayer game integrity, Detects cheating/desync early and Separates network concerns from game logic.

It should look something like this:
"public class NetworkSyncValidator
{
    public bool ValidateGameState(GameStateSnapshot local, GameStateSnapshot remote);
    public bool ValidatePlayerAction(PlayerAction action, GameState currentState);
    public List<ValidationError> GetSyncErrors();
}"

---

Can you help me create a class called HandEvaluator(Located in DeuxCentsCardGame/Gameplay/HandEvaluator.cs) to Extract from BaseAIPlayer. The Purpose is to Analyze hand strength and composition, Both AI and UI can use same logic, it can Show hints to human players and its Reusable for analytics/statistics

It should look something like this:
"public class HandEvaluator
{
    public int CalculateHandStrength(List<Card> hand);
    public Dictionary<CardSuit, int> GetSuitCounts(List<Card> hand);
    public CardSuit GetStrongestSuit(List<Card> hand);
    public int CountHighCards(List<Card> hand, int threshold);
    public bool HasVoid(List<Card> hand, CardSuit suit);
}"

---

Can you help me create a class called TrickAnalyzer (Located in DeuxCentsCardGame/Gameplay/TrickAnalyzer.cs) to Extract from HardAIPlayer.

The Purpose is to Analyze current trick state, can be Shared between all AI difficulties, Can power UI hints/animations and is Testable in isolation

It should look something like this:
"public class TrickAnalyzer
{
    public Card GetCurrentWinningCard(List<(Card, Player)> trick, CardSuit? trump, CardSuit? leading);
    public int CalculateTrickValue(List<(Card, Player)> trick);
    public bool IsPartnerWinning(List<(Card, Player)> trick, int playerIndex, ITeamManager teamManager);
    public List<Card> GetWinningCards(List<Card> hand, Card currentWinner, CardSuit? trump, CardSuit? leading);
}"

---

Can you help me create a class called GameSnapshot (State Serialization Helper) Located in DeuxCentsCardGame/Models/GameSnapshot.cs. the Purpose is to Capture game state for save/load/network which is Essential for Unity multiplayer state sync, can Save/load functionality and can Replay/spectator mode.

It should look something like this:
"[Serializable]
public class GameSnapshot
{
    public GameState CurrentState { get; set; }
    public int RoundNumber { get; set; }
    public int DealerIndex { get; set; }
    public CardSuit? TrumpSuit { get; set; }
    public List<PlayerSnapshot> Players { get; set; }
    public ScoreSnapshot Scores { get; set; }
    
    public byte[] Serialize();
    public static GameSnapshot Deserialize(byte[] data);
}"

---

Can you ensure my code does not use any Static UI References.

---

Can you ensure my code does not use Nested Loops or Complex Logic. In general i'd like to break down into smaller, testable methods.

---

Can you help me make my Game Logic Async-Friendly to prepare for Unity multiplayer since it often requires async operations.

---

Since I'll be porting this console app to a Unity multiplayer game, and In events, Unity prefers Action/UnityEvent patterns, can you help me do this for my events?