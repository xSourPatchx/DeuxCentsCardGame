using DeuxCentsCardGame.Models;
using DeuxCentsCardGame.Core;
using DeuxCentsCardGame.Events;
using DeuxCentsCardGame.Events.EventArgs;

namespace DeuxCentsCardGame.Tests
{
    public class BettingStateTests
    {
        private readonly List<Player> _players;
        private int _dealerIndex;
        private readonly GameEventManager _eventManager;

        public BettingStateTests()
        {
            // _mockUI = new Mock<IUIGameView>();
            _players = new List<Player>
            {
                new Player("Player1"),
                new Player("Player2"),
                new Player("Player3"),
                new Player("Player4")
            };
            _dealerIndex = 3; // Player 4 is dealer, Player 1 starts betting
            _eventManager = new GameEventManager();
        }

        [Fact]
        public void Constructor_ShouldInitializeBettingState()
        {
            // Arrange & Act
            var bettingState = new BettingState(_players, _dealerIndex, _eventManager);

            // Assert
            Assert.Equal(0, bettingState.CurrentWinningBid);
            Assert.Equal(0, bettingState.CurrentWinningBidIndex);
            Assert.False(bettingState.IsBettingRoundComplete);
        }

        [Fact]
        public void BettingConstants_ShouldHaveCorrectValues()
        {
            // Assert
            Assert.Equal(50, BettingState.MinimumBet);
            Assert.Equal(100, BettingState.MaximumBet);
            Assert.Equal(5, BettingState.BetIncrement);
        }

        [Fact]
        public void ResetBettingRound_ShouldResetPlayerBettingStates()
        {
            // Arrange
            var bettingState = new BettingState(_players, _dealerIndex, _eventManager);
            _players.ForEach(p =>
            {
                p.CurrentBid = 50;
                p.HasBet = true;
                p.HasPassed = true;
            });

            // Act
            bettingState.ResetBettingRound();

            // Assert
            Assert.All(_players, p => Assert.Equal(0, p.CurrentBid));
            Assert.All(_players, p => Assert.False(p.HasBet));
            Assert.All(_players, p => Assert.False(p.HasPassed));
            Assert.Equal(0, bettingState.CurrentWinningBid);
            Assert.Equal(0, bettingState.CurrentWinningBidIndex);
            Assert.False(bettingState.IsBettingRoundComplete);
        }

        [Fact]
        public void ResetBettingRound_ShouldHandleDifferentPlayerCounts()
        {
            // Arrange
            var twoPlayers = new List<Player> { new("P1"), new("P2") };
            var bettingState = new BettingState(twoPlayers, 0, _eventManager);

            // Act
            bettingState.ResetBettingRound();

            // Assert
            Assert.All(twoPlayers, p => Assert.Equal(0, p.CurrentBid));
            Assert.All(twoPlayers, p => Assert.False(p.HasBet));
            Assert.All(twoPlayers, p => Assert.False(p.HasPassed));
        }

        [Fact]
        public void ExecuteBettingRound_WithMaximumBet_ShouldEndImmediately()
        {
            // Arrange
            _dealerIndex = 3; // P4 is dealer, P1 (index 0) starts
            var bettingState = new BettingState(_players, _dealerIndex, _eventManager);
            bettingState.ResetBettingRound();

            _eventManager.BetInput += (sender, e) =>
            {
                if (e.CurrentPlayer == _players[0]) e.Response = "100";
            };

            // Act
            bettingState.ExecuteBettingRound();

            // Assert
            Assert.True(bettingState.IsBettingRoundComplete);
            Assert.Equal(100, bettingState.CurrentWinningBid);
            Assert.Equal(0, bettingState.CurrentWinningBidIndex); // Player1 index
            Assert.Equal(100, _players[0].CurrentBid);
            Assert.True(_players[0].HasBet);

            // Other players should be marked as passed
            Assert.True(_players[1].HasPassed);
            Assert.True(_players[2].HasPassed);
            Assert.True(_players[3].HasPassed);
        }

        [Fact]
        public void ExecuteBettingRound_WithThreePasses_ShouldForceLastPlayerToBet()
        {
            // Arrange
            _dealerIndex = 3; // P4 is dealer, P1 (idx 0) starts
            var bettingState = new BettingState(_players, _dealerIndex, _eventManager);
            bettingState.ResetBettingRound();

            var betQueue = new Queue<string>(new[] { "pass", "pass", "pass" });
            _eventManager.BetInput += (sender, e) => { e.Response = betQueue.Dequeue(); };

            // Act
            bettingState.ExecuteBettingRound();

            // Assert
            Assert.True(bettingState.IsBettingRoundComplete);
            Assert.Equal(50, bettingState.CurrentWinningBid); // Minimum bet
            Assert.Equal(3, bettingState.CurrentWinningBidIndex); // Player4 (dealer) is forced to bet
            Assert.Equal(50, _players[3].CurrentBid);
            Assert.True(_players[3].HasBet);
        }

        [Fact]
        public void ExecuteBettingRound_WithValidBets_ShouldDetermineWinner()
        {
            // Arrange
            _dealerIndex = 3; // P4 dealer, P1 (idx 0) starts
            var bettingState = new BettingState(_players, _dealerIndex, _eventManager);
            bettingState.ResetBettingRound();

            var betQueue = new Queue<string>(new[] { "55", "70", "pass", "pass" });
            _eventManager.BetInput += (sender, e) => {
                if (betQueue.Any()) e.Response = betQueue.Dequeue();
            };

            // Act
            bettingState.ExecuteBettingRound();

            // Assert
            Assert.True(bettingState.IsBettingRoundComplete);
            Assert.Equal(70, bettingState.CurrentWinningBid);
            Assert.Equal(1, bettingState.CurrentWinningBidIndex); // Player2 index
        }

        [Theory]
        [InlineData("45")] // Below minimum
        [InlineData("105")] // Above maximum
        [InlineData("52")] // Not increment of 5
        [InlineData("abc")] // Invalid format
        [InlineData("")] // Empty string
        public void ExecuteBettingRound_WithInvalidBets_ShouldPromptRetry(string invalidBet)
        {
            // Arrange
            _dealerIndex = 3; // P1 (idx 0) starts
            var bettingState = new BettingState(_players, _dealerIndex, _eventManager);
            bettingState.ResetBettingRound();

            var betQueue = new Queue<string>(new[] { invalidBet, "50", "pass", "pass", "pass" });
            _eventManager.BetInput += (sender, e) => { e.Response = betQueue.Dequeue(); };

            int invalidBetCount = 0;
            _eventManager.InvalidBet += (sender, e) => { invalidBetCount++; };

            // Act
            bettingState.ExecuteBettingRound();

            // Assert
            Assert.Equal(1, invalidBetCount);
            Assert.Equal(50, _players[0].CurrentBid); // Valid bet was accepted
        }

        [Fact]
        public void ExecuteBettingRound_WithDuplicateBets_ShouldRejectDuplicate()
        {
            // Arrange
            _dealerIndex = 3; // P1 (idx 0) starts
            var bettingState = new BettingState(_players, _dealerIndex, _eventManager);
            bettingState.ResetBettingRound();

            var betQueue = new Queue<string>(new[] { "60", "60", "65", "pass", "pass" });
            _eventManager.BetInput += (sender, e) => { e.Response = betQueue.Dequeue(); };

            int invalidBetCount = 0;
            _eventManager.InvalidBet += (sender, e) => { invalidBetCount++; };

            // Act
            bettingState.ExecuteBettingRound();

            // Assert
            Assert.Equal(1, invalidBetCount);
            Assert.Equal(60, _players[0].CurrentBid);
            Assert.Equal(65, _players[1].CurrentBid);
            Assert.Equal(65, bettingState.CurrentWinningBid);
        }

        [Fact]
        public void ExecuteBettingRound_ShouldSkipPlayersWhoPassed()
        {
            // Arrange
            _dealerIndex = 3; // P1 (idx 0) starts
            var bettingState = new BettingState(_players, _dealerIndex, _eventManager);
            bettingState.ResetBettingRound();

            // P1 bets 55, P2 passes, P3 bets 70, P4 passes.
            // Round continues. P1 passes. Round ends. P3 wins.
            var betQueue = new Queue<string>(new[] { "55", "pass", "70", "pass", "pass" });
            _eventManager.BetInput += (sender, e) => {
                if (betQueue.Any()) e.Response = betQueue.Dequeue();
            };

            // Act
            bettingState.ExecuteBettingRound();

            // Assert
            Assert.True(_players[1].HasPassed); // Player2 passed
            Assert.Equal(70, bettingState.CurrentWinningBid);
            Assert.Equal(2, bettingState.CurrentWinningBidIndex); // Player3 won
        }

        [Fact]
        public void IsBettingRoundComplete_ShouldBePrivateSet()
        {
            // Assert
            var property = typeof(BettingState).GetProperty("IsBettingRoundComplete");
            Assert.NotNull(property);
            Assert.True(property.CanRead);
            Assert.False(property.SetMethod?.IsPublic ?? false);
        }

        [Fact]
        public void ExecuteBettingRound_ShouldSetCompleteFlagCorrectly()
        {
            // Arrange
            _dealerIndex = 3; // P1 starts
            var bettingState = new BettingState(_players, _dealerIndex, _eventManager);
            bettingState.ResetBettingRound();

            _eventManager.BetInput += (sender, e) => { e.Response = "100"; };

            // Act
            Assert.False(bettingState.IsBettingRoundComplete); // Before execution
            bettingState.ExecuteBettingRound();

            // Assert
            Assert.True(bettingState.IsBettingRoundComplete); // After execution
        }

        [Fact]
        public void PlayerBids_ShouldTrackNegativeOneForPasses()
        {
            // Arrange
            _dealerIndex = 3; // P1 (idx 0) starts
            var bettingState = new BettingState(_players, _dealerIndex, _eventManager);
            bettingState.ResetBettingRound();

            var betQueue = new Queue<string>(new[] { "pass", "50", "pass", "pass" });
            _eventManager.BetInput += (sender, e) => { e.Response = betQueue.Dequeue(); };

            // Act
            bettingState.ExecuteBettingRound();

            // Assert
            Assert.Equal(-1, _players[0].CurrentBid); // Player1 passed
            Assert.Equal(50, _players[1].CurrentBid); // Player2 bet
            Assert.Equal(-1, _players[2].CurrentBid); // Player3 passed
            Assert.Equal(-1, _players[3].CurrentBid); // Player4 passed
            Assert.Equal(50, bettingState.CurrentWinningBid);
        }
    }
}