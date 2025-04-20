namespace DeuxCentsCardGame
{
    public interface ICard
    {
        string CardFace { get; }
        string CardSuit { get; }
        int CardFaceValue { get; }
        int CardPointValue { get; }
        string ToString();
    }
}