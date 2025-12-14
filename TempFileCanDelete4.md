 Can you help me create NetworkSyncValidator (Located in DeuxCentsCardGame/Network/NetworkSyncValidator.cs) to future-proof for Unity Netcode. The purpose is to Validate game state consistency which is Essential for multiplayer game integrity, Detects cheating/desync early and Separates network concerns from game logic.

It should look something like this:
"public class NetworkSyncValidator
{
    public bool ValidateGameState(GameStateSnapshot local, GameStateSnapshot remote);
    public bool ValidatePlayerAction(PlayerAction action, GameState currentState);
    public List<ValidationError> GetSyncErrors();
}"

---

Can you ensure my code does not use Nested Loops or Complex Logic. In general i'd like to break down into smaller, testable methods.

---

Since I'll be porting this console app to a Unity multiplayer game, and In events, Unity prefers Action/UnityEvent patterns, can you help me do this for my events?

---

My console app is getting quite large and I'd like simplify and remove some of the code that is not needed, basically use princinples like KISS (Keep It Simple, Stupid), DRY (Don't Repeat Yourself) and YAGNI (You Ain't Gonna Need It). Can you help me identify what can be removed?