using Moq;
using DeuxCentsCardGame.Interfaces.Events;
using DeuxCentsCardGame.Managers;
using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Tests.Managers
{
    public class DealingManagerTests
    {
        private readonly Mock<IGameEventManager> _mockEventManager;
        private readonly DealingManager _dealingManager;

        public DealingManagerTests()
        {
            _mockEventManager = new Mock<IGameEventManager>();
            _dealingManager = new DealingManager(_mockEventManager.Object);
        }

        [Fact]
        public void DealCards_ClearsPlayerHandsBeforeDealing()
        {
            // Arrange
            var deck = new Deck(); // Assuming Deck constructor creates 40-card deck
            var players = new List<Player>
            {
                new Player("Player1"),
                new Player("Player2"),
                new Player("Player3"),
                new Player("Player4")
            };

            // Add some cards to hands first            
            players[0].AddCard(new Card(CardSuit.Hearts, CardFace.Ace, 10, 10));
            players[1].AddCard(new Card(CardSuit.Spades, CardFace.King, 9, 0));

            // Act
            _dealingManager.DealCards(deck, players);

            // Assert - 40 cards / 4 players = 10 cards each
            Assert.Equal(10, players[0].Hand.Count);
            Assert.Equal(10, players[1].Hand.Count);
            Assert.Equal(10, players[2].Hand.Count);
            Assert.Equal(10, players[3].Hand.Count);
        }

        [Fact]
        public void DealCards_DistributesAllCardsEvenly()
        {
            // Arrange
            var deck = new Deck(); // 40 cards for modified deck
            var players = new List<Player>
            {
                new Player("Player1"),
                new Player("Player2"),
                new Player("Player3"),
                new Player("Player4")
            };

            // Act
            _dealingManager.DealCards(deck, players);

            // Assert - Each player should have 10 cards (40 / 4)
            Assert.All(players, player => Assert.Equal(10, player.Hand.Count));
        }

        [Fact]
        public void DealCards_DistributesCardsInRoundRobinFashion()
        {
            // Arrange
            var deck = new Deck();
            var players = new List<Player>
            {
                new Player("Player1"),
                new Player("Player2"),
                new Player("Player3"),
                new Player("Player4")
            };

            // Act
            _dealingManager.DealCards(deck, players);

            // Assert - Cards should be distributed alternately
            Assert.Equal(deck.Cards[0], players[0].Hand[0]);
            Assert.Equal(deck.Cards[1], players[1].Hand[0]);
            Assert.Equal(deck.Cards[2], players[2].Hand[0]);
            Assert.Equal(deck.Cards[3], players[3].Hand[0]);
            Assert.Equal(deck.Cards[4], players[0].Hand[1]);
            Assert.Equal(deck.Cards[5], players[1].Hand[1]);
        }

        [Fact]
        public void RotateDealerIndex_IncrementsCorrectly()
        {
            // Arrange
            int currentDealerIndex = 2;
            int totalPlayers = 4;

            // Act
            int newDealerIndex = _dealingManager.RotateDealerIndex(currentDealerIndex, totalPlayers);

            // Assert
            Assert.Equal(3, newDealerIndex);
        }

        [Fact]
        public void RotateDealerIndex_WrapsAround()
        {
            // Arrange
            int currentDealerIndex = 3;
            int totalPlayers = 4;

            // Act
            int newDealerIndex = _dealingManager.RotateDealerIndex(currentDealerIndex, totalPlayers);

            // Assert
            Assert.Equal(0, newDealerIndex);
        }

        [Fact]
        public void RaiseCardsDealtEvent_CallsEventManager()
        {
            // Arrange
            var players = new List<Player>
            {
                new Player("Player1"),
                new Player("Player2")
            };
            int dealerIndex = 1;

            // Act
            _dealingManager.RaiseCardsDealtEvent(players, dealerIndex);

            // Assert
            _mockEventManager.Verify(x => x.RaiseCardsDealt(players, dealerIndex), Times.Once);
        }
    }
}