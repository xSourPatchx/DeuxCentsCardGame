namespace DeuxCentsCardGame
{
    public enum CardSuit
    {
        Clubs,
        Diamonds,
        Hearts,
        Spades
    }

    public enum CardFace
    {
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        Jack,
        Queen,
        King,
        Ace
    }

    public class Card(CardSuit cardSuit, CardFace cardFace, int cardFaceValue, int cardPointValue) : ICard // initializing Card object
    {
        // fields
        public CardSuit CardSuit { get; init; } = cardSuit;
        public CardFace CardFace { get; init; } = cardFace;
        public int CardFaceValue { get; init; } = cardFaceValue;
        public int CardPointValue { get; init; } = cardPointValue;

        public override string ToString() =>
            $"{GetCardFaceString(CardFace)} of {GetCardSuitString(CardSuit)} ({CardPointValue} pts)";

        private static string GetCardFaceString(CardFace cardFace)
        {
            return cardFace switch
            {
                CardFace.Jack => "J",
                CardFace.Queen => "Q",
                CardFace.King => "K",
                CardFace.Ace => "A",
                _ => ((int)cardFace + 5).ToString()
            };
        }

        private static string GetCardSuitString(CardSuit cardSuit) => cardSuit.ToString().ToLower();
    }
}