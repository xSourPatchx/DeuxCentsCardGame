### **Master TODO**
1. **Refactoring**
   - [x] Use enums for `CardSuit` and `CardFace` in `Card.cs`
   - [x] Consider making `Card` immutable (remove setters, use `init` properties)
   - [ ] Add validation in `Card` constructor
   - [ ] Consider encapsulate Card Logic by adding methods like `IsHigherThan(Card other)` directly in the `Card` class.
  
2. **Deck Improvements**
   - [ ] Implement Fisher-Yates shuffle algorithm in `Deck.cs`
   - [ ] Add a method to cut the deck
   - [ ] Consider adding a `DeckBuilder` pattern for custom decks

3. **Game State Management**
   - [ ] Implement a state machine pattern for game flow (e.g., `GameState` enum with transitions)
   - [ ] Create separate classes for different game states (`StartState`, `DealState`, `PlayState`, `EndState`, `BettingState`, `PlayingState`, etc.) to control flow
   - [ ] Use events for game state changes (e.g., `OnCardPlayed`, `OnGameOver`, `OnRoundStarted`, `OnTrickCompleted`) for a decoupled design.
   - [ ] Support saving and loading Game State by serializing the current game state to JSON to persist or debug state easily.


### Architecture Improvements
4. **Dependency Injection**
   - [ ] Extract interfaces for all major components
   - [ ] Allow injection of components like `UIConsoleGameView` so you can swap it with a Unity UI system later.
   - [ ] Set up DI container (Microsoft.Extensions.DependencyInjection)
   - [ ] Make UI and game logic fully decoupled
   - [ ] Ensure logic and view are completely decoupled. Core classes should never call `Console.WriteLine`.


5. **Configuration**
   - [ ] Move game constants to config files (appsettings.json)
   - [ ] Create a `GameSettings` class to hold configuration
   - [ ] Implement hot-reload for configuration changes

6. **Abstraction of All I/O and UI Calls**
   - [ ] Create a proper `IGameView` interface for UI to easily swap `Console` view with Unity view.
   - [ ] Separate game logic from console-specific UI concerns
   - [ ] Prepare for Unity UI by making rendering abstract


### Gameplay Features
7. **Player System**
   - [ ] Implement player turn manager system to encapsulate turn rotation logic and current player management.
   - [ ] Add player types (Human vs AI) and add basic AI strategies now to prepare for eventual CPU players in Unity.
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
    - [ ] Make all game objects serializable by marking key classes with `[Serializable]` so Unity can handle them in the editor/inspector.
    - [ ] Use events for UI updates instead of direct calls
    - [ ] Minimize static references to favor instance-based architecture for Unity compatibility and testability.
    - [ ] Separate game simulation from rendering, since logic happens in `Update`, rendering in UI. 
    - [ ] Create MonoBehaviour wrappers for core classes

11. **Performance**
    - [ ] Optimize card comparisons
    - [ ] Consider object pooling for cards
    - [ ] Profile critical paths

12. **Testing**
    - [ ] Add unit tests for core game logic to cover `Deck`, `Player`, and `Game` mechanics.
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
    - [ ] Add game event logging and integrate a logger (e.g., `ILogger`) to debug without cluttering UI. This logger can adapt to Unity’s `Debug.Log`. 
    - [ ] Create debug view of game state
