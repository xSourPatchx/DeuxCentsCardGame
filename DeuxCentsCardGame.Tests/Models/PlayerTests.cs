using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Tests.Models
{
    public class PlayerTests
    {
        #region Constructor Tests

        [Fact]
        public void Constructor_InitializesPlayerWithName()
        {
            // Arrange & Act
            var player = new Player("TestPlayer");

            // Assert
            Assert.Equal("TestPlayer", player.Name);
        }

        [Fact]
        public void Constructor_InitializesEmptyHand()
        {
            // Arrange & Act
            var player = new Player("TestPlayer");

            // Assert
            Assert.NotNull(player.Hand);
            Assert.Empty(player.Hand);
        }

        [Fact]
        public void Constructor_InitializesBettingPropertiesWithDefaultValues()
        {
            // Arrange & Act
            var player = new Player("TestPlayer");

            // Assert
            Assert.False(player.HasBet);
            Assert.False(player.HasPassed);
            Assert.Equal(0, player.CurrentBid);
        }

        [Theory]
        [InlineData("Alice")]
        [InlineData("Player1")]
        [InlineData("Bob Smith")]
        [InlineData("X")]
        [InlineData("VeryLongPlayerNameWithNumbers123")]
        public void Constructor_AcceptsVariousNameFormats(string name)
        {
            // Arrange & Act
            var player = new Player(name);

            // Assert
            Assert.Equal(name, player.Name);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void Constructor_AcceptsEmptyOrWhitespaceNames(string name)
        {
            // Arrange & Act
            var player = new Player(name);

            // Assert
            Assert.Equal(name, player.Name);
        }

        #endregion

        #region Property Tests

        [Fact]
        public void Name_PropertyIsReadOnly()
        {
            // Arrange
            var nameProperty = typeof(Player).GetProperty("Name");

            // Act & Assert
            Assert.NotNull(nameProperty);
            Assert.True(nameProperty.CanRead);
            Assert.False(nameProperty.SetMethod?.IsPublic ?? false);
        }

        [Fact]
        public void Hand_PropertyIsReadOnly()
        {
            // Arrange
            var handProperty = typeof(Player).GetProperty("Hand");

            // Act & Assert
            Assert.NotNull(handProperty);
            Assert.True(handProperty.CanRead);
            Assert.False(handProperty.SetMethod?.IsPublic ?? false);
        }

        [Fact]
        public void BettingProperties_ArePubliclySettable()
        {
            // Arrange
            var player = new Player("TestPlayer");

            // Act
            player.HasBet = true;
            player.HasPassed = true;
            player.CurrentBid = 100;

            // Assert
            Assert.True(player.HasBet);
            Assert.True(player.HasPassed);
            Assert.Equal(100, player.CurrentBid);
        }

        #endregion

        #region AddCard Tests

        [Fact]
        public void AddCard_AddsCardToHand()
        {
            // Arrange
            var player = new Player("TestPlayer");
            var card = new Card(CardSuit.Hearts, CardFace.Ace, 10, 10);

            // Act
            player.AddCard(card);

            // Assert
            Assert.Single(player.Hand);
            Assert.Contains(card, player.Hand);
        }

        [Fact]
        public void AddCard_AddsMultipleCards()
        {
            // Arrange
            var player = new Player("TestPlayer");
            var card1 = new Card(CardSuit.Hearts, CardFace.Ace, 10, 10);
            var card2 = new Card(CardSuit.Spades, CardFace.King, 9, 0);
            var card3 = new Card(CardSuit.Clubs, CardFace.Five, 1, 5);

            // Act
            player.AddCard(card1);
            player.AddCard(card2);
            player.AddCard(card3);

            // Assert
            Assert.Equal(3, player.Hand.Count);
            Assert.Contains(card1, player.Hand);
            Assert.Contains(card2, player.Hand);
            Assert.Contains(card3, player.Hand);
        }

        [Fact]
        public void AddCard_MaintainsInsertionOrder()
        {
            // Arrange
            var player = new Player("TestPlayer");
            var card1 = new Card(CardSuit.Hearts, CardFace.Ace, 10, 10);
            var card2 = new Card(CardSuit.Spades, CardFace.King, 9, 0);
            var card3 = new Card(CardSuit.Clubs, CardFace.Five, 1, 5);

            // Act
            player.AddCard(card1);
            player.AddCard(card2);
            player.AddCard(card3);

            // Assert
            Assert.Equal(card1, player.Hand[0]);
            Assert.Equal(card2, player.Hand[1]);
            Assert.Equal(card3, player.Hand[2]);
        }

        [Fact]
        public void AddCard_AllowsDuplicateCards()
        {
            // Arrange
            var player = new Player("TestPlayer");
            var card1 = new Card(CardSuit.Hearts, CardFace.Ace, 10, 10);
            var card2 = new Card(CardSuit.Hearts, CardFace.Ace, 10, 10);

            // Act
            player.AddCard(card1);
            player.AddCard(card2);

            // Assert
            Assert.Equal(2, player.Hand.Count);
        }

        [Fact]
        public void AddCard_CanAddEntireDeck()
        {
            // Arrange
            var player = new Player("TestPlayer");
            var deck = new Deck();

            // Act
            foreach (var card in deck.Cards)
            {
                player.AddCard(card);
            }

            // Assert
            Assert.Equal(40, player.Hand.Count);
        }

        #endregion

        #region RemoveCard Tests

        [Fact]
        public void RemoveCard_RemovesCardFromHand()
        {
            // Arrange
            var player = new Player("TestPlayer");
            var card = new Card(CardSuit.Hearts, CardFace.Ace, 10, 10);
            player.AddCard(card);

            // Act
            player.RemoveCard(card);

            // Assert
            Assert.Empty(player.Hand);
            Assert.DoesNotContain(card, player.Hand);
        }

        [Fact]
        public void RemoveCard_RemovesOnlyFirstOccurrence()
        {
            // Arrange
            var player = new Player("TestPlayer");
            var card1 = new Card(CardSuit.Hearts, CardFace.Ace, 10, 10);
            var card2 = new Card(CardSuit.Hearts, CardFace.Ace, 10, 10);
            var card3 = new Card(CardSuit.Spades, CardFace.King, 9, 0);

            player.AddCard(card1);
            player.AddCard(card2);
            player.AddCard(card3);

            // Act
            player.RemoveCard(card1);

            // Assert
            Assert.Equal(2, player.Hand.Count);
            Assert.DoesNotContain(card1, player.Hand);
            Assert.Contains(card2, player.Hand);
            Assert.Contains(card3, player.Hand);
        }

        [Fact]
        public void RemoveCard_WithNonExistentCard_DoesNotChangeHand()
        {
            // Arrange
            var player = new Player("TestPlayer");
            var cardInHand = new Card(CardSuit.Hearts, CardFace.Ace, 10, 10);
            var cardNotInHand = new Card(CardSuit.Spades, CardFace.King, 9, 0);
            player.AddCard(cardInHand);

            // Act
            player.RemoveCard(cardNotInHand);

            // Assert
            Assert.Single(player.Hand);
            Assert.Contains(cardInHand, player.Hand);
        }

        [Fact]
        public void RemoveCard_FromEmptyHand_DoesNotThrow()
        {
            // Arrange
            var player = new Player("TestPlayer");
            var card = new Card(CardSuit.Hearts, CardFace.Ace, 10, 10);

            // Act & Assert
            var exception = Record.Exception(() => player.RemoveCard(card));
            Assert.Null(exception);
            Assert.Empty(player.Hand);
        }

        [Fact]
        public void RemoveCard_MaintainsRemainingCardOrder()
        {
            // Arrange
            var player = new Player("TestPlayer");
            var card1 = new Card(CardSuit.Hearts, CardFace.Ace, 10, 10);
            var card2 = new Card(CardSuit.Spades, CardFace.King, 9, 0);
            var card3 = new Card(CardSuit.Clubs, CardFace.Five, 1, 5);
            var card4 = new Card(CardSuit.Diamonds, CardFace.Ten, 6, 10);

            player.AddCard(card1);
            player.AddCard(card2);
            player.AddCard(card3);
            player.AddCard(card4);

            // Act - Remove middle card
            player.RemoveCard(card2);

            // Assert
            Assert.Equal(3, player.Hand.Count);
            Assert.Equal(card1, player.Hand[0]);
            Assert.Equal(card3, player.Hand[1]);
            Assert.Equal(card4, player.Hand[2]);
        }

        #endregion

        #region ResetBettingState Tests

        [Fact]
        public void ResetBettingState_ResetsAllBettingProperties()
        {
            // Arrange
            var player = new Player("TestPlayer");
            player.HasBet = true;
            player.HasPassed = true;
            player.CurrentBid = 150;

            // Act
            player.ResetBettingState();

            // Assert
            Assert.False(player.HasBet);
            Assert.False(player.HasPassed);
            Assert.Equal(0, player.CurrentBid);
        }

        [Fact]
        public void ResetBettingState_DoesNotAffectHand()
        {
            // Arrange
            var player = new Player("TestPlayer");
            var card = new Card(CardSuit.Hearts, CardFace.Ace, 10, 10);
            player.AddCard(card);
            player.HasBet = true;
            player.CurrentBid = 100;

            // Act
            player.ResetBettingState();

            // Assert
            Assert.Single(player.Hand);
            Assert.Contains(card, player.Hand);
        }

        [Fact]
        public void ResetBettingState_DoesNotAffectName()
        {
            // Arrange
            var player = new Player("TestPlayer");
            player.HasBet = true;
            player.CurrentBid = 100;

            // Act
            player.ResetBettingState();

            // Assert
            Assert.Equal("TestPlayer", player.Name);
        }

        [Fact]
        public void ResetBettingState_CanBeCalledMultipleTimes()
        {
            // Arrange
            var player = new Player("TestPlayer");
            player.HasBet = true;
            player.HasPassed = true;
            player.CurrentBid = 200;

            // Act
            player.ResetBettingState();
            player.ResetBettingState();
            player.ResetBettingState();

            // Assert
            Assert.False(player.HasBet);
            Assert.False(player.HasPassed);
            Assert.Equal(0, player.CurrentBid);
        }

        [Fact]
        public void ResetBettingState_OnNewPlayer_HasNoEffect()
        {
            // Arrange
            var player = new Player("TestPlayer");

            // Act
            player.ResetBettingState();

            // Assert
            Assert.False(player.HasBet);
            Assert.False(player.HasPassed);
            Assert.Equal(0, player.CurrentBid);
        }

        #endregion

        #region Betting Workflow Tests

        [Fact]
        public void BettingWorkflow_PlayerCanPlaceBid()
        {
            // Arrange
            var player = new Player("TestPlayer");

            // Act
            player.HasBet = true;
            player.CurrentBid = 100;

            // Assert
            Assert.True(player.HasBet);
            Assert.False(player.HasPassed);
            Assert.Equal(100, player.CurrentBid);
        }

        [Fact]
        public void BettingWorkflow_PlayerCanPass()
        {
            // Arrange
            var player = new Player("TestPlayer");

            // Act
            player.HasPassed = true;

            // Assert
            Assert.False(player.HasBet);
            Assert.True(player.HasPassed);
            Assert.Equal(0, player.CurrentBid);
        }

        [Fact]
        public void BettingWorkflow_PlayerCanIncreaseBid()
        {
            // Arrange
            var player = new Player("TestPlayer");
            player.HasBet = true;
            player.CurrentBid = 100;

            // Act
            player.CurrentBid = 150;

            // Assert
            Assert.True(player.HasBet);
            Assert.Equal(150, player.CurrentBid);
        }

        [Fact]
        public void BettingWorkflow_CompleteRoundCycle()
        {
            // Arrange
            var player = new Player("TestPlayer");

            // Act - Round 1: Place bid
            player.HasBet = true;
            player.CurrentBid = 100;

            // Round 1: Reset for next round
            player.ResetBettingState();

            // Round 2: Pass
            player.HasPassed = true;

            // Round 2: Reset for next round
            player.ResetBettingState();

            // Assert
            Assert.False(player.HasBet);
            Assert.False(player.HasPassed);
            Assert.Equal(0, player.CurrentBid);
        }

        #endregion

        #region Integration Tests

        [Fact]
        public void Player_CompleteGameScenario()
        {
            // Arrange
            var player = new Player("Alice");
            var deck = new Deck();

            // Act - Deal 5 cards
            for (int i = 0; i < 5; i++)
            {
                player.AddCard(deck.Cards[i]);
            }

            // Place a bet
            player.HasBet = true;
            player.CurrentBid = 120;

            // Play a card
            var playedCard = player.Hand[0];
            player.RemoveCard(playedCard);

            // Assert
            Assert.Equal("Alice", player.Name);
            Assert.Equal(4, player.Hand.Count);
            Assert.True(player.HasBet);
            Assert.Equal(120, player.CurrentBid);
            Assert.DoesNotContain(playedCard, player.Hand);
        }

        [Fact]
        public void Player_CanManageFullHandThroughMultipleTricks()
        {
            // Arrange
            var player = new Player("TestPlayer");
            var deck = new Deck();

            // Deal initial 5 cards
            for (int i = 0; i < 5; i++)
            {
                player.AddCard(deck.Cards[i]);
            }

            // Act - Play all 5 cards (5 tricks)
            for (int i = 0; i < 5; i++)
            {
                var cardToPlay = player.Hand[0];
                player.RemoveCard(cardToPlay);
            }

            // Assert
            Assert.Empty(player.Hand);
        }

        [Fact]
        public void MultiplePlayerInstances_AreIndependent()
        {
            // Arrange & Act
            var player1 = new Player("Player 1");
            var player2 = new Player("Player 2");

            var card1 = new Card(CardSuit.Hearts, CardFace.Ace, 10, 10);
            var card2 = new Card(CardSuit.Spades, CardFace.King, 9, 0);

            player1.AddCard(card1);
            player1.HasBet = true;
            player1.CurrentBid = 100;

            player2.AddCard(card2);
            player2.HasPassed = true;

            // Assert
            Assert.Equal("Player 1", player1.Name);
            Assert.Equal("Player 2", player2.Name);
            Assert.Single(player1.Hand);
            Assert.Single(player2.Hand);
            Assert.True(player1.HasBet);
            Assert.False(player1.HasPassed);
            Assert.Equal(100, player1.CurrentBid);
            Assert.False(player2.HasBet);
            Assert.True(player2.HasPassed);
            Assert.Equal(0, player2.CurrentBid);
        }

        #endregion
    }
}