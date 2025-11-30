using DeuxCentsCardGame.Controllers;
using DeuxCentsCardGame.GameStates;
using DeuxCentsCardGame.Interfaces.Managers;
using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Services
{
    // Restores game state from snapshots.
    // Handles player state, scores, betting, and game flow restoration.
    public class GameStateRestorer : IGameStateRestorer
    {
        private readonly IPlayerManager _playerManager;
        private readonly IScoringManager _scoringManager;
        private readonly IBettingManager _bettingManager;
        private readonly IPlayerTurnManager _playerTurnManager;

        public GameStateRestorer(
            IPlayerManager playerManager,
            IScoringManager scoringManager,
            IBettingManager bettingManager,
            IPlayerTurnManager playerTurnManager)
        {
            _playerManager = playerManager ?? throw new ArgumentNullException(nameof(playerManager));
            _scoringManager = scoringManager ?? throw new ArgumentNullException(nameof(scoringManager));
            _bettingManager = bettingManager ?? throw new ArgumentNullException(nameof(bettingManager));
            _playerTurnManager = playerTurnManager ?? throw new ArgumentNullException(nameof(playerTurnManager));
        }

        // Restores complete game state from a snapshot.
        public bool RestoreGameState(GameSnapshot snapshot, GameController gameController)
        {
            if (snapshot == null || !snapshot.IsValid())
                return false;

            try
            {
                // Restore in order of dependency
                RestorePlayers(snapshot.Players);
                RestoreScores(snapshot.Scores);
                RestoreBetting(snapshot.Betting);
                RestoreGameStateData(snapshot, gameController);
                RestoreRoundState(snapshot, gameController);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error restoring game state: {ex.Message}");
                return false;
            }
        }

        // Restores player state including hands and betting status.
        public bool RestorePlayers(List<PlayerSnapshot> playerSnapshots)
        {
            if (playerSnapshots == null || playerSnapshots.Count != _playerManager.Players.Count)
                return false;

            try
            {
                for (int i = 0; i < playerSnapshots.Count; i++)
                {
                    var playerSnapshot = playerSnapshots[i];
                    var player = _playerManager.GetPlayer(i);

                    // Clear existing hand
                    player.Hand.Clear();

                    // Restore hand
                    foreach (var cardSnapshot in playerSnapshot.Hand)
                    {
                        player.AddCard(cardSnapshot.ToCard());
                    }

                    // Restore betting state
                    player.HasBet = playerSnapshot.HasBet;
                    player.HasPassed = playerSnapshot.HasPassed;
                    player.CurrentBid = playerSnapshot.CurrentBid;

                    // Note: PlayerType is set at initialization and shouldn't change
                    // If you need to change it, uncomment:
                    // player.PlayerType = playerSnapshot.PlayerType;
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error restoring players: {ex.Message}");
                return false;
            }
        }

        // Restores scoring state.
        public bool RestoreScores(ScoreSnapshot scoreSnapshot)
        {
            if (scoreSnapshot == null)
                return false;

            try
            {
                // Note: This uses reflection to set private fields
                // In production, you might want to add SetScore methods to IScoringManager
                
                var scoringManagerType = _scoringManager.GetType();
                
                SetPrivateProperty(scoringManagerType, _scoringManager, "TeamOneRoundPoints", scoreSnapshot.TeamOneRoundPoints);
                SetPrivateProperty(scoringManagerType, _scoringManager, "TeamTwoRoundPoints", scoreSnapshot.TeamTwoRoundPoints);
                SetPrivateProperty(scoringManagerType, _scoringManager, "TeamOneTotalPoints", scoreSnapshot.TeamOneTotalPoints);
                SetPrivateProperty(scoringManagerType, _scoringManager, "TeamTwoTotalPoints", scoreSnapshot.TeamTwoTotalPoints);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error restoring scores: {ex.Message}");
                return false;
            }
        }

        // Restores betting state.
        public bool RestoreBetting(BettingSnapshot bettingSnapshot)
        {
            if (bettingSnapshot == null)
                return false;

            try
            {
                var bettingManagerType = _bettingManager.GetType();

                SetPrivateProperty(bettingManagerType, _bettingManager, "CurrentWinningBid", bettingSnapshot.CurrentWinningBid);
                SetPrivateProperty(bettingManagerType, _bettingManager, "CurrentWinningBidIndex", bettingSnapshot.CurrentWinningBidIndex);
                SetPrivateProperty(bettingManagerType, _bettingManager, "IsBettingRoundComplete", bettingSnapshot.IsBettingRoundComplete);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error restoring betting: {ex.Message}");
                return false;
            }
        }

        // Restores game state data (state machine, round, trick).
        private void RestoreGameStateData(GameSnapshot snapshot, GameController gameController)
        {
            // Access the private _gameStateData field
            var controllerType = gameController.GetType();
            var gameStateDataField = controllerType.GetField("_gameStateData", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (gameStateDataField != null)
            {
                var gameStateData = gameStateDataField.GetValue(gameController) as GameStateData;
                if (gameStateData != null)
                {
                    gameStateData.CurrentState = snapshot.CurrentState;
                    gameStateData.PreviousState = snapshot.PreviousState;
                    gameStateData.CurrentRound = snapshot.RoundNumber;
                    gameStateData.CurrentTrick = snapshot.TrickNumber;
                    gameStateData.IsPaused = snapshot.IsPaused;
                }
            }
        }

        // Restores round-specific state (dealer, trump).
        private void RestoreRoundState(GameSnapshot snapshot, GameController gameController)
        {
            // Restore dealer index
            var controllerType = gameController.GetType();
            var dealerField = controllerType.GetField("DealerIndex",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            
            if (dealerField != null)
            {
                dealerField.SetValue(gameController, snapshot.DealerIndex);
            }

            // Restore trump suit in RoundController
            var roundControllerField = controllerType.GetField("_roundController",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (roundControllerField != null)
            {
                var roundController = roundControllerField.GetValue(gameController);
                if (roundController != null)
                {
                    var trumpProperty = roundController.GetType().GetProperty("TrumpSuit");
                    if (trumpProperty != null && trumpProperty.CanWrite)
                    {
                        trumpProperty.SetValue(roundController, snapshot.TrumpSuit);
                    }
                }
            }

            // Restore player turn if in playing state
            if (snapshot.CurrentState == GameState.Playing && snapshot.CurrentTrick != null)
            {
                _playerTurnManager.InitializeTurnSequence(snapshot.CurrentTrick.StartingPlayerIndex);
                _playerTurnManager.SetCurrentPlayer(snapshot.CurrentTrick.CurrentPlayerIndex);
            }
        }

        // Helper method to set private properties using reflection.
        private void SetPrivateProperty(Type type, object instance, string propertyName, object value)
        {
            var property = type.GetProperty(propertyName);
            if (property != null && property.CanWrite)
            {
                property.SetValue(instance, value);
            }
            else
            {
                // Try as field if not a property
                var field = type.GetField($"_{char.ToLower(propertyName[0])}{propertyName.Substring(1)}",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                
                if (field != null)
                {
                    field.SetValue(instance, value);
                }
            }
        }
    }
}