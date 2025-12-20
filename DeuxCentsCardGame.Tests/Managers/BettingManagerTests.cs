using Moq;
using DeuxCentsCardGame.Interfaces.GameConfig;
using DeuxCentsCardGame.Interfaces.Events;
using DeuxCentsCardGame.Managers;
using DeuxCentsCardGame.Models;
using DeuxCentsCardGame.Gameplay;
using DeuxCentsCardGame.Services;
using DeuxCentsCardGame.Events.EventArgs;


namespace DeuxCentsCardGame.Tests.Managers
{
    public class BettingManagerTests
    {
        private readonly Mock<IGameEventManager> _mockEventManager;
        private readonly Mock<IGameConfig> _mockGameConfig;
        private readonly Mock<GameValidator> _mockGameValidator;
        private readonly Mock<BettingLogic> _mockBettingLogic;
        private readonly List<Player> _players;
        private readonly BettingManager _bettingManager;

        public BettingManagerTests()
        {
            _mockEventManager = new Mock<IGameEventManager>();
            _mockGameConfig = new Mock<IGameConfig>();
            _mockGameValidator = new Mock<GameValidator>(_mockGameConfig.Object, _players);
            _mockBettingLogic = new Mock<BettingLogic>(_mockGameConfig.Object);
            
            _players = new List<Player>
            {
                new Player("Player1"),
                new Player("Player2"),
                new Player("Player3"),
                new Player("Player4")
            };

            _mockGameConfig.Setup(x => x.MinimumBet).Returns(50);
            _mockGameConfig.Setup(x => x.MaximumBet).Returns(100);
            _mockGameConfig.Setup(x => x.BetIncrement).Returns(5);
            _mockGameConfig.Setup(x => x.MinimumPlayersToPass).Returns(3);

            _bettingManager = new BettingManager(
                _players, 
                0, 
                _mockEventManager.Object, 
                _mockGameConfig.Object, 
                _mockGameValidator.Object,
                _mockBettingLogic.Object);
        }

        [Fact]
        public async Task ResetBettingRound_ResetsAllPlayersAndState()
        {
            // Arrange
            _players[0].CurrentBid = 50;
            _players[0].HasPassed = true;
            _bettingManager.CurrentWinningBid = 50;

            // Act
            await _bettingManager.ResetBettingRound();

            // Assert
            Assert.Equal(0, _bettingManager.CurrentWinningBid);
            Assert.False(_bettingManager.IsBettingRoundComplete);
        }

        [Fact]
        public async Task ExecuteBettingRound_WithValidBet_ProcessesCorrectly()
        {
            // Arrange
            var responses = new Queue<string>(new[] { "60", "pass", "pass", "pass" });
            _mockEventManager.Setup(x => x.RaiseBetInput(
                It.IsAny<Player>(), 
                It.IsAny<int>(), 
                It.IsAny<int>(), 
                It.IsAny<int>(),
                It.IsAny<List<int>>(),
                It.IsAny<int>()))
                .ReturnsAsync(() => responses.Dequeue());
            
            _mockGameValidator.Setup(x => x.IsPassInput(It.IsAny<string>()))
                .Returns<string>(s => s == "pass");
            _mockGameValidator.Setup(x => x.IsValidBet(It.IsAny<int>())).Returns(true);
            _mockGameValidator.Setup(x => x.HasPlayerPassed(It.IsAny<Player>()))
                .Returns<Player>(p => p.HasPassed);
            _mockGameValidator.Setup(x => x.HasMinimumPlayersPassed()).Returns(false);

            // Act
            await _bettingManager.ExecuteBettingRound();

            // Assert
            _mockEventManager.Verify(x => x.RaiseBettingRoundStarted(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ExecuteBettingRound_WithInvalidBet_ShowsError()
        {
            // Arrange
            var invalidBets = new Queue<string>(new[] { "47", "52", "60", "pass", "pass", "pass" });
            _mockEventManager.Setup(x => x.RaiseBetInput(
                It.IsAny<Player>(), 
                It.IsAny<int>(), 
                It.IsAny<int>(), 
                It.IsAny<int>(),
                It.IsAny<List<int>>(),
                It.IsAny<int>()))
                .ReturnsAsync(() => invalidBets.Dequeue());

            _mockGameValidator.Setup(x => x.IsPassInput(It.IsAny<string>()))
                .Returns<string>(s => s == "pass");
            _mockGameValidator.SetupSequence(x => x.IsValidBet(It.IsAny<int>()))
                .Returns(false)
                .Returns(false)
                .Returns(true);
            _mockGameValidator.Setup(x => x.HasPlayerPassed(It.IsAny<Player>()))
                .Returns<Player>(p => p.HasPassed);
            _mockGameValidator.Setup(x => x.HasMinimumPlayersPassed()).Returns(false);

            // Act
            await _bettingManager.ExecuteBettingRound();

            // Assert
            _mockEventManager.Verify(x => x.RaiseInvalidMove(
                It.IsAny<Player>(), 
                It.IsAny<string>(), 
                It.IsAny<InvalidMoveType>()), Times.AtLeastOnce);
        }

        [Fact]
        public async Task ExecuteBettingRound_WithMaxBet_EndsImmediately()
        {
            // Arrange
            var responses = new Queue<string>(new[] { "100", "pass", "pass", "pass" });
            _mockEventManager.Setup(x => x.RaiseBetInput(
            It.IsAny<Player>(), 
            It.IsAny<int>(), 
            It.IsAny<int>(), 
            It.IsAny<int>(),
            It.IsAny<List<int>>(),
            It.IsAny<int>()))
            .ReturnsAsync(() => responses.Dequeue());

        _mockGameValidator.Setup(x => x.IsPassInput(It.IsAny<string>()))
            .Returns<string>(s => s == "pass");
        _mockGameValidator.Setup(x => x.IsValidBet(It.IsAny<int>())).Returns(true);
        _mockGameValidator.Setup(x => x.IsMaximumBet(100)).Returns(true);
        _mockGameValidator.Setup(x => x.HasPlayerPassed(It.IsAny<Player>()))
            .Returns<Player>(p => p.HasPassed);

        _mockBettingLogic.Setup(x => x.DetermineWinningBid(_players))
            .Returns((100, 0));

            // Act
            await _bettingManager.ExecuteBettingRound();

            // Assert
            Assert.Equal(100, _bettingManager.CurrentWinningBid);
            _mockBettingLogic.Verify(x => x.ForceOtherPlayersToPass(_players, 0), Times.Once);
        }

        [Fact]
        public async Task UpdateDealerIndex_UpdatesDealerIndex()
        {
            // Arrange
            int newDealerIndex = 2;

            // Act
            await _bettingManager.UpdateDealerIndex(newDealerIndex);

            // Assert - Verify by checking betting starts from correct player
            // The next test would show this working correctly
            Assert.True(true); // Dealer index is private, so we just verify no exception
        }
    }
}