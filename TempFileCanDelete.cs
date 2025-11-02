// ## Can you help me with my BettingManager.cs refactoring
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

// Problem: Complex condition
// if (int.TryParse(betInput, out int bet) && 
//     bet >= _gameConfig.MinimumBet &&
//     bet <= _gameConfig.MaximumBet &&
//     bet % _gameConfig.BetIncrement == 0 &&
//     !_players.Any(player => player.CurrentBid == bet))

// Solution: Clear, testable methods
// private bool IsValidBet(int bet)
// {
//     return IsBetInValidRange(bet) && 
//            IsBetValidIncrement(bet) && 
//            IsBetUnique(bet);
// }


// ## Additional Refactoring Opportunities

// **Card.cs - `WinsAgainst` method**
// Current method has 5 nested conditions. Could be extracted:
// WinsAgainst
// ├── BothAreTrump
// ├── OnlyThisIsTrump
// ├── OnlyOtherIsTrump
// ├── BothMatchLeadingSuit
// └── CompareFaceValues

// **GameEventHandler.cs**
// Some display methods could be simplified:
// OnBettingCompleted
// ├── FormatBidText (extract bid display logic)
// ├── DisplayBiddingResults
// └── ShowWinnerHand

// **DeckManager.cs - `ShuffleDeck`**
// Fisher-Yates shuffle could be extracted:
// ShuffleDeck
// ├── LogShuffleStart
// └── PerformFisherYatesShuffle

// ---