using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Interfaces
{
    public interface ICard
    {
        CardSuit CardSuit { get; }
        CardFace CardFace { get; }
        int CardFaceValue { get; }
        int CardPointValue { get; }
        string ToString();
    }
}