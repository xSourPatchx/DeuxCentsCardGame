using DeuxCentsCardGame.Events.EventArgs;
using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Events
{
    public class GameEventManager
    {
        // Round events
        public event EventHandler<RoundStartedEventArgs>? RoundStarted;
        public event EventHandler<CardsDealtEventArgs>? CardsDealt;

        // Betting events
        public event EventHandler<BettingRoundStartedEventArgs>? BettingRoundStarted;
        public event EventHandler<BetInputEventArgs>? BetInput;
        public event EventHandler<InvalidBetEventArgs>? InvalidBet;
        public event EventHandler<BettingActionEventArgs>? BettingAction;
        public event EventHandler<BettingCompletedEventArgs>? BettingCompleted;

        // Trump selection events
        public event EventHandler<TrumpSelectionInputEventArgs>? TrumpSelectionInput;
        public event EventHandler<TrumpSelectedEventArgs>? TrumpSelected;

        // Card playing events
        public event EventHandler<PlayerTurnEventArgs>? PlayerTurn;
        public event EventHandler<CardSelectionInputEventArgs>? CardSelectionInput;
        public event EventHandler<CardPlayedEventArgs>? CardPlayed;
        public event EventHandler<InvalidCardEventArgs>? InvalidCard;
        public event EventHandler<TrickCompletedEventArgs>? TrickCompleted;

        // Scoring events
        public event EventHandler<ScoreUpdatedEventArgs>? ScoreUpdated;
        public event EventHandler<TeamScoringEventArgs>? TeamScoring;
        public event EventHandler<TrickPointsAwardedEventArgs>? TrickPointsAwarded;

        // Game end events
        public event EventHandler<GameOverEventArgs>? GameOver;
        public event EventHandler<NextRoundEventArgs>? NextRoundPrompt;

        // Event raising methods
        // Round events
        protected virtual void OnRoundStarted(RoundStartedEventArgs e)
        {
            RoundStarted?.Invoke(this, e);
        }

        protected virtual void OnCardsDealt(CardsDealtEventArgs e)
        {
            CardsDealt?.Invoke(this, e);
        }

        // Betting events
        protected virtual void OnBettingRoundStarted(BettingRoundStartedEventArgs e)
        {
            BettingRoundStarted?.Invoke(this, e);
        }

        protected virtual void OnBetInput(BetInputEventArgs e)
        {
            BetInput?.Invoke(this, e);
        }
        
        protected virtual void OnInvalidBet(InvalidBetEventArgs e)
        {
            InvalidBet?.Invoke(this, e);
        }

        protected virtual void OnBettingAction(BettingActionEventArgs e)
        {
            BettingAction?.Invoke(this, e);
        }

        protected virtual void OnBettingCompleted(BettingCompletedEventArgs e)
        {
            BettingCompleted?.Invoke(this, e);
        }

        // Trump selection events
        protected virtual void OnTrumpSelectionInput(TrumpSelectionInputEventArgs e)
        {
            TrumpSelectionInput?.Invoke(this, e);
        }

        protected virtual void OnTrumpSelected(TrumpSelectedEventArgs e)
        {
            TrumpSelected?.Invoke(this, e);
        }

        // Card playing events
        protected virtual void OnPlayerTurn(PlayerTurnEventArgs e)
        {
            PlayerTurn?.Invoke(this, e);
        }

        protected virtual void OnCardSelectionInput(CardSelectionInputEventArgs e)
        {
            CardSelectionInput?.Invoke(this, e);
        }

        protected virtual void OnCardPlayed(CardPlayedEventArgs e)
        {
            CardPlayed?.Invoke(this, e);
        }

        protected virtual void OnInvalidCard(InvalidCardEventArgs e)
        {
            InvalidCard?.Invoke(this, e);
        }

        protected virtual void OnTrickCompleted(TrickCompletedEventArgs e)
        {
            TrickCompleted?.Invoke(this, e);
        }

        // Scoring events
        protected virtual void OnScoreUpdated(ScoreUpdatedEventArgs e)
        {
            ScoreUpdated?.Invoke(this, e);
        }

        protected virtual void OnTeamScoring(TeamScoringEventArgs e)
        {
            TeamScoring?.Invoke(this, e);
        }

        protected virtual void OnTrickPointsAwarded(TrickPointsAwardedEventArgs e)
        {
            TrickPointsAwarded?.Invoke(this, e);
        }

        // Game end events
        protected virtual void OnGameOver(GameOverEventArgs e)
        {
            GameOver?.Invoke(this, e);
        }

        protected virtual void OnNextRoundPrompt(NextRoundEventArgs e)
        {
            NextRoundPrompt?.Invoke(this, e);
        }

        // Public methods to trigger events from game logic
        // Round events
        public void RaiseRoundStarted(int roundNumber, Player dealer)
        {
            OnRoundStarted(new RoundStartedEventArgs(roundNumber, dealer));
        }

        public void RaiseCardsDealt(List<Player> players, int dealerIndex)
        {
            OnCardsDealt(new CardsDealtEventArgs(players, dealerIndex));
        }

        // Betting events
        public void RaiseBettingRoundStarted(string message)
        {
            OnBettingRoundStarted(new BettingRoundStartedEventArgs(message));
        }
        
        // here im using event to return response, might not be used in Unity
        public string RaiseBetInput(Player currentPlayer, int minBet, int maxBet, int betIncrement)
        {
            var args = new BetInputEventArgs(currentPlayer, minBet, maxBet, betIncrement);
            OnBetInput(args);
            return args.Response;
        }

        public void RaiseInvalidBet(string message)
        {
            OnInvalidBet(new InvalidBetEventArgs(message));
        }

        public void RaiseBettingAction(Player player, int bet, bool hasPassed = false)
        {
            OnBettingAction(new BettingActionEventArgs(player, bet, hasPassed));
        }

        public void RaiseBettingCompleted(Player winningBidder, int winningBid, Dictionary<Player, int> allBids)
        {
            OnBettingCompleted(new BettingCompletedEventArgs(winningBidder, winningBid, allBids));
        }

        // Trump selection events
        public string RaiseTrumpSelectionInput(Player selectingPlayer)
        {
            var args = new TrumpSelectionInputEventArgs(selectingPlayer);
            OnTrumpSelectionInput(args);
            return args.Response;
        }

        public void RaiseTrumpSelected(CardSuit trumpSuit, Player selectedBy)
        {
            OnTrumpSelected(new TrumpSelectedEventArgs(trumpSuit, selectedBy));
        }

        // Card playing events
        public void RaisePlayerTurn(Player player, CardSuit? leadingSuit, CardSuit? trumpSuit, int trickNumber)
        {
            OnPlayerTurn(new PlayerTurnEventArgs(player, leadingSuit, trumpSuit, trickNumber));
        }

        public int RaiseCardSelectionInput(Player currentPlayer, CardSuit? leadingSuit, CardSuit? trumpSuit, List<Card> hand)
        {
            var args = new CardSelectionInputEventArgs(currentPlayer, leadingSuit, trumpSuit, hand);
            OnCardSelectionInput(args);
            return args.Response;
        }

        public void RaiseCardPlayed(Player player, Card card, int trickNumber, CardSuit? leadingSuit, CardSuit? trumpSuit)
        {
            OnCardPlayed(new CardPlayedEventArgs(player, card, trickNumber, leadingSuit, trumpSuit));
        }

        public void RaiseInvalidCard(string message)
        {
            OnInvalidCard(new InvalidCardEventArgs(message));
        }

        public void RaiseTrickCompleted(int trickNumber, Player winningPlayer, Card winningCard, List<(Card card, Player player)> playedCards, int trickPoints)
        {
            OnTrickCompleted(new TrickCompletedEventArgs(trickNumber, winningPlayer, winningCard, playedCards, trickPoints));
        }

        // Scoring events
        public void RaiseScoreUpdated(int teamOneRoundPoints, int teamTwoRoundPoints, int teamOneTotalPoints, int teamTwoTotalPoints, bool isBidWinnerTeamOne, int winningBid)
        {
            OnScoreUpdated(new ScoreUpdatedEventArgs(teamOneRoundPoints, teamTwoRoundPoints, teamOneTotalPoints, teamTwoTotalPoints, isBidWinnerTeamOne, winningBid));
        }

        public void RaiseTeamScoring(string teamName, int roundPoints, int winningBid, bool madeBid, bool cannotScore, int awardedPoints)
        {
            OnTeamScoring(new TeamScoringEventArgs(teamName, roundPoints, winningBid, madeBid, cannotScore, awardedPoints));
        }

        public void RaiseTrickPointsAwarded(Player player, int trickPoints, string teamName)
        {
            OnTrickPointsAwarded(new TrickPointsAwardedEventArgs(player, trickPoints, teamName));
        }

        // Game end events
        public void RaiseGameOver(int teamOneFinalScore, int teamTwoFinalScore)
        {
            OnGameOver(new GameOverEventArgs(teamOneFinalScore, teamTwoFinalScore));
        }

        public void RaiseNextRoundPrompt(string message = "Press any key to start the next round...")
        {
            OnNextRoundPrompt(new NextRoundEventArgs(message));
        }
    }
}