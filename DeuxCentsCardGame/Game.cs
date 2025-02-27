namespace DeuxCentsCardGame
{
    public class Game
    {
        // List<Card> deck = new List<Card>();
        private Deck deck;

        public Game()
        {
            deck = new Deck();
        }

        public void Start()
        {
            // shuffle deck 3 times
            deck.ShuffleDeck();
            deck.ShuffleDeck();
            deck.ShuffleDeck();

            // DealCards()
            // DisplayHand() // x4 for each player
            // BettingRound() // display result within method
            // DisplayHand() // bet winner's hand
            // SelectTrumpSuit // display trump selection within method
            // PlayRound() // display points within method


            // left off here
            // begin initialize game varaible/object
            // go though game logic

        }
    }
}