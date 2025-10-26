using DeuxCentsCardGame.Models;
using DeuxCentsCardGame.Events.EventArgs;

namespace DeuxCentsCardGame.Interfaces.Events
{
    public interface IGameEventManager
    {
        // Round events
        void RaiseRoundStarted(int roundNumber, Player dealer);
        void RaiseRoundEnded(int roundNumber, int teamOneRoundPoints, int teamTwoRoundPoints, Player winningBidder, int winningBid);
        void RaiseDeckShuffled(string message);
        int RaiseDeckCutInput(Player cuttingPlayer, int deckSize);
        void RaiseDeckCut(Player cuttingPlayer, int cutPosition);
        void RaiseCardsDealt(List<Player> players, int dealerIndex);
        void RaiseInvalidMove(Player player, string message, InvalidMoveType moveType);

        // Betting events
        void RaiseBettingRoundStarted(string message);
        string RaiseBetInput(Player currentPlayer, int minBet, int maxBet, int betIncrement);
        // void RaiseInvalidBet(string message);
        void RaiseBettingAction(Player player, int bet, bool hasPassed = false, bool hasBet = false);
        void RaiseBettingCompleted(Player winningBidder, int winningBid, Dictionary<Player, int> allBids);

        // Trump selection events
        string RaiseTrumpSelectionInput(Player selectingPlayer);
        void RaiseTrumpSelected(CardSuit trumpSuit, Player selectedBy);

        // Card playing events
        void RaisePlayerTurn(Player player, CardSuit? leadingSuit, CardSuit? trumpSuit, int trickNumber);
        int RaiseCardSelectionInput(Player currentPlayer, CardSuit? leadingSuit, CardSuit? trumpSuit, List<Card> hand);
        void RaiseCardPlayed(Player player, Card card, int trickNumber, CardSuit? leadingSuit, CardSuit? trumpSuit);
        // void RaiseInvalidCard(string message);
        void RaiseTrickCompleted(int trickNumber, Player winningPlayer, Card winningCard, List<(Card card, Player player)> playedCards, int trickPoints);

        // Scoring events
        void RaiseScoreUpdated(int teamOneRoundPoints, int teamTwoRoundPoints, int teamOneTotalPoints, int teamTwoTotalPoints, bool isBidWinnerTeamOne, int winningBid);
        void RaiseTeamScoring(string teamName, int roundPoints, int winningBid, bool madeBid, bool cannotScore, int awardedPoints);
        void RaiseTrickPointsAwarded(Player player, int trickPoints, string teamName);

        // Game end events
        void RaiseGameOver(int teamOneFinalScore, int teamTwoFinalScore);
        void RaiseNextRoundPrompt(string message = "Press any key to start the next round...");
    }
}