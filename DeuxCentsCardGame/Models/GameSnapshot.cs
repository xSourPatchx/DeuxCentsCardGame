using DeuxCentsCardGame.GameStates;
using System.Text.Json;

namespace DeuxCentsCardGame.Models
{
    // Captures complete game state for save/load, network sync, and replay functionality.
    // Serializable for network transmission and file storage.
    [Serializable]
    public class GameSnapshot
    {
        // Core game state
        public GameState CurrentState { get; set; }
        public GameState PreviousState { get; set; }
        public int RoundNumber { get; set; }
        public int TrickNumber { get; set; }
        public int DealerIndex { get; set; }
        public CardSuit? TrumpSuit { get; set; }
        public bool IsPaused { get; set; }
        public DateTime Timestamp { get; set; }

        // Player state
        public List<PlayerSnapshot> Players { get; set; } = new();

        // Score state
        public ScoreSnapshot Scores { get; set; } = new();

        // Betting state
        public BettingSnapshot Betting { get; set; } = new();

        // Current trick state (for mid-trick snapshots)
        public TrickSnapshot? CurrentTrick { get; set; }

        // Metadata
        public string SnapshotId { get; set; } = Guid.NewGuid().ToString();
        public string? Description { get; set; }
        public int Version { get; set; } = 1; // For future compatibility

        // Default constructor for deserialization.
        public GameSnapshot()
        {
            Timestamp = DateTime.UtcNow;
        }

        // Creates a snapshot from current game state.
        public GameSnapshot(
            GameState currentState,
            GameState previousState,
            int roundNumber,
            int trickNumber,
            int dealerIndex,
            CardSuit? trumpSuit,
            bool isPaused,
            List<Player> players,
            ScoreSnapshot scores,
            BettingSnapshot betting,
            TrickSnapshot? currentTrick = null,
            string? description = null)
        {
            CurrentState = currentState;
            PreviousState = previousState;
            RoundNumber = roundNumber;
            TrickNumber = trickNumber;
            DealerIndex = dealerIndex;
            TrumpSuit = trumpSuit;
            IsPaused = isPaused;
            Timestamp = DateTime.UtcNow;
            Description = description;

            // Create player snapshots
            Players = players.Select(p => new PlayerSnapshot(p)).ToList();

            Scores = scores;
            Betting = betting;
            CurrentTrick = currentTrick;
        }

        // Serializes the snapshot to a byte array using JSON.
        public byte[] Serialize()
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = false,
                PropertyNameCaseInsensitive = true
            };

            string json = JsonSerializer.Serialize(this, options);
            return System.Text.Encoding.UTF8.GetBytes(json);
        }

        // Deserializes a snapshot from a byte array.
        public static GameSnapshot? Deserialize(byte[] data)
        {
            if (data == null || data.Length == 0)
                return null;

            try
            {
                string json = System.Text.Encoding.UTF8.GetString(data);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                return JsonSerializer.Deserialize<GameSnapshot>(json, options);
            }
            catch (Exception ex)
            {
                // Log error in production
                Console.WriteLine($"Failed to deserialize GameSnapshot: {ex.Message}");
                return null;
            }
        }

        // Serializes to JSON string (human-readable).
        public string SerializeToJson()
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNameCaseInsensitive = true
            };

            return JsonSerializer.Serialize(this, options);
        }

        // Deserializes from JSON string.
        public static GameSnapshot? DeserializeFromJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return null;

            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                return JsonSerializer.Deserialize<GameSnapshot>(json, options);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to deserialize GameSnapshot from JSON: {ex.Message}");
                return null;
            }
        }

        // Creates a clone of this snapshot with a new ID and timestamp.
        public GameSnapshot Clone()
        {
            var json = SerializeToJson();
            var clone = DeserializeFromJson(json);
            
            if (clone != null)
            {
                clone.SnapshotId = Guid.NewGuid().ToString();
                clone.Timestamp = DateTime.UtcNow;
            }

            return clone ?? this;
        }

        // Validates the snapshot for consistency.
        public bool IsValid()
        {
            // Check required fields
            if (Players == null || Players.Count != 4)
                return false;

            if (Scores == null)
                return false;

            if (Betting == null)
                return false;

            if (DealerIndex < 0 || DealerIndex >= 4)
                return false;

            if (RoundNumber < 1)
                return false;

            return true;
        }
    }

    // Captures the state of a single player.
    [Serializable]
    public class PlayerSnapshot
    {
        public string Name { get; set; } = string.Empty;
        public PlayerType PlayerType { get; set; }
        public List<CardSnapshot> Hand { get; set; } = new();
        public bool HasBet { get; set; }
        public bool HasPassed { get; set; }
        public int CurrentBid { get; set; }

        // Default constructor for deserialization.
        public PlayerSnapshot() { }

        // Creates a snapshot from a player.
        public PlayerSnapshot(Player player)
        {
            Name = player.Name;
            PlayerType = player.PlayerType;
            Hand = player.Hand.Select(c => new CardSnapshot(c)).ToList();
            HasBet = player.HasBet;
            HasPassed = player.HasPassed;
            CurrentBid = player.CurrentBid;
        }

        // Converts snapshot back to a Player object.
        public Player ToPlayer()
        {
            var player = new Player(Name, PlayerType)
            {
                HasBet = HasBet,
                HasPassed = HasPassed,
                CurrentBid = CurrentBid
            };

            // Restore hand
            foreach (var cardSnapshot in Hand)
            {
                player.AddCard(cardSnapshot.ToCard());
            }

            return player;
        }
    }

    // Captures the state of a single card.
    [Serializable]
    public class CardSnapshot
    {
        public CardSuit CardSuit { get; set; }
        public CardFace CardFace { get; set; }
        public int CardFaceValue { get; set; }
        public int CardPointValue { get; set; }

        // Default constructor for deserialization.
        public CardSnapshot() { }

        // Creates a snapshot from a card.
        public CardSnapshot(Card card)
        {
            CardSuit = card.CardSuit;
            CardFace = card.CardFace;
            CardFaceValue = card.CardFaceValue;
            CardPointValue = card.CardPointValue;
        }

        // Converts snapshot back to a Card object.
        public Card ToCard()
        {
            return new Card(CardSuit, CardFace, CardFaceValue, CardPointValue);
        }
    }

    // Captures the scoring state.
    [Serializable]
    public class ScoreSnapshot
    {
        public int TeamOneRoundPoints { get; set; }
        public int TeamTwoRoundPoints { get; set; }
        public int TeamOneTotalPoints { get; set; }
        public int TeamTwoTotalPoints { get; set; }

        // Default constructor for deserialization.
        public ScoreSnapshot() { }

        // Creates a snapshot from current scores.
        public ScoreSnapshot(
            int teamOneRoundPoints,
            int teamTwoRoundPoints,
            int teamOneTotalPoints,
            int teamTwoTotalPoints)
        {
            TeamOneRoundPoints = teamOneRoundPoints;
            TeamTwoRoundPoints = teamTwoRoundPoints;
            TeamOneTotalPoints = teamOneTotalPoints;
            TeamTwoTotalPoints = teamTwoTotalPoints;
        }
    }

    // Captures the betting state.
    [Serializable]
    public class BettingSnapshot
    {
        public int CurrentWinningBid { get; set; }
        public int CurrentWinningBidIndex { get; set; }
        public bool IsBettingRoundComplete { get; set; }
        public List<int> PlayerBids { get; set; } = new(); // Index corresponds to player

        // Default constructor for deserialization.
        public BettingSnapshot() { }

        // Creates a snapshot from betting state.
        public BettingSnapshot(
            int currentWinningBid,
            int currentWinningBidIndex,
            bool isBettingRoundComplete,
            List<Player> players)
        {
            CurrentWinningBid = currentWinningBid;
            CurrentWinningBidIndex = currentWinningBidIndex;
            IsBettingRoundComplete = isBettingRoundComplete;
            PlayerBids = players.Select(p => p.CurrentBid).ToList();
        }
    }

    // Captures the state of the current trick in progress.
    [Serializable]
    public class TrickSnapshot
    {
        public List<TrickCardSnapshot> CardsPlayed { get; set; } = new();
        public CardSuit? LeadingSuit { get; set; }
        public int CurrentPlayerIndex { get; set; }
        public int StartingPlayerIndex { get; set; }

        // Default constructor for deserialization.
        public TrickSnapshot() { }

        // Creates a snapshot from current trick state.
        public TrickSnapshot(
            List<(Card card, Player player)> cardsPlayed,
            CardSuit? leadingSuit,
            int currentPlayerIndex,
            int startingPlayerIndex,
            List<Player> allPlayers)
        {
            CardsPlayed = cardsPlayed.Select(cp => new TrickCardSnapshot
            {
                Card = new CardSnapshot(cp.card),
                PlayerIndex = allPlayers.IndexOf(cp.player)
            }).ToList();

            LeadingSuit = leadingSuit;
            CurrentPlayerIndex = currentPlayerIndex;
            StartingPlayerIndex = startingPlayerIndex;
        }
    }

    // Captures a card played in a trick with the player who played it.
    [Serializable]
    public class TrickCardSnapshot
    {
        public CardSnapshot Card { get; set; } = new();
        public int PlayerIndex { get; set; }
    }

    // Builder pattern for creating GameSnapshots.
    public class GameSnapshotBuilder
    {
        private readonly GameSnapshot _snapshot = new();

        public GameSnapshotBuilder WithGameState(GameState current, GameState previous)
        {
            _snapshot.CurrentState = current;
            _snapshot.PreviousState = previous;
            return this;
        }

        public GameSnapshotBuilder WithRound(int roundNumber, int trickNumber)
        {
            _snapshot.RoundNumber = roundNumber;
            _snapshot.TrickNumber = trickNumber;
            return this;
        }

        public GameSnapshotBuilder WithDealer(int dealerIndex)
        {
            _snapshot.DealerIndex = dealerIndex;
            return this;
        }

        public GameSnapshotBuilder WithTrump(CardSuit? trumpSuit)
        {
            _snapshot.TrumpSuit = trumpSuit;
            return this;
        }

        public GameSnapshotBuilder WithPaused(bool isPaused)
        {
            _snapshot.IsPaused = isPaused;
            return this;
        }

        public GameSnapshotBuilder WithPlayers(List<Player> players)
        {
            _snapshot.Players = players.Select(p => new PlayerSnapshot(p)).ToList();
            return this;
        }

        public GameSnapshotBuilder WithScores(ScoreSnapshot scores)
        {
            _snapshot.Scores = scores;
            return this;
        }

        public GameSnapshotBuilder WithBetting(BettingSnapshot betting)
        {
            _snapshot.Betting = betting;
            return this;
        }

        public GameSnapshotBuilder WithCurrentTrick(TrickSnapshot? trick)
        {
            _snapshot.CurrentTrick = trick;
            return this;
        }

        public GameSnapshotBuilder WithDescription(string description)
        {
            _snapshot.Description = description;
            return this;
        }

        public GameSnapshot Build()
        {
            if (!_snapshot.IsValid())
                throw new InvalidOperationException("GameSnapshot is not valid. Check required fields.");

            return _snapshot;
        }
    }
}