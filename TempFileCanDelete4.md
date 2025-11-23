Can you help me make a Helper Classes called GameStateTransitions, Located in DeuxCentsCardGame/GameStates/GameStateTransitions.cs. The Purpose is to Encapsulate valid state transition logic so that Unity multiplayer has strict state validation, Prevents desync between clients and Centralizes transition rules.

It should look something like this: 
"public class GameStateTransitions
{
    private readonly Dictionary<GameState, List<GameState>> _validTransitions;
    
    public bool IsValidTransition(GameState from, GameState to);
    public List<GameState> GetValidNextStates(GameState current);
    public void ValidateTransition(GameState from, GameState to);
}"

---

Can you ensure my code does not use any Static UI References.

---

Can you ensure my code does not use Nested Loops or Complex Logic. In general i'd like to break down into smaller, testable methods.

---

Can you help me make my Game Logic Async-Friendly to prepare for Unity multiplayer since it often requires async operations.

---

Since I'll be porting this console app to a Unity multiplayer game, and In events, Unity prefers Action/UnityEvent patterns, can you help me do this for my events?