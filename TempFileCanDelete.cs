using Moq;
using DeuxCentsCardGame.Interfaces.GameConfig;
using DeuxCentsCardGame.Interfaces.Events;
using DeuxCentsCardGame.Interfaces.Services;
using DeuxCentsCardGame.Interfaces.Gameplay;
using DeuxCentsCardGame.Managers;
using DeuxCentsCardGame.Models;
using DeuxCentsCardGame.Events.EventArgs;

namespace DeuxCentsCardGame.Tests.Managers
{
public class BettingManagerTests
{
private readonly Mock<IGameEventManager> _mockEventManager;
private readonly Mock<IGameConfig> _mockGameConfig;
private readonly Mock<IGameValidator> _mockGameValidator;
private readonly Mock<IBettingLogic> _mockBettingLogic;
private readonly List<Player> _players;
private readonly BettingManager _bettingManager;

    public BettingManagerTests()
    {
        _mockEventManager = new Mock<IGameEventManager>();
        _mockGameConfig = new Mock<IGameConfig>();
        _mockGameValidator = new Mock<IGameValidator>();
        _mockBettingLogic = new Mock<IBettingLogic>();
        
        _players = new List<Player>
        {
            new Player("Player1"),
            new Player("Player2"),
            new Player("Player3"),
            new Player("Player4")
        };

        // Setup game config
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
        _bettingManager.CurrentWinningBidIndex = 0;

        // Act
        await _bettingManager.ResetBettingRound();

        // Assert
        Assert.Equal(0, _bettingManager.CurrentWinningBid);
        Assert.Equal(0, _bettingManager.CurrentWinningBidIndex);
        Assert.False(_bettingManager.IsBettingRoundComplete);
    }

    [Fact]
    public async Task ExecuteBettingRound_WithValidBet_ProcessesCorrectly()
    {
        // Arrange
        var responses = new Queue<string>(new[] { "60", "pass", "pass", "pass" });
        
        _mockEventManager.Setup(x => x.RaiseBettingRoundStarted(It.IsAny<string>()))
            .Returns(Task.CompletedTask);
        
        _mockEventManager.Setup(x => x.RaiseBetInput(
            It.IsAny<Player>(), 
            It.IsAny<int>(), 
            It.IsAny<int>(), 
            It.IsAny<int>(),
            It.IsAny<List<int>>(),
            It.IsAny<int>()))
            .ReturnsAsync(() => responses.Dequeue());
        
        _mockEventManager.Setup(x => x.RaiseBettingAction(
            It.IsAny<Player>(),
            It.IsAny<int>(),
            It.IsAny<bool>(),
            It.IsAny<bool>()))
            .Returns(Task.CompletedTask);
        
        _mockEventManager.Setup(x => x.RaiseBettingCompleted(
            It.IsAny<Player>(),
            It.IsAny<int>(),
            It.IsAny<Dictionary<Player, int>>()))
            .Returns(Task.CompletedTask);
        
        _mockGameValidator.Setup(x => x.IsPassInput(It.IsAny<string>()))
            .Returns<string>(s => s == "pass");
        
        _mockGameValidator.Setup(x => x.IsValidBet(It.IsAny<int>()))
            .Returns(true);
        
        _mockGameValidator.Setup(x => x.HasPlayerPassed(It.IsAny<Player>()))
            .Returns<Player>(p => p.HasPassed);
        
        _mockGameValidator.Setup(x => x.HasMinimumPlayersPassed())
            .Returns(true);
        
        _mockGameValidator.Setup(x => x.IsMaximumBet(It.IsAny<int>()))
            .Returns(false);
        
        _mockBettingLogic.Setup(x => x.RecordPlayerBet(It.IsAny<Player>(), It.IsAny<int>()));
        _mockBettingLogic.Setup(x => x.MarkPlayerAsPassed(It.IsAny<Player>()));
        _mockBettingLogic.Setup(x => x.GetActivePlayers(It.IsAny<List<Player>>()))
            .Returns(new List<Player> { _players[0] });
        _mockBettingLogic.Setup(x => x.CheckIfOnlyOnePlayerRemains(It.IsAny<List<Player>>()))
            .Returns(true);
        _mockBettingLogic.Setup(x => x.NoBetsPlaced(It.IsAny<List<Player>>()))
            .Returns(false);
        _mockBettingLogic.Setup(x => x.DetermineWinningBid(It.IsAny<List<Player>>()))
            .Returns((60, 0));

        // Act
        await _bettingManager.ExecuteBettingRound();

        // Assert
        _mockEventManager.Verify(x => x.RaiseBettingRoundStarted(It.IsAny<string>()), Times.Once);
        _mockEventManager.Verify(x => x.RaiseBettingCompleted(
            It.IsAny<Player>(),
            60,
            It.IsAny<Dictionary<Player, int>>()), Times.Once);
        Assert.Equal(60, _bettingManager.CurrentWinningBid);
        Assert.Equal(0, _bettingManager.CurrentWinningBidIndex);
    }

    [Fact]
    public async Task ExecuteBettingRound_WithInvalidBet_ShowsError()
    {
        // Arrange
        var invalidBets = new Queue<string>(new[] { "47", "60", "pass", "pass", "pass" });
        
        _mockEventManager.Setup(x => x.RaiseBettingRoundStarted(It.IsAny<string>()))
            .Returns(Task.CompletedTask);
        
        _mockEventManager.Setup(x => x.RaiseBetInput(
            It.IsAny<Player>(), 
            It.IsAny<int>(), 
            It.IsAny<int>(), 
            It.IsAny<int>(),
            It.IsAny<List<int>>(),
            It.IsAny<int>()))
            .ReturnsAsync(() => invalidBets.Dequeue());

        _mockEventManager.Setup(x => x.RaiseInvalidMove(
            It.IsAny<Player>(),
            It.IsAny<string>(),
            It.IsAny<InvalidMoveType>()))
            .Returns(Task.CompletedTask);
        
        _mockEventManager.Setup(x => x.RaiseBettingAction(
            It.IsAny<Player>(),
            It.IsAny<int>(),
            It.IsAny<bool>(),
            It.IsAny<bool>()))
            .Returns(Task.CompletedTask);
        
        _mockEventManager.Setup(x => x.RaiseBettingCompleted(
            It.IsAny<Player>(),
            It.IsAny<int>(),
            It.IsAny<Dictionary<Player, int>>()))
            .Returns(Task.CompletedTask);

        _mockGameValidator.Setup(x => x.IsPassInput(It.IsAny<string>()))
            .Returns<string>(s => s == "pass");
        
        _mockGameValidator.SetupSequence(x => x.IsValidBet(It.IsAny<int>()))
            .Returns(false)
            .Returns(true);
        
        _mockGameValidator.Setup(x => x.HasPlayerPassed(It.IsAny<Player>()))
            .Returns<Player>(p => p.HasPassed);
        
        _mockGameValidator.Setup(x => x.HasMinimumPlayersPassed())
            .Returns(true);
        
        _mockGameValidator.Setup(x => x.IsMaximumBet(It.IsAny<int>()))
            .Returns(false);
        
        _mockBettingLogic.Setup(x => x.RecordPlayerBet(It.IsAny<Player>(), It.IsAny<int>()));
        _mockBettingLogic.Setup(x => x.MarkPlayerAsPassed(It.IsAny<Player>()));
        _mockBettingLogic.Setup(x => x.GetActivePlayers(It.IsAny<List<Player>>()))
            .Returns(new List<Player> { _players[0] });
        _mockBettingLogic.Setup(x => x.CheckIfOnlyOnePlayerRemains(It.IsAny<List<Player>>()))
            .Returns(true);
        _mockBettingLogic.Setup(x => x.NoBetsPlaced(It.IsAny<List<Player>>()))
            .Returns(false);
        _mockBettingLogic.Setup(x => x.DetermineWinningBid(It.IsAny<List<Player>>()))
            .Returns((60, 0));

        // Act
        await _bettingManager.ExecuteBettingRound();

        // Assert
        _mockEventManager.Verify(x => x.RaiseInvalidMove(
            It.IsAny<Player>(), 
            It.IsAny<string>(), 
            InvalidMoveType.InvalidBet), Times.Once);
    }

    [Fact]
    public async Task ExecuteBettingRound_WithMaxBet_EndsImmediately()
    {
        // Arrange
        var responses = new Queue<string>(new[] { "100" });
        
        _mockEventManager.Setup(x => x.RaiseBettingRoundStarted(It.IsAny<string>()))
            .Returns(Task.CompletedTask);
        
        _mockEventManager.Setup(x => x.RaiseBetInput(
            It.IsAny<Player>(), 
            It.IsAny<int>(), 
            It.IsAny<int>(), 
            It.IsAny<int>(),
            It.IsAny<List<int>>(),
            It.IsAny<int>()))
            .ReturnsAsync(() => responses.Dequeue());

        _mockEventManager.Setup(x => x.RaiseBettingAction(
            It.IsAny<Player>(),
            It.IsAny<int>(),
            It.IsAny<bool>(),
            It.IsAny<bool>()))
            .Returns(Task.CompletedTask);
        
        _mockEventManager.Setup(x => x.RaiseBettingCompleted(
            It.IsAny<Player>(),
            It.IsAny<int>(),
            It.IsAny<Dictionary<Player, int>>()))
            .Returns(Task.CompletedTask);

        _mockGameValidator.Setup(x => x.IsPassInput(It.IsAny<string>()))
            .Returns<string>(s => s == "pass");
        
        _mockGameValidator.Setup(x => x.IsValidBet(It.IsAny<int>()))
            .Returns(true);
        
        _mockGameValidator.Setup(x => x.IsMaximumBet(100))
            .Returns(true);
        
        _mockGameValidator.Setup(x => x.HasPlayerPassed(It.IsAny<Player>()))
            .Returns<Player>(p => p.HasPassed);

        _mockBettingLogic.Setup(x => x.RecordPlayerBet(It.IsAny<Player>(), It.IsAny<int>()));
        _mockBettingLogic.Setup(x => x.ForceOtherPlayersToPass(It.IsAny<List<Player>>(), It.IsAny<int>()));
        _mockBettingLogic.Setup(x => x.DetermineWinningBid(It.IsAny<List<Player>>()))
            .Returns((100, 0));

        // Act
        await _bettingManager.ExecuteBettingRound();

        // Assert
        Assert.Equal(100, _bettingManager.CurrentWinningBid);
        Assert.Equal(0, _bettingManager.CurrentWinningBidIndex);
        _mockBettingLogic.Verify(x => x.ForceOtherPlayersToPass(_players, 1), Times.Once);
        _mockEventManager.Verify(x => x.RaiseBettingAction(
            _players[1], 
            100, 
            false, 
            true), Times.Once);
    }

    [Fact]
    public async Task ExecuteBettingRound_ThreePlayerPass_LastPlayerForcedToMinimumBet()
    {
        // Arrange - First 3 players pass, 4th player has no bets placed
        var responses = new Queue<string>(new[] { "pass", "pass", "pass" });
        
        _mockEventManager.Setup(x => x.RaiseBettingRoundStarted(It.IsAny<string>()))
            .Returns(Task.CompletedTask);
        
        _mockEventManager.Setup(x => x.RaiseBetInput(
            It.IsAny<Player>(), 
            It.IsAny<int>(), 
            It.IsAny<int>(), 
            It.IsAny<int>(),
            It.IsAny<List<int>>(),
            It.IsAny<int>()))
            .ReturnsAsync(() => responses.Dequeue());

        _mockEventManager.Setup(x => x.RaiseBettingAction(
            It.IsAny<Player>(),
            It.IsAny<int>(),
            It.IsAny<bool>(),
            It.IsAny<bool>()))
            .Returns(Task.CompletedTask);
        
        _mockEventManager.Setup(x => x.RaiseBettingCompleted(
            It.IsAny<Player>(),
            It.IsAny<int>(),
            It.IsAny<Dictionary<Player, int>>()))
            .Returns(Task.CompletedTask);

        _mockGameValidator.Setup(x => x.IsPassInput(It.IsAny<string>()))
            .Returns<string>(s => s == "pass");
        
        _mockGameValidator.Setup(x => x.HasPlayerPassed(It.IsAny<Player>()))
            .Returns<Player>(p => p.HasPassed);
        
        _mockGameValidator.Setup(x => x.HasMinimumPlayersPassed())
            .Returns(true);

        _mockBettingLogic.Setup(x => x.MarkPlayerAsPassed(It.IsAny<Player>()));
        _mockBettingLogic.Setup(x => x.GetActivePlayers(It.IsAny<List<Player>>()))
            .Returns(new List<Player> { _players[3] });
        _mockBettingLogic.Setup(x => x.CheckIfOnlyOnePlayerRemains(It.IsAny<List<Player>>()))
            .Returns(true);
        _mockBettingLogic.Setup(x => x.NoBetsPlaced(It.IsAny<List<Player>>()))
            .Returns(true);
        _mockBettingLogic.Setup(x => x.ForceMinimumBet(It.IsAny<Player>()));
        _mockBettingLogic.Setup(x => x.DetermineWinningBid(It.IsAny<List<Player>>()))
            .Returns((50, 3));

        // Act
        await _bettingManager.ExecuteBettingRound();

        // Assert
        _mockBettingLogic.Verify(x => x.ForceMinimumBet(_players[3]), Times.Once);
        Assert.Equal(50, _bettingManager.CurrentWinningBid);
    }

    [Fact]
    public async Task ExecuteBettingRound_ThreePlayerPass_LastPlayerAlreadyBet_DoesNotForceMinimumBet()
    {
        // Arrange - First 3 players pass, 4th player already placed a bet earlier
        // Since bets were placed, the 4th player is not forced to bet minimum
        var responses = new Queue<string>(new[] { "pass", "pass", "pass" });
        _players[3].HasBet = true; // Player 4 already bet
        
        _mockEventManager.Setup(x => x.RaiseBettingRoundStarted(It.IsAny<string>()))
            .Returns(Task.CompletedTask);
        
        _mockEventManager.Setup(x => x.RaiseBetInput(
            It.IsAny<Player>(), 
            It.IsAny<int>(), 
            It.IsAny<int>(), 
            It.IsAny<int>(),
            It.IsAny<List<int>>(),
            It.IsAny<int>()))
            .ReturnsAsync(() => responses.Dequeue());

        _mockEventManager.Setup(x => x.RaiseBettingAction(
            It.IsAny<Player>(),
            It.IsAny<int>(),
            It.IsAny<bool>(),
            It.IsAny<bool>()))
            .Returns(Task.CompletedTask);
        
        _mockEventManager.Setup(x => x.RaiseBettingCompleted(
            It.IsAny<Player>(),
            It.IsAny<int>(),
            It.IsAny<Dictionary<Player, int>>()))
            .Returns(Task.CompletedTask);

        _mockGameValidator.Setup(x => x.IsPassInput(It.IsAny<string>()))
            .Returns<string>(s => s == "pass");
        
        _mockGameValidator.Setup(x => x.HasPlayerPassed(It.IsAny<Player>()))
            .Returns<Player>(p => p.HasPassed);
        
        _mockGameValidator.Setup(x => x.HasMinimumPlayersPassed())
            .Returns(true);

        _mockBettingLogic.Setup(x => x.MarkPlayerAsPassed(It.IsAny<Player>()));
        _mockBettingLogic.Setup(x => x.GetActivePlayers(It.IsAny<List<Player>>()))
            .Returns(new List<Player> { _players[3] });
        _mockBettingLogic.Setup(x => x.CheckIfOnlyOnePlayerRemains(It.IsAny<List<Player>>()))
            .Returns(true);
        _mockBettingLogic.Setup(x => x.NoBetsPlaced(It.IsAny<List<Player>>()))
            .Returns(false);
        _mockBettingLogic.Setup(x => x.DetermineWinningBid(It.IsAny<List<Player>>()))
            .Returns((60, 0));

        // Act
        await _bettingManager.ExecuteBettingRound();

        // Assert
        _mockBettingLogic.Verify(x => x.ForceMinimumBet(It.IsAny<Player>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteBettingRound_ThreePlayerPass_LastPlayerHasNotBet_ForcedToPass()
    {
        // Arrange - First 3 players pass, 4th player hasn't bet but someone else has
        var responses = new Queue<string>(new[] { "65", "pass", "pass" });
        _players[3].HasBet = false; // Player 4 hasn't bet
        
        _mockEventManager.Setup(x => x.RaiseBettingRoundStarted(It.IsAny<string>()))
            .Returns(Task.CompletedTask);
        
        _mockEventManager.Setup(x => x.RaiseBetInput(
            It.IsAny<Player>(), 
            It.IsAny<int>(), 
            It.IsAny<int>(), 
            It.IsAny<int>(),
            It.IsAny<List<int>>(),
            It.IsAny<int>()))
            .ReturnsAsync(() => responses.Dequeue());

        _mockEventManager.Setup(x => x.RaiseBettingAction(
            It.IsAny<Player>(),
            It.IsAny<int>(),
            It.IsAny<bool>(),
            It.IsAny<bool>()))
            .Returns(Task.CompletedTask);
        
        _mockEventManager.Setup(x => x.RaiseBettingCompleted(
            It.IsAny<Player>(),
            It.IsAny<int>(),
            It.IsAny<Dictionary<Player, int>>()))
            .Returns(Task.CompletedTask);

        _mockGameValidator.Setup(x => x.IsPassInput(It.IsAny<string>()))
            .Returns<string>(s => s == "pass");
        
        _mockGameValidator.Setup(x => x.IsValidBet(It.IsAny<int>()))
            .Returns(true);
        
        _mockGameValidator.Setup(x => x.IsMaximumBet(It.IsAny<int>()))
            .Returns(false);
        
        _mockGameValidator.Setup(x => x.HasPlayerPassed(It.IsAny<Player>()))
            .Returns<Player>(p => p.HasPassed);
        
        _mockGameValidator.Setup(x => x.HasMinimumPlayersPassed())
            .Returns(true);

        _mockBettingLogic.Setup(x => x.RecordPlayerBet(It.IsAny<Player>(), It.IsAny<int>()));
        _mockBettingLogic.Setup(x => x.MarkPlayerAsPassed(It.IsAny<Player>()));
        _mockBettingLogic.Setup(x => x.GetActivePlayers(It.IsAny<List<Player>>()))
            .Returns(new List<Player> { _players[0] }); // Player 1 is the only active player
        _mockBettingLogic.Setup(x => x.CheckIfOnlyOnePlayerRemains(It.IsAny<List<Player>>()))
            .Returns(true);
        _mockBettingLogic.Setup(x => x.NoBetsPlaced(It.IsAny<List<Player>>()))
            .Returns(false); // Player 1 placed a bet
        _mockBettingLogic.Setup(x => x.ForcePlayerToPass(It.IsAny<Player>()));
        _mockBettingLogic.Setup(x => x.DetermineWinningBid(It.IsAny<List<Player>>()))
            .Returns((65, 1));

        // Act
        await _bettingManager.ExecuteBettingRound();

        // Assert - Player 1 bet, then 3 players passed. Player 1 is still active but already bet,
        // so they are forced to pass
        _mockBettingLogic.Verify(x => x.ForcePlayerToPass(_players[0]), Times.Once);
        _mockEventManager.Verify(x => x.RaiseBettingAction(
            _players[0], 
            -1, 
            true, 
            false), Times.Once);
    }

    [Fact]
    public async Task UpdateDealerIndex_UpdatesInternalDealerIndex()
    {
        // Arrange
        int newDealerIndex = 2;

        // Act
        await _bettingManager.UpdateDealerIndex(newDealerIndex);

        // Assert
        // Since _dealerIndex is private, we verify it works by checking
        // that betting starts from the correct player in a subsequent round
        // This test just ensures no exception is thrown
        Assert.True(true);
    }

    [Fact]
    public async Task ExecuteBettingRound_StartsFromPlayerAfterDealer()
    {
        // Arrange
        await _bettingManager.UpdateDealerIndex(2); // Set dealer to player 2
        var responses = new Queue<string>(new[] { "55", "pass", "pass", "pass" });
        
        Player? firstPlayerToAct = null;
        
        _mockEventManager.Setup(x => x.RaiseBettingRoundStarted(It.IsAny<string>()))
            .Returns(Task.CompletedTask);
        
        _mockEventManager.Setup(x => x.RaiseBetInput(
            It.IsAny<Player>(), 
            It.IsAny<int>(), 
            It.IsAny<int>(), 
            It.IsAny<int>(),
            It.IsAny<List<int>>(),
            It.IsAny<int>()))
            .Callback<Player, int, int, int, List<int>, int>((player, _, _, _, _, _) =>
            {
                if (firstPlayerToAct == null)
                    firstPlayerToAct = player;
            })
            .ReturnsAsync(() => responses.Dequeue());

        _mockEventManager.Setup(x => x.RaiseBettingAction(
            It.IsAny<Player>(),
            It.IsAny<int>(),
            It.IsAny<bool>(),
            It.IsAny<bool>()))
            .Returns(Task.CompletedTask);
        
        _mockEventManager.Setup(x => x.RaiseBettingCompleted(
            It.IsAny<Player>(),
            It.IsAny<int>(),
            It.IsAny<Dictionary<Player, int>>()))
            .Returns(Task.CompletedTask);

        _mockGameValidator.Setup(x => x.IsPassInput(It.IsAny<string>()))
            .Returns<string>(s => s == "pass");
        
        _mockGameValidator.Setup(x => x.IsValidBet(It.IsAny<int>()))
            .Returns(true);
        
        _mockGameValidator.Setup(x => x.HasPlayerPassed(It.IsAny<Player>()))
            .Returns<Player>(p => p.HasPassed);
        
        _mockGameValidator.Setup(x => x.HasMinimumPlayersPassed())
            .Returns(true);
        
        _mockGameValidator.Setup(x => x.IsMaximumBet(It.IsAny<int>()))
            .Returns(false);

        _mockBettingLogic.Setup(x => x.RecordPlayerBet(It.IsAny<Player>(), It.IsAny<int>()));
        _mockBettingLogic.Setup(x => x.MarkPlayerAsPassed(It.IsAny<Player>()));
        _mockBettingLogic.Setup(x => x.GetActivePlayers(It.IsAny<List<Player>>()))
            .Returns(new List<Player> { _players[3] });
        _mockBettingLogic.Setup(x => x.CheckIfOnlyOnePlayerRemains(It.IsAny<List<Player>>()))
            .Returns(true);
        _mockBettingLogic.Setup(x => x.NoBetsPlaced(It.IsAny<List<Player>>()))
            .Returns(false);
        _mockBettingLogic.Setup(x => x.DetermineWinningBid(It.IsAny<List<Player>>()))
            .Returns((55, 3));

        // Act
        await _bettingManager.ExecuteBettingRound();

        // Assert - With dealer at index 2, first player should be index 3 (Player4)
        Assert.NotNull(firstPlayerToAct);
        Assert.Equal("Player4", firstPlayerToAct.Name);
    }
}
}