// Failed tests
DeuxCentsCardGame.Tests.Managers.BettingManagerTests.ExecuteBettingRound_WithInvalidBet_ShowsError
DeuxCentsCardGame.Tests.Managers.BettingManagerTests.ExecuteBettingRound_WithMaxBet_EndsImmediately
DeuxCentsCardGame.Tests.Managers.BettingManagerTests.ExecuteBettingRound_WithValidBet_ProcessesCorrectly
DeuxCentsCardGame.Tests.Managers.BettingManagerTests.ExecuteBettingRound_ThreePassesForcesLastPlayerToBet50

DeuxCentsCardGame.Tests.Managers.DealingManagerTests.RaiseCardsDealtEvent_CallsEventManager

DeuxCentsCardGame.Tests.Managers.DeckManagerTests.ShuffleDeck_ChangesCardOrder

DeuxCentsCardGame.Tests.Managers.ScoringManagerTests.IsGameOver_WhenTeamReaches200_ReturnsTrue

DeuxCentsCardGame.Tests.Managers.TrumpSelectionManagerTests.SelectTrumpSuit_VariousInputs_ReturnsCorrectSuit(input: "Spades", expectedSuit: Spades)
DeuxCentsCardGame.Tests.Managers.TrumpSelectionManagerTests.SelectTrumpSuit_VariousInputs_ReturnsCorrectSuit(input: "Hearts", expectedSuit: Hearts)
DeuxCentsCardGame.Tests.Managers.TrumpSelectionManagerTests.SelectTrumpSuit_VariousInputs_ReturnsCorrectSuit(input: "Diamonds", expectedSuit: Diamonds)
DeuxCentsCardGame.Tests.Managers.TrumpSelectionManagerTests.SelectTrumpSuit_VariousInputs_ReturnsCorrectSuit(input: "Clubs", expectedSuit: Clubs)
DeuxCentsCardGame.Tests.Managers.TrumpSelectionManagerTests.SelectTrumpSuit_ReturnsValidSuit


// next
// Currently at 5. Unity Naming Convention Adjustments
//
// In events, Unity prefers Action/UnityEvent patterns
// MonoBehaviour-ready components (for future Unity transition)
// SerializeField for Unity inspector
// Unity lifecycle methods
// ScriptableObject configuration (Unity approach)
// Error Handling: Add more comprehensive error handling
// Immutability: Consider making more objects immutable
//
// Should fix diplay betting results
// Should fix displaying hand before playing