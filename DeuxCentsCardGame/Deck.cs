namespace DeuxCentsCardGame
{
    public class Deck
    {
        public List<Card> Cards;
        static readonly Random random = new ();

        private static readonly string[] cardSuits = ["clubs", "diamonds", "hearts", "spades"];
        private static readonly string[] cardFaces = ["5", "6", "7", "8", "9", "10", "J", "Q", "K", "A"];
        private static readonly int[] cardfaceValues = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];
        private static readonly int[] cardPointValues = [5, 0, 0, 0, 0, 10, 0, 0, 0, 10];

        public Deck()
        {
            Cards = [];
            InitializeCards();
        }
        private void InitializeCards()
        {
            foreach (string suit in cardSuits)
            {
                for (int c = 0; c < cardFaces.Length; c++)
                {
                    Cards.Add(new Card(cardFaces[c], suit, cardfaceValues[c], cardPointValues[c]));
                }
            }
        }
    
        public void ShuffleDeck(int shuffleCount) // method to shuffle deck
        {
            Console.WriteLine("Shuffling cards...");
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
            Thread.Sleep(200);
        }

        public static string[] GetCardSuits()
        {
            return cardSuits;
        }
    }
}