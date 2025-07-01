// // step 1
// using DeuxCentsCardGame.Models;

// namespace DeuxCentsCardGame.Events.EventArgs
// {
//     public class BetInputEventArgs : System.EventArgs
//     {
//         public Player CurrentPlayer { get; }
//         public int MinimumBet { get; }
//         public int MaximumBet { get; }
//         public int BetIncrement { get; }
//         public string Response { get; set; } = string.Empty;
        
//         public BetInputEventArgs(Player currentPlayer, int minimumBet, int maximumBet, int betIncrement)
//         {
//             CurrentPlayer = currentPlayer;
//             MinimumBet = minimumBet;
//             MaximumBet = maximumBet;
//             BetIncrement = betIncrement;
//         }
//     }
// }


// // step 2: GameEventManager.cs
// public event EventHandler<BetInputEventArgs>? BetInput;

// protected virtual void OnBetInput(BetInputEventArgs e)
// {
//     BetInput?.Invoke(this, e);
// }

// public string RaiseBetInput(Player currentPlayer, int minBet, int maxBet, int betIncrement)
// {
//     var args = new BetInputEventArgs(currentPlayer, minBet, maxBet, betIncrement);
//     OnBetInput(args);
//     return args.Response;
// }

// // step 3: GameEventHandler.cs
// _eventManager.BetInput += OnBetInput;

// private void OnBetInput(object? sender, BetInputEventArgs e)
// {
//     string prompt = $"{e.CurrentPlayer.Name}, enter a bet (between {e.MinimumBet}-{e.MaximumBet}, intervals of {e.BetIncrement}) or 'pass': ";
//     string betInput = _ui.GetUserInput(prompt).ToLower();
    
//     e.Response = betInput;
// }

// _eventManager.BetInput -= OnBetInput;


// // step 4
// string betInput = _eventManager.RaiseBetInput(
//     _players[currentPlayerIndex], 
//     MinimumBet, 
//     MaximumBet, 
//     BetIncrement
// );