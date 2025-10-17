namespace DeuxCentsCardGame.Constants
{
    public static class GameConstants
    {
        // Team Player Indices
        public const int TEAM_ONE_PLAYER_1 = 0;
        public const int TEAM_ONE_PLAYER_2 = 2;
        public const int TEAM_TWO_PLAYER_1 = 1;
        public const int TEAM_TWO_PLAYER_2 = 3;

        // Game Setup
        public const int PLAYERS_PER_TEAM = 2;
        public const int TOTAL_PLAYERS = 4;
        public const int TOTAL_TEAMS = 2;

        // Scoring
        public const int WINNING_SCORE = 200;
        public const int CANNOT_SCORE_THRESHOLD = 100;

        // Betting
        public const int MINIMUM_BET = 50;
        public const int MAXIMUM_BET = 100;
        public const int BET_INCREMENT = 5;
        public const int MINIMUM_PLAYERS_TO_PASS = 3;
        public const int PASSED_BID_VALUE = -1;

        // Dealer
        public const int INITIAL_DEALER_INDEX = 3;

        // Card Values
        public const int MINIMUM_CARD_FACE_VALUE = 1;
        public const int MAXIMUM_CARD_FACE_VALUE = 10;
        public const int CARD_POINT_VALUE_FIVE = 5;
        public const int CARD_POINT_VALUE_TEN = 10;
        public const int CARD_POINT_VALUE_ZERO = 0;

        // Deck
        public const int CARDS_PER_SUIT = 10;
        public const int TOTAL_SUITS = 4;
        public const int TOTAL_CARDS_IN_DECK = CARDS_PER_SUIT * TOTAL_SUITS; // 40

        // UI Display
        public const int HAND_DISPLAY_SEPARATOR_LENGTH = 40;
        public const int ALL_HANDS_SEPARATOR_LENGTH = 60;
        public const int GAME_OVER_SEPARATOR_LENGTH = 50;
    }
}