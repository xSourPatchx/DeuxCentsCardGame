### **Master TODO**
1. **Refactoring**
   - [x] Use enums for `CardSuit` and `CardFace` in `Card.cs`
   - [x] Consider making `Card` immutable (remove setters, use `init` properties)
   - [x] Add validation in `Card` constructor
   - [x] Consider encapsulate Card Logic by adding methods like `IsHigher(Card other)` directly in the `Card` class
   - [x] Adjust Game class accordingly with new helper methods
   - [x] The DetermineTrickWinner now returns a tuple of (Card, index) instead return (Card, Player) instead.
   - [x] Remove Unused Variables by cleaning up commented-out code and unused variables like `_winningPlayerIndex` and `// int trickWinnerIndex;`.
   - [x] Have Consistent Naming & Access Modifiers, make fields `private readonly` where possible. Use consistent naming (some fields have `_` prefix, others don't) and remove public fields that should be private.
   - [x] Remove all `Thread.Sleep()` calls from your UI class - these will cause Unity to freeze. Replace with Unity-appropriate timing mechanisms later.
   - [x] Fix `ProcessPlayerBets` return type to return just `bool`, currently returns `(bool bettingRoundEnded, int bet)` but the `bet` value is unused.
   - [x] Improve Data Structures, instead of parallel arrays `_hasBet`, have more cohesive structure by creating `PlayerBettingInfo` class with `HasBet`, `BetAmount` and `HasPassed` properties.
   - [x] Ditch the quadrant display bullshit
   - [x] In BettingActionEventArgs class, should use bool HasBet property to show who placed bets
   - [x] If player lost the bet, display player has placed a bet, betting amount not necessary
   - [x] Fix Unit test
   - [ ] Simplify Complex Methods and break down large methods into smaller, focused ones: 
       - [x] `ProcessBettingRound`: Extract validation logic.
       - [x] `PlayAllTricks`: Add `PlaySingleTrick()` to simplify.
       - [ ] `UpdateTeamPoints`: Extract team logic into helper methods.
   - [x] Eliminate Static Dependencies in `UIConsoleGameView` interface which will cause issues in Unity.
   - [ ] Consider adding GetValidCards Helper Method, could be useful for Unity implementation where you might want to highlight valid cards in the UI.
   - [ ] Add supporting helper classes to separated concerns, such as BettingAction, BetValidationResult and BettingActionType. Have in seperate .cs class file, view helperclassesexample.cs.
   - [ ] Do the same as above point, but in the Game.cs class.
   - [x] Extract Magic Numbers to Constants by creating class GameConstants `TEAM_ONE_PLAYER_1 = 0; TEAM_ONE_PLAYER_2 = 2; TEAM_TWO_PLAYER_1 = 1;TEAM_TWO_PLAYER_2 = 3; PLAYERS_PER_TEAM = 2; TOTAL_PLAYERS = 4;`
   - [ ] Improve Team Logic by creating proper team management system with Team enum with `TeamHelper` class with `GetPlayerTeam`  and `GetTeamPlayerIndices` methods.
   - [ ] Review files organization structure and class names
   
   **(For later)**

   - [ ] Extract Game Data from Game Logic by creating separate `GameData` class to hold state that Unity can serialize: DealerIndex, WinningBidIndex, WinningBid, TrumpSuit, TeamOneTotalPoints, TeamTwoTotalPoints, other state data
   - [ ] Remove Static UI References, replace `UIConsoleGameView.DisplayAllHands(_players, _dealerIndex)` with instance calls through the injected interface.
   - [ ] Simplify Method Signatures by reducing complex parameter lists. Consider creating small data classes called TrickData to handle CardsPlayed, LeadingSuit, TrickNumber.
   - [ ] Simplify Complex Methods, `ProcessPlayerBets` method returns a tuple and has side effects, split it into ProcessPlayerBet method and BetResult class
   - [ ] Remove Nested Loops and Complex Logic, betting round logic is too complex. Break it into smaller, testable methods: ProcessBettingRound(), SetWinningBid(bettingState.GetWinningBid())
   - [ ] Make Game Logic Async-Friendly since unity multiplayer often requires async operations. Prepare for this: `public async Task<Card> GetPlayerCardChoice(Player player, CardSuit? leadingSuit)`
   - [ ] Avoid Console-Specific Code by remove or abstract away `Console.SetCursorPosition()` and similar console-specific methods.

2. **Deck Improvements**
   - [x] Implement Fisher-Yates shuffle algorithm in `Deck.cs`
   - [x] Add a method to cut the deck
   - [ ] Consider adding a `DeckBuilder` pattern for custom decks

3. **Game State Management**
   - [x] Implement a state machine pattern for game flow (e.g., `GameState` enum with transitions)
   - [x] Create separate classes for different game states (`StartState`, `DealState`, `PlayState`, `EndState`, `BettingState`, `PlayingState`, etc.) to control flow
   - [x] Use events for game state changes (e.g., `OnCardPlayed`, `OnGameOver`, `OnRoundStarted`, `OnTrickCompleted`) for a decoupled design
   - [ ] Add events for TrickStartedEventArgs, DealerRotatedEventArgs, GameStateChangedEventArgs, BetRaisedEventArgs, LeadingSuitEstablishedEventArgs, PlayerConnectionEventArgs, HandUpdatedArgs, Game state changes.
   - [ ] Support saving and loading Game State by serializing the current game state to JSON to persist or debug state easily

### Architecture Improvements
4. **Dependency Injection**
   - [ ] Extract interfaces for all major components
   - [ ] Allow injection of components like `UIConsoleGameView` so you can swap it with a Unity UI system later
   - [ ] Set up DI container (Microsoft.Extensions.DependencyInjection)
   - [ ] Make UI and game logic fully decoupled
   - [ ] Ensure logic and view are completely decoupled. Core classes should never call `Console.WriteLine`

5. **Configuration**
   - [x] Move game constants to config files (appsettings.json)
   - [x] Create a `GameSettings` or `GameConfig` class to hold configuration
   - [ ] Implement hot-reload for configuration changes

6. **Abstraction of All I/O and UI Calls**
   - [ ] Create a proper `IGameView` interface for UI to easily swap `Console` view with Unity view
   - [x] Separate game logic from console-specific UI concerns
   - [ ] Prepare for Unity UI by making rendering abstract


### Gameplay Features
7. **Player System**
   - [x] Implement player turn manager system to encapsulate turn rotation logic and current player management
   - [x] Add player types (Human vs AI) and add basic AI strategies now to prepare for eventual CPU players in Unity
   - [ ] Create base AI class for Unity adaptation

8. **Scoring System**
   - [ ] Extract scoring logic to separate class
   - [ ] Implement proper score history tracking
   - [ ] Add validation for score calculations

9. **Error Handling**
   - [ ] Add comprehensive input validation
   - [ ] Implement proper exception handling
   - [ ] Add game state validation checks


### Unity Preparation
10. **Unity-Friendly Patterns**
    - [x] Make all game objects serializable by marking key classes with `[Serializable]` so Unity can handle them in the editor/inspector
    - [ ] Use events for UI updates instead of direct calls
    - [x] Minimize static references to favor instance-based architecture for Unity compatibility and testability
    - [ ] Separate game simulation from rendering, since logic happens in `Update`, rendering in UI
    - [ ] Create MonoBehaviour wrappers for core classes

11. **Performance**
    - [ ] Optimize card comparisons
    - [ ] Consider object pooling for cards
    - [ ] Profile critical paths

12. **Testing**
    - [ ] Add unit tests for core game logic to cover `Deck`, `Player`, and `Game` mechanics
    - [ ] Create test scenarios for edge cases
    - [ ] Implement debug mode with cheat commands


### Additional Features
13. **Game Flow**
    - [ ] Add proper round/game ending transitions
    - [ ] Implement game pausing
    - [ ] Add replay/save system

14. **Localization**
    - [ ] Extract all strings for easy translation
    - [ ] Create resource files for text

15. **Logging**
    - [ ] Add game event logging and integrate a logger (e.g., `ILogger`) to debug without cluttering UI. This logger can adapt to Unity’s `Debug.Log`
    - [ ] Create debug view of game state
    - [ ] Add self documenting commment using XML for documentation