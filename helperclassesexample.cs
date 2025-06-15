// Supporting classes for better code organization
public enum BettingActionType
{
    Pass,
    Bet
}

public class BettingAction
{
    public BettingActionType Type { get; set; }
    public int Amount { get; set; }
}

public class BetValidationResult
{
    public bool IsValid { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}