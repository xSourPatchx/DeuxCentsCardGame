namespace DeuxCentsCardGame
{
    public class Card // initializing Card object
    {
        // fields
        public string cardFace; // 5, 6, 7, 8, 9, 10, J, Q, K, A
        public string cardSuit; // clubs, diamonds, hearts, spades
        public int cardFaceValue; // 1, 2, 3, 4, 5, 6, 7, 8, 9, 10
        public int cardPointValue; // 0, 5, 10
        public Card(string cardFace, string cardSuit, int cardFaceValue, int cardPointValue)
        {
            this.cardFace = cardFace; //constructors
            this.cardSuit = cardSuit;
            this.cardFaceValue = cardFaceValue;
            this.cardPointValue = cardPointValue;
        }

        public override string ToString()
        {
            return cardFace + " of " + cardSuit + " = " + cardPointValue + " points";
        }
    }
}