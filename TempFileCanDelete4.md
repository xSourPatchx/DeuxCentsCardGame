Can you review my code to add any supporting helper classes to separated concerns, note that this will be later ported over into a Unity mulitplayer game, so I'd like to use Unity best practices.

---

For my AI/CPU player, I have 3 places where I can select the player types. The program class is the ultimate truth, however i'd like to only have one centralize place to configure this. So far there is "playerManager.InitializePlayersWithTypes(PlayerType.Human, PlayerType.AI, PlayerType.Human, PlayerType.AI);" in Program class, ""PlayerTypes": ["Human", "Human", "Human", "Human"]," in appsettings.json and "return new List<Player>
{
    new("Player 1", PlayerType.Human),
    new("Player 2", PlayerType.Human),
    new("Player 3", PlayerType.Human),
    new("Player 4", PlayerType.Human)
};" in PlayerManager. Can you suggest the best way to move forward with this?

---

Can you ensure my code does not use any Static UI References.

---

Can you ensure my code does not use Nested Loops or Complex Logic. In general i'd like to break down into smaller, testable methods.

---

Can you help me make my Game Logic Async-Friendly to prepare for Unity multiplayer since it often requires async operations.

---

Since I'll be porting this console app to a Unity multiplayer game, and In events, Unity prefers Action/UnityEvent patterns, can you help me do this for my events?