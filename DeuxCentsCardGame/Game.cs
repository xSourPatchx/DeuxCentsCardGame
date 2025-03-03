namespace DeuxCentsCardGame
{
    public class Game
    {
        // List<Card> deck = new List<Card>();
        private Deck deck;
        private List<Player> players;
        List<Card>[] playersDecks;

        public Game()
        {
            deck = new Deck();
            players = new List<Player>
            {
                new Player("Player 1"),
                new Player("Player 2"),
                new Player("Player 3"),
                new Player("Player 4"),
            }; 
        }

        public void Start()
        {
            // shuffle deck 3 times
            Console.WriteLine("Shuffling and dealing cards...");
            deck.ShuffleDeck();
            deck.ShuffleDeck();
            deck.ShuffleDeck();

            // deal cards
            //for (int i = 0; i < deck.Cards.Count; i++)
            //{
            //    players[i % 4].AddCard(deck.Cards[i]);
            //}

            DealCards();

            // display hands normally
            //players[0].DisplayHand();
            //players[1].DisplayHand();
            //players[2].DisplayHand();
            //players[3].DisplayHand();

            // diplay hands using quadrants
            Player.DisplayPlayerHand(players[0], 0, 0);
            Player.DisplayPlayerHand(players[1], Console.WindowWidth / 2, 0);
            Player.DisplayPlayerHand(players[2], 0, Console.WindowHeight / 2);
            Player.DisplayPlayerHand(players[3], Console.WindowWidth / 2, Console.WindowHeight / 2);




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

        public void DealCards()
        {
            for (int i = 0; i < deck.Cards.Count; i++)
            {
                players[i % 4].AddCard(deck.Cards[i]);
            }
        }
    }
}