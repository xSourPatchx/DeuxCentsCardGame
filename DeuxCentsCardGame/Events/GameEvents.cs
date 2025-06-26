using DeuxCentsCardGame.Events.EventArgs;
using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Events
{
    public class GameEventManager
    {
        // Round events
        public event EventHandler<RoundEventArgs>? RoundStarted;
        public event EventHandler<CardsDealtEventArgs>? CardsDealt;

        // Betting and trump selection events
        public event EventHandler<BettingEventArgs>? BettingAction;
        public event EventHandler<BettingCompletedEventArgs>? BettingCompleted;
        public event EventHandler<TrumpSelectedEventArgs>? TrumpSelected;

        // Card playing events
        public event EventHandler<PlayerTurnEventArgs>? PlayerTurn;
        public event EventHandler<CardPlayedEventArgs>? CardPlayed;
        public event EventHandler<TrickCompletedEventArgs>? TrickCompleted;

        // Scoring events
        public event EventHandler<ScoreEventArgs>? ScoreUpdated;
        public event EventHandler<TeamScoringEventArgs>? TeamScoring;
        public event EventHandler<TrickPointsAwardedEventArgs>? TrickPointsAwarded;

        // Game end events
        public event EventHandler<GameOverEventArgs>? GameOver;
        public event EventHandler<NextRoundEventArgs>? NextRoundPrompt;

        // Event raising methods
        protected virtual void OnRoundStarted(RoundEventArgs e)
        {
            RoundStarted?.Invoke(this, e);
        }

        protected virtual void OnCardsDealt(CardsDealtEventArgs e)
        {
            CardsDealt?.Invoke(this, e);
        }

        protected virtual void OnBettingAction(BettingEventArgs e)
        {
            BettingAction?.Invoke(this, e);
        }

        protected virtual void OnBettingCompleted(BettingCompletedEventArgs e)
        {
            BettingCompleted?.Invoke(this, e);
        }

        protected virtual void OnTrumpSelected(TrumpSelectedEventArgs e)
        {
            TrumpSelected?.Invoke(this, e);
        }

        protected virtual void OnPlayerTurn(PlayerTurnEventArgs e)
        {
            PlayerTurn?.Invoke(this, e);
        }

        protected virtual void OnCardPlayed(CardPlayedEventArgs e)
        {
            CardPlayed?.Invoke(this, e);
        }

        protected virtual void OnTrickCompleted(TrickCompletedEventArgs e)
        {
            TrickCompleted?.Invoke(this, e);
        }

        protected virtual void OnScoreUpdated(ScoreEventArgs e)
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

        protected virtual void OnGameOver(GameOverEventArgs e)
        {
            GameOver?.Invoke(this, e);
        }

        protected virtual void OnNextRoundPrompt(NextRoundEventArgs e)
        {
            NextRoundPrompt?.Invoke(this, e);
        }

        // Public methods to trigger events from game logic
        public void RaiseRoundStarted(int roundNumber, Player dealer)
        {
            OnRoundStarted(new RoundEventArgs(roundNumber, dealer));
        }

        public void RaiseCardsDealt(List<Player> players, int dealerIndex)
        {
            OnCardsDealt(new CardsDealtEventArgs(players, dealerIndex));
        }

        public void RaiseBettingAction(Player player, int bet, bool hasPassed = false)
        {
            OnBettingAction(new BettingEventArgs(player, bet, hasPassed));
        }

        public void RaiseBettingCompleted(Player winningBidder, int winningBid, Dictionary<Player, int> allBids)
        {
            OnBettingCompleted(new BettingCompletedEventArgs(winningBidder, winningBid, allBids));
        }

        public void RaiseTrumpSelected(CardSuit trumpSuit, Player selectedBy)
        {
            OnTrumpSelected(new TrumpSelectedEventArgs(trumpSuit, selectedBy));
        }

        public void RaisePlayerTurn(Player player, CardSuit? leadingSuit, CardSuit? trumpSuit, int trickNumber)
        {
            OnPlayerTurn(new PlayerTurnEventArgs(player, leadingSuit, trumpSuit, trickNumber));
        }

        public void RaiseCardPlayed(Player player, Card card, int trickNumber, CardSuit? leadingSuit, CardSuit? trumpSuit)
        {
            OnCardPlayed(new CardPlayedEventArgs(player, card, trickNumber, leadingSuit, trumpSuit));
        }

        public void RaiseTrickCompleted(int trickNumber, Player winningPlayer, Card winningCard, List<(Card card, Player player)> playedCards, int trickPoints)
        {
            OnTrickCompleted(new TrickCompletedEventArgs(trickNumber, winningPlayer, winningCard, playedCards, trickPoints));
        }

        public void RaiseScoreUpdated(int teamOneRoundPoints, int teamTwoRoundPoints, int teamOneTotalPoints, int teamTwoTotalPoints, bool isBidWinnerTeamOne, int winningBid)
        {
            OnScoreUpdated(new ScoreEventArgs(teamOneRoundPoints, teamTwoRoundPoints, teamOneTotalPoints, teamTwoTotalPoints, isBidWinnerTeamOne, winningBid));
        }

        public void RaiseTeamScoring(string teamName, int roundPoints, int winningBid, bool madeBid, bool cannotScore, int awardedPoints)
        {
            OnTeamScoring(new TeamScoringEventArgs(teamName, roundPoints, winningBid, madeBid, cannotScore, awardedPoints));
        }

        public void RaiseTrickPointsAwarded(Player player, int trickPoints, string teamName)
        {
            OnTrickPointsAwarded(new TrickPointsAwardedEventArgs(player, trickPoints, teamName));
        }

        public void RaiseGameOver(int teamOneFinalScore, int teamTwoFinalScore)
        {
            OnGameOver(new GameOverEventArgs(teamOneFinalScore, teamTwoFinalScore));
        }
    }
}