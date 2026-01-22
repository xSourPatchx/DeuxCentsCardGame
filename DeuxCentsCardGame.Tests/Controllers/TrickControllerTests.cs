using DeuxCentsCardGame.Controllers;
using DeuxCentsCardGame.Interfaces.Events;
using DeuxCentsCardGame.Interfaces.Gameplay;
using DeuxCentsCardGame.Interfaces.Managers;
using DeuxCentsCardGame.Interfaces.Services;
using DeuxCentsCardGame.Models;
using Moq;

namespace DeuxCentsCardGame.Tests.Controllers
{
    public class TrickControllerTests
    {
        private readonly Mock<IGameEventManager> _mockEventManager;
        private readonly Mock<IPlayerManager> _mockPlayerManager;
        private readonly Mock<IPlayerTurnManager> _mockPlayerTurnManager;
        private readonly Mock<IScoringManager> _mockScoringManager;
        private readonly Mock<ICardLogic> _mockCardComparer;
        private readonly Mock<IGameValidator> _mockGameValidator;
        private readonly TrickController _trickController;
        private readonly List<Player> _testPlayers;

        public TrickControllerTests()
        {
            _mockEventManager = new Mock<IGameEventManager>();
            _mockPlayerManager = new Mock<IPlayerManager>();
            _mockPlayerTurnManager = new Mock<IPlayerTurnManager>();
            _mockScoringManager = new Mock<IScoringManager>();
            _mockCardComparer = new Mock<ICardLogic>();
            _mockGameValidator = new Mock<IGameValidator>();

            _testPlayers = new List<Player>
            {
                new Player("Player 1"),
                new Player("Player 2"),
                new Player("Player 3"),
                new Player("Player 4")
            };

            _mockPlayerManager.Setup(m => m.Players).Returns(_testPlayers.AsReadOnly());
            _mockPlayerManager.Setup(m => m.GetPlayer(It.IsAny<int>()))
                .Returns<int>(index => _testPlayers[index]);
            _mockPlayerTurnManager.Setup(m => m.GetTurnOrder())
                .Returns(new List<int> { 0, 1, 2, 3 });
            _mockGameValidator.Setup(m => m.GetValidCardFromPlayer(It.IsAny<Player>(), It.IsAny<CardSuit?>(), It.IsAny<CardSuit?>()))
                .ReturnsAsync(new Card(CardSuit.Hearts, CardFace.Five, 1, 5));
            _mockCardComparer.Setup(m => m.WinsAgainst(It.IsAny<Card>(), It.IsAny<Card>(), It.IsAny<CardSuit?>(), It.IsAny<CardSuit?>()))
                .Returns(false);

            _mockEventManager.Setup(m => m.RaisePlayerTurn(It.IsAny<Player>(), It.IsAny<CardSuit?>(), It.IsAny<CardSuit?>(), It.IsAny<int>())).Returns(Task.CompletedTask);
            _mockEventManager.Setup(m => m.RaiseCardPlayed(It.IsAny<Player>(), It.IsAny<Card>(), It.IsAny<int>(), It.IsAny<CardSuit?>(), It.IsAny<CardSuit?>())).Returns(Task.CompletedTask);
            _mockEventManager.Setup(m => m.RaiseTrickCompleted(It.IsAny<int>(), It.IsAny<Player>(), It.IsAny<Card>(), It.IsAny<List<(Card, Player)>>(), It.IsAny<int>())).Returns(Task.CompletedTask);

            _trickController = new TrickController(
                _mockEventManager.Object,
                _mockPlayerManager.Object,
                _mockPlayerTurnManager.Object,
                _mockScoringManager.Object,
                _mockCardComparer.Object,
                _mockGameValidator.Object
            );
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_WithValidDependencies_InitializesSuccessfully()
        {
            Assert.NotNull(_trickController);
        }

        [Fact]
        public void Constructor_WithNullEventManager_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new TrickController(
                null!,
                _mockPlayerManager.Object,
                _mockPlayerTurnManager.Object,
                _mockScoringManager.Object,
                _mockCardComparer.Object,
                _mockGameValidator.Object
            ));
        }

        [Fact]
        public void Constructor_WithNullPlayerManager_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new TrickController(
                _mockEventManager.Object,
                null!,
                _mockPlayerTurnManager.Object,
                _mockScoringManager.Object,
                _mockCardComparer.Object,
                _mockGameValidator.Object
            ));
        }

        [Fact]
        public void Constructor_WithNullPlayerTurnManager_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new TrickController(
                _mockEventManager.Object,
                _mockPlayerManager.Object,
                null!,
                _mockScoringManager.Object,
                _mockCardComparer.Object,
                _mockGameValidator.Object
            ));
        }

        [Fact]
        public void Constructor_WithNullScoringManager_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new TrickController(
                _mockEventManager.Object,
                _mockPlayerManager.Object,
                _mockPlayerTurnManager.Object,
                null!,
                _mockCardComparer.Object,
                _mockGameValidator.Object
            ));
        }

        [Fact]
        public void Constructor_WithNullCardComparer_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new TrickController(
                _mockEventManager.Object,
                _mockPlayerManager.Object,
                _mockPlayerTurnManager.Object,
                _mockScoringManager.Object,
                null!,
                _mockGameValidator.Object
            ));
        }

        [Fact]
        public void Constructor_WithNullGameValidator_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new TrickController(
                _mockEventManager.Object,
                _mockPlayerManager.Object,
                _mockPlayerTurnManager.Object,
                _mockScoringManager.Object,
                _mockCardComparer.Object,
                null!
            ));
        }

        #endregion

        #region PlayAllTricks Tests

        [Fact]
        public async Task PlayAllTricks_InitializesTurnSequence()
        {
            SetupPlayersWithCards(5);

            await _trickController.PlayAllTricks(0, CardSuit.Hearts);

            _mockPlayerTurnManager.Verify(m => m.InitializeTurnSequence(0), Times.Once);
        }

        [Fact]
        public async Task PlayAllTricks_PlaysCorrectNumberOfTricks()
        {
            SetupPlayersWithCards(10);

            await _trickController.PlayAllTricks(0, CardSuit.Hearts);

            _mockEventManager.Verify(m => m.RaiseTrickCompleted(
                It.IsAny<int>(),
                It.IsAny<Player>(),
                It.IsAny<Card>(),
                It.IsAny<List<(Card, Player)>>(),
                It.IsAny<int>()), Times.Exactly(10));
        }

        [Fact]
        public async Task PlayAllTricks_PlaysAllCardsFromAllPlayers()
        {
            SetupPlayersWithCards(8);

            await _trickController.PlayAllTricks(0, CardSuit.Hearts);

            // 8 tricks * 4 players = 32 cards played
            _mockEventManager.Verify(m => m.RaiseCardPlayed(
                It.IsAny<Player>(),
                It.IsAny<Card>(),
                It.IsAny<int>(),
                It.IsAny<CardSuit?>(),
                It.IsAny<CardSuit?>()), Times.Exactly(32));
        }

        [Fact]
        public async Task PlayAllTricks_StartsWithCorrectPlayer()
        {
            SetupPlayersWithCards(5);
            _mockPlayerTurnManager.Setup(m => m.GetTurnOrder())
                .Returns(new List<int> { 2, 3, 0, 1 });

            await _trickController.PlayAllTricks(2, CardSuit.Hearts);

            _mockPlayerTurnManager.Verify(m => m.InitializeTurnSequence(2), Times.Once);
        }

        #endregion

        #region ExecuteSingleTrick Tests

        [Fact]
        public async Task ExecuteSingleTrick_RaisesPlayerTurnForEachPlayer()
        {
            SetupPlayersWithCards(5);

            await _trickController.ExecuteSingleTrick(0, CardSuit.Hearts);

            _mockEventManager.Verify(m => m.RaisePlayerTurn(
                It.IsAny<Player>(),
                It.IsAny<CardSuit?>(),
                It.IsAny<CardSuit?>(),
                0), Times.Exactly(4));
        }

        [Fact]
        public async Task ExecuteSingleTrick_GetValidCardFromEachPlayer()
        {
            SetupPlayersWithCards(5);

            await _trickController.ExecuteSingleTrick(0, CardSuit.Hearts);

            _mockGameValidator.Verify(m => m.GetValidCardFromPlayer(
                It.IsAny<Player>(),
                It.IsAny<CardSuit?>(),
                CardSuit.Hearts), Times.Exactly(4));
        }

        [Fact]
        public async Task ExecuteSingleTrick_RemovesPlayedCardFromPlayer()
        {
            SetupPlayersWithCards(5);
            var testCard = new Card(CardSuit.Spades, CardFace.King, 4, 13);
            _mockGameValidator.Setup(m => m.GetValidCardFromPlayer(It.IsAny<Player>(), It.IsAny<CardSuit?>(), It.IsAny<CardSuit?>()))
                .ReturnsAsync(testCard);

            var initialHandSize = _testPlayers[0].Hand.Count;

            await _trickController.ExecuteSingleTrick(0, CardSuit.Hearts);

            Assert.Equal(initialHandSize - 1, _testPlayers[0].Hand.Count);
        }

        [Fact]
        public async Task ExecuteSingleTrick_RaisesCardPlayedForEachPlayer()
        {
            SetupPlayersWithCards(5);

            await _trickController.ExecuteSingleTrick(0, CardSuit.Hearts);

            _mockEventManager.Verify(m => m.RaiseCardPlayed(
                It.IsAny<Player>(),
                It.IsAny<Card>(),
                0,
                It.IsAny<CardSuit?>(),
                CardSuit.Hearts), Times.Exactly(4));
        }

        [Fact]
        public async Task ExecuteSingleTrick_DeterminesTrickWinner()
        {
            SetupPlayersWithCards(5);

            await _trickController.ExecuteSingleTrick(0, CardSuit.Hearts);

            _mockCardComparer.Verify(m => m.WinsAgainst(
                It.IsAny<Card>(),
                It.IsAny<Card>(),
                CardSuit.Hearts,
                It.IsAny<CardSuit?>()), Times.AtLeastOnce);
        }

        [Fact]
        public async Task ExecuteSingleTrick_AwardsTrickPoints()
        {
            SetupPlayersWithCards(5);

            await _trickController.ExecuteSingleTrick(0, CardSuit.Hearts);

            _mockScoringManager.Verify(m => m.AwardTrickPoints(
                It.IsAny<int>(),
                It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task ExecuteSingleTrick_RaisesTrickCompletedEvent()
        {
            SetupPlayersWithCards(5);

            await _trickController.ExecuteSingleTrick(0, CardSuit.Hearts);

            _mockEventManager.Verify(m => m.RaiseTrickCompleted(
                0,
                It.IsAny<Player>(),
                It.IsAny<Card>(),
                It.IsAny<List<(Card, Player)>>(),
                It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task ExecuteSingleTrick_UpdatesCurrentPlayerToTrickWinner()
        {
            SetupPlayersWithCards(5);
            _mockPlayerManager.Setup(m => m.Players).Returns(_testPlayers.AsReadOnly());

            await _trickController.ExecuteSingleTrick(0, CardSuit.Hearts);

            _mockPlayerTurnManager.Verify(m => m.SetCurrentPlayer(It.IsAny<int>()), Times.Once);
        }

        #endregion

        #region Leading Suit Tests

        [Fact]
        public async Task ExecuteSingleTrick_FirstCardSetsLeadingSuit()
        {
            SetupPlayersWithCards(5);
            var leadCard = new Card(CardSuit.Diamonds, CardFace.Queen, 3, 12);
            _mockGameValidator.SetupSequence(m => m.GetValidCardFromPlayer(It.IsAny<Player>(), It.IsAny<CardSuit?>(), It.IsAny<CardSuit?>()))
                .ReturnsAsync(leadCard)
                .ReturnsAsync(new Card(CardSuit.Hearts, CardFace.Five, 1, 5))
                .ReturnsAsync(new Card(CardSuit.Hearts, CardFace.Six, 1, 6))
                .ReturnsAsync(new Card(CardSuit.Hearts, CardFace.Seven, 1, 7));

            await _trickController.ExecuteSingleTrick(0, CardSuit.Hearts);

            _mockEventManager.Verify(m => m.RaiseCardPlayed(
                It.IsAny<Player>(),
                It.IsAny<Card>(),
                It.IsAny<int>(),
                CardSuit.Diamonds,
                It.IsAny<CardSuit?>()), Times.AtLeast(3));
        }

        [Fact]
        public async Task ExecuteSingleTrick_SubsequentPlayersFollowLeadingSuit()
        {
            SetupPlayersWithCards(5);
            var leadCard = new Card(CardSuit.Clubs, CardFace.Ace, 4, 14);
            _mockGameValidator.SetupSequence(m => m.GetValidCardFromPlayer(It.IsAny<Player>(), It.IsAny<CardSuit?>(), It.IsAny<CardSuit?>()))
                .ReturnsAsync(leadCard)
                .ReturnsAsync(new Card(CardSuit.Clubs, CardFace.Five, 1, 5))
                .ReturnsAsync(new Card(CardSuit.Clubs, CardFace.Six, 1, 6))
                .ReturnsAsync(new Card(CardSuit.Clubs, CardFace.Seven, 1, 7));

            await _trickController.ExecuteSingleTrick(0, CardSuit.Hearts);

            _mockGameValidator.Verify(m => m.GetValidCardFromPlayer(
                It.IsAny<Player>(),
                CardSuit.Clubs,
                It.IsAny<CardSuit?>()), Times.Exactly(3));
        }

        #endregion

        #region Trick Winner Determination Tests

        [Fact]
        public async Task ExecuteSingleTrick_FirstPlayerWinsByDefault()
        {
            SetupPlayersWithCards(5);
            _mockCardComparer.Setup(m => m.WinsAgainst(It.IsAny<Card>(), It.IsAny<Card>(), It.IsAny<CardSuit?>(), It.IsAny<CardSuit?>()))
                .Returns(false);

            await _trickController.ExecuteSingleTrick(0, CardSuit.Hearts);

            _mockScoringManager.Verify(m => m.AwardTrickPoints(0, It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task ExecuteSingleTrick_CorrectPlayerWinsWhenComparerReturnsTrue()
        {
            SetupPlayersWithCards(5);
            _mockCardComparer.SetupSequence(m => m.WinsAgainst(It.IsAny<Card>(), It.IsAny<Card>(), It.IsAny<CardSuit?>(), It.IsAny<CardSuit?>()))
                .Returns(false)  // Player 1 doesn't win
                .Returns(true)   // Player 2 wins
                .Returns(false); // Player 3 doesn't win

            await _trickController.ExecuteSingleTrick(0, CardSuit.Hearts);

            _mockScoringManager.Verify(m => m.AwardTrickPoints(1, It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task ExecuteSingleTrick_LastPlayerCanWinTrick()
        {
            SetupPlayersWithCards(5);
            _mockCardComparer.SetupSequence(m => m.WinsAgainst(It.IsAny<Card>(), It.IsAny<Card>(), It.IsAny<CardSuit?>(), It.IsAny<CardSuit?>()))
                .Returns(false)
                .Returns(false)
                .Returns(true);

            await _trickController.ExecuteSingleTrick(0, CardSuit.Hearts);

            _mockScoringManager.Verify(m => m.AwardTrickPoints(3, It.IsAny<int>()), Times.Once);
        }

        #endregion

        #region Trick Points Calculation Tests

        [Fact]
        public async Task ExecuteSingleTrick_CalculatesCorrectTrickPoints()
        {
            SetupPlayersWithCards(5);
            var cards = new[]
            {
                new Card(CardSuit.Hearts, CardFace.Five, 1, 5),
                new Card(CardSuit.Hearts, CardFace.Ten, 1, 10),
                new Card(CardSuit.Hearts, CardFace.King, 4, 13),
                new Card(CardSuit.Hearts, CardFace.Ace, 4, 14)
            };

            var cardIndex = 0;
            _mockGameValidator.Setup(m => m.GetValidCardFromPlayer(It.IsAny<Player>(), It.IsAny<CardSuit?>(), It.IsAny<CardSuit?>()))
                .ReturnsAsync(() => cards[cardIndex++]);

            await _trickController.ExecuteSingleTrick(0, CardSuit.Hearts);

            // 5 + 10 + 13 + 14 = 42 points
            _mockScoringManager.Verify(m => m.AwardTrickPoints(It.IsAny<int>(), 42), Times.Once);
        }

        [Fact]
        public async Task ExecuteSingleTrick_ZeroPointsWhenNoPointCards()
        {
            SetupPlayersWithCards(5);
            var cards = new[]
            {
                new Card(CardSuit.Hearts, CardFace.Six, 0, 6),
                new Card(CardSuit.Hearts, CardFace.Seven, 0, 7),
                new Card(CardSuit.Hearts, CardFace.Eight, 0, 8),
                new Card(CardSuit.Hearts, CardFace.Nine, 0, 9)
            };

            var cardIndex = 0;
            _mockGameValidator.Setup(m => m.GetValidCardFromPlayer(It.IsAny<Player>(), It.IsAny<CardSuit?>(), It.IsAny<CardSuit?>()))
                .ReturnsAsync(() => cards[cardIndex++]);

            await _trickController.ExecuteSingleTrick(0, CardSuit.Hearts);

            _mockScoringManager.Verify(m => m.AwardTrickPoints(It.IsAny<int>(), 0), Times.Once);
        }

        #endregion

        #region Turn Order Tests

        [Fact]
        public async Task ExecuteSingleTrick_FollowsCorrectTurnOrder()
        {
            SetupPlayersWithCards(5);
            _mockPlayerTurnManager.Setup(m => m.GetTurnOrder())
                .Returns(new List<int> { 2, 3, 0, 1 });

            var playerOrder = new List<int>();
            _mockEventManager.Setup(m => m.RaisePlayerTurn(It.IsAny<Player>(), It.IsAny<CardSuit?>(), It.IsAny<CardSuit?>(), It.IsAny<int>()))
                .Callback<Player, CardSuit?, CardSuit?, int>((p, ls, ts, tn) => 
                    playerOrder.Add(_testPlayers.IndexOf(p)))
                .Returns(Task.CompletedTask);

            await _trickController.ExecuteSingleTrick(0, CardSuit.Hearts);

            Assert.Equal(new List<int> { 2, 3, 0, 1 }, playerOrder);
        }

        #endregion

        #region Integration Tests

        [Fact]
        public async Task PlayAllTricks_CompleteGameFlow()
        {
            SetupPlayersWithCards(5);

            await _trickController.PlayAllTricks(0, CardSuit.Hearts);

            // Verify complete flow
            _mockPlayerTurnManager.Verify(m => m.InitializeTurnSequence(0), Times.Once);
            _mockEventManager.Verify(m => m.RaisePlayerTurn(It.IsAny<Player>(), It.IsAny<CardSuit?>(), It.IsAny<CardSuit?>(), It.IsAny<int>()), Times.Exactly(20));
            _mockEventManager.Verify(m => m.RaiseCardPlayed(It.IsAny<Player>(), It.IsAny<Card>(), It.IsAny<int>(), It.IsAny<CardSuit?>(), It.IsAny<CardSuit?>()), Times.Exactly(20));
            _mockEventManager.Verify(m => m.RaiseTrickCompleted(It.IsAny<int>(), It.IsAny<Player>(), It.IsAny<Card>(), It.IsAny<List<(Card, Player)>>(), It.IsAny<int>()), Times.Exactly(5));
            _mockScoringManager.Verify(m => m.AwardTrickPoints(It.IsAny<int>(), It.IsAny<int>()), Times.Exactly(5));
        }

        [Fact]
        public async Task ExecuteSingleTrick_ExecutesInCorrectOrder()
        {
            SetupPlayersWithCards(5);
            var callOrder = new List<string>();

            _mockEventManager.Setup(m => m.RaisePlayerTurn(It.IsAny<Player>(), It.IsAny<CardSuit?>(), It.IsAny<CardSuit?>(), It.IsAny<int>()))
                .Callback(() => callOrder.Add("PlayerTurn"))
                .Returns(Task.CompletedTask);
            _mockGameValidator.Setup(m => m.GetValidCardFromPlayer(It.IsAny<Player>(), It.IsAny<CardSuit?>(), It.IsAny<CardSuit?>()))
                .Callback(() => callOrder.Add("GetCard"))
                .ReturnsAsync(new Card(CardSuit.Hearts, CardFace.Five, 1, 5));
            _mockEventManager.Setup(m => m.RaiseCardPlayed(It.IsAny<Player>(), It.IsAny<Card>(), It.IsAny<int>(), It.IsAny<CardSuit?>(), It.IsAny<CardSuit?>()))
                .Callback(() => callOrder.Add("CardPlayed"))
                .Returns(Task.CompletedTask);
            _mockScoringManager.Setup(m => m.AwardTrickPoints(It.IsAny<int>(), It.IsAny<int>()))
                .Callback(() => callOrder.Add("AwardPoints"));
            _mockEventManager.Setup(m => m.RaiseTrickCompleted(It.IsAny<int>(), It.IsAny<Player>(), It.IsAny<Card>(), It.IsAny<List<(Card, Player)>>(), It.IsAny<int>()))
                .Callback(() => callOrder.Add("TrickCompleted"))
                .Returns(Task.CompletedTask);
            _mockPlayerTurnManager.Setup(m => m.SetCurrentPlayer(It.IsAny<int>()))
                .Callback(() => callOrder.Add("SetCurrentPlayer"));

            await _trickController.ExecuteSingleTrick(0, CardSuit.Hearts);

            // Verify pattern repeats for each player
            Assert.Contains("PlayerTurn", callOrder);
            Assert.Contains("GetCard", callOrder);
            Assert.Contains("CardPlayed", callOrder);
            Assert.Contains("AwardPoints", callOrder);
            Assert.Contains("TrickCompleted", callOrder);
            Assert.Contains("SetCurrentPlayer", callOrder);
            
            // Verify proper ordering
            var firstPlayerTurnIndex = callOrder.IndexOf("PlayerTurn");
            var firstGetCardIndex = callOrder.IndexOf("GetCard");
            var firstCardPlayedIndex = callOrder.IndexOf("CardPlayed");
            Assert.True(firstPlayerTurnIndex < firstGetCardIndex);
            Assert.True(firstGetCardIndex < firstCardPlayedIndex);
        }

        [Fact]
        public async Task PlayAllTricks_EmptiesAllPlayerHands()
        {
            SetupPlayersWithCards(10);

            await _trickController.PlayAllTricks(0, CardSuit.Hearts);

            foreach (var player in _testPlayers)
            {
                Assert.Empty(player.Hand);
            }
        }

        #endregion

        #region Helper Methods

        private void SetupPlayersWithCards(int cardsPerPlayer)
        {
            foreach (var player in _testPlayers)
            {
                player.Hand.Clear();
                for (int i = 0; i < cardsPerPlayer; i++)
                {
                    player.AddCard(new Card(CardSuit.Hearts, CardFace.Five, 1, 5));
                }
            }
        }

        #endregion
    }
}