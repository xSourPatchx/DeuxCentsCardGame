# Code Review: Recommended Helper Classes for Unity Multiplayer

## Current Strengths

- Clean separation of concerns with Controllers, Managers, and Services
- Good use of interfaces for testability
- Event-driven architecture suitable for Unity

## Recommended Helper Classes

### 1. *GameStateTransitions* (State Machine Helper)

*Location:* DeuxCentsCardGame/GameStates/GameStateTransitions.cs

*Purpose:* Encapsulate valid state transition logic

csharp
public class GameStateTransitions
{
    private readonly Dictionary<GameState, List<GameState>> _validTransitions;
    
    public bool IsValidTransition(GameState from, GameState to);
    public List<GameState> GetValidNextStates(GameState current);
    public void ValidateTransition(GameState from, GameState to);
}


*Why:*

- Unity multiplayer needs strict state validation
- Prevents desync between clients
- Centralized transition rules

-----

### 2. *TurnOrderCalculator* (Separation from PlayerTurnManager)

*Location:* DeuxCentsCardGame/Gameplay/TurnOrderCalculator.cs

*Purpose:* Pure calculation logic for turn order

csharp
public class TurnOrderCalculator
{
    public int CalculateNextPlayer(int current, int total);
    public int CalculatePlayerLeftOf(int index, int total);
    public int CalculatePlayerRightOf(int index, int total);
    public List<int> CalculateTurnOrder(int starting, int total);
    public int CalculateDistanceFromPlayer(int from, int to, int total);
}


*Why:*

- Stateless, pure functions (easier to test)
- Can be used on both client and server
- PlayerTurnManager becomes state holder only

-----

### 3. *NetworkSyncValidator* (Future-proofing for Unity Netcode)

*Location:* DeuxCentsCardGame/Network/NetworkSyncValidator.cs

*Purpose:* Validate game state consistency

csharp
public class NetworkSyncValidator
{
    public bool ValidateGameState(GameStateSnapshot local, GameStateSnapshot remote);
    public bool ValidatePlayerAction(PlayerAction action, GameState currentState);
    public List<ValidationError> GetSyncErrors();
}


*Why:*

- Essential for multiplayer game integrity
- Detects cheating/desync early
- Separates network concerns from game logic

-----

### 4. *HandEvaluator* (Extract from BaseAIPlayer)

*Location:* DeuxCentsCardGame/Gameplay/HandEvaluator.cs

*Purpose:* Analyze hand strength and composition

csharp
public class HandEvaluator
{
    public int CalculateHandStrength(List<Card> hand);
    public Dictionary<CardSuit, int> GetSuitCounts(List<Card> hand);
    public CardSuit GetStrongestSuit(List<Card> hand);
    public int CountHighCards(List<Card> hand, int threshold);
    public bool HasVoid(List<Card> hand, CardSuit suit);
}


*Why:*

- Both AI and UI can use same logic
- Show hints to human players
- Reusable for analytics/statistics

-----

### 5. *TrickAnalyzer* (Extract from HardAIPlayer)

*Location:* DeuxCentsCardGame/Gameplay/TrickAnalyzer.cs

*Purpose:* Analyze current trick state

csharp
public class TrickAnalyzer
{
    public Card GetCurrentWinningCard(List<(Card, Player)> trick, CardSuit? trump, CardSuit? leading);
    public int CalculateTrickValue(List<(Card, Player)> trick);
    public bool IsPartnerWinning(List<(Card, Player)> trick, int playerIndex, ITeamManager teamManager);
    public List<Card> GetWinningCards(List<Card> hand, Card currentWinner, CardSuit? trump, CardSuit? leading);
}


*Why:*

- Shared between all AI difficulties
- Can power UI hints/animations
- Testable in isolation

-----

### 6. *CardPlayStrategyFactory* (Strategy Pattern for AI)

*Location:* DeuxCentsCardGame/AI/Strategies/CardPlayStrategyFactory.cs

*Purpose:* Create strategy objects instead of inheritance

csharp
public interface ICardPlayStrategy
{
    Card ChooseCard(List<Card> hand, CardSuit? leading, CardSuit? trump, List<(Card, Player)> trick);
}

public class CardPlayStrategyFactory
{
    public ICardPlayStrategy GetLeadingStrategy(AIDifficulty difficulty);
    public ICardPlayStrategy GetFollowingStrategy(AIDifficulty difficulty);
    public ICardPlayStrategy GetLastPlayerStrategy(AIDifficulty difficulty);
}


*Why:*

- More flexible than inheritance
- Can mix strategies (e.g., “aggressive leading, defensive following”)
- Unity ScriptableObjects work well with this pattern

-----

### 7. *BettingRangeCalculator* (Extract from BettingManager)

*Location:* DeuxCentsCardGame/Gameplay/BettingRangeCalculator.cs

*Purpose:* Calculate valid betting ranges

csharp
public class BettingRangeCalculator
{
    public List<int> GetAvailableBets(int min, int max, int increment, List<int> taken);
    public int GetClosestValidBet(int target, List<int> available);
    public bool IsBetInRange(int bet, int min, int max, int increment);
    public int CalculateMinimumViableBet(int currentHighest, int increment);
}


*Why:*

- Pure calculation logic
- Easier to unit test
- Can be shared with UI for bet slider validation

-----

### 8. *CardCollectionHelper* (Utility for Card Lists)

*Location:* DeuxCentsCardGame/Utilities/CardCollectionHelper.cs

*Purpose:* Common card collection operations

csharp
public class CardCollectionHelper
{
    public List<Card> FilterBysuit(List<Card> cards, CardSuit suit);
    public List<Card> FilterByTrump(List<Card> cards, CardSuit? trump);
    public List<Card> SortBySuit(List<Card> cards);
    public List<Card> SortByValue(List<Card> cards);
    public Card GetHighestCard(List<Card> cards);
    public Card GetLowestCard(List<Card> cards);
}


*Why:*

- Reduce code duplication
- Consistent card manipulation across codebase
- Unity UI can use for hand display

-----

### 9. *GameSnapshot* (State Serialization Helper)

*Location:* DeuxCentsCardGame/Models/GameSnapshot.cs

*Purpose:* Capture game state for save/load/network

csharp
[Serializable]
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
}


*Why:*

- Essential for Unity multiplayer state sync
- Save/load functionality
- Replay/spectator mode

-----

### 10. *InputParser* (Separate from GameEventHandler)

*Location:* DeuxCentsCardGame/Input/InputParser.cs

*Purpose:* Parse and validate user input

csharp
public class InputParser
{
    public bool TryParseBet(string input, out int bet);
    public bool TryParseSuit(string input, out CardSuit suit);
    public bool TryParseCardIndex(string input, int handSize, out int index);
    public bool IsPassCommand(string input);
}


*Why:*

- GameEventHandler is too large (300+ lines)
- Input parsing separate from event handling
- In Unity, input comes from UI events, not console

-----

## Unity-Specific Recommendations

### 11. *NetworkMessageFactory* (Prepare for Unity Netcode/Mirror)

csharp
public class NetworkMessageFactory
{
    public NetworkMessage CreateBetMessage(int playerId, int bet);
    public NetworkMessage CreateCardPlayMessage(int playerId, Card card);
    public NetworkMessage CreateStateTransitionMessage(GameState newState);
}


### 12. *AudioEventMapper* (Unity Audio Integration)

csharp
public class AudioEventMapper
{
    public AudioClip GetClipForEvent(GameEventType eventType);
    public void MapGameEventsToAudio(IGameEventManager eventManager);
}


### 13. *AnimationEventMapper* (Unity Animation Integration)

csharp
public class AnimationEventMapper
{
    public void TriggerCardPlayAnimation(Card card, Vector3 position);
    public void TriggerScoreUpdateAnimation(int teamIndex, int points);
    public void TriggerTrickWinAnimation(Player winner);
}


-----

## Refactoring Priority

### High Priority (Do First)

1. *HandEvaluator* - AI code is using this everywhere
1. *TrickAnalyzer* - Duplicated across AI classes
1. *CardCollectionHelper* - Reduce duplication
1. *GameSnapshot* - Critical for multiplayer

### Medium Priority

1. *TurnOrderCalculator* - Clean up PlayerTurnManager
1. *BettingRangeCalculator* - Simplify BettingManager
1. *InputParser* - Reduce GameEventHandler size
1. *GameStateTransitions* - Better state management

### Low Priority (Nice to Have)

1. *CardPlayStrategyFactory* - More flexible AI
1. *NetworkSyncValidator* - Add when implementing multiplayer
1. *AudioEventMapper* - Add with Unity integration
1. *AnimationEventMapper* - Add with Unity integration

-----

## Example Refactor: HandEvaluator

*Before (in BaseAIPlayer):*

csharp
protected int CalculateHandStrength(List<Card> hand)
{
    return hand.Sum(card => card.CardPointValue + card.CardFaceValue);
}


*After (in HandEvaluator):*

csharp
public class HandEvaluator
{
    public int CalculateBasicStrength(List<Card> hand)
    {
        return hand.Sum(card => card.CardPointValue + card.CardFaceValue);
    }
    
    public int CalculateAdvancedStrength(List<Card> hand)
    {
        int strength = CalculateBasicStrength(hand);
        strength += CountHighCards(hand, 8) * 3;
        strength += GetSuitDistributionBonus(hand);
        return strength;
    }
    
    public int CountHighCards(List<Card> hand, int threshold)
    {
        return hand.Count(c => c.CardFaceValue >= threshold);
    }
    
    private int GetSuitDistributionBonus(List<Card> hand)
    {
        var suitCounts = GetSuitCounts(hand);
        return suitCounts.Values.Sum(count => count >= 4 ? 5 : count >= 3 ? 2 : 0);
    }
}


*Usage in BaseAIPlayer:*

csharp
public abstract class BaseAIPlayer : IAIPlayer
{
    protected readonly HandEvaluator _handEvaluator;
    
    protected BaseAIPlayer(IRandomService randomService, ICardUtility cardUtility, 
                          HandEvaluator handEvaluator, AIDifficulty difficulty)
    {
        _handEvaluator = handEvaluator;
        // ...
    }
}


-----

## Benefits for Unity Multiplayer

1. *State Synchronization*: GameSnapshot and NetworkSyncValidator
1. *Client Prediction*: Pure calculation classes (TurnOrderCalculator, BettingRangeCalculator)
1. *Server Authority*: Validators can run on server to prevent cheating
1. *Bandwidth Optimization*: Send minimal state, recalculate derived values
1. *Testability*: All helpers are unit-testable without Unity dependencies
1. *Separation of Concerns*: Game logic separate from Unity-specific code

-----

## Next Steps

1. Create HandEvaluator and refactor AI classes
1. Create TrickAnalyzer and extract from HardAIPlayer
1. Create GameSnapshot for state serialization
1. Plan network architecture with Unity Netcode/Mirror
1. Create interface layer between game logic and Unity

-----

## Code Complexity Issues Found

### Issues with Nested Loops & Complex Logic

#### 1. *DeckManager.PerformFisherYatesShuffle()*

*Current:* Inline loop logic

csharp
private void PerformFisherYatesShuffle()
{
    var cards = _currentDeck.Cards;
    for (int cardIndex = 0; cardIndex < cards.Count; cardIndex++)
    {
        int randomCardIndex = _randomService.Next(cardIndex, cards.Count);
        Card temp = cards[randomCardIndex];
        cards[randomCardIndex] = cards[cardIndex];
        cards[cardIndex] = temp;
    }
}


*Refactor:* Extract swap logic

csharp
private void PerformFisherYatesShuffle()
{
    var cards = _currentDeck.Cards;
    for (int i = 0; i < cards.Count; i++)
    {
        int randomIndex = GetRandomCardIndex(i, cards.Count);
        SwapCards(cards, i, randomIndex);
    }
}

private int GetRandomCardIndex(int startIndex, int count)
{
    return _randomService.Next(startIndex, count);
}

private void SwapCards(List<Card> cards, int index1, int index2)
{
    (cards[index1], cards[index2]) = (cards[index2], cards[index1]);
}


-----

#### 2. *BettingManager.ProcessBettingRound()* ⚠ NESTED LOOP

*Current:* While loop containing for loop (nested)

csharp
private void ProcessBettingRound()
{
    int startingIndex = (_dealerIndex + 1) % _players.Count;
    while (!IsBettingRoundComplete)
    {
        IsBettingRoundComplete = ProcessSingleBettingRound(startingIndex);
    }
}

private bool ProcessSingleBettingRound(int startingIndex)
{
    for (int i = 0; i < _players.Count; i++)
    {
        int currentPlayerIndex = (startingIndex + i) % _players.Count;
        Player currentPlayer = _players[currentPlayerIndex];
        if (_bettingValidator.HasPlayerPassed(currentPlayer))
            continue;
        if (ProcessPlayerBid(currentPlayerIndex))
            return true;
        if (_bettingValidator.HasMinimumPlayersPassed())
            return HandleThreePassesScenario();
    }
    return false;
}


*Refactor:* Extract to BettingRoundExecutor helper

csharp
public class BettingRoundExecutor
{
    private readonly BettingValidator _validator;
    private readonly BettingActionHandler _actionHandler;
    
    public void ExecuteRound(List<Player> players, int startingIndex)
    {
        var roundState = new BettingRoundState(players, startingIndex);
        
        while (!roundState.IsComplete)
        {
            ProcessNextPlayer(roundState);
        }
    }
    
    private void ProcessNextPlayer(BettingRoundState state)
    {
        var player = state.GetNextActivePlayer();
        if (player == null)
        {
            state.Complete();
            return;
        }
        
        var action = _actionHandler.GetPlayerAction(player);
        ApplyBettingAction(state, player, action);
        
        if (ShouldEndRound(state))
        {
            state.Complete();
        }
    }
    
    private bool ShouldEndRound(BettingRoundState state)
    {
        return state.HasMaximumBet() || state.HasThreePasses();
    }
}


-----

#### 3. *TrickController.PlayAllTricks()* ⚠ NESTED LOOP

*Current:* Outer loop for tricks, inner loop for players

csharp
public void PlayAllTricks(int startingPlayerIndex, CardSuit? trumpSuit)
{
    var startingPlayer = _playerManager.GetPlayer(startingPlayerIndex);
    int totalTricks = startingPlayer.Hand.Count;
    _playerTurnManager.InitializeTurnSequence(startingPlayerIndex);

    for (int trickNumber = 0; trickNumber < totalTricks; trickNumber++)
    {
        ExecuteSingleTrick(trickNumber, trumpSuit);
    }
}

private void PlayTrickCards(ref CardSuit? leadingSuit, List<(Card card, Player player)> currentTrick, 
                            int trickNumber, CardSuit? trumpSuit)
{
    var turnOrder = _playerTurnManager.GetTurnOrder();
    foreach (int playerIndex in turnOrder)
    {
        Player currentPlayer = _playerManager.GetPlayer(playerIndex);
        ExecutePlayerTurn(currentPlayer, ref leadingSuit, trickNumber, currentTrick, trumpSuit);
    }
}


*Refactor:* Extract to TrickExecutor

csharp
public class TrickExecutor
{
    private readonly IPlayerManager _playerManager;
    private readonly IPlayerTurnManager _turnManager;
    private readonly TrickStateTracker _stateTracker;
    
    public List<TrickResult> ExecuteAllTricks(int startingPlayerIndex, CardSuit? trumpSuit)
    {
        var results = new List<TrickResult>();
        int totalTricks = GetTrickCount(startingPlayerIndex);
        
        InitializeTricks(startingPlayerIndex);
        
        for (int i = 0; i < totalTricks; i++)
        {
            results.Add(ExecuteSingleTrick(i, trumpSuit));
        }
        
        return results;
    }
    
    private TrickResult ExecuteSingleTrick(int trickNumber, CardSuit? trumpSuit)
    {
        var trick = new Trick(trickNumber);
        
        foreach (var playerIndex in _turnManager.GetTurnOrder())
        {
            PlaySingleCard(playerIndex, trick, trumpSuit);
        }
        
        return ResolveTrick(trick, trumpSuit);
    }
    
    private void PlaySingleCard(int playerIndex, Trick trick, CardSuit? trumpSuit)
    {
        var player = _playerManager.GetPlayer(playerIndex);
        var card = GetPlayerCard(player, trick, trumpSuit);
        trick.AddCard(card, player);
    }
}


-----

#### 4. *HardAIPlayer.DecideBet()* - Complex Nested If/Else

*Current:* Deep nesting with complex thresholds

csharp
public override int DecideBet(List<Card> hand, int minBet, int maxBet, int betIncrement, 
                            int currentHighestBid, List<int> takenBids)
{
    int handStrength = CalculateAdvancedHandStrength(hand);
    
    if (handStrength < 75)
        return -1;

    int targetBet;
    if (handStrength < 85)
        targetBet = minBet;
    else if (handStrength < 95)
        targetBet = minBet + betIncrement;
    else if (handStrength < 105)
        targetBet = minBet + (betIncrement * 3);
    else if (handStrength < 115)
        targetBet = maxBet - (betIncrement * 2);
    else
        targetBet = maxBet;

    if (currentHighestBid > targetBet)
        return -1;

    var availableBets = new List<int>();
    for (int bet = minBet; bet <= maxBet; bet += betIncrement)
    {
        if (!takenBids.Contains(bet) && bet > currentHighestBid)
            availableBets.Add(bet);
    }

    if (availableBets.Count == 0)
        return -1;

    return availableBets.OrderBy(b => Math.Abs(b - targetBet)).First();
}


*Refactor:* Extract to separate methods with strategy pattern

csharp
public class BettingStrategy
{
    private readonly HandStrengthEvaluator _evaluator;
    private readonly BetCalculator _calculator;
    
    public int DecideBet(BettingContext context)
    {
        int strength = _evaluator.Evaluate(context.Hand);
        
        if (!MeetsMinimumStrength(strength))
            return PassBet();
        
        int targetBet = CalculateTargetBet(strength, context);
        
        if (!CanAffordBet(targetBet, context.CurrentHighestBid))
            return PassBet();
        
        return FindClosestAvailableBet(targetBet, context);
    }
    
    private bool MeetsMinimumStrength(int strength)
    {
        return strength >= BettingThresholds.Minimum;
    }
    
    private int CalculateTargetBet(int strength, BettingContext context)
    {
        return _calculator.GetBetForStrength(strength, context.MinBet, 
                                            context.MaxBet, context.Increment);
    }
    
    private bool CanAffordBet(int target, int currentHigh)
    {
        return target >= currentHigh;
    }
    
    private int FindClosestAvailableBet(int target, BettingContext context)
    {
        var available = GetAvailableBets(context);
        return available.Any() 
            ? available.MinBy(b => Math.Abs(b - target)) 
            : PassBet();
    }
}

public class BetCalculator
{
    private static readonly BetThreshold[] Thresholds = 
    {
        new(85, (min, inc) => min),
        new(95, (min, inc) => min + inc),
        new(105, (min, inc) => min + (inc * 3)),
        new(115, (max, inc) => max - (inc * 2)),
        new(int.MaxValue, (max, inc) => max)
    };
    
    public int GetBetForStrength(int strength, int min, int max, int increment)
    {
        var threshold = Thresholds.First(t => strength < t.Strength);
        return threshold.CalculateBet(min, max, increment);
    }
}


-----

#### 5. *CardLogic.WinsAgainst()* - Complex Nested Conditionals

*Current:* Multiple nested if statements

csharp
public bool WinsAgainst(Card thisCard, Card otherCard, CardSuit? trumpSuit, CardSuit? leadingSuit)
{
    bool thisCardIsTrump = thisCard.IsTrump(trumpSuit);
    bool otherCardIsTrump = otherCard.IsTrump(trumpSuit);

    if (thisCardIsTrump || otherCardIsTrump)
        return HandleTrumpComparison(thisCard, otherCard, thisCardIsTrump, otherCardIsTrump);

    if (leadingSuit.HasValue)
        return HandleLeadingSuitComparison(thisCard, otherCard, leadingSuit.Value);

    return HandleSameSuitComparison(thisCard, otherCard);
}


*Refactor:* Chain of Responsibility pattern

csharp
public interface ICardComparisonRule
{
    bool CanHandle(CardComparisonContext context);
    bool Evaluate(CardComparisonContext context);
}

public class CardComparisonChain
{
    private readonly List<ICardComparisonRule> _rules;
    
    public CardComparisonChain()
    {
        _rules = new List<ICardComparisonRule>
        {
            new TrumpComparisonRule(),
            new LeadingSuitComparisonRule(),
            new SameSuitComparisonRule(),
            new DefaultComparisonRule()
        };
    }
    
    public bool Evaluate(Card thisCard, Card otherCard, CardSuit? trump, CardSuit? leading)
    {
        var context = new CardComparisonContext(thisCard, otherCard, trump, leading);
        
        var rule = _rules.First(r => r.CanHandle(context));
        return rule.Evaluate(context);
    }
}

public class TrumpComparisonRule : ICardComparisonRule
{
    public bool CanHandle(CardComparisonContext ctx)
    {
        return ctx.ThisCard.IsTrump(ctx.TrumpSuit) || 
               ctx.OtherCard.IsTrump(ctx.TrumpSuit);
    }
    
    public bool Evaluate(CardComparisonContext ctx)
    {
        bool thisIsTrump = ctx.ThisCard.IsTrump(ctx.TrumpSuit);
        bool otherIsTrump = ctx.OtherCard.IsTrump(ctx.TrumpSuit);
        
        if (thisIsTrump && !otherIsTrump) return true;
        if (!thisIsTrump && otherIsTrump) return false;
        
        return ctx.ThisCard.CardFaceValue > ctx.OtherCard.CardFaceValue;
    }
}


-----

#### 6. *Deck.InitializeCards()* ⚠ NESTED LOOP

*Current:* Nested foreach loops

csharp
private void InitializeCards()
{
    var cardSuits = _cardUtility.GetAllCardSuits();
    var cardFaces = _cardUtility.GetAllCardFaces();
    var cardFaceValues = _cardUtility.GetCardFaceValues();
    var cardPointValues = _cardUtility.GetCardPointValues();

    foreach (CardSuit cardSuit in cardSuits)
    {
        for (int cardIndex = 0; cardIndex < cardFaces.Length; cardIndex++)
        {
            var suit = cardSuit;
            var face = cardFaces[cardIndex];
            var faceValue = cardFaceValues[cardIndex];
            var pointValue = cardPointValues[cardIndex];

            _cardValidator.ValidateCard(suit, face, faceValue, pointValue);
            Cards.Add(new Card(suit, face, faceValue, pointValue));
        }
    }
}


*Refactor:* Extract CardFactory

csharp
public class CardFactory
{
    private readonly ICardUtility _utility;
    private readonly CardValidator _validator;
    
    public List<Card> CreateFullDeck()
    {
        var suits = _utility.GetAllCardSuits();
        return suits.SelectMany(CreateCardsForSuit).ToList();
    }
    
    private IEnumerable<Card> CreateCardsForSuit(CardSuit suit)
    {
        var faces = _utility.GetAllCardFaces();
        
        for (int i = 0; i < faces.Length; i++)
        {
            yield return CreateCard(suit, i);
        }
    }
    
    private Card CreateCard(CardSuit suit, int faceIndex)
    {
        var faces = _utility.GetAllCardFaces();
        var faceValues = _utility.GetCardFaceValues();
        var pointValues = _utility.GetCardPointValues();
        
        var card = new Card(
            suit,
            faces[faceIndex],
            faceValues[faceIndex],
            pointValues[faceIndex]
        );
        
        _validator.ValidateCard(suit, faces[faceIndex], 
                               faceValues[faceIndex], pointValues[faceIndex]);
        
        return card;
    }
}

// Usage in Deck
public class Deck : IDeck
{
    public List<Card> Cards { get; set; }
    
    public Deck(CardFactory cardFactory)
    {
        Cards = cardFactory.CreateFullDeck();
    }
}


-----

#### 7. *GameEventHandler.SubscribeToEvents()* - Too Many Subscriptions

*Current:* 20+ event subscriptions in one method

csharp
private void SubscribeToEvents()
{
    _eventManager.RoundStarted += OnRoundStarted;
    _eventManager.RoundEnded += OnRoundEnded;
    _eventManager.DeckShuffled += OnDeckShuffled;
    // ... 20 more lines
}


*Refactor:* Group by category with EventSubscriber helpers

csharp
public class GameEventSubscriptionManager
{
    private readonly List<IEventSubscriber> _subscribers;
    
    public GameEventSubscriptionManager(IGameEventManager eventManager, IUIGameView ui)
    {
        _subscribers = new List<IEventSubscriber>
        {
            new RoundEventSubscriber(eventManager, ui),
            new BettingEventSubscriber(eventManager, ui),
            new TrickEventSubscriber(eventManager, ui),
            new ScoringEventSubscriber(eventManager, ui)
        };
    }
    
    public void SubscribeAll()
    {
        foreach (var subscriber in _subscribers)
        {
            subscriber.Subscribe();
        }
    }
    
    public void UnsubscribeAll()
    {
        foreach (var subscriber in _subscribers)
        {
            subscriber.Unsubscribe();
        }
    }
}

public class RoundEventSubscriber : IEventSubscriber
{
    private readonly IGameEventManager _events;
    private readonly IUIGameView _ui;
    
    public void Subscribe()
    {
        _events.RoundStarted += OnRoundStarted;
        _events.RoundEnded += OnRoundEnded;
        _events.DeckShuffled += OnDeckShuffled;
        _events.DeckCut += OnDeckCut;
        _events.CardsDealt += OnCardsDealt;
    }
    
    public void Unsubscribe()
    {
        _events.RoundStarted -= OnRoundStarted;
        _events.RoundEnded -= OnRoundEnded;
        // etc.
    }
}


-----

#### 8. *ScoringManager.ScoreRound()* - Complex Method

*Current:* Multiple responsibilities

csharp
public void ScoreRound(int winningBidIndex, int winningBid)
{
    bool bidWinnerIsTeamOne = _teamManager.IsPlayerOnTeamOne(winningBidIndex);

    ScoreTeam(Team.TeamOne, bidWinnerIsTeamOne, winningBid);
    ScoreTeam(Team.TeamTwo, !bidWinnerIsTeamOne, winningBid);

    _eventManager.RaiseScoreUpdated(
        TeamOneRoundPoints, TeamTwoRoundPoints,
        TeamOneTotalPoints, TeamTwoTotalPoints,
        bidWinnerIsTeamOne, winningBid);
}

private void ScoreTeam(Team team, bool isBidWinner, int winningBid)
{
    int teamRoundPoints = GetTeamRoundPoints(team);
    int teamTotalPoints = GetTeamTotalPoints(team);

    var (player1Index, player2Index) = _teamManager.GetTeamPlayerIndices(team);
    bool teamCannotScore = _scoringLogic.DetermineIfTeamCannotScore(
        teamTotalPoints,
        _players[player1Index].HasBet,
        _players[player2Index].HasBet);

    int awardedPoints = _scoringLogic.CalculateAwardedPoints(
        teamRoundPoints,
        teamCannotScore,
        isBidWinner,
        winningBid);

    bool madeBid = _scoringLogic.DetermineBidSuccess(isBidWinner, teamRoundPoints, winningBid);

    RaiseTeamScoringEvent(team, teamRoundPoints, winningBid, madeBid, teamCannotScore, awardedPoints);

    AwardPointsToTeam(team, awardedPoints);
}


*Refactor:* Extract TeamScorer

csharp
public class TeamScorer
{
    private readonly IScoringLogic _logic;
    private readonly ITeamManager _teamManager;
    
    public TeamScoringResult ScoreTeam(TeamScoringContext context)
    {
        var cannotScore = DetermineCannotScore(context);
        var awardedPoints = CalculatePoints(context, cannotScore);
        var madeBid = DetermineBidSuccess(context);
        
        return new TeamScoringResult
        {
            Team = context.Team,
            AwardedPoints = awardedPoints,
            MadeBid = madeBid,
            CannotScore = cannotScore
        };
    }
    
    private bool DetermineCannotScore(TeamScoringContext context)
    {
        var (p1, p2) = _teamManager.GetTeamPlayerIndices(context.Team);
        return _logic.DetermineIfTeamCannotScore(
            context.TeamTotalPoints,
            context.Players[p1].HasBet,
            context.Players[p2].HasBet
        );
    }
    
    private int CalculatePoints(TeamScoringContext context, bool cannotScore)
    {
        return _logic.CalculateAwardedPoints(
            context.TeamRoundPoints,
            cannotScore,
            context.IsBidWinner,
            context.WinningBid
        );
    }
}

// Usage in ScoringManager
public void ScoreRound(int winningBidIndex, int winningBid)
{
    var results = ScoreAllTeams(winningBidIndex, winningBid);
    ApplyResults(results);
    RaiseScoreUpdatedEvent(results, winningBid);
}


-----

## Summary of Issues

### Nested Loops Found:

1. ✅ *BettingManager.ProcessBettingRound()* - while + for
1. ✅ *TrickController.PlayAllTricks()* - for + foreach
1. ✅ *Deck.InitializeCards()* - foreach + for

### Complex Methods:

1. ✅ *HardAIPlayer.DecideBet()* - deep if/else nesting
1. ✅ *CardLogic.WinsAgainst()* - nested conditionals
1. ✅ *GameEventHandler.SubscribeToEvents()* - too many subscriptions
1. ✅ *ScoringManager.ScoreRound()* - multiple responsibilities

### Recommendations:

- Extract all nested loops into separate classes
- Use Chain of Responsibility for complex conditionals
- Create context objects to pass data between methods
- Split large methods (>20 lines) into smaller units
- Use LINQ to reduce explicit loops where possible

Would you like me to provide full implementations for any of these refactorings?