// ## 1. Can you help me with myScoringManager.cs Refactoring
// **Problem:**
// - Too many responsibilities
// - Hard to test individual logic
// - Difficult to understand flow
// **Desired Solution:**
// ScoreTeam
// ├── GetTeamRoundPoints (1 line)
// ├── GetTeamTotalPoints (1 line)
// ├── DetermineIfTeamCannotScore (5 lines)
// ├── RaiseTeamScoringEvent (7 lines)
// ├── ApplyPointsToTeam (7 lines)
// └── CalculateAwardedPoints
//     └── CalculateBidWinnerPoints (1 line)

// ---

// ## 2. Can you help me with my GameController class Refactoring
// Problem: Multiple complex methods
// - `NewRound` (11 lines of mixed concerns)
// - `PlaySingleTrick` (10 lines, complex flow)
// - `PlayPlayerTurn` (15 lines, multiple responsibilities)
// - `GetValidCardFromPlayer` (15 lines, validation + UI)

// Desired Solution: Hierarchical decomposition
// NewRound (orchestrator)
// ├── InitializeRound
// │   └── ResetRound
// ├── PrepareGameState
// │   ├── ShuffleDeck
// │   └── DealCards
// ├── ExecuteRoundPhases
// │   ├── ExecuteBettingRound
// │   ├── SelectTrumpSuit
// │   └── PlayRound
// │       └── PlayAllTricks
// │           └── ExecuteSingleTrick
// │               ├── PlayTrickCards
// │               │   └── ExecutePlayerTurn
// │               │       ├── RaisePlayerTurnEvent
// │               │       ├── GetValidCardFromPlayer
// │               │       ├── UpdateLeadingSuit
// │               │       └── RaiseCardPlayedEvent
// │               ├── DetermineTrickWinner
// │               ├── CalculateTrickPoints
// │               └── AwardTrickPointsAndNotify
// └── FinalizeRound
//     ├── ScoreRound
//     ├── EndGameCheck
//     └── RotateDealer

// #### `GetValidCardFromPlayer` → Break into:
// - `RequestCardSelection()` - Gets user input
// - `IsCardValid()` - Validates card
// - `DisplayInvalidCardMessage()` - Shows error

// #### `ExecutePlayerTurn` → Break into:
// - `RaisePlayerTurnEvent()` - Event notification
// - `GetValidCardFromPlayer()` - Card selection
// - `UpdateLeadingSuit()` - Game state update
// - `RaiseCardPlayedEvent()` - Event notification

// ---

// ## 3. Can you help me with my BettingManager.cs Refactoring
// **Problems: `HandleBettingRound`**
// - Complex nested logic
// - Multiple exit points
// - Hard to follow betting rules

// ### Desired Solution: Hierarchical structure
// ExecuteBettingRound
// ├── StartBettingRound
// ├── ProcessBettingRound
// │   └── ProcessSingleBettingRound
// │       ├── ShouldSkipPlayer
// │       ├── ProcessPlayerBid
// │       │   ├── RequestBetInput
// │       │   ├── IsPassInput
// │       │   ├── TryParseAndValidateBet
// │       │   │   ├── IsBetInValidRange
// │       │   │   ├── IsBetValidIncrement
// │       │   │   └── IsBetUnique
// │       │   ├── HandlePassInput
// │       │   │   ├── MarkPlayerAsPassed
// │       │   │   └── RaiseBettingActionEvent
// │       │   └── HandleValidBet
// │       │       ├── RecordPlayerBet
// │       │       ├── IsMaximumBet
// │       │       └── HandleMaximumBetScenario
// │       │           └── ForceOtherPlayersToPass
// │       ├── ShouldEndBettingRound
// │       └── HandleThreePassesScenario
// │           ├── GetActivePlayers
// │           ├── HasSingleActivePlayer
// │           └── HandleSingleActivePlayer
// │               ├── NoBetsPlaced
// │               ├── ForceMinimumBet
// │               └── ForcePlayerToPass
// └── CompleteBettingRound
//     ├── DetermineWinningBid
//     │   ├── GetValidBids
//     │   ├── SetWinningBid
//     │   └── ClearWinningBid
//     └── RaiseBettingCompletedEvent

// ### Key Improvements:
// #### Validation Split:
// // Problem: Complex condition
// if (int.TryParse(betInput, out int bet) && 
//     bet >= _gameConfig.MinimumBet &&
//     bet <= _gameConfig.MaximumBet &&
//     bet % _gameConfig.BetIncrement == 0 &&
//     !_players.Any(player => player.CurrentBid == bet))

// // Solution: Clear, testable methods
// private bool IsValidBet(int bet)
// {
//     return IsBetInValidRange(bet) && 
//            IsBetValidIncrement(bet) && 
//            IsBetUnique(bet);
// }


// ## Additional Refactoring Opportunities

// ### 1. **Card.cs - `WinsAgainst` method**
// Current method has 5 nested conditions. Could be extracted:
// WinsAgainst
// ├── BothAreTrump
// ├── OnlyThisIsTrump
// ├── OnlyOtherIsTrump
// ├── BothMatchLeadingSuit
// └── CompareFaceValues

// ### 2. **GameEventHandler.cs**
// Some display methods could be simplified:
// OnBettingCompleted
// ├── FormatBidText (extract bid display logic)
// ├── DisplayBiddingResults
// └── ShowWinnerHand

// ### 3. **DeckManager.cs - `ShuffleDeck`**
// Fisher-Yates shuffle could be extracted:
// ShuffleDeck
// ├── LogShuffleStart
// └── PerformFisherYatesShuffle

// ---

// ## Refactoring Principles Applied

// ### 1. **Single Responsibility Principle (SRP)**
// Each method does one thing well

// ### 2. **Extract Method Pattern**
// - Pull complex logic into named methods
// - Method names document intent
// - Parameters make dependencies explicit

// ### 3. **Composition over Complexity**
// - Small methods compose to create complex behavior
// - Easy to rearrange and modify

// ### 4. **Self-Documenting Code**
// - Method names replace comments
// - Clear what each piece does
// - Reduces cognitive load

// ### 5. **Testability**
// - Small methods are easy to unit test
// - Mock/stub individual components
// - Test edge cases in isolation