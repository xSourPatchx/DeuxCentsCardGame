using DeuxCentsCardGame.Events.EventArgs;

namespace DeuxCentsCardGame.Interfaces.Events
{
    public interface IGameEventHandler
    {
        // Event handlers that might be called directly if needed
        void OnRoundStarted(RoundStartedEventArgs e);
        void OnRoundEnded(RoundEndedEventArgs e);
        void OnDeckShuffled(DeckShuffledEventArgs e);
        void OnDeckCutInput(DeckCutInputEventArgs e);
        void OnDeckCut(DeckCutEventArgs e);
        void OnCardsDealt(CardsDealtEventArgs e);
        void OnInvalidMove(InvalidMoveEventArgs e);
        void OnBettingRoundStarted(BettingRoundStartedEventArgs e);
        void OnBetInput(BetInputEventArgs e);
        void OnBettingAction(BettingActionEventArgs e);
        void OnBettingCompleted(BettingCompletedEventArgs e);
        void OnTrumpSelectionInput(TrumpSelectionInputEventArgs e);
        void OnTrumpSelected(TrumpSelectedEventArgs e);
        void OnPlayerTurn(PlayerTurnEventArgs e);
        void OnCardSelectionInput(CardSelectionInputEventArgs e);
        void OnCardPlayed(CardPlayedEventArgs e);
        void OnTrickCompleted(TrickCompletedEventArgs e);
        void OnTrickPointsAwarded(TrickPointsAwardedEventArgs e);
        void OnTeamScoring(TeamScoringEventArgs e);
        void OnScoreUpdated(ScoreUpdatedEventArgs e);
        void OnGameOver(GameOverEventArgs e);
        void OnNextRoundPrompt(NextRoundEventArgs e);
        void UnsubscribeFromEvents();
    }
}