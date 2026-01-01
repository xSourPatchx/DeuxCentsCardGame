using DeuxCentsCardGame.AI;
using DeuxCentsCardGame.Constants;
using DeuxCentsCardGame.Events.EventArgs;
using DeuxCentsCardGame.Interfaces.Gameplay;
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
        private readonly IGameEventManager _eventManager;
        private readonly IUIGameView _ui;
        private readonly IAIService _aiService;
        private readonly AIDifficulty _defaultAIDifficulty = AIDifficulty.Medium;

        // Track current trick cards for AI decision-making
        private List<(Card card, Player player)> _currentTrickCards = [];

        public GameEventHandler(
            IGameEventManager eventManager, 
            IUIGameView ui, 
            IGameConfig gameConfig, 
            ICardUtility cardUtility, 
            IAIService aiService)
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
        public async void OnRoundStarted(RoundStartedEventArgs e)
        {
            await _ui.ClearScreen();
            await _ui.DisplayFormattedMessage("\nRound {0} Started. Dealer: {1}\n", e.RoundNumber, e.Dealer.Name);
        }

        public async void OnRoundEnded(RoundEndedEventArgs e)
        {
            await _ui.DisplayMessage("\n=== Round Complete ===");
            await _ui.DisplayFormattedMessage("Round {0} has ended.", e.RoundNumber);
            await _ui.DisplayFormattedMessage("Winning Bidder: {0} (Bid: {1})", e.WinningBidder.Name, e.WinningBid);
            await _ui.DisplayFormattedMessage("Team One Points: {0}", e.TeamOneRoundPoints);
            await _ui.DisplayFormattedMessage("Team Two Points: {0}", e.TeamTwoRoundPoints);
        }

        public async void OnDeckShuffled(DeckShuffledEventArgs e)
        {
            await _ui.DisplayFormattedMessage(e.Message);
        }

        public async void OnDeckCutInput(DeckCutInputEventArgs e)
        {
            if (e.CuttingPlayer.IsAI())
            {
                // AI decides where to cut
                e.Response = new Random().Next(1, e.DeckSize - 1);
                await _ui.DisplayFormattedMessage("{0} (AI) cuts the deck at position {1}", e.CuttingPlayer.Name, e.Response);
            }
            else
            {
                string prompt = $"{e.CuttingPlayer.Name}, where would you like to cut the deck? (enter a number between 1-{e.DeckSize - 1}):";
                e.Response = await _ui.GetIntInput(prompt, 1, e.DeckSize - 1);
            }
        }

        public async void OnDeckCut(DeckCutEventArgs e)
        {
            await _ui.DisplayFormattedMessage("{0} cut the deck at position {1}\n", e.CuttingPlayer.Name, e.CutPosition);
        }

        public async void OnCardsDealt(CardsDealtEventArgs e)
        {
            await _ui.DisplayMessage("Dealing cards...\n");
            await _ui.DisplayFormattedMessage("\nCards dealt to all {0} players. Dealer index: {1}", e.Players.Count, e.DealerIndex);

            List<IPlayer> playersAsInterface = e.Players.Cast<IPlayer>().ToList();
            await _ui.DisplayAllHands(playersAsInterface, e.DealerIndex + 1);
        }

        public async void OnInvalidMove(InvalidMoveEventArgs e)
        {
            string moveTypeText = FormatMoveTypeText(e.MoveType);
            await _ui.DisplayFormattedMessage("[{0}] {1}: {2}", moveTypeText, e.Player.Name, e.Message);
        }

        public async void OnBettingRoundStarted(BettingRoundStartedEventArgs e)
        {
            await _ui.DisplayMessage(e.Message);
        }

        public async void OnBetInput(BetInputEventArgs e)
        {
            if (e.CurrentPlayer.IsAI())
            {
                // Debug output - remove after testing
                await _ui.DisplayFormattedMessage("[DEBUG] Taken bids: {0}",
                    string.Join(", ", e.TakenBids));
                await _ui.DisplayFormattedMessage("[DEBUG] Current highest bid: {0}",
                    e.CurrentHighestBid);

                // Let AI make decision
                int aiBet = await _aiService.MakeAIBettingDecision(
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
                await _ui.DisplayFormattedMessage("[AI] {0} thinking...", e.CurrentPlayer.Name);
                await _ui.DisplayFormattedMessage("[AI] {0} decision: {1}", 
                    e.CurrentPlayer.Name, 
                    aiBet == -1 ? "pass" : $"bet {aiBet}");
            }
            else
            {
                // Human player input
                string prompt = $"{e.CurrentPlayer.Name}, enter a bet (between {e.MinimumBet}-{e.MaximumBet}, intervals of {e.BetIncrement}) or 'pass': ";
                string betInput = await _ui.GetUserInput(prompt);
                e.Response = betInput.ToLower();
            }
        }

        public async void OnBettingAction(BettingActionEventArgs e)
        {
            if (e.HasPassed)
            {
                await _ui.DisplayFormattedMessage("{0} passed\n", e.Player.Name);
            }
            else
            {
                await _ui.DisplayFormattedMessage("{0} bet {1}\n", e.Player.Name, e.Bet);

                if (e.Bet == _gameConfig.MaximumBet)
                {
                    await _ui.DisplayFormattedMessage("{0} bid the maximum bet, betting round ends.", e.Player.Name);
                }
            }
        }

        public async void OnBettingCompleted(BettingCompletedEventArgs e)
        {
            await _ui.DisplayMessage("\nBetting round complete.");
            await DisplayBiddingResults(e.AllBids, e.WinningBidder);
            await DisplayWinningBidder(e.WinningBidder, e.WinningBid);
            
            await ShowWinnerHand(e.WinningBidder);
        }

        public async void OnTrumpSelectionInput(TrumpSelectionInputEventArgs e)
        {
            if (e.SelectingPlayer.IsAI())
            {
                // Let AI select trump suit
                CardSuit aiTrumpSuitChoice = await _aiService.MakeAITrumpSelection(e.SelectingPlayer, _defaultAIDifficulty);

                e.Response = _cardUtility.CardSuitToString(aiTrumpSuitChoice);

                await _ui.DisplayFormattedMessage("[AI] {0} selecting trump...", e.SelectingPlayer.Name);
                await _ui.DisplayFormattedMessage("[AI] {0} selected {1} as trump", e.SelectingPlayer.Name, aiTrumpSuitChoice);           
            }
            else
            {
                // Human player input
                string[] validSuits = Enum.GetNames<CardSuit>().Select(suit => suit.ToLower()).ToArray();
                string prompt = $"{e.SelectingPlayer.Name}, please choose a trump suit. (enter \"clubs\", \"diamonds\", \"hearts\", \"spades\")";
                e.Response = await _ui.GetOptionInput(prompt, validSuits);            
            }
        }

        public async void OnTrumpSelected(TrumpSelectedEventArgs e)
        {
            await _ui.DisplayFormattedMessage("\nTrump suit is {0} by {1}\n", e.TrumpSuit, e.SelectedBy.Name);
        }

        public async void OnPlayerTurn(PlayerTurnEventArgs e)
        {
            // Reset trick cards if this is the first player
            if (_currentTrickCards.Count == 0 || _currentTrickCards.Count == 4)
            {
                _currentTrickCards.Clear();
            }

            if (e.Player.IsHuman())
            {
                await _ui.DisplayHand(e.Player);
            }
            else
            {
                await _ui.DisplayMessage($"\n[AI] {e.Player.Name}'s turn...");
            }

            await DisplayTurnInformation(e.Player, e.TrickNumber, e.LeadingSuit, e.TrumpSuit);
        }

        public async void OnCardSelectionInput(CardSelectionInputEventArgs e)
        {
            if (e.CurrentPlayer.IsAI())
            {
                // Let AI choose card
                int aiCardIndex = await _aiService.MakeAICardSelection(
                    e.CurrentPlayer, 
                    _defaultAIDifficulty,
                    e.LeadingSuit,
                    e.TrumpSuit,
                    _currentTrickCards
                );

                e.Response = aiCardIndex;

                await _ui.DisplayFormattedMessage("[AI] {0} is selecting card...", e.CurrentPlayer.Name);
            }
            else
            {
                // Human player input
                string leadingSuitString = e.LeadingSuit.HasValue ? _cardUtility.CardSuitToString(e.LeadingSuit.Value) : "none";
                string trumpSuitString = e.TrumpSuit.HasValue ? _cardUtility.CardSuitToString(e.TrumpSuit.Value) : "none";

                string prompt = $"{e.CurrentPlayer.Name}, choose a card to play (enter index 0-{e.Hand.Count - 1}" +
                    (e.LeadingSuit.HasValue ? $", leading suit is {leadingSuitString}" : "") +
                    $" and trump suit is {trumpSuitString}):";

                e.Response = await _ui.GetIntInput(prompt, 0, e.Hand.Count - 1);
            }
        }

        public async void OnCardPlayed(CardPlayedEventArgs e)
        {
            // Track current trick cards for AI decision-making
            _currentTrickCards.Add((e.Card, e.Player));
            await _ui.DisplayFormattedMessage("{0} played {1} in trick {2}\n", e.Player.Name, e.Card, e.TrickNumber + 1);
        }

        public async void OnTrickCompleted(TrickCompletedEventArgs e)
        {
            await _ui.DisplayFormattedMessage("\nTrick #{0} complete.", e.TrickNumber + 1);
            await DisplayPlayedCards(e.PlayedCards);
            await DisplayTrickWinner(e.WinningPlayer, e.WinningCard, e.TrickPoints);

            // Clear current trick cards for next trick
            _currentTrickCards.Clear();
        }

        public async void OnTrickPointsAwarded(TrickPointsAwardedEventArgs e)
        {
            await _ui.DisplayFormattedMessage("{0} collected {1} points for {2}\n", e.Player.Name, e.TrickPoints, e.TeamName);
        }

        public async void OnTeamScoring(TeamScoringEventArgs e)
        {
            string message = FormatTeamScoringMessage(e);
            await _ui.DisplayFormattedMessage(message);
        }

        public async void OnScoreUpdated(ScoreUpdatedEventArgs e)
        {
            await DisplayRoundScoring(e);
            await DisplayRunningTotals(e.TeamOneTotalPoints, e.TeamTwoTotalPoints);
        }

        public async void OnGameOver(GameOverEventArgs e)
        {
            string winner = e.IsTeamOneWinner ? "Team One" : "Team Two";

            await _ui.DisplayMessage("\n" + new string('-', GameConstants.GAME_OVER_SEPARATOR_LENGTH));
            await _ui.DisplayMessage("GAME OVER");
            await _ui.DisplayMessage($"Final Scores:");
            await _ui.DisplayFormattedMessage("Team One: {0} points", e.TeamOneFinalScore);
            await _ui.DisplayFormattedMessage("Team Two: {0} points", e.TeamTwoFinalScore);
            await _ui.DisplayFormattedMessage("\n {0} WINS!!", winner);
            await _ui.DisplayMessage(new string('-', GameConstants.GAME_OVER_SEPARATOR_LENGTH));
        }

        public async void OnNextRoundPrompt(NextRoundEventArgs e)
        {
            await _ui.WaitForUser("\nPress any key to start the next round...");
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

        private async Task DisplayBiddingResults(Dictionary<Player, int> allBids, Player winningBidder)
        {
            await _ui.DisplayMessage("Results:");

            foreach (var bid in allBids)
            {
                string bidText = FormatBidText(bid.Key, winningBidder, bid.Value);
                await _ui.DisplayFormattedMessage("{0}: {1}", bid.Key.Name, bidText);
            }
        }

        private async Task DisplayWinningBidder(Player winningBidder, int winningBid)
        {
            await _ui.DisplayFormattedMessage("\nWinning bidder is {0} with {1}\n", winningBidder.Name, winningBid);
        }

        private async Task ShowWinnerHand(Player winningBidder)
        {
            await _ui.DisplayHand(winningBidder);
        }

        private async Task DisplayTurnInformation(Player player, int trickNumber, CardSuit? leadingSuit, CardSuit? trumpSuit)
        {
            await _ui.DisplayFormattedMessage("It's {0}'s turn to play in trick {1}", player.Name, trickNumber + 1);

            if (leadingSuit.HasValue)
            {
                await _ui.DisplayFormattedMessage("Leading suit: {0}", leadingSuit);
            }

            string trumpSuitInfo = trumpSuit?.ToString() ?? "Not yet set.";
            await _ui.DisplayFormattedMessage("Trump suit: {0}", trumpSuitInfo);
        }

        private async Task DisplayPlayedCards(List<(Card card, Player player)> playedCards)
        {
            await _ui.DisplayMessage("Cards played:");
            foreach (var (card, player) in playedCards)
            {
                await _ui.DisplayFormattedMessage("{0}: {1}", player.Name, card);
            }
        }

        private async Task DisplayTrickWinner(Player winningPlayer, Card winningCard, int trickPoints)
        {
            await _ui.DisplayFormattedMessage("\nTrick winner: {0} with {1}", winningPlayer.Name, winningCard);
            await _ui.DisplayFormattedMessage("Trick points: {0}", trickPoints);
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

        private async Task DisplayRoundScoring(ScoreUpdatedEventArgs e)
        {
            await _ui.DisplayMessage("\n--- Round Scoring ---");
            await _ui.DisplayFormattedMessage("Team One round points: {0}", e.TeamOneRoundPoints);
            await _ui.DisplayFormattedMessage("Team Two round points: {0}", e.TeamTwoRoundPoints);
            await _ui.DisplayFormattedMessage("Winning bid: {0}", e.WinningBid);
            await _ui.DisplayMessage($"Bid winner: {(e.IsBidWinnerTeamOne ? "Team One" : "Team Two")}");
        }

        private async Task DisplayRunningTotals(int teamOneTotalPoints, int teamTwoTotalPoints)
        {
            await _ui.DisplayMessage("\nRunning totals:");
            await _ui.DisplayFormattedMessage("Team One: {0} points", teamOneTotalPoints);
            await _ui.DisplayFormattedMessage("Team Two: {0} points", teamTwoTotalPoints);
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