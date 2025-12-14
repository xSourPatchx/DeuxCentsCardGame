using DeuxCentsCardGame.GameStates;
using DeuxCentsCardGame.Models;
using DeuxCentsCardGame.Unity;

namespace DeuxCentsCardGame.Network
{
    // Validates game state consistency for multiplayer synchronization.
    // Ensures game integrity by detecting desyncs and invalid actions early.
    public class NetworkSyncValidator
    {
        private readonly List<ValidationError> _errors = new();
        private readonly ValidationConfig _config;

        public NetworkSyncValidator(ValidationConfig? config = null)
        {
            _config = config ?? ValidationConfig.CreateDefault();
        }

        #region Game State Validation

        // Validates that local and remote game states are consistent.
        // Returns true if states match within acceptable tolerance.
        public bool ValidateGameState(GameSnapshot local, GameSnapshot remote)
        {
            _errors.Clear();

            if (local == null || remote == null)
            {
                AddError(ValidationErrorType.NullState, "Local or remote state is null");
                return false;
            }

            // Critical validations - must match exactly
            ValidateCriticalState(local, remote);
            
            // Player state validation
            ValidatePlayerStates(local.Players, remote.Players);
            
            // Score validation
            ValidateScores(local.Scores, remote.Scores);
            
            // Betting validation
            ValidateBetting(local.Betting, remote.Betting);

            // Current trick validation (if in playing state)
            if (local.CurrentState == GameState.Playing && local.CurrentTrick != null)
            {
                ValidateCurrentTrick(local.CurrentTrick, remote.CurrentTrick);
            }

            return !_errors.Any();
        }

        private void ValidateCriticalState(GameSnapshot local, GameSnapshot remote)
        {
            // Game state must match exactly
            if (local.CurrentState != remote.CurrentState)
            {
                AddError(ValidationErrorType.StateMismatch, 
                    $"Game state mismatch: Local={local.CurrentState}, Remote={remote.CurrentState}");
            }

            // Round number must match
            if (local.RoundNumber != remote.RoundNumber)
            {
                AddError(ValidationErrorType.RoundMismatch,
                    $"Round number mismatch: Local={local.RoundNumber}, Remote={remote.RoundNumber}");
            }

            // Trick number must match during playing state
            if (local.CurrentState == GameState.Playing && local.TrickNumber != remote.TrickNumber)
            {
                AddError(ValidationErrorType.TrickMismatch,
                    $"Trick number mismatch: Local={local.TrickNumber}, Remote={remote.TrickNumber}");
            }

            // Dealer must match
            if (local.DealerIndex != remote.DealerIndex)
            {
                AddError(ValidationErrorType.DealerMismatch,
                    $"Dealer index mismatch: Local={local.DealerIndex}, Remote={remote.DealerIndex}");
            }

            // Trump suit must match (if set)
            if (local.TrumpSuit != remote.TrumpSuit)
            {
                AddError(ValidationErrorType.TrumpMismatch,
                    $"Trump suit mismatch: Local={local.TrumpSuit}, Remote={remote.TrumpSuit}");
            }

            // Timestamp difference check (detect significant time desync)
            var timeDiff = Math.Abs((local.Timestamp - remote.Timestamp).TotalSeconds);
            if (timeDiff > _config.MaxTimestampDifferenceSeconds)
            {
                AddError(ValidationErrorType.TimestampDesync,
                    $"Timestamp desync: {timeDiff:F2}s difference");
            }
        }

        private void ValidatePlayerStates(List<PlayerSnapshot> local, List<PlayerSnapshot> remote)
        {
            if (local.Count != remote.Count)
            {
                AddError(ValidationErrorType.PlayerCountMismatch,
                    $"Player count mismatch: Local={local.Count}, Remote={remote.Count}");
                return;
            }

            for (int i = 0; i < local.Count; i++)
            {
                var localPlayer = local[i];
                var remotePlayer = remote[i];

                // Validate hand sizes match
                if (localPlayer.Hand.Count != remotePlayer.Hand.Count)
                {
                    AddError(ValidationErrorType.HandSizeMismatch,
                        $"Player {i} hand size mismatch: Local={localPlayer.Hand.Count}, Remote={remotePlayer.Hand.Count}");
                }

                // Validate betting state
                if (localPlayer.HasBet != remotePlayer.HasBet)
                {
                    AddError(ValidationErrorType.BettingStateMismatch,
                        $"Player {i} HasBet mismatch: Local={localPlayer.HasBet}, Remote={remotePlayer.HasBet}");
                }

                if (localPlayer.HasPassed != remotePlayer.HasPassed)
                {
                    AddError(ValidationErrorType.BettingStateMismatch,
                        $"Player {i} HasPassed mismatch: Local={localPlayer.HasPassed}, Remote={remotePlayer.HasPassed}");
                }

                if (localPlayer.CurrentBid != remotePlayer.CurrentBid)
                {
                    AddError(ValidationErrorType.BidMismatch,
                        $"Player {i} bid mismatch: Local={localPlayer.CurrentBid}, Remote={remotePlayer.CurrentBid}");
                }

                // Validate card hashes match (don't reveal actual cards)
                if (_config.ValidateCardHashes)
                {
                    var localHash = GetCardCollectionHash(localPlayer.Hand);
                    var remoteHash = GetCardCollectionHash(remotePlayer.Hand);
                    
                    if (localHash != remoteHash)
                    {
                        AddError(ValidationErrorType.CardHashMismatch,
                            $"Player {i} card hash mismatch");
                    }
                }
            }
        }

        private void ValidateScores(ScoreSnapshot local, ScoreSnapshot remote)
        {
            // Total scores must match exactly
            if (local.TeamOneTotalPoints != remote.TeamOneTotalPoints)
            {
                AddError(ValidationErrorType.ScoreMismatch,
                    $"Team One total score mismatch: Local={local.TeamOneTotalPoints}, Remote={remote.TeamOneTotalPoints}");
            }

            if (local.TeamTwoTotalPoints != remote.TeamTwoTotalPoints)
            {
                AddError(ValidationErrorType.ScoreMismatch,
                    $"Team Two total score mismatch: Local={local.TeamTwoTotalPoints}, Remote={remote.TeamTwoTotalPoints}");
            }

            // Round scores must match
            if (local.TeamOneRoundPoints != remote.TeamOneRoundPoints)
            {
                AddError(ValidationErrorType.ScoreMismatch,
                    $"Team One round score mismatch: Local={local.TeamOneRoundPoints}, Remote={remote.TeamOneRoundPoints}");
            }

            if (local.TeamTwoRoundPoints != remote.TeamTwoRoundPoints)
            {
                AddError(ValidationErrorType.ScoreMismatch,
                    $"Team Two round score mismatch: Local={local.TeamTwoRoundPoints}, Remote={remote.TeamTwoRoundPoints}");
            }
        }

        private void ValidateBetting(BettingSnapshot local, BettingSnapshot remote)
        {
            if (local.CurrentWinningBid != remote.CurrentWinningBid)
            {
                AddError(ValidationErrorType.BettingMismatch,
                    $"Winning bid mismatch: Local={local.CurrentWinningBid}, Remote={remote.CurrentWinningBid}");
            }

            if (local.CurrentWinningBidIndex != remote.CurrentWinningBidIndex)
            {
                AddError(ValidationErrorType.BettingMismatch,
                    $"Winning bidder index mismatch: Local={local.CurrentWinningBidIndex}, Remote={remote.CurrentWinningBidIndex}");
            }

            if (local.IsBettingRoundComplete != remote.IsBettingRoundComplete)
            {
                AddError(ValidationErrorType.BettingMismatch,
                    $"Betting completion state mismatch: Local={local.IsBettingRoundComplete}, Remote={remote.IsBettingRoundComplete}");
            }

            // Validate all player bids match
            if (local.PlayerBids.Count == remote.PlayerBids.Count)
            {
                for (int i = 0; i < local.PlayerBids.Count; i++)
                {
                    if (local.PlayerBids[i] != remote.PlayerBids[i])
                    {
                        AddError(ValidationErrorType.BidMismatch,
                            $"Player {i} bid mismatch: Local={local.PlayerBids[i]}, Remote={remote.PlayerBids[i]}");
                    }
                }
            }
        }

        private void ValidateCurrentTrick(TrickSnapshot? local, TrickSnapshot? remote)
        {
            if (local == null && remote == null)
                return;

            if (local == null || remote == null)
            {
                AddError(ValidationErrorType.TrickMismatch,
                    "One client has trick data, other doesn't");
                return;
            }

            // Validate cards played match
            if (local.CardsPlayed.Count != remote.CardsPlayed.Count)
            {
                AddError(ValidationErrorType.TrickMismatch,
                    $"Cards played count mismatch: Local={local.CardsPlayed.Count}, Remote={remote.CardsPlayed.Count}");
                return;
            }

            // Validate each card and player matches
            for (int i = 0; i < local.CardsPlayed.Count; i++)
            {
                var localCard = local.CardsPlayed[i];
                var remoteCard = remote.CardsPlayed[i];

                if (!CardsMatch(localCard.Card, remoteCard.Card))
                {
                    AddError(ValidationErrorType.CardMismatch,
                        $"Card {i} in trick doesn't match");
                }

                if (localCard.PlayerIndex != remoteCard.PlayerIndex)
                {
                    AddError(ValidationErrorType.PlayerIndexMismatch,
                        $"Player index mismatch for card {i}: Local={localCard.PlayerIndex}, Remote={remoteCard.PlayerIndex}");
                }
            }

            // Validate leading suit
            if (local.LeadingSuit != remote.LeadingSuit)
            {
                AddError(ValidationErrorType.LeadingSuitMismatch,
                    $"Leading suit mismatch: Local={local.LeadingSuit}, Remote={remote.LeadingSuit}");
            }

            // Validate player indices
            if (local.CurrentPlayerIndex != remote.CurrentPlayerIndex)
            {
                AddError(ValidationErrorType.PlayerIndexMismatch,
                    $"Current player mismatch: Local={local.CurrentPlayerIndex}, Remote={remote.CurrentPlayerIndex}");
            }

            if (local.StartingPlayerIndex != remote.StartingPlayerIndex)
            {
                AddError(ValidationErrorType.PlayerIndexMismatch,
                    $"Starting player mismatch: Local={local.StartingPlayerIndex}, Remote={remote.StartingPlayerIndex}");
            }
        }

        #endregion

        #region Player Action Validation


        // Validates that a player action is legal in the current game state.
        // Returns true if action is valid.
        public bool ValidatePlayerAction(PlayerAction action, GameSnapshot currentState)
        {
            _errors.Clear();

            if (action == null)
            {
                AddError(ValidationErrorType.InvalidAction, "Action is null");
                return false;
            }

            if (currentState == null)
            {
                AddError(ValidationErrorType.NullState, "Current state is null");
                return false;
            }

            if (!action.IsValid())
            {
                AddError(ValidationErrorType.InvalidAction, "Action failed basic validation");
                return false;
            }

            // Find the player
            var playerIndex = FindPlayerIndex(action.PlayerId, currentState);
            if (playerIndex == -1)
            {
                AddError(ValidationErrorType.InvalidPlayer, $"Player {action.PlayerId} not found");
                return false;
            }

            // Validate action is appropriate for current game state
            if (!IsActionValidForState(action.ActionType, currentState.CurrentState))
            {
                AddError(ValidationErrorType.InvalidActionForState,
                    $"Action {action.ActionType} not valid in state {currentState.CurrentState}");
                return false;
            }

            // Validate specific action types
            switch (action.ActionType)
            {
                case PlayerActionType.Bet:
                    return ValidateBetAction(action, currentState, playerIndex);
                
                case PlayerActionType.Pass:
                    return ValidatePassAction(action, currentState, playerIndex);
                
                case PlayerActionType.PlayCard:
                    return ValidateCardPlayAction(action, currentState, playerIndex);
                
                case PlayerActionType.SelectTrump:
                    return ValidateTrumpSelectionAction(action, currentState, playerIndex);
                
                default:
                    AddError(ValidationErrorType.UnknownActionType, $"Unknown action type: {action.ActionType}");
                    return false;
            }
        }

        private bool ValidateBetAction(PlayerAction action, GameSnapshot state, int playerIndex)
        {
            var player = state.Players[playerIndex];

            // Check player hasn't already bet or passed
            if (player.HasBet)
            {
                AddError(ValidationErrorType.DuplicateAction, "Player has already bet");
                return false;
            }

            if (player.HasPassed)
            {
                AddError(ValidationErrorType.InvalidAction, "Player has already passed");
                return false;
            }

            // Validate bet amount
            if (action.BetAmount < 50 || action.BetAmount > 100)
            {
                AddError(ValidationErrorType.InvalidBetAmount, 
                    $"Bet amount {action.BetAmount} outside valid range [50-100]");
                return false;
            }

            // Validate bet increment (must be multiple of 5)
            if (action.BetAmount % 5 != 0)
            {
                AddError(ValidationErrorType.InvalidBetAmount,
                    $"Bet amount {action.BetAmount} not a multiple of 5");
                return false;
            }

            // Validate bet is unique
            if (state.Betting.PlayerBids.Any(b => b == action.BetAmount && b > 0))
            {
                AddError(ValidationErrorType.DuplicateBet, 
                    $"Bet amount {action.BetAmount} already taken");
                return false;
            }

            // Validate bet is higher than current winning bid
            if (action.BetAmount <= state.Betting.CurrentWinningBid)
            {
                AddError(ValidationErrorType.InvalidBetAmount,
                    $"Bet {action.BetAmount} must be higher than current bid {state.Betting.CurrentWinningBid}");
                return false;
            }

            return true;
        }

        private bool ValidatePassAction(PlayerAction action, GameSnapshot state, int playerIndex)
        {
            var player = state.Players[playerIndex];

            // Check player hasn't already passed
            if (player.HasPassed)
            {
                AddError(ValidationErrorType.DuplicateAction, "Player has already passed");
                return false;
            }

            // Check if this would be the 4th pass (might be forced to bet minimum)
            int passCount = state.Players.Count(p => p.HasPassed);
            if (passCount >= 3 && !state.Players.Any(p => p.HasBet))
            {
                AddError(ValidationErrorType.InvalidAction, 
                    "Cannot pass - would be 4th player to pass with no bets");
                return false;
            }

            return true;
        }

        private bool ValidateCardPlayAction(PlayerAction action, GameSnapshot state, int playerIndex)
        {
            var player = state.Players[playerIndex];

            // Validate player has cards
            if (player.Hand.Count == 0)
            {
                AddError(ValidationErrorType.EmptyHand, "Player has no cards");
                return false;
            }

            // Validate card exists in hand
            if (action.CardIndex < 0 || action.CardIndex >= player.Hand.Count)
            {
                AddError(ValidationErrorType.InvalidCardIndex,
                    $"Card index {action.CardIndex} out of range [0-{player.Hand.Count - 1}]");
                return false;
            }

            var cardToPlay = player.Hand[action.CardIndex];

            // Validate card matches the one in action
            if (action.CardPlayed != null && !CardsMatch(cardToPlay, action.CardPlayed))
            {
                AddError(ValidationErrorType.CardMismatch,
                    "Card in action doesn't match card at index in hand");
                return false;
            }

            // Validate following suit rules if there's a leading suit
            if (state.CurrentTrick?.LeadingSuit != null)
            {
                var leadingSuit = state.CurrentTrick.LeadingSuit.Value;
                var hasLeadingSuit = player.Hand.Any(c => c.CardSuit == leadingSuit);

                // Must follow suit if possible
                if (hasLeadingSuit && cardToPlay.CardSuit != leadingSuit)
                {
                    AddError(ValidationErrorType.MustFollowSuit,
                        $"Must follow suit {leadingSuit}");
                    return false;
                }
            }

            return true;
        }

        private bool ValidateTrumpSelectionAction(PlayerAction action, GameSnapshot state, int playerIndex)
        {
            // Validate player is the winning bidder
            if (state.Betting.CurrentWinningBidIndex != playerIndex)
            {
                AddError(ValidationErrorType.NotWinningBidder,
                    "Only winning bidder can select trump");
                return false;
            }

            // Validate trump suit is valid
            if (!action.TrumpSuit.HasValue)
            {
                AddError(ValidationErrorType.InvalidTrumpSelection, "Trump suit not specified");
                return false;
            }

            if (!Enum.IsDefined(typeof(CardSuit), action.TrumpSuit.Value))
            {
                AddError(ValidationErrorType.InvalidTrumpSelection,
                    $"Invalid trump suit: {action.TrumpSuit.Value}");
                return false;
            }

            return true;
        }

        #endregion

        #region Helper Methods

        private bool IsActionValidForState(PlayerActionType actionType, GameState gameState)
        {
            return (actionType, gameState) switch
            {
                (PlayerActionType.Bet, GameState.Betting) => true,
                (PlayerActionType.Pass, GameState.Betting) => true,
                (PlayerActionType.SelectTrump, GameState.TrumpSelection) => true,
                (PlayerActionType.PlayCard, GameState.Playing) => true,
                _ => false
            };
        }

        private int FindPlayerIndex(string playerId, GameSnapshot state)
        {
            // Try to match by player name/ID
            for (int i = 0; i < state.Players.Count; i++)
            {
                if (state.Players[i].Name == playerId)
                    return i;
            }
            return -1;
        }

        private bool CardsMatch(CardSnapshot card1, CardSnapshot card2)
        {
            return card1.CardSuit == card2.CardSuit &&
                card1.CardFace == card2.CardFace &&
                card1.CardFaceValue == card2.CardFaceValue &&
                card1.CardPointValue == card2.CardPointValue;
        }

        private string GetCardCollectionHash(List<CardSnapshot> cards)
        {
            // Simple hash based on sorted card values
            // In production, use a proper cryptographic hash
            var sortedCards = cards.OrderBy(c => c.CardSuit)
                                .ThenBy(c => c.CardFaceValue)
                                .Select(c => $"{c.CardSuit}{c.CardFaceValue}");
            
            return string.Join(",", sortedCards);
        }

        private void AddError(ValidationErrorType type, string message)
        {
            _errors.Add(new ValidationError(type, message, DateTime.UtcNow));
        }

        #endregion

        #region Public API

        // Gets all validation errors from the last validation operation.
        public List<ValidationError> GetSyncErrors()
        {
            return new List<ValidationError>(_errors);
        }

        // Gets validation errors of a specific type.
        public List<ValidationError> GetErrorsOfType(ValidationErrorType type)
        {
            return _errors.Where(e => e.Type == type).ToList();
        }

        // Checks if there are any critical errors that require immediate resync.
        public bool HasCriticalErrors()
        {
            var criticalTypes = new[]
            {
                ValidationErrorType.StateMismatch,
                ValidationErrorType.ScoreMismatch,
                ValidationErrorType.RoundMismatch,
                ValidationErrorType.PlayerCountMismatch
            };

            return _errors.Any(e => criticalTypes.Contains(e.Type));
        }

        // Clears all validation errors.
        public void ClearErrors()
        {
            _errors.Clear();
        }

        #endregion
    }

    #region Supporting Types

    // Represents a validation error with type, message, and timestamp.
    public class ValidationError
    {
        public ValidationErrorType Type { get; }
        public string Message { get; }
        public DateTime Timestamp { get; }

        public ValidationError(ValidationErrorType type, string message, DateTime timestamp)
        {
            Type = type;
            Message = message;
            Timestamp = timestamp;
        }

        public override string ToString()
        {
            return $"[{Type}] {Message} (at {Timestamp:HH:mm:ss})";
        }
    }

    // Types of validation errors that can occur during sync validation.
    public enum ValidationErrorType
    {
        // Critical errors (require resync)
        NullState,
        StateMismatch,
        RoundMismatch,
        TrickMismatch,
        PlayerCountMismatch,
        ScoreMismatch,

        // State consistency errors
        DealerMismatch,
        TrumpMismatch,
        BettingMismatch,
        TimestampDesync,

        // Player state errors
        HandSizeMismatch,
        CardHashMismatch,
        BettingStateMismatch,
        BidMismatch,

        // Trick state errors
        CardMismatch,
        PlayerIndexMismatch,
        LeadingSuitMismatch,

        // Action validation errors
        InvalidAction,
        InvalidActionForState,
        DuplicateAction,
        InvalidPlayer,
        InvalidBetAmount,
        DuplicateBet,
        InvalidCardIndex,
        EmptyHand,
        MustFollowSuit,
        NotWinningBidder,
        InvalidTrumpSelection,
        UnknownActionType
    }

    // Configuration for validation behavior and tolerances.
    public class ValidationConfig
    {
        // Maximum allowed timestamp difference in seconds before flagging desync.
        public double MaxTimestampDifferenceSeconds { get; set; } = 5.0;

        // Whether to validate card hashes (more expensive but more secure).
        public bool ValidateCardHashes { get; set; } = true;

        // Whether to perform strict validation (fails on any mismatch).
        public bool StrictValidation { get; set; } = true;

        public static ValidationConfig CreateDefault()
        {
            return new ValidationConfig();
        }

        public static ValidationConfig CreateLenient()
        {
            return new ValidationConfig
            {
                MaxTimestampDifferenceSeconds = 10.0,
                ValidateCardHashes = false,
                StrictValidation = false
            };
        }

        public static ValidationConfig CreateStrict()
        {
            return new ValidationConfig
            {
                MaxTimestampDifferenceSeconds = 2.0,
                ValidateCardHashes = true,
                StrictValidation = true
            };
        }
    }

#endregion
}