namespace DeuxCentsCardGame
{
    public class Deck
    {
        public List<Card> Cards;
        static readonly Random random = new ();


        public Deck()
        {
            Cards = new List<Card>();
            InitializeCards();
        }
        private void InitializeCards()
        {
            string[] cardSuits = { "clubs", "diamonds", "hearts", "spades" };
            string[] cardFaces = { "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A" };
            int[] cardfaceValues = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            int[] cardPointValues = { 5, 0, 0, 0, 0, 10, 0, 0, 0, 10 };

            foreach (string suit in cardSuits)
            {
                for (int c = 0; c < cardFaces.Length; c++)
                {
                    Cards.Add(new Card(cardFaces[c], suit, cardfaceValues[c], cardPointValues[c]));
                }
            }
            // append each Card to Cards list
            /*
            Cards.Add(new Card("5", "clubs", 1, 5)); // clubs
            Cards.Add(new Card("6", "clubs", 2, 0));
            Cards.Add(new Card("7", "clubs", 3, 0));
            Cards.Add(new Card("8", "clubs", 4, 0));
            Cards.Add(new Card("9", "clubs", 5, 0));
            Cards.Add(new Card("10", "clubs", 6, 10));
            Cards.Add(new Card("J", "clubs", 7, 0));
            Cards.Add(new Card("Q", "clubs", 8, 0));
            Cards.Add(new Card("K", "clubs", 9, 0));
            Cards.Add(new Card("A", "clubs", 10, 10));
            Cards.Add(new Card("5", "diamonds", 1, 5)); // diamonds
            Cards.Add(new Card("6", "diamonds", 2, 0));
            Cards.Add(new Card("7", "diamonds", 3, 0));
            Cards.Add(new Card("8", "diamonds", 4, 0));
            Cards.Add(new Card("9", "diamonds", 5, 0));
            Cards.Add(new Card("10", "diamonds", 6, 10));
            Cards.Add(new Card("J", "diamonds", 7, 0));
            Cards.Add(new Card("Q", "diamonds", 8, 0));
            Cards.Add(new Card("K", "diamonds", 9, 0));
            Cards.Add(new Card("A", "diamonds", 10, 10));
            Cards.Add(new Card("5", "hearts", 1, 5)); // hearts
            Cards.Add(new Card("6", "hearts", 2, 0));
            Cards.Add(new Card("7", "hearts", 3, 0));
            Cards.Add(new Card("8", "hearts", 4, 0));
            Cards.Add(new Card("9", "hearts", 5, 0));
            Cards.Add(new Card("10", "hearts", 6, 10));
            Cards.Add(new Card("J", "hearts", 7, 0));
            Cards.Add(new Card("Q", "hearts", 8, 0));
            Cards.Add(new Card("K", "hearts", 9, 0));
            Cards.Add(new Card("A", "hearts", 10, 10));
            Cards.Add(new Card("5", "spades", 1, 5)); // spades
            Cards.Add(new Card("6", "spades", 2, 0));
            Cards.Add(new Card("7", "spades", 3, 0));
            Cards.Add(new Card("8", "spades", 4, 0));
            Cards.Add(new Card("9", "spades", 5, 0));
            Cards.Add(new Card("10", "spades", 6, 10));
            Cards.Add(new Card("J", "spades", 7, 0));
            Cards.Add(new Card("Q", "spades", 8, 0));
            Cards.Add(new Card("K", "spades", 9, 0));
            Cards.Add(new Card("A", "spades", 10, 10));
            */

        }
    
        public void ShuffleDeck(int shuffleCount) // method to shuffle deck
        {
            Console.WriteLine("Shuffling cards...");
            Thread.Sleep(200);
            for (int s = 0; s < shuffleCount; s++)
            {
                for (int i = 0; i < Cards.Count; i++)
                {
                    int randomCardIndex = random.Next(i, Cards.Count);
                    Card temp = Cards[randomCardIndex];
                    Cards[randomCardIndex] = Cards[i];
                    Cards[i] = temp;
                }
            }
        }
    }
}