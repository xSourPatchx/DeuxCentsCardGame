using DeuxCentsCardGame.Events.EventArgs;
using DeuxCentsCardGame.GameStates;
using DeuxCentsCardGame.Interfaces.Events;
using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Events
{
    public class GameEventManager : IGameEventManager
    {
        // Round events
        public Action<RoundStartedEventArgs> RoundStarted;
        public Action<RoundEndedEventArgs> RoundEnded;
        public Action<DeckShuffledEventArgs> DeckShuffled;
        public Action<DeckCutInputEventArgs> DeckCutInput;
        public Action<DeckCutEventArgs> DeckCut;
        public Action<CardsDealtEventArgs> CardsDealt;
        public Action<InvalidMoveEventArgs> InvalidMove;

        // Game state change events
        public Action<StateChangedEventArgs> StateChanged;
        public Action<GamePausedEventArgs> GamePaused;
        public Action<GameResumedEventArgs> GameResumed;

        // Betting events
        public Action<BettingRoundStartedEventArgs> BettingRoundStarted;
        public Action<BetInputEventArgs> BetInput;
        public Action<BettingActionEventArgs> BettingAction;
        public Action<BettingCompletedEventArgs> BettingCompleted;

        // Trump selection events
        public Action<TrumpSelectionInputEventArgs> TrumpSelectionInput;
        public Action<TrumpSelectedEventArgs> TrumpSelected;

        // Card playing events
        public Action<PlayerTurnEventArgs> PlayerTurn;
        public Action<CardSelectionInputEventArgs> CardSelectionInput;
        public Action<CardPlayedEventArgs> CardPlayed;
        public Action<TrickCompletedEventArgs> TrickCompleted;

        // Scoring events
        public Action<ScoreUpdatedEventArgs> ScoreUpdated;
        public Action<TeamScoringEventArgs> TeamScoring;
        public Action<TrickPointsAwardedEventArgs> TrickPointsAwarded;

        // Game end events
        public Action<GameOverEventArgs> GameOver;
        public Action<NextRoundEventArgs> NextRoundPrompt;


        // Public methods to trigger events from game logic //
        // Round events
        public async Task RaiseRoundStarted(int roundNumber, Player dealer)
        {
            RoundStarted?.Invoke(new RoundStartedEventArgs(roundNumber, dealer));
            await Task.CompletedTask;
        }

        public async Task RaiseRoundEnded(int roundNumber, int teamOneRoundPoints, int teamTwoRoundPoints, Player winningBidder, int winningBid)
        {
            RoundEnded?.Invoke(new RoundEndedEventArgs(roundNumber, teamOneRoundPoints, teamTwoRoundPoints, winningBidder, winningBid));
            await Task.CompletedTask;
        }

        public async Task RaiseDeckShuffled(string message)
        {
            DeckShuffled?.Invoke(new DeckShuffledEventArgs(message));
            await Task.CompletedTask;
        }

        public async Task<int> RaiseDeckCutInput(Player cuttingPlayer, int deckSize)
        {
            var args = new DeckCutInputEventArgs(cuttingPlayer, deckSize);
            DeckCutInput?.Invoke(args);
            await Task.CompletedTask;
            return args.Response;
        }

        public async Task RaiseDeckCut(Player cuttingPlayer, int cutPosition)
        {
            DeckCut?.Invoke(new DeckCutEventArgs(cuttingPlayer, cutPosition));
            await Task.CompletedTask;
        }

        public async Task RaiseCardsDealt(List<Player> players, int dealerIndex)
        {
            CardsDealt?.Invoke(new CardsDealtEventArgs(players, dealerIndex));
            await Task.CompletedTask;
        }

        public async Task RaiseInvalidMove(Player player, string message, InvalidMoveType moveType)
        {
            InvalidMove?.Invoke(new InvalidMoveEventArgs(player, message, moveType));
            await Task.CompletedTask;
        }

        // Game state change events
        public async Task RaiseStateChanged(GameState previousState, GameState newState)
        {
            StateChanged?.Invoke(new StateChangedEventArgs(previousState, newState));
            await Task.CompletedTask;
        }

        public async Task RaiseGamePaused(GameState currentState)
        {
            GamePaused?.Invoke(new GamePausedEventArgs(currentState));
            await Task.CompletedTask;
        }

        public async Task RaiseGameResumed(GameState resumingToState)
        {
            GameResumed?.Invoke(new GameResumedEventArgs(resumingToState));
            await Task.CompletedTask;
        }

        // Betting events
        public async Task RaiseBettingRoundStarted(string message)
        {
            BettingRoundStarted?.Invoke(new BettingRoundStartedEventArgs(message));
            await Task.CompletedTask;
        }
        
        public async Task<string> RaiseBetInput(Player currentPlayer, int minBet, int maxBet, int betIncrement, 
            List<int> takenBids, int currentHighestBid)
        {
            var args = new BetInputEventArgs(currentPlayer, minBet, maxBet, betIncrement, takenBids, currentHighestBid);
            BetInput?.Invoke(args);
            await Task.CompletedTask;
            return args.Response;
        }

        public async Task RaiseBettingAction(Player player, int bet, bool hasPassed = false, bool hasBet = false)
        {
            BettingAction?.Invoke(new BettingActionEventArgs(player, bet, hasPassed, hasBet));
            await Task.CompletedTask;
        }

        public async Task RaiseBettingCompleted(Player winningBidder, int winningBid, Dictionary<Player, int> allBids)
        {
            BettingCompleted?.Invoke(new BettingCompletedEventArgs(winningBidder, winningBid, allBids));
            await Task.CompletedTask;
        }

        // Trump selection events
        public async Task<string> RaiseTrumpSelectionInput(Player selectingPlayer)
        {
            var args = new TrumpSelectionInputEventArgs(selectingPlayer);
            TrumpSelectionInput?.Invoke(args);
            await Task.CompletedTask;
            return args.Response;
        }

        public async Task RaiseTrumpSelected(CardSuit trumpSuit, Player selectedBy)
        {
            TrumpSelected?.Invoke(new TrumpSelectedEventArgs(trumpSuit, selectedBy));
            await Task.CompletedTask;
        }

        // Card playing events
        public async Task RaisePlayerTurn(Player player, CardSuit? leadingSuit, CardSuit? trumpSuit, int trickNumber)
        {
            PlayerTurn?.Invoke(new PlayerTurnEventArgs(player, leadingSuit, trumpSuit, trickNumber));
            await Task.CompletedTask;
        }

        public async Task<int> RaiseCardSelectionInput(Player currentPlayer, CardSuit? leadingSuit, CardSuit? trumpSuit, List<Card> hand)
        {
            var args = new CardSelectionInputEventArgs(currentPlayer, leadingSuit, trumpSuit, hand);
            CardSelectionInput?.Invoke(args);
            await Task.CompletedTask;
            return args.Response;
        }

        public async Task RaiseCardPlayed(Player player, Card card, int trickNumber, CardSuit? leadingSuit, CardSuit? trumpSuit)
        {
            CardPlayed?.Invoke(new CardPlayedEventArgs(player, card, trickNumber, leadingSuit, trumpSuit));
            await Task.CompletedTask;
        }

        public async Task RaiseTrickCompleted(int trickNumber, Player winningPlayer, Card winningCard, List<(Card card, Player player)> playedCards, int trickPoints)
        {
            TrickCompleted?.Invoke(new TrickCompletedEventArgs(trickNumber, winningPlayer, winningCard, playedCards, trickPoints));
            await Task.CompletedTask;
        }

        // Scoring events
        public async Task RaiseScoreUpdated(int teamOneRoundPoints, int teamTwoRoundPoints, int teamOneTotalPoints, int teamTwoTotalPoints, bool isBidWinnerTeamOne, int winningBid)
        {
            ScoreUpdated?.Invoke(new ScoreUpdatedEventArgs(teamOneRoundPoints, teamTwoRoundPoints, teamOneTotalPoints, teamTwoTotalPoints, isBidWinnerTeamOne, winningBid));
            await Task.CompletedTask;
        }

        public async Task RaiseTeamScoring(string teamName, int roundPoints, int winningBid, bool madeBid, bool cannotScore, int awardedPoints)
        {
            TeamScoring?.Invoke(new TeamScoringEventArgs(teamName, roundPoints, winningBid, madeBid, cannotScore, awardedPoints));
            await Task.CompletedTask;
        }

        public async Task RaiseTrickPointsAwarded(Player player, int trickPoints, string teamName)
        {
            TrickPointsAwarded?.Invoke(new TrickPointsAwardedEventArgs(player, trickPoints, teamName));
            await Task.CompletedTask;
        }

        // Game end events
        public async Task RaiseGameOver(int teamOneFinalScore, int teamTwoFinalScore)
        {
            GameOver?.Invoke(new GameOverEventArgs(teamOneFinalScore, teamTwoFinalScore));
            await Task.CompletedTask;
        }

        public async Task RaiseNextRoundPrompt(string message = "Press any key to start the next round...")
        {
            NextRoundPrompt?.Invoke(new NextRoundEventArgs(message));
            await Task.CompletedTask;
        }

        // Clear all event subscriptions (useful for cleanup)
        public void ClearAllEvents()
        {
            RoundStarted = null;
            RoundEnded = null;
            DeckShuffled = null;
            DeckCutInput = null;
            DeckCut = null;
            CardsDealt = null;
            InvalidMove = null;
            StateChanged = null;
            GamePaused = null;
            GameResumed = null;
            BettingRoundStarted = null;
            BetInput = null;
            BettingAction = null;
            BettingCompleted = null;
            TrumpSelectionInput = null;
            TrumpSelected = null;
            PlayerTurn = null;
            CardSelectionInput = null;
            CardPlayed = null;
            TrickCompleted = null;
            ScoreUpdated = null;
            TeamScoring = null;
            TrickPointsAwarded = null;
            GameOver = null;
            NextRoundPrompt = null;
        }
    }
}