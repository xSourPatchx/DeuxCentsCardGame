using Xunit;
using DeuxCentsCardGame.Core;
using DeuxCentsCardGame.Models;
using DeuxCentsCardGame.Events;
using System.Collections.Generic;
using System.Linq;

namespace DeuxCentsCardGame.Tests
{
    public class BettingStateTests
    {
        private readonly List<Player> _players;
        private readonly TestGameEventManager _eventManager;
        private readonly int _dealerIndex = 0;
        private BettingService _bettingState;

        public BettingStateTests()
        {
            // Setup 4 players for testing - using proper constructor
            _players = new List<Player>
            {
                new Player("Player1"),
                new Player("Player2"),
                new Player("Player3"),
                new Player("Player4")
            };

            _eventManager = new TestGameEventManager();
            _bettingState = new BettingService(_players, _dealerIndex, _eventManager);
        }

        [Fact]
        public void Constructor_SetsInitialState_Correctly()
        {
            // Arrange & Act
            var bettingState = new BettingService(_players, _dealerIndex, _eventManager);

            // Assert
            Assert.Equal(0, bettingState.CurrentWinningBid);
            Assert.Equal(0, bettingState.CurrentWinningBidIndex);
            Assert.False(bettingState.IsBettingRoundComplete);
        }

        [Fact]
        public void ResetBettingRound_ResetsAllStateCorrectly()
        {
            // Arrange
            _players[0].CurrentBid = 75;
            _players[0].HasBet = true;
            _players[1].HasPassed = true;
            _bettingState.CurrentWinningBid = 75;

            // Act
            _bettingState.ResetBettingRound();

            // Assert
            Assert.All(_players, player => 
            {
                Assert.Equal(0, player.CurrentBid);
                Assert.False(player.HasBet);
                Assert.False(player.HasPassed);
            });
            Assert.Equal(0, _bettingState.CurrentWinningBid);
            Assert.Equal(0, _bettingState.CurrentWinningBidIndex);
            Assert.False(_bettingState.IsBettingRoundComplete);
        }

        [Fact]
        public void ExecuteBettingRound_ThreePlayersPass_FourthPlayerBetsMinimum()
        {
            // Arrange
            var eventManager = new TestGameEventManager();
            eventManager.SetBetInputResponses(new[] { "pass", "pass", "pass" }); // First 3 pass
            _bettingState = new BettingService(_players, _dealerIndex, eventManager);

            // Act
            _bettingState.ExecuteBettingRound();

            // Assert
            Assert.Equal(3, _players.Count(p => p.HasPassed));
            Assert.Single(_players.Where(p => !p.HasPassed));
            
            var lastPlayer = _players.Single(p => !p.HasPassed);
            Assert.Equal(BettingService.MinimumBet, lastPlayer.CurrentBid);
            Assert.True(lastPlayer.HasBet);
            Assert.Equal(BettingService.MinimumBet, _bettingState.CurrentWinningBid);
        }

        [Fact]
        public void ExecuteBettingRound_PlayerBetsMaximum_AllOthersPass()
        {
            // Arrange
            var eventManager = new TestGameEventManager();
            eventManager.SetBetInputResponses(new[] { "100" }); // Player 2 bets maximum
            _bettingState = new BettingService(_players, _dealerIndex, eventManager);

            // Act
            _bettingState.ExecuteBettingRound();

            // Assert
            Assert.Equal(BettingService.MaximumBet, _players[1].CurrentBid); // Player to left of dealer
            Assert.True(_players[1].HasBet);
            Assert.Equal(3, _players.Count(p => p.HasPassed));
            Assert.Equal(BettingService.MaximumBet, _bettingState.CurrentWinningBid);
        }

        [Fact]
        public void ExecuteBettingRound_ValidBettingSequence_DeterminesWinner()
        {
            // Arrange
            var eventManager = new TestGameEventManager();
            eventManager.SetBetInputResponses(new[] { "55", "60", "pass", "70" });
            _bettingState = new BettingService(_players, _dealerIndex, eventManager);

            // Act
            _bettingState.ExecuteBettingRound();

            // Assert
            Assert.Equal(70, _bettingState.CurrentWinningBid);
            Assert.Equal(0, _bettingState.CurrentWinningBidIndex); // Player 1 (index 0) bid 70 in second round
            Assert.Equal(70, _players[0].CurrentBid);
            Assert.True(_players[0].HasBet);
        }

        [Fact]
        public void IsValidBet_ValidBet_ReturnsTrue()
        {
            // This tests the private IsValidBet method indirectly through ExecuteBettingRound
            var eventManager = new TestGameEventManager();
            eventManager.SetBetInputResponses(new[] { "55", "pass", "pass", "pass" });
            _bettingState = new BettingService(_players, _dealerIndex, eventManager);

            // Act
            _bettingState.ExecuteBettingRound();

            // Assert
            Assert.Equal(55, _players[1].CurrentBid); // Valid bet was accepted
            Assert.True(_players[1].HasBet);
        }

        [Fact]
        public void IsValidBet_DuplicateBet_HandlesProperly()
        {
            // Arrange
            var eventManager = new TestGameEventManager();
            // First player bets 60, second tries to bet 60 (invalid), then bets 65
            eventManager.SetBetInputResponses(new[] { "60", "60", "65", "pass", "pass" });
            _bettingState = new BettingService(_players, _dealerIndex, eventManager);

            // Act
            _bettingState.ExecuteBettingRound();

            // Assert
            Assert.Equal(60, _players[1].CurrentBid); // Player 2 (left of dealer)
            Assert.Equal(65, _players[2].CurrentBid); // Player 3
            Assert.Equal(65, _bettingState.CurrentWinningBid); // Highest valid bid wins
            Assert.True(eventManager.InvalidBetRaised); // Invalid bet was detected
        }

        [Theory]
        [InlineData(45)] // Below minimum
        [InlineData(105)] // Above maximum  
        [InlineData(52)] // Not increment of 5
        public void InvalidBets_AreRejectedProperly(int invalidBet)
        {
            // Arrange
            var eventManager = new TestGameEventManager();
            eventManager.SetBetInputResponses(new[] { invalidBet.ToString(), "55", "pass", "pass", "pass" });
            _bettingState = new BettingService(_players, _dealerIndex, eventManager);

            // Act
            _bettingState.ExecuteBettingRound();

            // Assert
            Assert.Equal(55, _players[1].CurrentBid); // Valid bet after invalid one
            Assert.True(eventManager.InvalidBetRaised); // Invalid bet event was raised
        }

        [Fact]
        public void ExecuteBettingRound_TracksHasBetCorrectly()
        {
            // Arrange
            var eventManager = new TestGameEventManager();
            eventManager.SetBetInputResponses(new[] { "55", "pass", "60", "pass" });
            _bettingState = new BettingService(_players, _dealerIndex, eventManager);

            // Act
            _bettingState.ExecuteBettingRound();

            // Assert
            Assert.True(_players[1].HasBet); // Player who bet 55
            Assert.False(_players[2].HasBet); // Player who passed without betting
            Assert.True(_players[3].HasBet); // Player who bet 60
            Assert.False(_players[0].HasBet); // Player who passed without betting
        }

        [Fact]
        public void ExecuteBettingRound_BettingOrder_StartsWithPlayerLeftOfDealer()
        {
            // Arrange - Dealer at index 2
            var dealerIndex = 2;
            var eventManager = new TestGameEventManager();
            eventManager.SetBetInputResponses(new[] { "50", "pass", "pass", "pass" });
            var bettingState = new BettingService(_players, dealerIndex, eventManager);

            // Act
            bettingState.ExecuteBettingRound();

            // Assert
            // Player at index 3 (left of dealer at index 2) should bet first
            Assert.Equal(50, _players[3].CurrentBid);
            Assert.True(_players[3].HasBet);
        }

        [Fact]
        public void ExecuteBettingRound_TeamCannotScoreScenario_TracksHasBetCorrectly()
        {
            // This tests the important rule: "When a team surpasses 100 points, 
            // they must still place a bet in the betting phase to collect points"
            
            // Arrange
            var eventManager = new TestGameEventManager();
            eventManager.SetBetInputResponses(new[] { "55", "pass", "60", "pass" });
            _bettingState = new BettingService(_players, _dealerIndex, eventManager);

            // Act
            _bettingState.ExecuteBettingRound();

            // Assert - Verify HasBet is tracked correctly for scoring logic
            Assert.True(_players[1].HasBet); // Player who bet 55
            Assert.False(_players[2].HasBet); // Player who passed without betting
            Assert.True(_players[3].HasBet); // Player who bet 60
            Assert.False(_players[0].HasBet); // Player who passed without betting
            
            // This is critical for the Game class ScoreBidWinningTeam/ScoreBidLosingTeam methods
            // where it checks: !player.HasBet to determine if team can score when >= 100 points
        }
    }

    // Test-specific GameEventManager that can be controlled for testing
    public class TestGameEventManager : GameEventManager
    {
        private Queue<string> _betInputResponses = new Queue<string>();
        public bool InvalidBetRaised { get; private set; }
        public bool BettingRoundStartedRaised { get; private set; }
        public bool BettingCompletedRaised { get; private set; }
        public Player LastWinningBidder { get; private set; }
        public int LastWinningBid { get; private set; }
        public List<string> BettingActions { get; private set; } = new List<string>();

        public void SetBetInputResponses(string[] responses)
        {
            _betInputResponses.Clear();
            foreach (var response in responses)
            {
                _betInputResponses.Enqueue(response);
            }
        }

        // Override the methods we need for testing by creating a new implementation
        // that intercepts the calls we care about
        public override string RaiseBetInput(Player player, int minBet, int maxBet, int increment)
        {
            if (_betInputResponses.Count > 0)
            {
                return _betInputResponses.Dequeue();
            }
            return "pass"; // Default to pass if no more responses
        }

        public override void RaiseInvalidBet(string message)
        {
            InvalidBetRaised = true;
            // Still call base to maintain any other functionality
            base.RaiseInvalidBet(message);
        }

        public override void RaiseBettingRoundStarted(string message) 
        { 
            BettingRoundStartedRaised = true;
            base.RaiseBettingRoundStarted(message);
        }
        
        public override void RaiseBettingAction(Player player, int bid, bool hasPassed) 
        { 
            string action = hasPassed ? $"{player.Name} passed" : $"{player.Name} bet {bid}";
            BettingActions.Add(action);
            base.RaiseBettingAction(player, bid, hasPassed);
        }
        
        public override void RaiseBettingCompleted(Player winningBidder, int winningBid, Dictionary<Player, int> allBids) 
        { 
            BettingCompletedRaised = true;
            LastWinningBidder = winningBidder;
            LastWinningBid = winningBid;
            base.RaiseBettingCompleted(winningBidder, winningBid, allBids);
        }
    }
}