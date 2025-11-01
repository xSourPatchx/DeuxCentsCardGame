using DeuxCentsCardGame.Interfaces.GameConfig;

namespace DeuxCentsCardGame.Interfaces.GameConfig
{
    public interface IGameConfig
    {
        // Team Configuration
        int TeamOnePlayer1 { get; set; }
        int TeamOnePlayer2 { get; set; }
        int TeamTwoPlayer1 { get; set; }
        int TeamTwoPlayer2 { get; set; }
        int TotalPlayers { get; set; }
        int PlayersPerTeam { get; set; }

        // Scoring Configuration
        int WinningScore { get; set; }
        int CannotScoreThreshold { get; set; }

        // Betting Configuration
        int MinimumBet { get; set; }
        int MaximumBet { get; set; }
        int BetIncrement { get; set; }
        int MinimumPlayersToPass { get; set; }
        int PassedBidValue { get; set; }

        // Dealer Configuration
        int InitialDealerIndex { get; set; }

        // Card Values Configuration
        int MinimumCardFaceValue { get; set; }
        int MaximumCardFaceValue { get; set; }
        int CardPointValueFive { get; set; }
        int CardPointValueTen { get; set; }
        int CardPointValueZero { get; set; }

        // Deck Configuration
        int CardsPerSuit { get; set; }
        int TotalSuits { get; set; }
        int TotalCardsInDeck { get; set; }

        // UI Configuration
        int HandDisplaySeparatorLength { get; set; }
        int AllHandsSeparatorLength { get; set; }
        int GameOverSeparatorLength { get; set; }
    }
}