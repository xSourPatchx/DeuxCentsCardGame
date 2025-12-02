using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Interfaces.Services
{
    public interface ICardCollectionHelper
    {
        List<Card> FilterBySuit(List<Card> cards, CardSuit suit);
        List<Card> FilterByTrump(List<Card> cards, CardSuit? trumpSuit);
        List<Card> FilterByNonTrump(List<Card> cards, CardSuit? trumpSuit);
        List<Card> SortBySuit(List<Card> cards);
        List<Card> SortByValue(List<Card> cards, bool descending = false);
        List<Card> SortByPointValue(List<Card> cards, bool descending = false);
        Card? GetHighestCard(List<Card> cards);
        Card? GetLowestCard(List<Card> cards);
        Card? GetHighestCardOfSuit(List<Card> cards, CardSuit suit);
        Card? GetLowestCardOfSuit(List<Card> cards, CardSuit suit);
        Dictionary<CardSuit, List<Card>> GroupBySuit(List<Card> cards);
        Dictionary<CardSuit, int> CountBySuit(List<Card> cards);
        List<Card> GetPointCards(List<Card> cards);
        List<Card> GetNonPointCards(List<Card> cards);
        List<Card> GetHighValueCards(List<Card> cards, int threshold = 8);
        List<Card> GetLowValueCards(List<Card> cards, int threshold = 3);
        int CalculateTotalPoints(List<Card> cards);
        int CalculateTotalFaceValue(List<Card> cards);
        bool HasSuit(List<Card> cards, CardSuit suit);
        bool HasTrump(List<Card> cards, CardSuit? trumpSuit);
        Card? GetRandomCard(List<Card> cards, Random random);
        Card? GetMiddleCard(List<Card> cards);
        List<Card> Clone(List<Card> cards);
        List<Card> GetPlayableCards(List<Card> hand, CardSuit? leadingSuit);
    }
}