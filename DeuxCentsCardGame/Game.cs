namespace DeuxCentsCardGame
{
    public class Game
    {
        // List<Card> deck = new List<Card>();
        private Deck deck;
        private List<Player> players;
        private int highestBid;
        private int winningPlayerIndex;
        private string trumpSuit;
        private int teamOnePoints = 0;
        private int teamTwoPoints = 0;


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
            Console.WriteLine("Shuffling and dealing cards...");
            // shuffle deck 3 times
            deck.ShuffleDeck();
            deck.ShuffleDeck();
            deck.ShuffleDeck();
            
            // deal cards to all (Player) players
            DealCards();

            // to display hands normally
            //players[0].DisplayHand();
            //players[1].DisplayHand();
            //players[2].DisplayHand();
            //players[3].DisplayHand();

            // diplay hands using quadrants, could be optimized by putting all in method
            Player.DisplayPlayerHand(players[0], 0, 0);
            Console.WriteLine("\n#########################\n");
            Player.DisplayPlayerHand(players[1], Console.WindowWidth / 2, 0);
            Console.WriteLine("\n#########################\n");
            Player.DisplayPlayerHand(players[2], 0, Console.WindowHeight / 2);
            Console.WriteLine("\n#########################\n");
            Player.DisplayPlayerHand(players[3], Console.WindowWidth / 2, Console.WindowHeight / 2); 
            Console.WriteLine("\n#########################\n");

            // Betting round
            BettingRound();
            Console.WriteLine("\n#########################\n");
            Player.DisplayHand(players[winningPlayerIndex]);
            Console.WriteLine();

            // Trump suit selection
            SelectTrumpSuit();

            PlayRound();

            // left off here
            // next steps..
            // PlayRound() // display points within method

            // begin initialize game variable/object
            // go though game logic
        }

        private void DealCards()
        {
            for (int i = 0; i < deck.Cards.Count; i++)
            {
                players[i % 4].AddCard(deck.Cards[i]);
            }
        }

        private void BettingRound()
        {
            Console.WriteLine("Betting round begins!\n");
            List<int> bets = new List<int>(); // store bets
            bool[] hasBet = new bool[players.Count]; // track if a player has ever placed a bet
            bool[] hasPassed = new bool[players.Count]; // track if a player has passed
            bool bettingRoundEnded = false;

            while (!bettingRoundEnded)
            {
                for (int i = 0; i < players.Count; i++)
                {
                    if (hasPassed[i])
                    {
                        continue; // Skip players who have already passed
                    }

                    Console.WriteLine($"{players[i].Name}, enter a bet (between 50-100, intervals of 5) or 'pass': ");
                    string betInput = Console.ReadLine().ToLower();

                    if (betInput == "pass")
                    {
                        Console.WriteLine($"{players[i].Name} passed\n");
                        hasPassed[i] = true;
                        if (bets.Count <= i)
                            bets.Add(-1);
                        else
                            bets[i] = -1;
                    }
                    else if (int.TryParse(betInput, out int bet))
                    {
                        if (bet >= 50 && bet <= 100 && bet % 5 == 0 && !bets.Contains(bet))
                        {
                            if (bets.Count <= i)
                            {
                                bets.Add(bet);
                            }
                            else
                            {
                                bets[i] = bet;
                            }
                            hasBet[i] = true;
                            Console.WriteLine();

                            // Check if the bet is 100
                            if (bet == 100)
                            {
                                bettingRoundEnded = true; // End the betting round immediately
                                Console.WriteLine($"{players[i].Name} bet 100. Betting round ends.\n");

                                // Set all subsequent players who haven't placed a bet to -1
                                for (int j = i + 1; j < players.Count; j++)
                                {
                                    if (j != i && !hasPassed[j])
                                    {
                                        hasPassed[j] = true;
                                        if (bets.Count <= j)
                                            bets.Add(-1);
                                        else
                                            bets[j] = -1;
                                        Console.WriteLine($"{players[j].Name} automatically passes.");
                                    }
                                }
                                break; // Exit the for loop
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid bet");
                            i--;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid input");
                        i--;
                    }
                    // End the betting round if 3 players have passed
                    if (hasPassed.Count(p => p) >= 3)
                    {
                        Console.WriteLine("Betting round ends");
                        // Inserting default bet of 50 to player 4 if all prior players passed
                        if (bets.Count == 4)
                        {
                            bettingRoundEnded = true;
                            break;
                        }
                        else
                        {
                            bets.Add(50);
                            hasBet[i + 1] = true;
                            bettingRoundEnded = true;
                            break;
                        }
                    }
                }
            }

            // Showing betting results, might not need this
            Console.WriteLine("\nBetting round complete, here are the results:");
            for (int i = 0; i < players.Count; i++)
            {
                string result;
                if (hasPassed[i])
                {
                    result = hasBet[i] ? "Passed after betting" : "Passed";
                }
                else
                {
                    result = $"Bet {bets[i]}";
                }
                Console.WriteLine($"{players[i].Name} : {result}");
            }

            highestBid = bets.Max();
            winningPlayerIndex = bets.IndexOf(highestBid);
            Console.WriteLine();
            Console.WriteLine($"{players[winningPlayerIndex].Name} won the bid.");
        }

        private void SelectTrumpSuit()
        {
            Console.WriteLine($"{players[winningPlayerIndex].Name}, please choose a trump suit. (enter \"clubs\", \"diamonds\", \"hearts\", \"spades\")");
            trumpSuit = Console.ReadLine().ToLower();

            while (trumpSuit != "clubs" && trumpSuit != "diamonds" && trumpSuit != "hearts" && trumpSuit != "spades")
            {
                Console.WriteLine($"{trumpSuit} is an invalid input, please try again.");
                trumpSuit = Console.ReadLine().ToLower();
            }

            Console.WriteLine();
            Console.WriteLine($"Trump suit is {trumpSuit}.");
        }

        private void PlayRound()
        {
            int currentPlayerIndex = winningPlayerIndex;
            string leadingSuit = null;
            int playerIndex;
            Player currentPlayer;
            bool validInput = false;
            int cardIndex = -1; // initializing invalid input
            int trickPoints = 0;

            // for the 10 tricks
            for (int trick = 0; trick < 10; trick++)
            {
                Console.WriteLine("\n#########################\n");
                Console.WriteLine($"Trick #{trick + 1}:");

                List<Card> currentTrick = new List<Card>(); // empty list to hold tricks

                for (int i = 0; i < 4; i++)
                {
                    playerIndex = (currentPlayerIndex + i) % 4; // ensuring player who won the bet goes first
                    currentPlayer = players[playerIndex];

                    Player.DisplayHand(currentPlayer); // display current players hand

                    // adding loop to validate user input
                    while (!validInput)
                    {
                        Console.WriteLine($"{currentPlayer.Name}, choose a card to play (enter index 0-{currentPlayer.Hand.Count - 1}, leading suit is {leadingSuit} and trump suit is {trumpSuit}):");
                        string Input = Console.ReadLine();

                        if (int.TryParse(Input, out cardIndex) && cardIndex < currentPlayer.Hand.Count && cardIndex >= 0)
                        {
                            if (i == 0)
                            {
                                validInput = true;
                            }
                            else
                            {
                                if (currentPlayer.Hand[cardIndex].cardSuit != leadingSuit && currentPlayer.Hand.Any(card => card.cardSuit == leadingSuit))
                                {
                                    Console.WriteLine();
                                    Console.WriteLine($"You must play a card of ({leadingSuit}) since its in your deck, try again.");
                                    Console.WriteLine();
                                }
                                else
                                {
                                    validInput = true;
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine($"{cardIndex} is an invalid input, please try again.");
                        }
                    }

                    Card playedCard = currentPlayer.Hand[cardIndex];
                    currentPlayer.RemoveCard(playedCard);

                    if (i == 0)
                    {
                        leadingSuit = playedCard.cardSuit;
                    }

                    currentTrick.Add(playedCard);
                    Console.WriteLine($"{currentPlayer.Name} played {playedCard}");
                }

                // determine the winner of the trick
                winningPlayerIndex = DetermineTrickWinnerIndex(currentTrick, trumpSuit);
                currentPlayerIndex = (currentPlayerIndex + winningPlayerIndex) % 4;
                Console.WriteLine($"{players[currentPlayerIndex].Name} won the trick with {currentTrick[winningPlayerIndex]}");

                // adding all points to trickPoints
                trickPoints = currentTrick.Sum(card => card.cardPointValue);
                if (currentPlayerIndex == 0 || currentPlayerIndex == 2)
                {
                    Console.WriteLine($"{players[currentPlayerIndex].Name} collected {trickPoints} points for Team 1");
                    teamOnePoints += trickPoints;
                }
                else
                {
                    Console.WriteLine($"{players[currentPlayerIndex].Name} collected {trickPoints} points for Team 2");
                    teamTwoPoints += trickPoints;
                }
            }
        }

        private int DetermineTrickWinnerIndex(List<Card> trick, string trumpSuit)
        {
            winningPlayerIndex = 0;

            for (int i = 1; i < trick.Count; i++)
            {
                // Check if the current card is a trump card AND the winning card is not a trump card
                if (trick[i].cardSuit == trumpSuit && trick[winningPlayerIndex].cardSuit != trumpSuit)
                {
                    winningPlayerIndex = i;
                }
                // Check if both cards are trump cards or both are not trump cards
                else if (trick[i].cardSuit == trick[winningPlayerIndex].cardSuit)
                {
                    if (trick[i].cardFaceValue > trick[winningPlayerIndex].cardFaceValue)
                    {
                        winningPlayerIndex = i;
                    }
                }
            }

            return winningPlayerIndex;    
        }
    }
}