### **Step-by-Step Implementation Guide**
   - [x] Define a custom EventArgs class (if you need to pass data) 
   - [x] Create the event in your publisher class (e.g., GameEngine)
   - [x] Subscribe to events in your subscriber class (e.g., UI or AI)
   - [x] Using Events in Your Card Game

### Notes
Think of the event system like a radio broadcast system. You have the radio station (publisher), the radio waves (events), the message format (EventArgs), and the radio receivers (subscribers with their event handlers).
The Four Core Components of .NET Events:
1. Delegates: These are the foundation - they're like "function pointers" that can hold references to one or more methods. In our case, EventHandler<T> is a predefined delegate type.
2. Events: These are special delegates that provide encapsulation and safety. They're declared in your classes and can only be triggered from within that class.
3. EventArgs: These are the data containers - they carry information about what happened when the event occurred.
4. Event Handlers: These are the methods that respond when events are triggered - they're the subscribers to your event "broadcast."

// step 1
using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Events.EventArgs
{
    public class BetInputEventArgs : System.EventArgs
    {
        public Player CurrentPlayer { get; }
        public int MinimumBet { get; }
        public int MaximumBet { get; }
        public int BetIncrement { get; }
        public string Response { get; set; } = string.Empty;
        
        public BetInputEventArgs(Player currentPlayer, int minimumBet, int maximumBet, int betIncrement)
        {
            CurrentPlayer = currentPlayer;
            MinimumBet = minimumBet;
            MaximumBet = maximumBet;
            BetIncrement = betIncrement;
        }
    }
}


// step 2: GameEventManager.cs
public event EventHandler<BetInputEventArgs>? BetInput;

protected virtual void OnBetInput(BetInputEventArgs e)
{
    BetInput?.Invoke(this, e);
}

public string RaiseBetInput(Player currentPlayer, int minBet, int maxBet, int betIncrement)
{
    var args = new BetInputEventArgs(currentPlayer, minBet, maxBet, betIncrement);
    OnBetInput(args);
    return args.Response;
}

// step 3: GameEventHandler.cs
_eventManager.BetInput += OnBetInput;

private void OnBetInput(object? sender, BetInputEventArgs e)
{
    string prompt = $"{e.CurrentPlayer.Name}, enter a bet (between {e.MinimumBet}-{e.MaximumBet}, intervals of {e.BetIncrement}) or 'pass': ";
    string betInput = _ui.GetUserInput(prompt).ToLower();
    
    e.Response = betInput;
}

_eventManager.BetInput -= OnBetInput;


// step 4
string betInput = _eventManager.RaiseBetInput(
    _players[currentPlayerIndex], 
    MinimumBet, 
    MaximumBet, 
    BetIncrement
);