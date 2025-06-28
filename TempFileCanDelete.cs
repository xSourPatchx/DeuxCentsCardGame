// SIMPLE APPROACH: Event System with Response Property

// 1. Enhanced BetInputEventArgs.cs (already provided, but showing for completeness)
using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Events.EventArgs
{
    public class BetInputEventArgs : System.EventArgs
    {
        public Player CurrentPlayer { get; }
        public int MinBet { get; }
        public int MaxBet { get; }
        public int BetIncrement { get; }
        public string Response { get; set; } = string.Empty; // Response property for the event handler to set
        
        public BetInputEventArgs(Player currentPlayer, int minBet, int maxBet, int betIncrement)
        {
            CurrentPlayer = currentPlayer;
            MinBet = minBet;
            MaxBet = maxBet;
            BetIncrement = betIncrement;
        }
    }
}

// 2. Add to GameEventManager.cs

// Add this event declaration:
public event EventHandler<BetInputEventArgs>? BetInput;

// Add this event raising method:
protected virtual void OnBetInput(BetInputEventArgs e)
{
    BetInput?.Invoke(this, e);
}

// Add this public method:
public string RaiseBetInput(Player currentPlayer, int minBet, int maxBet, int betIncrement)
{
    var args = new BetInputEventArgs(currentPlayer, minBet, maxBet, betIncrement);
    OnBetInput(args);
    
    // Return the response that was set by the event handler
    return args.Response;
}

// 3. Add to GameEventHandlers.cs

// Add to SubscribeToEvents() method:
_eventManager.BetInput += OnBetInput;

// Add this new event handler method:
private void OnBetInput(object? sender, BetInputEventArgs e)
{
    string prompt = $"{e.CurrentPlayer.Name}, enter a bet (between {e.MinBet}-{e.MaxBet}, intervals of {e.BetIncrement}) or 'pass': ";
    string betInput = _ui.GetUserInput(prompt).ToLower();
    
    // Set the response in the event args
    e.Response = betInput;
}

// Add to UnsubscribeFromEvents() method:
_eventManager.BetInput -= OnBetInput;

// 4. Modified BettingState.cs - HandlePlayerBids method

private bool HandlePlayerBids(int currentPlayerIndex)
{
    while (true)
    {
        // Replace the two lines with this single call:
        string betInput = _eventManager.RaiseBetInput(
            _players[currentPlayerIndex], 
            MinimumBet, 
            MaximumBet, 
            BetIncrement
        );

        if (betInput == "pass")
        {
            HandlePassInput(currentPlayerIndex);
            return false;
        }

        if (int.TryParse(betInput, out int bet) && IsValidBet(bet))
        {
            return HandleValidBet(currentPlayerIndex, bet);
        }

        _ui.DisplayMessage("Invalid bet, please try again");
    }
}

// NOTE: No other method signatures need to change - everything stays synchronous

/* 
HOW THIS WORKS:

1. BettingState calls _eventManager.RaiseBetInput()
2. GameEventManager fires the BetInput event
3. GameEventHandlers.OnBetInput() handles the event:
   - Shows the prompt
   - Gets user input
   - Sets e.Response = betInput
4. Control returns to BettingState with the input

BENEFITS:
- Simple and straightforward
- No async complexity
- Easy to understand and debug
- Clean separation of concerns
- Ready for future Unity conversion (just change the event handler)

MIGRATION TO UNITY:
When you move to Unity, you'll only need to change the OnBetInput method:
- Instead of console UI, show Unity UI
- Instead of immediate response, you can still use this pattern with Unity's event system
- The game logic in BettingState remains unchanged
*/