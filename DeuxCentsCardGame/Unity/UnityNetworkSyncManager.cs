using DeuxCentsCardGame.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeuxCentsCardGame.Unity
{
    // Manages network synchronization for multiplayer games.
    // This is a reference implementation - adapt to your Unity networking solution.
    public class UnityNetworkSyncManager
    {
        private readonly INetworkSyncService _networkService;
        private GameSnapshot? _lastSyncedState;
        private readonly Queue<PlayerAction> _actionQueue = new();

        public event EventHandler<GameSnapshot>? OnStateSync;
        public event EventHandler<PlayerAction>? OnPlayerAction;

        public UnityNetworkSyncManager(INetworkSyncService networkService)
        {
            _networkService = networkService ?? throw new ArgumentNullException(nameof(networkService));
            
            // Subscribe to network events
            _networkService.GameStateReceived += OnGameStateReceived;
            _networkService.PlayerActionReceived += OnPlayerActionReceived;
        }

        #region State Synchronization

        // Broadcasts current game state to all connected players.
        // Call this after significant game state changes (end of trick, round, etc.)
        public async Task SyncGameStateAsync(GameSnapshot snapshot)
        {
            if (!_networkService.IsHost)
            {
                Console.WriteLine("Only host can broadcast game state");
                return;
            }

            try
            {
                await _networkService.BroadcastGameStateAsync(snapshot);
                _lastSyncedState = snapshot;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error syncing game state: {ex.Message}");
            }
        }

        // Requests current game state from host.
        // Call this when joining a game in progress.
        public async Task RequestStateAsync()
        {
            if (_networkService.IsHost)
            {
                Console.WriteLine("Host doesn't need to request state");
                return;
            }

            try
            {
                await _networkService.RequestGameStateAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error requesting game state: {ex.Message}");
            }
        }

        // Checks if state needs to be synced based on time elapsed.
        public bool ShouldSyncState(TimeSpan timeSinceLastSync, TimeSpan syncInterval)
        {
            return timeSinceLastSync >= syncInterval;
        }

        #endregion

        #region Player Actions

        // Sends a player action to be processed.
        public async Task SendActionAsync(PlayerAction action)
        {
            if (!_networkService.IsConnected)
            {
                Console.WriteLine("Not connected to network");
                return;
            }

            try
            {
                await _networkService.SendPlayerActionAsync(action);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending action: {ex.Message}");
            }
        }

        // Creates a betting action.
        public PlayerAction CreateBettingAction(string playerId, int bet)
        {
            return new PlayerAction
            {
                ActionId = Guid.NewGuid().ToString(),
                PlayerId = playerId,
                ActionType = PlayerActionType.Bet,
                BetAmount = bet,
                Timestamp = DateTime.UtcNow
            };
        }

        // Creates a pass action.
        public PlayerAction CreatePassAction(string playerId)
        {
            return new PlayerAction
            {
                ActionId = Guid.NewGuid().ToString(),
                PlayerId = playerId,
                ActionType = PlayerActionType.Pass,
                Timestamp = DateTime.UtcNow
            };
        }

        // Creates a card play action.
        public PlayerAction CreateCardPlayAction(string playerId, CardSnapshot card, int cardIndex)
        {
            return new PlayerAction
            {
                ActionId = Guid.NewGuid().ToString(),
                PlayerId = playerId,
                ActionType = PlayerActionType.PlayCard,
                CardPlayed = card,
                CardIndex = cardIndex,
                Timestamp = DateTime.UtcNow
            };
        }

        // Creates a trump selection action.
        public PlayerAction CreateTrumpSelectionAction(string playerId, CardSuit trumpSuit)
        {
            return new PlayerAction
            {
                ActionId = Guid.NewGuid().ToString(),
                PlayerId = playerId,
                ActionType = PlayerActionType.SelectTrump,
                TrumpSuit = trumpSuit,
                Timestamp = DateTime.UtcNow
            };
        }

        #endregion

        #region Event Handlers

        private void OnGameStateReceived(object? sender, GameSnapshot snapshot)
        {
            if (snapshot == null || !snapshot.IsValid())
                return;

            _lastSyncedState = snapshot;
            OnStateSync?.Invoke(this, snapshot);
        }

        private void OnPlayerActionReceived(object? sender, PlayerAction action)
        {
            if (action == null || !action.IsValid())
                return;

            _actionQueue.Enqueue(action);
            OnPlayerAction?.Invoke(this, action);
        }

        #endregion

        #region Action Queue Processing

        // Gets the next pending action from the queue.
        public PlayerAction? GetNextAction()
        {
            return _actionQueue.Count > 0 ? _actionQueue.Dequeue() : null;
        }

        // Checks if there are pending actions.
        public bool HasPendingActions()
        {
            return _actionQueue.Count > 0;
        }

        // Clears all pending actions.
        public void ClearActionQueue()
        {
            _actionQueue.Clear();
        }

        #endregion

        #region Validation

        // Validates if an action can be executed based on current state.
        public bool ValidateAction(PlayerAction action, GameSnapshot currentState)
        {
            if (action == null || currentState == null)
                return false;

            // Find the player
            var playerIndex = currentState.Players.FindIndex(p => p.Name == action.PlayerId);
            if (playerIndex == -1)
                return false;

            // Validate based on action type and game state
            return action.ActionType switch
            {
                PlayerActionType.Bet => currentState.CurrentState == GameStates.GameState.Betting,
                PlayerActionType.Pass => currentState.CurrentState == GameStates.GameState.Betting,
                PlayerActionType.PlayCard => currentState.CurrentState == GameStates.GameState.Playing,
                PlayerActionType.SelectTrump => currentState.CurrentState == GameStates.GameState.TrumpSelection,
                _ => false
            };
        }

        #endregion
    }

    #region Supporting Classes

    // Represents a player action for network synchronization.
    [Serializable]
    public class PlayerAction
    {
        public string ActionId { get; set; } = string.Empty;
        public string PlayerId { get; set; } = string.Empty;
        public PlayerActionType ActionType { get; set; }
        public DateTime Timestamp { get; set; }
        
        // Betting
        public int BetAmount { get; set; }
        
        // Card play
        public CardSnapshot? CardPlayed { get; set; }
        public int CardIndex { get; set; }
        
        // Trump selection
        public CardSuit? TrumpSuit { get; set; }

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(ActionId) 
                && !string.IsNullOrWhiteSpace(PlayerId)
                && Timestamp != default;
        }

        // Serializes action to byte array for network transmission.
        public byte[] Serialize()
        {
            var options = new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = false
            };
            string json = System.Text.Json.JsonSerializer.Serialize(this, options);
            return System.Text.Encoding.UTF8.GetBytes(json);
        }

        // Deserializes action from byte array.
        public static PlayerAction? Deserialize(byte[] data)
        {
            if (data == null || data.Length == 0)
                return null;

            try
            {
                string json = System.Text.Encoding.UTF8.GetString(data);
                return System.Text.Json.JsonSerializer.Deserialize<PlayerAction>(json);
            }
            catch
            {
                return null;
            }
        }
    }

    // Types of player actions that can be synchronized.
    public enum PlayerActionType
    {
        Bet,
        Pass,
        PlayCard,
        SelectTrump,
        CutDeck
    }

    #endregion

    #region Unity Coroutine Helper

    // Helper for Unity coroutines to sync state periodically.
    // Add this to a MonoBehaviour in Unity.
    public class UnitySyncHelper
    {
        private readonly UnityNetworkSyncManager _syncManager;
        private DateTime _lastSyncTime;
        private readonly TimeSpan _syncInterval;

        public UnitySyncHelper(UnityNetworkSyncManager syncManager, float syncIntervalSeconds = 1.0f)
        {
            _syncManager = syncManager;
            _syncInterval = TimeSpan.FromSeconds(syncIntervalSeconds);
            _lastSyncTime = DateTime.UtcNow;
        }

        // Call this in Unity's Update() method.
        public async Task UpdateAsync(GameSnapshot currentSnapshot)
        {
            var timeSinceLastSync = DateTime.UtcNow - _lastSyncTime;

            if (_syncManager.ShouldSyncState(timeSinceLastSync, _syncInterval))
            {
                await _syncManager.SyncGameStateAsync(currentSnapshot);
                _lastSyncTime = DateTime.UtcNow;
            }
        }
    }

    #endregion
}