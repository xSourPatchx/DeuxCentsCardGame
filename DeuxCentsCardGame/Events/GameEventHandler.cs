using DeuxCentsCardGame.AI;
using DeuxCentsCardGame.Constants;
using DeuxCentsCardGame.Events.EventArgs;
using DeuxCentsCardGame.Interfaces.Events;
using DeuxCentsCardGame.Interfaces.GameConfig;
using DeuxCentsCardGame.Interfaces.Models;
using DeuxCentsCardGame.Interfaces.Services;
using DeuxCentsCardGame.Interfaces.UI;
using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Events
{
    public class GameEventHandler : IGameEventHandler
    {
        private readonly ICardUtility _cardUtility;
        private readonly IGameConfig _gameConfig;
        private readonly GameEventManager _eventManager;
        private readonly IUIGameView _ui;
        private readonly IAIService _aiService;
        private readonly AIDifficulty _defaultAIDifficulty = AIDifficulty.Medium;

        // Track current trick cards for AI decision-making
        private List<(Card card, Player player)> _currentTrickCards = [];

        public GameEventHandler(GameEventManager eventManager, IUIGameView ui, IGameConfig gameConfig, ICardUtility cardUtility, IAIService aiService)
        {
            _cardUtility = cardUtility ?? throw new ArgumentNullException(nameof(cardUtility));
            _eventManager = eventManager ?? throw new ArgumentNullException(nameof(eventManager));
            _ui = ui ?? throw new ArgumentNullException(nameof(ui));
            _gameConfig = gameConfig ?? throw new ArgumentNullException(nameof(gameConfig));
            _aiService = aiService ?? throw new ArgumentNullException(nameof(aiService));

            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            // Round events
            _eventManager.RoundStarted += OnRoundStarted;
            _eventManager.RoundEnded += OnRoundEnded;
            _eventManager.DeckShuffled += OnDeckShuffled;
            _eventManager.DeckCutInput += OnDeckCutInput;
            _eventManager.DeckCut += OnDeckCut;
            _eventManager.CardsDealt += OnCardsDealt;
            _eventManager.InvalidMove += OnInvalidMove;

            // Betting events
            _eventManager.BettingRoundStarted += OnBettingRoundStarted;
            _eventManager.BetInput += OnBetInput;
            _eventManager.BettingAction += OnBettingAction;
            _eventManager.BettingCompleted += OnBettingCompleted;

            // Trump selection events
            _eventManager.TrumpSelectionInput += OnTrumpSelectionInput;
            _eventManager.TrumpSelected += OnTrumpSelected;

            // Card playing events
            _eventManager.PlayerTurn += OnPlayerTurn;
            _eventManager.CardSelectionInput += OnCardSelectionInput;
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
        public void OnRoundStarted(object? sender, RoundStartedEventArgs e)
        {
            _ui.ClearScreen();
            _ui.DisplayFormattedMessage("\nRound {0} Started. Dealer: {1}\n", e.RoundNumber, e.Dealer.Name);
        }

        public void OnRoundEnded(object? sender, RoundEndedEventArgs e)
        {
            _ui.DisplayMessage("\n=== Round Complete ===");
            _ui.DisplayFormattedMessage("Round {0} has ended.", e.RoundNumber);
            _ui.DisplayFormattedMessage("Winning Bidder: {0} (Bid: {1})", e.WinningBidder.Name, e.WinningBid);
            _ui.DisplayFormattedMessage("Team One Points: {0}", e.TeamOneRoundPoints);
            _ui.DisplayFormattedMessage("Team Two Points: {0}", e.TeamTwoRoundPoints);
        }

        public void OnDeckShuffled(object? sender, DeckShuffledEventArgs e)
        {
            _ui.DisplayFormattedMessage(e.Message);
        }

        public void OnDeckCutInput(object? sender, DeckCutInputEventArgs e)
        {
            if (e.CuttingPlayer.IsAI())
            {
                // AI decides where to cut
                e.Response = new Random().Next(1, e.DeckSize - 1);
                _ui.DisplayFormattedMessage("{0} (AI) cuts the deck at position {1}", e.CuttingPlayer.Name, e.Response);
            }
            else
            {
                string prompt = $"{e.CuttingPlayer.Name}, where would you like to cut the deck? (enter a number between 1-{e.DeckSize - 1}):";
                e.Response = _ui.GetIntInput(prompt, 1, e.DeckSize - 1);
            }
        }

        public void OnDeckCut(object? sender, DeckCutEventArgs e)
        {
            _ui.DisplayFormattedMessage("{0} cut the deck at position {1}\n", e.CuttingPlayer.Name, e.CutPosition);
        }

        public void OnCardsDealt(object? sender, CardsDealtEventArgs e)
        {
            _ui.DisplayMessage("Dealing cards...\n");
            _ui.DisplayFormattedMessage("\nCards dealt to all {0} players. Dealer index: {1}", e.Players.Count, e.DealerIndex);

            List<IPlayer> playersAsInterface = e.Players.Cast<IPlayer>().ToList();
            _ui.DisplayAllHands(playersAsInterface, e.DealerIndex + 1);
        }

        public void OnInvalidMove(object? sender, InvalidMoveEventArgs e)
        {
            string moveTypeText = FormatMoveTypeText(e.MoveType);
            _ui.DisplayFormattedMessage("[{0}] {1}: {2}", moveTypeText, e.Player.Name, e.Message);
        }

        public void OnBettingRoundStarted(object? sender, BettingRoundStartedEventArgs e)
        {
            _ui.DisplayMessage(e.Message);
        }

        public void OnBetInput(object? sender, BetInputEventArgs e)
        {
            if (e.CurrentPlayer.IsAI())
            {
                // Let AI make decision
                int aiBet = _aiService.MakeAIBettingDecision(
                    e.CurrentPlayer, 
                    _defaultAIDifficulty,
                    e.MinimumBet, 
                    e.MaximumBet, 
                    e.BetIncrement,
                    e.CurrentHighestBid,
                    e.TakenBids
                );

                e.Response = aiBet == -1 ? "pass" : aiBet.ToString();

                // Display AI decision
                _ui.DisplayFormattedMessage("[AI] {0} thinking...", e.CurrentPlayer.Name);
                _ui.DisplayFormattedMessage("[AI] {0} decision: {1}", 
                    e.CurrentPlayer.Name, 
                    aiBet == -1 ? "pass" : $"bet {aiBet}");
            }
            else
            {
                // Human player input
                string prompt = $"{e.CurrentPlayer.Name}, enter a bet (between {e.MinimumBet}-{e.MaximumBet}, intervals of {e.BetIncrement}) or 'pass': ";
                string betInput = _ui.GetUserInput(prompt).ToLower();
                e.Response = betInput;
            }
        }

        public void OnBettingAction(object? sender, BettingActionEventArgs e)
        {
            if (e.HasPassed)
            {
                _ui.DisplayFormattedMessage("{0} passed\n", e.Player.Name);
            }
            else
            {
                _ui.DisplayFormattedMessage("{0} bet {1}\n", e.Player.Name, e.Bet);

                if (e.Bet == _gameConfig.MaximumBet)
                {
                    _ui.DisplayFormattedMessage("{0} bid the maximum bet, betting round ends.", e.Player.Name);
                }
            }
        }

        public void OnBettingCompleted(object? sender, BettingCompletedEventArgs e)
        {
            _ui.DisplayMessage("\nBetting round complete.");
            DisplayBiddingResults(e.AllBids, e.WinningBidder);
            DisplayWinningBidder(e.WinningBidder, e.WinningBid);
            
            ShowWinnerHand(e.WinningBidder);
        }

        public void OnTrumpSelectionInput(object? sender, TrumpSelectionInputEventArgs e)
        {
            if (e.SelectingPlayer.IsAI())
            {
                // Let AI select trump suit
                CardSuit aiTrumpSuitChoice = _aiService.MakeAITrumpSelection(e.SelectingPlayer, _defaultAIDifficulty);

                e.Response = _cardUtility.CardSuitToString(aiTrumpSuitChoice);

                _ui.DisplayFormattedMessage("[AI] {0} selecting trump...", e.SelectingPlayer.Name);
                _ui.DisplayFormattedMessage("[AI] {0} selected {1} as trump", e.SelectingPlayer.Name, aiTrumpSuitChoice);           
            }
            else
            {
                // Human player input
                string[] validSuits = Enum.GetNames<CardSuit>().Select(suit => suit.ToLower()).ToArray();
                string prompt = $"{e.SelectingPlayer.Name}, please choose a trump suit. (enter \"clubs\", \"diamonds\", \"hearts\", \"spades\")";
                e.Response = _ui.GetOptionInput(prompt, validSuits);            
            }
        }

        public void OnTrumpSelected(object? sender, TrumpSelectedEventArgs e)
        {
            _ui.DisplayFormattedMessage("\nTrump suit is {0} by {1}\n", e.TrumpSuit, e.SelectedBy.Name);
        }

        public void OnPlayerTurn(object? sender, PlayerTurnEventArgs e)
        {
            // Reset trick cards if this is the first player
            if (_currentTrickCards.Count == 0 || _currentTrickCards.Count == 4)
            {
                _currentTrickCards.Clear();
            }

            if (e.Player.IsHuman())
            {
                _ui.DisplayHand(e.Player);
            }
            else
            {
                _ui.DisplayMessage($"\n[AI] {e.Player.Name}'s turn...");
            }

            DisplayTurnInformation(e.Player, e.TrickNumber, e.LeadingSuit, e.TrumpSuit);
        }

        public void OnCardSelectionInput(object? sender, CardSelectionInputEventArgs e)
        {
            if (e.CurrentPlayer.IsAI())
            {
                // Let AI choose card
                int aiCardIndex = _aiService.MakeAICardSelection(
                    e.CurrentPlayer, 
                    _defaultAIDifficulty,
                    e.LeadingSuit,
                    e.TrumpSuit,
                    _currentTrickCards
                );

                e.Response = aiCardIndex;

                _ui.DisplayFormattedMessage("[AI] {0} is selecting card...", e.CurrentPlayer.Name);
            }
            else
            {
                // Human player input
                string leadingSuitString = e.LeadingSuit.HasValue ? _cardUtility.CardSuitToString(e.LeadingSuit.Value) : "none";
                string trumpSuitString = e.TrumpSuit.HasValue ? _cardUtility.CardSuitToString(e.TrumpSuit.Value) : "none";

                string prompt = $"{e.CurrentPlayer.Name}, choose a card to play (enter index 0-{e.Hand.Count - 1}" +
                    (e.LeadingSuit.HasValue ? $", leading suit is {leadingSuitString}" : "") +
                    $" and trump suit is {trumpSuitString}):";

                e.Response = _ui.GetIntInput(prompt, 0, e.Hand.Count - 1);
            }
        }

        public void OnCardPlayed(object? sender, CardPlayedEventArgs e)
        {
            // Track current trick cards for AI decision-making
            _currentTrickCards.Add((e.Card, e.Player));
            _ui.DisplayFormattedMessage("{0} played {1} in trick {2}\n", e.Player.Name, e.Card, e.TrickNumber + 1);
        }

        public void OnTrickCompleted(object? sender, TrickCompletedEventArgs e)
        {
            _ui.DisplayFormattedMessage("\nTrick #{0} complete.", e.TrickNumber + 1);
            DisplayPlayedCards(e.PlayedCards);
            DisplayTrickWinner(e.WinningPlayer, e.WinningCard, e.TrickPoints);

            // Clear current trick cards for next trick
            _currentTrickCards.Clear();
        }

        public void OnTrickPointsAwarded(object? sender, TrickPointsAwardedEventArgs e)
        {
            _ui.DisplayFormattedMessage("{0} collected {1} points for {2}\n", e.Player.Name, e.TrickPoints, e.TeamName);
        }

        public void OnTeamScoring(object? sender, TeamScoringEventArgs e)
        {
            string message = FormatTeamScoringMessage(e);
            _ui.DisplayFormattedMessage(message);
        }

        public void OnScoreUpdated(object? sender, ScoreUpdatedEventArgs e)
        {
            DisplayRoundScoring(e);
            DisplayRunningTotals(e.TeamOneTotalPoints, e.TeamTwoTotalPoints);
        }

        public void OnGameOver(object? sender, GameOverEventArgs e)
        {
            string winner = e.IsTeamOneWinner ? "Team One" : "Team Two";

            _ui.DisplayMessage("\n" + new string('-', GameConstants.GAME_OVER_SEPARATOR_LENGTH));
            _ui.DisplayMessage("GAME OVER");
            _ui.DisplayMessage($"Final Scores:");
            _ui.DisplayFormattedMessage("Team One: {0} points", e.TeamOneFinalScore);
            _ui.DisplayFormattedMessage("Team Two: {0} points", e.TeamTwoFinalScore);
            _ui.DisplayFormattedMessage("\n {0} WINS!!", winner);
            _ui.DisplayMessage(new string('-', GameConstants.GAME_OVER_SEPARATOR_LENGTH));
        }

        public void OnNextRoundPrompt(object? sender, NextRoundEventArgs e)
        {
            _ui.WaitForUser("\nPress any key to start the next round...");
        }

        // Helper methods for formatting and displaying information
        private string FormatMoveTypeText(InvalidMoveType moveType)
        {
            return moveType switch
            {
                InvalidMoveType.InvalidCard => "Invalid Card",
                InvalidMoveType.InvalidBet => "Invalid Bet",
                InvalidMoveType.OutOfTurn => "Out of Turn",
                InvalidMoveType.InvalidTrumpSelection => "Invalid Trump Selection",
                _ => "Invalid Move"
            };
        }

        private string FormatBidText(Player player, Player winningBidder, int bidAmount)
        {
            if (player == winningBidder)
            {
                return $"Bet {bidAmount}";
            }

            return player.HasBet ? "Bet placed" : "Passed";
        }

        private void DisplayBiddingResults(Dictionary<Player, int> allBids, Player winningBidder)
        {
            _ui.DisplayMessage("Results:");

            foreach (var bid in allBids)
            {
                string bidText = FormatBidText(bid.Key, winningBidder, bid.Value);
                _ui.DisplayFormattedMessage("{0}: {1}", bid.Key.Name, bidText);
            }
        }

        private void DisplayWinningBidder(Player winningBidder, int winningBid)
        {
            _ui.DisplayFormattedMessage("\nWinning bidder is {0} with {1}\n", winningBidder.Name, winningBid);
        }

        private void ShowWinnerHand(Player winningBidder)
        {
            _ui.DisplayHand(winningBidder);
        }

        private void DisplayTurnInformation(Player player, int trickNumber, CardSuit? leadingSuit, CardSuit? trumpSuit)
        {
            _ui.DisplayFormattedMessage("It's {0}'s turn to play in trick {1}", player.Name, trickNumber + 1);

            if (leadingSuit.HasValue)
            {
                _ui.DisplayFormattedMessage("Leading suit: {0}", leadingSuit);
            }

            string trumpSuitInfo = trumpSuit.HasValue ? trumpSuit.ToString() : "Not yet set.";
            _ui.DisplayFormattedMessage("Trump suit: {0}", trumpSuitInfo);
        }

        private void DisplayPlayedCards(List<(Card card, Player player)> playedCards)
        {
            _ui.DisplayMessage("Cards played:");
            foreach (var (card, player) in playedCards)
            {
                _ui.DisplayFormattedMessage("{0}: {1}", player.Name, card);
            }
        }

        private void DisplayTrickWinner(Player winningPlayer, Card winningCard, int trickPoints)
        {
            _ui.DisplayFormattedMessage("\nTrick winner: {0} with {1}", winningPlayer.Name, winningCard);
            _ui.DisplayFormattedMessage("Trick points: {0}", trickPoints);
        }

        private string FormatTeamScoringMessage(TeamScoringEventArgs e)
        {
            if (e.CannotScore)
            {
                return $"{e.TeamName} did not place any bets and has over 100 points, so they score 0 points this round.";
            }

            if (e.MadeBid)
            {
                return $"{e.TeamName} made their bet of {e.WinningBid} and wins {e.AwardedPoints} points.";
            }
        
            if (e.AwardedPoints < 0)
            {
                return $"{e.TeamName} did not make their bet of {e.WinningBid} and loses {e.WinningBid} points.";
            }

            return $"{e.TeamName} did not make their bet of {e.WinningBid} and scores {e.AwardedPoints} points.";
        }

        private void DisplayRoundScoring(ScoreUpdatedEventArgs e)
        {
            _ui.DisplayMessage("\n--- Round Scoring ---");
            _ui.DisplayFormattedMessage("Team One round points: {0}", e.TeamOneRoundPoints);
            _ui.DisplayFormattedMessage("Team Two round points: {0}", e.TeamTwoRoundPoints);
            _ui.DisplayFormattedMessage("Winning bid: {0}", e.WinningBid);
            _ui.DisplayMessage($"Bid winner: {(e.IsBidWinnerTeamOne ? "Team One" : "Team Two")}");
        }

        private void DisplayRunningTotals(int teamOneTotalPoints, int teamTwoTotalPoints)
        {
            _ui.DisplayMessage("\nRunning totals:");
            _ui.DisplayFormattedMessage("Team One: {0} points", teamOneTotalPoints);
            _ui.DisplayFormattedMessage("Team Two: {0} points", teamTwoTotalPoints);
        }

        // Method to unsubscribe from events for cleanup
        public void UnsubscribeFromEvents()
        {
            _eventManager.RoundStarted -= OnRoundStarted;
            _eventManager.RoundEnded -= OnRoundEnded;
            _eventManager.DeckShuffled -= OnDeckShuffled;
            _eventManager.DeckCutInput -= OnDeckCutInput;
            _eventManager.DeckCut -= OnDeckCut;
            _eventManager.CardsDealt -= OnCardsDealt;
            _eventManager.InvalidMove -= OnInvalidMove;
            _eventManager.BettingRoundStarted -= OnBettingRoundStarted;
            _eventManager.BetInput -= OnBetInput;
            _eventManager.BettingAction -= OnBettingAction;
            _eventManager.BettingCompleted -= OnBettingCompleted;
            _eventManager.TrumpSelectionInput -= OnTrumpSelectionInput;
            _eventManager.TrumpSelected -= OnTrumpSelected;
            _eventManager.PlayerTurn -= OnPlayerTurn;
            _eventManager.CardSelectionInput -= OnCardSelectionInput;
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