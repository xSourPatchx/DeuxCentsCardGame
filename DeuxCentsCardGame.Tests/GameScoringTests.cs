// using System;
// using System.Collections.Generic;
// using System.Reflection;
// using Xunit;
// using Moq;
// using DeuxCentsCardGame;

// namespace DeuxCentsCardGame.Tests
// {
//     public class GameScoringTests
//     {
//         private readonly Mock<IUIConsoleGameView> _mockUI;
        
//         public GameScoringTests()
//         {
//             _mockUI = new Mock<IUIConsoleGameView>();
//             // Setup common UI mock behavior
//             _mockUI.Setup(ui => ui.DisplayMessage(It.IsAny<string>())).Verifiable();
//             _mockUI.Setup(ui => ui.DisplayFormattedMessage(It.IsAny<string>(), It.IsAny<object[]>())).Verifiable();
//         }
        
//         // Helper method to create a testable Game instance
//         private Game CreateTestableGame()
//         {
//             var game = new Game(_mockUI.Object);
//             return game;
//         }
        
//         // Helper method to set private fields using reflection
//         private void SetPrivateField(object instance, string fieldName, object value)
//         {
//             var field = instance.GetType().GetField(fieldName, 
//                 BindingFlags.NonPublic | BindingFlags.Instance);
            
//             if (field != null)
//             {
//                 field.SetValue(instance, value);
//             }
//             else
//             {
//                 throw new ArgumentException($"Field {fieldName} not found");
//             }
//         }
        
//         [Fact]
//         public void UpdateTeamOnePoints_WhenTeamTwoOver100AndTeamOneDidNotBet_TeamTwoPointsReset()
//         {
//             // Arrange
//             var game = CreateTestableGame();
            
//             // Set up the required state using reflection to access private fields
//             SetPrivateField(game, "_teamOneTotalPoints", 50);
//             SetPrivateField(game, "_teamTwoTotalPoints", 120); // Team 2 over 100
//             SetPrivateField(game, "_teamOneRoundPoints", 30);
//             SetPrivateField(game, "_teamTwoRoundPoints", 25);
//             SetPrivateField(game, "_winningBid", 50);
//             SetPrivateField(game, "_winningBidIndex", 1); // Team Two won the bid
            
//             // Team One didn't place bets
//             var hasBet = new bool[4] { false, true, false, false };
//             SetPrivateField(game, "_hasBet", hasBet);
            
//             // Act - Call the private method using reflection
//             var method = typeof(Game).GetMethod("UpdateTeamOnePoints", 
//                 BindingFlags.NonPublic | BindingFlags.Instance);
//             method.Invoke(game, null);
            
//             // Get updated values
//             var teamTwoRoundPoints = (int)typeof(Game)
//                 .GetField("_teamTwoRoundPoints", BindingFlags.NonPublic | BindingFlags.Instance)
//                 .GetValue(game);
                
//             // Assert
//             Assert.Equal(0, teamTwoRoundPoints); // Team Two's round points should be reset
//         }
        
//         [Fact]
//         public void UpdateTeamTwoPoints_WhenTeamOneOver100AndTeamTwoDidNotBet_TeamOnePointsReset()
//         {
//             // Arrange
//             var game = CreateTestableGame();
            
//             // Set up the required state
//             SetPrivateField(game, "_teamOneTotalPoints", 120); // Team 1 over 100
//             SetPrivateField(game, "_teamTwoTotalPoints", 50);
//             SetPrivateField(game, "_teamOneRoundPoints", 25);
//             SetPrivateField(game, "_teamTwoRoundPoints", 30);
//             SetPrivateField(game, "_winningBid", 50);
//             SetPrivateField(game, "_winningBidIndex", 0); // Team One won the bid
            
//             // Team Two didn't place bets
//             var hasBet = new bool[4] { true, false, true, false };
//             SetPrivateField(game, "_hasBet", hasBet);
            
//             // Act - Call the private method using reflection
//             var method = typeof(Game).GetMethod("UpdateTeamTwoPoints", 
//                 BindingFlags.NonPublic | BindingFlags.Instance);
//             method.Invoke(game, null);
            
//             // Get updated values
//             var teamOneRoundPoints = (int)typeof(Game)
//                 .GetField("_teamOneRoundPoints", BindingFlags.NonPublic | BindingFlags.Instance)
//                 .GetValue(game);
                
//             // Assert
//             Assert.Equal(0, teamOneRoundPoints); // Team One's round points should be reset
//         }
//     }
// }