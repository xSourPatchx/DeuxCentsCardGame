namespace DeuxCentsCardGame.Config
{
    public class GameConfig
    {
        // Game configuration
        public const int TeamOnePlayer1 = 0;
        public const int TeamOnePlayer2 = 2;
        public const int TeamTwoPlayer1 = 1;
        public const int TeamTwoPlayer2 = 3;
        public const int TotalPlayers = 4;
        public const int PlayersPerTeam = 2;
        public const int WinningScore = 200;
        public const int CannotScoreThreshold = 100;


        // Betting configuration
        public const int MinimumBet = 50;
        public const int MaximumBet = 100;
        public const int BetIncrement = 5;
        public const int MinimumPlayersToPass = TotalPlayers - 1;
        public const int PassedBidValue = -1; // not using this yet


    }
}