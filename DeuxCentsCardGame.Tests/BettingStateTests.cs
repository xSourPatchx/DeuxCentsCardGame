using DeuxCentsCardGame.Interfaces;
using DeuxCentsCardGame.Models;
using DeuxCentsCardGame.Core;
using DeuxCentsCardGame.Events;
using Moq;

namespace DeuxCentsCardGame.Tests
{
    public class BettingStateTests
    {
        private readonly Mock<IUIGameView> _mockUI;
        private readonly List<Player> _players;
        private readonly int _dealerIndex;
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
            _dealerIndex = 0;
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
            Assert.Null(bettingState.PlayerBids);
            Assert.Null(bettingState.PlayerHasBet);
            Assert.Null(bettingState.PlayerHasPassed);
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
        public void ResetBettingRound_ShouldInitializeAllLists()
        {
            // Arrange
            var bettingState = new BettingState(_players, _dealerIndex, _eventManager);

            // Act
            bettingState.ResetBettingRound();

            // Assert
            Assert.Equal(4, bettingState.PlayerBids.Count);
            Assert.Equal(4, bettingState.PlayerHasBet.Count);
            Assert.Equal(4, bettingState.PlayerHasPassed.Count);
            Assert.All(bettingState.PlayerBids, bid => Assert.Equal(0, bid));
            Assert.All(bettingState.PlayerHasBet, hasBet => Assert.False(hasBet));
            Assert.All(bettingState.PlayerHasPassed, hasPassed => Assert.False(hasPassed));
            Assert.Equal(0, bettingState.CurrentWinningBid);
            Assert.Equal(0, bettingState.CurrentWinningBidIndex);
            Assert.False(bettingState.IsBettingRoundComplete);
        }

        [Fact]
        public void ResetBettingRound_ShouldHandleDifferentPlayerCounts()
        {
            // Arrange
            var twoPlayers = new List<Player> { new Player("P1"), new Player("P2") };
            var bettingState = new BettingState(twoPlayers, 0, _eventManager);

            // Act
            bettingState.ResetBettingRound();

            // Assert
            Assert.Equal(2, bettingState.PlayerBids.Count);
            Assert.Equal(2, bettingState.PlayerHasBet.Count);
            Assert.Equal(2, bettingState.PlayerHasPassed.Count);
        }

        [Fact]
        public void ExecuteBettingRound_WithMaximumBet_ShouldEndImmediately()
        {
            // Arrange
            var bettingState = new BettingState(_players, _dealerIndex, _eventManager);
            bettingState.ResetBettingRound();

            _mockUI.SetupSequence(ui => ui.GetUserInput(It.IsAny<string>()))
                   .Returns("100"); // Player2 (after dealer) bets maximum

            // Act
            bettingState.ExecuteBettingRound();

            // Assert
            Assert.True(bettingState.IsBettingRoundComplete);
            Assert.Equal(100, bettingState.CurrentWinningBid);
            Assert.Equal(1, bettingState.CurrentWinningBidIndex); // Player2 index
            Assert.Equal(100, bettingState.PlayerBids[1]);
            Assert.True(bettingState.PlayerHasBet[1]);
            
            // Other players should be marked as passed
            Assert.True(bettingState.PlayerHasPassed[0]);
            Assert.True(bettingState.PlayerHasPassed[2]);
            Assert.True(bettingState.PlayerHasPassed[3]);
        }

        [Fact]
        public void ExecuteBettingRound_WithThreePasses_ShouldForceMinimumBet()
        {
            // Arrange
            var bettingState = new BettingState(_players, _dealerIndex, _eventManager);
            bettingState.ResetBettingRound();

            _mockUI.SetupSequence(ui => ui.GetUserInput(It.IsAny<string>()))
                   .Returns("pass") // Player2 passes
                   .Returns("pass") // Player3 passes
                   .Returns("pass"); // Player4 passes

            // Act
            bettingState.ExecuteBettingRound();

            // Assert
            Assert.True(bettingState.IsBettingRoundComplete);
            Assert.Equal(50, bettingState.CurrentWinningBid); // Minimum bet
            Assert.Equal(0, bettingState.CurrentWinningBidIndex); // Player1 (dealer + 1) gets forced bet
            Assert.Equal(50, bettingState.PlayerBids[0]);
            Assert.True(bettingState.PlayerHasBet[0]);
        }

        [Fact]
        public void ExecuteBettingRound_WithValidBets_ShouldDetermineWinner()
        {
            // Arrange
            var bettingState = new BettingState(_players, _dealerIndex, _eventManager);
            bettingState.ResetBettingRound();

            _mockUI.SetupSequence(ui => ui.GetUserInput(It.IsAny<string>()))
                   .Returns("55")   // Player2 bets 55
                   .Returns("70")   // Player3 bets 70
                   .Returns("pass") // Player4 passes
                   .Returns("pass") // Player1 passes
                   .Returns("pass") // Player2 passes (second round)
                   .Returns("pass"); // Player3 passes (second round)

            // Act
            bettingState.ExecuteBettingRound();

            // Assert
            Assert.True(bettingState.IsBettingRoundComplete);
            Assert.Equal(70, bettingState.CurrentWinningBid);
            Assert.Equal(2, bettingState.CurrentWinningBidIndex); // Player3 index
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
            var bettingState = new BettingState(_players, _dealerIndex, _eventManager);
            bettingState.ResetBettingRound();

            _mockUI.SetupSequence(ui => ui.GetUserInput(It.IsAny<string>()))
                   .Returns(invalidBet) // Invalid bet first
                   .Returns("50")       // Valid bet second
                   .Returns("pass")     // Player3 passes
                   .Returns("pass")     // Player4 passes
                   .Returns("pass");    // Player1 passes

            // Act
            bettingState.ExecuteBettingRound();

            // Assert
            _mockUI.Verify(ui => ui.DisplayMessage("Invalid bet, please try again"), Times.Once);
            Assert.Equal(50, bettingState.PlayerBids[1]); // Valid bet was accepted
        }

        [Fact]
        public void ExecuteBettingRound_WithDuplicateBets_ShouldRejectDuplicate()
        {
            // Arrange
            var bettingState = new BettingState(_players, _dealerIndex, _eventManager);
            bettingState.ResetBettingRound();

            _mockUI.SetupSequence(ui => ui.GetUserInput(It.IsAny<string>()))
                   .Returns("60")   // Player2 bets 60
                   .Returns("60")   // Player3 tries to bet 60 (duplicate)
                   .Returns("65")   // Player3 bets 65 (valid)
                   .Returns("pass") // Player4 passes
                   .Returns("pass") // Player1 passes
                   .Returns("pass") // Player2 passes
                   .Returns("pass"); // Player3 passes

            // Act
            bettingState.ExecuteBettingRound();

            // Assert
            _mockUI.Verify(ui => ui.DisplayMessage("Invalid bet, please try again"), Times.Once);
            Assert.Equal(-1, bettingState.PlayerBids[1]);
            Assert.Equal(65, bettingState.PlayerBids[2]);
        }

        [Fact]
        public void ExecuteBettingRound_ShouldStartWithPlayerAfterDealer()
        {
            // Arrange
            var dealerIndex = 2; // Player3 is dealer
            var bettingState = new BettingState(_players, dealerIndex, _eventManager);
            bettingState.ResetBettingRound();

            _mockUI.Setup(ui => ui.GetUserInput(It.IsAny<string>()))
                   .Returns("pass");

            // Act
            bettingState.ExecuteBettingRound();

            // Assert - First prompt should be for Player4 (index 3), then Player1 (index 0)
            _mockUI.Verify(ui => ui.GetUserInput(It.Is<string>(s => s.Contains("Player4"))), Times.AtLeastOnce);
        }

        [Fact]
        public void ExecuteBettingRound_ShouldSkipPlayersWhoPassed()
        {
            // Arrange
            var bettingState = new BettingState(_players, _dealerIndex, _eventManager);
            bettingState.ResetBettingRound();

            _mockUI.SetupSequence(ui => ui.GetUserInput(It.IsAny<string>()))
                   .Returns("55")   // Player2 bets 55
                   .Returns("pass") // Player3 passes
                   .Returns("70")   // Player4 bets 70
                   .Returns("pass") // Player1 passes
                   .Returns("pass") // Player2 passes (second round)
                   // Player3 should be skipped in second round
                   .Returns("pass"); // Player4 passes (second round)

            // Act
            bettingState.ExecuteBettingRound();

            // Assert
            Assert.True(bettingState.PlayerHasPassed[2]); // Player3 passed
            Assert.Equal(70, bettingState.CurrentWinningBid); // Player4 won
        }

        [Fact]
        public void ExecuteBettingRound_ShouldDisplayCorrectMessages()
        {
            // Arrange
            var bettingState = new BettingState(_players, _dealerIndex, _eventManager);
            bettingState.ResetBettingRound();

            _mockUI.Setup(ui => ui.GetUserInput(It.IsAny<string>()))
                   .Returns("100"); // Maximum bet to end quickly

            // Act
            bettingState.ExecuteBettingRound();

            // Assert
            _mockUI.Verify(ui => ui.DisplayMessage("Betting round begins!\n"), Times.Once);
            _mockUI.Verify(ui => ui.DisplayMessage("\nBetting round complete, here are the results:"), Times.Once);
            _mockUI.Verify(ui => ui.DisplayFormattedMessage(It.Is<string>(s => s.Contains("won the bid")), It.IsAny<object>()), Times.Once);
        }

        [Fact]
        public void ExecuteBettingRound_ShouldHandlePassAfterBetting()
        {
            // Arrange
            var bettingState = new BettingState(_players, _dealerIndex, _eventManager);
            bettingState.ResetBettingRound();

            _mockUI.SetupSequence(ui => ui.GetUserInput(It.IsAny<string>()))
                   .Returns("55")   // Player2 bets 55
                   .Returns("70")   // Player3 bets 70
                   .Returns("pass") // Player4 passes
                   .Returns("pass") // Player1 passes
                   .Returns("pass") // Player2 passes after betting
                   .Returns("pass"); // Player3 passes after betting

            // Act
            bettingState.ExecuteBettingRound();

            // Assert
            Assert.True(bettingState.PlayerHasBet[1]); // Player2 bet
            Assert.True(bettingState.PlayerHasPassed[1]); // Player2 also passed
            Assert.Equal(70, bettingState.CurrentWinningBid);
        }

        // [Fact]
        // public void ExecuteBettingRound_WithTwoPlayersOnly_ShouldWork()
        // {
        //     // Arrange
        //     var twoPlayers = new List<Player> { new Player("P1"), new Player("P2") };
        //     var bettingState = new BettingState(twoPlayers, _mockUI.Object, 0);
        //     bettingState.ResetBettingRound();

        //     _mockUI.SetupSequence(ui => ui.GetUserInput(It.IsAny<string>()))
        //            .Returns("60")   // P2 bets 60
        //            .Returns("pass"); // P1 passes

        //     // Act
        //     bettingState.ExecuteBettingRound();

        //     // Assert
        //     Assert.True(bettingState.IsBettingRoundComplete);
        //     Assert.Equal(60, bettingState.CurrentWinningBid);
        //     Assert.Equal(1, bettingState.CurrentWinningBidIndex);
        // }

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
            var bettingState = new BettingState(_players, _dealerIndex, _eventManager);
            bettingState.ResetBettingRound();

            _mockUI.Setup(ui => ui.GetUserInput(It.IsAny<string>()))
                   .Returns("100"); // Maximum bet

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
            var bettingState = new BettingState(_players, _dealerIndex, _eventManager);
            bettingState.ResetBettingRound();

            _mockUI.SetupSequence(ui => ui.GetUserInput(It.IsAny<string>()))
                   .Returns("pass") // Player2 passes
                   .Returns("50")   // Player3 bets 50
                   .Returns("pass") // Player4 passes
                   .Returns("pass"); // Player1 passes

            // Act
            bettingState.ExecuteBettingRound();

            // Assert
            Assert.Equal(-1, bettingState.PlayerBids[1]); // Player2 passed
            Assert.Equal(50, bettingState.PlayerBids[2]); // Player3 bet
            Assert.Equal(-1, bettingState.PlayerBids[3]); // Player4 passed
            Assert.Equal(-1, bettingState.PlayerBids[0]); // Player1 passed
        }
    }
}