using DeuxCentsCardGame.Events.EventArgs;
using DeuxCentsCardGame.Interfaces.Events;
using DeuxCentsCardGame.Interfaces.GameConfig;
using DeuxCentsCardGame.Interfaces.UI;

namespace DeuxCentsCardGame.Interfaces.Events
{
    public interface IGameEventHandler
    {
        // Event handlers that might be called directly if needed
        void OnRoundStarted(object? sender, RoundStartedEventArgs e);
        void OnDeckCutInput(object? sender, DeckCutInputEventArgs e);
        void OnDeckCut(object? sender, DeckCutEventArgs e);
        void OnCardsDealt(object? sender, CardsDealtEventArgs e);
        void OnBettingRoundStarted(object? sender, BettingRoundStartedEventArgs e);
        void OnBetInput(object? sender, BetInputEventArgs e);
        void OnInvalidBet(object? sender, InvalidBetEventArgs e);
        void OnBettingAction(object? sender, BettingActionEventArgs e);
        void OnBettingCompleted(object? sender, BettingCompletedEventArgs e);
        void OnTrumpSelectionInput(object? sender, TrumpSelectionInputEventArgs e);
        void OnTrumpSelected(object? sender, TrumpSelectedEventArgs e);
        void OnPlayerTurn(object? sender, PlayerTurnEventArgs e);
        void OnCardSelectionInput(object? sender, CardSelectionInputEventArgs e);
        void OnCardPlayed(object? sender, CardPlayedEventArgs e);
        void OnInvalidCard(object? sender, InvalidCardEventArgs e);
        void OnTrickCompleted(object? sender, TrickCompletedEventArgs e);
        void OnTrickPointsAwarded(object? sender, TrickPointsAwardedEventArgs e);
        void OnTeamScoring(object? sender, TeamScoringEventArgs e);
        void OnScoreUpdated(object? sender, ScoreUpdatedEventArgs e);
        void OnGameOver(object? sender, GameOverEventArgs e);
        void OnNextRoundPrompt(object? sender, NextRoundEventArgs e);

        void UnsubscribeFromEvents();

    }
}