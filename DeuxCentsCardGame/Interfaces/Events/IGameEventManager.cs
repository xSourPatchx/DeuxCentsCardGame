using DeuxCentsCardGame.Events.EventArgs;
using DeuxCentsCardGame.GameStates;
using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Interfaces.Events
{
    public interface IGameEventManager
    {
        // Event properties that can be subscribed to
        Action<RoundStartedEventArgs>? RoundStarted { get; set; }
        Action<RoundEndedEventArgs>? RoundEnded { get; set; }
        Action<DeckShuffledEventArgs>? DeckShuffled { get; set; }
        Action<DeckCutInputEventArgs>? DeckCutInput { get; set; }
        Action<DeckCutEventArgs>? DeckCut { get; set; }
        Action<CardsDealtEventArgs>? CardsDealt { get; set; }
        Action<InvalidMoveEventArgs>? InvalidMove { get; set; }

        // Game state change events
        Action<StateChangedEventArgs>? StateChanged { get; set; }
        Action<GamePausedEventArgs>? GamePaused { get; set; }
        Action<GameResumedEventArgs>? GameResumed { get; set; }

        // Betting events
        Action<BettingRoundStartedEventArgs>? BettingRoundStarted { get; set; }
        Action<BetInputEventArgs>? BetInput { get; set; }
        Action<BettingActionEventArgs>? BettingAction { get; set; }
        Action<BettingCompletedEventArgs>? BettingCompleted { get; set; }

        // Trump selection events
        Action<TrumpSelectionInputEventArgs>? TrumpSelectionInput { get; set; }
        Action<TrumpSelectedEventArgs>? TrumpSelected { get; set; }

        // Card playing events
        Action<PlayerTurnEventArgs>? PlayerTurn { get; set; }
        Action<CardSelectionInputEventArgs>? CardSelectionInput { get; set; }
        Action<CardPlayedEventArgs>? CardPlayed { get; set; }
        Action<TrickCompletedEventArgs>? TrickCompleted { get; set; }

        // Scoring events
        Action<ScoreUpdatedEventArgs>? ScoreUpdated { get; set; }
        Action<TeamScoringEventArgs>? TeamScoring { get; set; }
        Action<TrickPointsAwardedEventArgs>? TrickPointsAwarded { get; set; }

        // Game end events
        Action<GameOverEventArgs>? GameOver { get; set; }
        Action<NextRoundEventArgs>? NextRoundPrompt { get; set; }

        // Methods to raise events
        Task RaiseStateChanged(GameState previousState, GameState newState);
        Task RaiseGamePaused(GameState currentState);
        Task RaiseGameResumed(GameState resumingToState);
        
        // Round events
        Task RaiseRoundStarted(int roundNumber, Player dealer);
        Task RaiseRoundEnded(int roundNumber, int teamOneRoundPoints, int teamTwoRoundPoints, Player winningBidder, int winningBid);
        Task RaiseDeckShuffled(string message);
        Task<int> RaiseDeckCutInput(Player cuttingPlayer, int deckSize);
        Task RaiseDeckCut(Player cuttingPlayer, int cutPosition);
        Task RaiseCardsDealt(List<Player> players, int dealerIndex);
        Task RaiseInvalidMove(Player player, string message, InvalidMoveType moveType);

        // Betting events
        Task RaiseBettingRoundStarted(string message);
        Task<string> RaiseBetInput(Player currentPlayer, int minBet, int maxBet, int betIncrement, List<int> takenBids, int currentHighestBid);
        Task RaiseBettingAction(Player player, int bet, bool hasPassed = false, bool hasBet = false);
        Task RaiseBettingCompleted(Player winningBidder, int winningBid, Dictionary<Player, int> allBids);

        // Trump selection events
        Task<string> RaiseTrumpSelectionInput(Player selectingPlayer);
        Task RaiseTrumpSelected(CardSuit trumpSuit, Player selectedBy);

        // Card playing events
        Task RaisePlayerTurn(Player player, CardSuit? leadingSuit, CardSuit? trumpSuit, int trickNumber);
        Task<int> RaiseCardSelectionInput(Player currentPlayer, CardSuit? leadingSuit, CardSuit? trumpSuit, List<Card> hand);
        Task RaiseCardPlayed(Player player, Card card, int trickNumber, CardSuit? leadingSuit, CardSuit? trumpSuit);
        Task RaiseTrickCompleted(int trickNumber, Player winningPlayer, Card winningCard, List<(Card card, Player player)> playedCards, int trickPoints);

        // Scoring events
        Task RaiseScoreUpdated(int teamOneRoundPoints, int teamTwoRoundPoints, int teamOneTotalPoints, int teamTwoTotalPoints, bool isBidWinnerTeamOne, int winningBid);
        Task RaiseTeamScoring(string teamName, int roundPoints, int winningBid, bool madeBid, bool cannotScore, int awardedPoints);
        Task RaiseTrickPointsAwarded(Player player, int trickPoints, string teamName);

        // Game end events
        Task RaiseGameOver(int teamOneFinalScore, int teamTwoFinalScore);
        Task RaiseNextRoundPrompt(string message = "Press any key to start the next round...");
    }
}