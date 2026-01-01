using DeuxCentsCardGame.Gameplay;
using DeuxCentsCardGame.Interfaces.Managers;
using DeuxCentsCardGame.Models;


namespace DeuxCentsCardGame.Interfaces.Gameplay
{
    public interface ITrickAnalyzer
    {
        List<Card> GetPlayableCards(List<Card> hand, CardSuit? leadingSuit);
        Card GetCurrentWinningCard(List<(Card card, Player player)> trick, CardSuit? trumpSuit, CardSuit? leadingSuit);
        int CalculateTrickValue(List<(Card card, Player player)> trick);
        bool IsPartnerWinning(List<(Card card, Player player)> trick, Player currentPlayer, List<Player> allPlayers, ITeamManager teamManager, CardSuit? trumpSuit, CardSuit? leadingSuit);
        List<Card> GetWinningCards(List<Card> hand, Card currentWinner, CardSuit? trumpSuit, CardSuit? leadingSuit);
        bool IsTrickValuable(List<(Card card, Player player)> trick, int threshold = 10);
        Player GetCurrentWinningPlayer(List<(Card card, Player player)> trick, CardSuit? trumpSuit, CardSuit? leadingSuit);
        TrickAnalysis AnalyzeTrick(List<(Card card, Player player)> trick, CardSuit? trumpSuit, CardSuit? leadingSuit);
    }
}