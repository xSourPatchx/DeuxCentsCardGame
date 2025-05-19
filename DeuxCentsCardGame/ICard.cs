namespace DeuxCentsCardGame
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