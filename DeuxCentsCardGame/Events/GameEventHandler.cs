using DeuxCentsCardGame.Events.EventArgs;
using DeuxCentsCardGame.Interfaces;

namespace DeuxCentsCardGame.Events
{
    public class GameEventHandlers
    {
        private readonly GameEventManager _eventManager;
        private readonly IUIGameView _ui;

        public GameEventHandlers(GameEventManager eventManager, IUIGameView ui)
        {
            _eventManager = eventManager;
            _ui = ui;

            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            // Round events
            _eventManager.RoundStarted += OnRoundStarted;
            _eventManager.CardsDealt += OnCardsDealt;

            // Betting and trump selection events
            _eventManager.BettingAction += OnBettingAction;
            _eventManager.BettingCompleted += OnBettingCompleted;
            _eventManager.TrumpSelected += OnTrumpSelected;

            // Card playing events
            _eventManager.CardPlayed += OnCardPlayed;
            _eventManager.TrickCompleted += OnTrickCompleted;

            // Scoring events
            _eventManager.ScoreUpdated += OnScoreUpdated;

            // Game end events
            _eventManager.GameOver += OnGameOver;
        }

        // Event handlers
        private void OnRoundStarted(object? sender, RoundEventArgs e)
        {
            // _ui.DisplayMessage($"\n=== Round {e.RoundNumber} Started ===");
            // _ui.DisplayMessage($"Dealer: {e.Dealer.Name}");

            _ui.DisplayFormattedMessage("\nRound {0} Started. Dealer: {1}", e.RoundNumber, e.Dealer.Name);
        }

        private void OnCardsDealt(object? sender, CardsDealtEventArgs e)
        {
            // _ui.DisplayMessage("Cards have been dealt to all players.");

            _ui.DisplayFormattedMessage("\nCards dealt to all {0} players. Dealer index: {1}", e.Players.Count, e.DealerIndex);
            // Update player hands in UI and enable betting/playing controls
        }

        private void OnBettingAction(object? sender, BettingEventArgs e)
        {
            // if (e.HasPassed)
            // {
            //     _ui.DisplayMessage($"{e.Player.Name} passed.");
            // }
            // else
            // {
            //     _ui.DisplayMessage($"{e.Player.Name} bet {e.Bet}.");
            // }

            string action = e.HasPassed ? "passed" : $"bid {e.Bet}";
            _ui.DisplayFormattedMessage("{0} {1}", e.Player.Name, action);
        }

        private void OnBettingCompleted(object? sender, BettingCompletedEventArgs e)
        {
            _ui.DisplayMessage("\nBetting Round Results: ");
            foreach (var bid in e.AllBids)
            {
                string bidText = bid.Value == -1 ? "Passed" : $"Bet {bid.Value}";
                _ui.DisplayFormattedMessage("{0}: {1}", bid.Key.Name, bidText);
            }
            _ui.DisplayFormattedMessage("\nWinning bidder: {0} with {1}",e.WinningBidder.Name, e.WinningBid);
            // _ui.DisplayMessage($"\nWinning bidder: {e.WinningBidder.Name} with {e.WinningBid}");
        }

        private void OnTrumpSelected(object? sender, TrumpSelectedEventArgs e)
        {
            _ui.DisplayFormattedMessage("\nTrump suit is {0} by {1}", e.TrumpSuit, e.SelectedBy.Name);
            // _ui.DisplayMessage($"\n{e.SelectedBy.Name} selected {e.TrumpSuit} as trump suit."); 
        }

        private void OnCardPlayed(object? sender, CardPlayedEventArgs e)
        {
            // string leadingInfo = e.LeadingSuit.HasValue ? $" (Leading: {e.LeadingSuit})" : "";
            // _ui.DisplayMessage($"{e.Player.Name} played {e.Card}{leadingInfo}");

            string leadingInfo = e.LeadingSuit.HasValue ? $" (Leading: {e.LeadingSuit})" : "";
            _ui.DisplayFormattedMessage("{0} played {1} in trick {2}", e.Player.Name, e.Card, e.TrickNumber);

        }

        private void OnTrickCompleted(object? sender, TrickCompletedEventArgs e)
        {
            _ui.DisplayFormattedMessage("\n Trick #{0} complete.", e.TrickNumber);
            _ui.DisplayFormattedMessage("Winner: {0} with {1}", e.WinningPlayer.Name, e.WinningCard);
            _ui.DisplayFormattedMessage("Trick points: {0}", e.TrickPoints);
            
            // Display all cards played in the trick
            _ui.DisplayMessage("Cards played:");
            foreach (var (card, player) in e.PlayedCards)
            {
                _ui.DisplayFormattedMessage("{0}: {1}", player.Name, card);
            }
        }

        private void OnScoreUpdated(object? sender, ScoreEventArgs e)
        {
            _ui.DisplayMessage("\n--- Round Scoring ---");
            _ui.DisplayFormattedMessage("Team One round points: {0}", e.TeamOneRoundPoints);
            _ui.DisplayFormattedMessage("Team Two round points: {0}", e.TeamTwoRoundPoints);
            _ui.DisplayFormattedMessage("Winning bid: {0}", e.WinningBid);
            _ui.DisplayMessage($"Bid winner: {(e.IsBidWinnerTeamOne ? "Team One" : "Team Two")}");
            _ui.DisplayMessage("\nRunning totals:");
            _ui.DisplayFormattedMessage("Team One: {0} points", e.TeamOneTotalPoints);
            _ui.DisplayFormattedMessage("Team Two: {0} points", e.TeamTwoTotalPoints);
        }

        private void OnGameOver(object? sender, GameOverEventArgs e)
        {
            string winner = e.IsTeamOneWinner ? "Team One" : "Team Two";

            _ui.DisplayMessage("\n" + new string('-', 50));

            _ui.DisplayMessage("GAME OVER");
            _ui.DisplayMessage($"Final Scores:");
            _ui.DisplayFormattedMessage("Team One: {0} points", e.TeamOneFinalScore);
            _ui.DisplayFormattedMessage("Team Two: {0} points", e.TeamTwoFinalScore);
            
            _ui.DisplayFormattedMessage("\n {0} WINS!!", winner);
            _ui.DisplayMessage(new string('-', 50));
        }

        // Method to unsubscribe from events for cleanup
        public void UnsubscribeFromEvents()
        {
            _eventManager.RoundStarted -= OnRoundStarted;
            _eventManager.CardsDealt -= OnCardsDealt;
            _eventManager.BettingAction -= OnBettingAction;
            _eventManager.BettingCompleted -= OnBettingCompleted;
            _eventManager.TrumpSelected -= OnTrumpSelected;
            _eventManager.CardPlayed -= OnCardPlayed;
            _eventManager.TrickCompleted -= OnTrickCompleted;
            _eventManager.ScoreUpdated -= OnScoreUpdated;
            _eventManager.GameOver -= OnGameOver;
        }
    }
}