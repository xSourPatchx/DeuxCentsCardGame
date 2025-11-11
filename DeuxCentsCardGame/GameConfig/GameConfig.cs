using DeuxCentsCardGame.Constants;
using DeuxCentsCardGame.Interfaces.GameConfig;
using Microsoft.Extensions.Configuration;

namespace DeuxCentsCardGame.GameConfig
{
    [Serializable]
    public class GameConfig : IGameConfig
    {
        // Team Configuration
        public int TeamOnePlayer1 { get; set; } = GameConstants.TEAM_ONE_PLAYER_1;
        public int TeamOnePlayer2 { get; set; } = GameConstants.TEAM_ONE_PLAYER_2;
        public int TeamTwoPlayer1 { get; set; } = GameConstants.TEAM_TWO_PLAYER_1;
        public int TeamTwoPlayer2 { get; set; } = GameConstants.TEAM_TWO_PLAYER_2;
        public int TotalPlayers { get; set; } = GameConstants.TOTAL_PLAYERS;
        public int PlayersPerTeam { get; set; } = GameConstants.PLAYERS_PER_TEAM;

        // Scoring Configuration
        public int WinningScore { get; set; } = GameConstants.WINNING_SCORE;
        public int CannotScoreThreshold { get; set; } = GameConstants.CANNOT_SCORE_THRESHOLD;

        // Betting Configuration
        public int MinimumBet { get; set; } = GameConstants.MINIMUM_BET;
        public int MaximumBet { get; set; } = GameConstants.MAXIMUM_BET;
        public int BetIncrement { get; set; } = GameConstants.BET_INCREMENT;
        public int MinimumPlayersToPass { get; set; } = GameConstants.MINIMUM_PLAYERS_TO_PASS;
        public int PassedBidValue { get; set; } = GameConstants.PASSED_BID_VALUE;

        // Dealer Configuration
        public int InitialDealerIndex { get; set; } = GameConstants.INITIAL_DEALER_INDEX;
        
        // Card Values Configuration
        public int MinimumCardFaceValue { get; set; } = GameConstants.MINIMUM_CARD_FACE_VALUE;
        public int MaximumCardFaceValue { get; set; } = GameConstants.MAXIMUM_CARD_FACE_VALUE;
        public int CardPointValueFive { get; set; } = GameConstants.CARD_POINT_VALUE_FIVE;
        public int CardPointValueTen { get; set; } = GameConstants.CARD_POINT_VALUE_TEN;
        public int CardPointValueZero { get; set; } = GameConstants.CARD_POINT_VALUE_ZERO;
        
        // Deck Configuration
        public int CardsPerSuit { get; set; } = GameConstants.CARDS_PER_SUIT;
        public int TotalSuits { get; set; } = GameConstants.TOTAL_SUITS;
        public int TotalCardsInDeck { get; set; } = GameConstants.TOTAL_CARDS_IN_DECK;
        
        // UI Configuration
        public int HandDisplaySeparatorLength { get; set; } = GameConstants.HAND_DISPLAY_SEPARATOR_LENGTH;
        public int AllHandsSeparatorLength { get; set; } = GameConstants.ALL_HANDS_SEPARATOR_LENGTH;
        public int GameOverSeparatorLength { get; set; } = GameConstants.GAME_OVER_SEPARATOR_LENGTH;

        public static IGameConfig CreateDefault()
        {
            return new GameConfig();
        }

        public static IGameConfig CreateFromConfiguration(IConfiguration configuration)
        {
            var gameConfig = new GameConfig();
            
            // Load team configuration
            var teamConfig = configuration.GetSection("GameSettings:TeamConfiguration");
            gameConfig.TeamOnePlayer1 = teamConfig.GetValue<int>("TeamOnePlayer1", GameConstants.TEAM_ONE_PLAYER_1);
            gameConfig.TeamOnePlayer2 = teamConfig.GetValue<int>("TeamOnePlayer2", GameConstants.TEAM_ONE_PLAYER_2);
            gameConfig.TeamTwoPlayer1 = teamConfig.GetValue<int>("TeamTwoPlayer1", GameConstants.TEAM_TWO_PLAYER_1);
            gameConfig.TeamTwoPlayer2 = teamConfig.GetValue<int>("TeamTwoPlayer2", GameConstants.TEAM_TWO_PLAYER_2);
            gameConfig.PlayersPerTeam = teamConfig.GetValue<int>("PlayersPerTeam", GameConstants.PLAYERS_PER_TEAM);
            gameConfig.TotalPlayers = teamConfig.GetValue<int>("TotalPlayers", GameConstants.TOTAL_PLAYERS);

            // Load scoring configuration
            var scoringConfig = configuration.GetSection("GameSettings:Scoring");
            gameConfig.WinningScore = scoringConfig.GetValue<int>("WinningScore", GameConstants.WINNING_SCORE);
            gameConfig.CannotScoreThreshold = scoringConfig.GetValue<int>("CannotScoreThreshold", GameConstants.CANNOT_SCORE_THRESHOLD);

            // Load betting configuration
            var bettingConfig = configuration.GetSection("GameSettings:Betting");
            gameConfig.MinimumBet = bettingConfig.GetValue<int>("MinimumBet", GameConstants.MINIMUM_BET);
            gameConfig.MaximumBet = bettingConfig.GetValue<int>("MaximumBet", GameConstants.MAXIMUM_BET);
            gameConfig.BetIncrement = bettingConfig.GetValue<int>("BetIncrement", GameConstants.BET_INCREMENT);
            gameConfig.MinimumPlayersToPass = bettingConfig.GetValue<int>("MinimumPlayersToPass", GameConstants.MINIMUM_PLAYERS_TO_PASS);
            gameConfig.PassedBidValue = bettingConfig.GetValue<int>("PassedBidValue", GameConstants.PASSED_BID_VALUE);

            // Load dealer configuration
            var dealerConfig = configuration.GetSection("GameSettings:Dealer");
            gameConfig.InitialDealerIndex = dealerConfig.GetValue<int>("InitialDealerIndex", GameConstants.INITIAL_DEALER_INDEX);
            
            // Load card values configuration
            var cardValuesConfig = configuration.GetSection("GameSettings:CardValues");
            gameConfig.MinimumCardFaceValue = cardValuesConfig.GetValue<int>("MinimumCardFaceValue", GameConstants.MINIMUM_CARD_FACE_VALUE);
            gameConfig.MaximumCardFaceValue = cardValuesConfig.GetValue<int>("MaximumCardFaceValue", GameConstants.MAXIMUM_CARD_FACE_VALUE);
            gameConfig.CardPointValueFive = cardValuesConfig.GetValue<int>("CardPointValueFive", GameConstants.CARD_POINT_VALUE_FIVE);
            gameConfig.CardPointValueTen = cardValuesConfig.GetValue<int>("CardPointValueTen", GameConstants.CARD_POINT_VALUE_TEN);
            gameConfig.CardPointValueZero = cardValuesConfig.GetValue<int>("CardPointValueZero", GameConstants.CARD_POINT_VALUE_ZERO);
            
            // Load deck configuration
            var deckConfig = configuration.GetSection("GameSettings:Deck");
            gameConfig.CardsPerSuit = deckConfig.GetValue<int>("CardsPerSuit", GameConstants.CARDS_PER_SUIT);
            gameConfig.TotalSuits = deckConfig.GetValue<int>("TotalSuits", GameConstants.TOTAL_SUITS);
            gameConfig.TotalCardsInDeck = deckConfig.GetValue<int>("TotalCardsInDeck", GameConstants.TOTAL_CARDS_IN_DECK);
            
            // Load UI configuration
            var uiConfig = configuration.GetSection("GameSettings:UI");
            gameConfig.HandDisplaySeparatorLength = uiConfig.GetValue<int>("HandDisplaySeparatorLength", GameConstants.HAND_DISPLAY_SEPARATOR_LENGTH);
            gameConfig.AllHandsSeparatorLength = uiConfig.GetValue<int>("AllHandsSeparatorLength", GameConstants.ALL_HANDS_SEPARATOR_LENGTH);
            gameConfig.GameOverSeparatorLength = uiConfig.GetValue<int>("GameOverSeparatorLength", GameConstants.GAME_OVER_SEPARATOR_LENGTH);

            return gameConfig;
        }
    }
}