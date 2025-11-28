### Medium Priority

5. *TurnOrderCalculator* - Clean up PlayerTurnManager
6. *BettingRangeCalculator* - Simplify BettingManager
7. *InputParser* - Reduce GameEventHandler size
8. *GameStateTransitions* - Better state management


-----


### 6. *CardPlayStrategyFactory* (Strategy Pattern for AI)

*Location:* DeuxCentsCardGame/AI/Strategies/CardPlayStrategyFactory.cs

*Purpose:* Create strategy objects instead of inheritance

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

