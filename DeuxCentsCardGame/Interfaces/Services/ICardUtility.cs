using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Interfaces.Services
{
    public interface ICardUtility
    {
        CardSuit StringToCardSuit(string cardSuitName);
        string CardSuitToString(CardSuit cardSuit);
        CardSuit[] GetAllCardSuits();
        CardFace[] GetAllCardFaces();
        int[] GetCardFaceValues();
        int[] GetCardPointValues();
        string FormatCardFace(CardFace face);
    }
}