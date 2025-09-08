using DeuxCentsCardGame.Core;
using DeuxCentsCardGame.Events.EventArgs;
using DeuxCentsCardGame.Interfaces;
using DeuxCentsCardGame.UI;

namespace DeuxCentsCardGame.Events
{
    public class GameEventHandler
    {
        private readonly GameEventManager _eventManager;
        private readonly IUIGameView _ui;

        public GameEventHandler(GameEventManager eventManager, IUIGameView ui)
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
            _eventManager.BettingRoundStarted += OnBettingRoundStarted;
            _eventManager.BetInput += OnBetInput;
            _eventManager.InvalidBet += OnInvalidBet;
            _eventManager.BettingAction += OnBettingAction;
            _eventManager.BettingCompleted += OnBettingCompleted;
            _eventManager.TrumpSelected += OnTrumpSelected;

            // Card playing events
            _eventManager.PlayerTurn += OnPlayerTurn;
            _eventManager.CardPlayed += OnCardPlayed;
            _eventManager.TrickCompleted += OnTrickCompleted;

            // Scoring events
            _eventManager.ScoreUpdated += OnScoreUpdated;
            _eventManager.TeamScoring += OnTeamScoring;
            _eventManager.TrickPointsAwarded += OnTrickPointsAwarded;

            // Game end events
            _eventManager.GameOver += OnGameOver;
            _eventManager.NextRoundPrompt += OnNextRoundPrompt;
        }

        // Event handlers
        private void OnRoundStarted(object? sender, EventArgs.RoundStartedEventArgs e)
        {
            _ui.ClearScreen();
            _ui.DisplayFormattedMessage("\nRound {0} Started. Dealer: {1}\n", e.RoundNumber, e.Dealer.Name);
        }

        private void OnCardsDealt(object? sender, CardsDealtEventArgs e)
        {
            _ui.DisplayMessage("Dealing cards...\n");
            _ui.DisplayFormattedMessage("\nCards dealt to all {0} players. Dealer index: {1}", e.Players.Count, e.DealerIndex);

            // display all players hands
            UIGameView.DisplayAllHands(e.Players, e.DealerIndex + 1);
        }

        private void OnBettingRoundStarted(object? sender, BettingRoundStartedEventArgs e)
        {
            _ui.DisplayMessage(e.Message);
        }

        private void OnBetInput(object? sender, BetInputEventArgs e)
        {
            string prompt = $"{e.CurrentPlayer.Name}, enter a bet (between {e.MinimumBet}-{e.MaximumBet}, intervals of {e.BetIncrement}) or 'pass': ";
            string betInput = _ui.GetUserInput(prompt).ToLower();
            
            e.Response = betInput;
        }

        private void OnInvalidBet(object? sender, InvalidBetEventArgs e)
        {
            _ui.DisplayMessage(e.Message);
        }

        private void OnBettingAction(object? sender, BettingActionEventArgs e)
        {
            if (e.HasPassed)
            {
                _ui.DisplayFormattedMessage("{0} passed\n", e.Player.Name);
            }
            else
            {
                _ui.DisplayFormattedMessage("{0} bet {1}\n", e.Player.Name, e.Bet);

                if (e.Bet == GameConfig.MaximumBet)
                {
                    _ui.DisplayFormattedMessage("{0} bid the maximum bet, betting round ends.", e.Player.Name);                
                }
            }
        }

        private void OnBettingCompleted(object? sender, BettingCompletedEventArgs e)
        {
            _ui.DisplayMessage("\nBetting round complete.");
            _ui.DisplayMessage("Results:");

            foreach (var bid in e.AllBids)
            {
                string bidText = bid.Value == -1 ? "Passed" : $"Bet {bid.Value}";
                _ui.DisplayFormattedMessage("{0}: {1}", bid.Key.Name, bidText);
            }
            _ui.DisplayFormattedMessage("\nThe winning bidder is {0} with {1}\n",e.WinningBidder.Name, e.WinningBid);

            // Display winner's hand
            UIGameView.DisplayHand(e.WinningBidder);
        }

        private void OnTrumpSelected(object? sender, TrumpSelectedEventArgs e)
        {
            _ui.DisplayFormattedMessage("\nTrump suit is {0} by {1}", e.TrumpSuit, e.SelectedBy.Name);
        }

        private void OnPlayerTurn(object? sender, PlayerTurnEventArgs e)
        {
            UIGameView.DisplayHand(e.Player);
            string leadingSuitInfo = e.LeadingSuit.HasValue ? $" (Leading suit is {e.LeadingSuit})" : "Not yet set.";
            string trumpSuitInfo = e.TrumpSuit.HasValue ? $" (Trump suit is {e.TrumpSuit})" : "Not yet set.";

            _ui.DisplayFormattedMessage("It's {0}'s turn to play in trick {1}", e.Player.Name, e.TrickNumber + 1);
            if (e.LeadingSuit.HasValue)
            {
                _ui.DisplayFormattedMessage("Leading suit: {0}", leadingSuitInfo);
            }
            _ui.DisplayFormattedMessage("Trump suit: {0}", trumpSuitInfo);
        }

        private void OnCardPlayed(object? sender, CardPlayedEventArgs e)
        {
            _ui.DisplayFormattedMessage("{0} played {1} in trick {2}\n", e.Player.Name, e.Card, e.TrickNumber + 1);
        }

        private void OnTrickCompleted(object? sender, TrickCompletedEventArgs e)
        {
            _ui.DisplayFormattedMessage("\nTrick #{0} complete.", e.TrickNumber + 1);

            // Display all cards played in the trick
            _ui.DisplayMessage("Cards played:");
            foreach (var (card, player) in e.PlayedCards)
            {
                _ui.DisplayFormattedMessage("{0}: {1}", player.Name, card);
            }

            _ui.DisplayFormattedMessage("Winner: {0} with {1}", e.WinningPlayer.Name, e.WinningCard);
            _ui.DisplayFormattedMessage("Trick points: {0}", e.TrickPoints);
        }
        
        private void OnTrickPointsAwarded(object? sender, TrickPointsAwardedEventArgs e)
        {
            _ui.DisplayFormattedMessage("{0} collected {1} points for {2}", e.Player.Name, e.TrickPoints, e.TeamName);
        }

        private void OnTeamScoring(object? sender, TeamScoringEventArgs e)
        {
            if (e.CannotScore)
            {
                _ui.DisplayFormattedMessage("{0} did not place any bets and has over 100 points, so they score 0 points this round.", e.TeamName);
            }
            else if (e.MadeBid)
            {
                _ui.DisplayFormattedMessage("{0} made their bet of {1} and wins {2} points.", e.TeamName, e.WinningBid, e.AwardedPoints);
            }
            else if (e.AwardedPoints < 0)
            {
                _ui.DisplayFormattedMessage("{0} did not make their bet of {1} and loses {1} points.", e.TeamName, e.WinningBid);
            }
            else
            {
                _ui.DisplayFormattedMessage("{0} did not win the bid, scores {1} points.", e.TeamName, e.AwardedPoints);
            }
        }

        private void OnScoreUpdated(object? sender, ScoreUpdatedEventArgs e)
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

        private void OnNextRoundPrompt(object? sender, NextRoundEventArgs e)
        {
            _ui.WaitForUser("\nPress any key to start the next round...");
        }

        // Method to unsubscribe from events for cleanup
        public void UnsubscribeFromEvents()
        {
            _eventManager.RoundStarted -= OnRoundStarted;
            _eventManager.CardsDealt -= OnCardsDealt;
            _eventManager.BettingRoundStarted -= OnBettingRoundStarted;
            _eventManager.BetInput -= OnBetInput;
            _eventManager.InvalidBet -= OnInvalidBet;
            _eventManager.BettingAction -= OnBettingAction;
            _eventManager.BettingCompleted -= OnBettingCompleted;
            _eventManager.TrumpSelected -= OnTrumpSelected;
            _eventManager.PlayerTurn -= OnPlayerTurn;
            _eventManager.CardPlayed -= OnCardPlayed;
            _eventManager.TrickCompleted -= OnTrickCompleted;
            _eventManager.ScoreUpdated -= OnScoreUpdated;
            _eventManager.TeamScoring -= OnTeamScoring;
            _eventManager.TrickPointsAwarded -= OnTrickPointsAwarded;
            _eventManager.GameOver -= OnGameOver;
            _eventManager.NextRoundPrompt -= OnNextRoundPrompt;
        }
    }
}