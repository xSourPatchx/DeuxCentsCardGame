 Can you help me create NetworkSyncValidator (Located in DeuxCentsCardGame/Network/NetworkSyncValidator.cs) to future-proof for Unity Netcode. The purpose is to Validate game state consistency which is Essential for multiplayer game integrity, Detects cheating/desync early and Separates network concerns from game logic.

It should look something like this:
"public class NetworkSyncValidator
{
    public bool ValidateGameState(GameStateSnapshot local, GameStateSnapshot remote);
    public bool ValidatePlayerAction(PlayerAction action, GameState currentState);
    public List<ValidationError> GetSyncErrors();
}"

---

Can you ensure my code does not use any Static UI References.

---

Can you ensure my code does not use Nested Loops or Complex Logic. In general i'd like to break down into smaller, testable methods.

---

Can you help me make my Game Logic Async-Friendly to prepare for Unity multiplayer since it often requires async operations.

---

Since I'll be porting this console app to a Unity multiplayer game, and In events, Unity prefers Action/UnityEvent patterns, can you help me do this for my events?
