namespace DeuxCentsCardGame
{
    public class Card // initializing Card object
    {
        // fields
        public string CardFace { get; } // 5, 6, 7, 8, 9, 10, J, Q, K, A
        public string CardSuit { get; } // clubs, diamonds, hearts, spades
        public int CardFaceValue { get; } // 1, 2, 3, 4, 5, 6, 7, 8, 9, 10
        public int CardPointValue { get; } // 0, 5, 10

        // constructor
        public Card(string cardFace, string cardSuit, int cardFaceValue, int cardPointValue)
        {
            CardFace = cardFace;
            CardSuit = cardSuit;
            CardFaceValue = cardFaceValue;
            CardPointValue = cardPointValue;
        }

        public override string ToString() => $"{ CardFace } of { CardSuit } ({ CardPointValue } pts)";
        
    }
}