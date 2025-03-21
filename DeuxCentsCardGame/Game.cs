using Microsoft.Win32.SafeHandles;

namespace DeuxCentsCardGame
{
    public class Game
    {
        private Deck deck;
        private List<Player> players;
        private bool gameEnded = false;
        private bool[] hasBet;
        private int dealerIndex = 3; // player 4 will start as the dealer
        private int winningBid;
        private int winningBidIndex;
        private int winningPlayerIndex;
        private string trumpSuit;
        private int teamOneRoundPoints = 0;
        private int teamTwoRoundPoints = 0;
        public int shuffleCount = 3;
        private int teamOneTotalPoints;
        private int teamTwoTotalPoints;


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
            while (!gameEnded)
            {
                Console.Clear();
                Console.WriteLine("Starting a new round...");
                ResetRound();
                deck.ShuffleDeck(shuffleCount);
                DealCards();
                Player.DisplayAllPlayersHandQuadrant(players[(dealerIndex + 0) % players.Count], 
                                                     players[(dealerIndex + 1) % players.Count], 
                                                     players[(dealerIndex + 2) % players.Count], 
                                                     players[(dealerIndex + 3) % players.Count]);
                BettingRound();
                SelectTrumpSuit();
                PlayRound();
                UpdateTotalPoints();
                EndGame();
            }
        }

        private void ResetRound()
        {
            deck = new Deck();
            teamOneRoundPoints = 0;
            teamTwoRoundPoints = 0;
            winningBid = 0;
            winningBidIndex = 0;
            RotateDealer();
        }

        private void RotateDealer()
        {
            dealerIndex = (dealerIndex + 1) % players.Count;
        }

        private void DealCards()
        {
            foreach (Player player in players)
            {
                player.Hand.Clear();
            }

            Console.WriteLine("Dealing cards...");
            for (int i = 0; i < deck.Cards.Count; i++)
            {
                players[i % players.Count].AddCard(deck.Cards[i]);
            }
        }

        // NEED TO FIX BETTING ROUND BUG
        public void BettingRound()
        {
            Console.WriteLine("Betting round begins!\n");
            List<int> bets = new List<int>(); // store bets
            bool[] hasPassed = new bool[players.Count]; // track if a player has passed
            bool bettingRoundEnded = false;
            hasBet = new bool[players.Count]; // track if a player has placed a bet

            int startingIndex = (dealerIndex) % players.Count;

            while (!bettingRoundEnded)
            {
                for (int i = 0; i < players.Count; i++)
                {
                    int playerIndex = (startingIndex + i) % players.Count;

                    if (hasPassed[playerIndex])
                    {
                        continue; // Skip players who have already passed
                    }

                    Console.WriteLine($"{players[playerIndex].Name}, enter a bet (between 50-100, intervals of 5) or 'pass': ");
                    string betInput = Console.ReadLine().ToLower();

                    if (betInput == "pass")
                    {
                        Console.WriteLine($"{players[playerIndex].Name} passed\n");
                        hasPassed[playerIndex] = true;
                        if (bets.Count <= playerIndex)
                            bets.Add(-1);
                        else
                            bets[playerIndex] = -1;
                    }
                    else if (int.TryParse(betInput, out int bet))
                    {
                        if (bet >= 50 && bet <= 100 && bet % 5 == 0 && !bets.Contains(bet))
                        {
                            if (bets.Count <= playerIndex)
                            {
                                bets.Add(bet);
                            }
                            else
                            {
                                bets[playerIndex] = bet;
                            }
                            hasBet[playerIndex] = true;
                            Console.WriteLine();

                            // Check if the bet is 100
                            if (bet == 100)
                            {
                                bettingRoundEnded = true; // End the betting round immediately
                                Console.WriteLine($"{players[playerIndex].Name} bet 100. Betting round ends.\n");

                                // Set all subsequent players who haven't placed a bet to -1
                                for (int j = playerIndex + 1; j < players.Count; j++)
                                {
                                    if (j != playerIndex && !hasPassed[j])
                                    {
                                        hasPassed[j] = true;
                                        if (bets.Count <= j)
                                            bets.Add(-1);
                                        else
                                            bets[j] = -1;
                                    }
                                }
                                break; // Exit the for loop
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid bet");
                            playerIndex--;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid input");
                        playerIndex--;
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
                            hasBet[playerIndex + 1] = true;
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

            winningBid = bets.Max();
            winningBidIndex = bets.IndexOf(winningBid);
            Console.WriteLine();
            Console.WriteLine($"{players[winningBidIndex].Name} won the bid.");
            Console.WriteLine("\n#########################\n");
            Player.DisplayHand(players[winningBidIndex]);
        }

        private void SelectTrumpSuit()
        {
            Console.WriteLine($"{players[winningBidIndex].Name}, please choose a trump suit. (enter \"clubs\", \"diamonds\", \"hearts\", \"spades\")");
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
            int playerIndex;
            Player currentPlayer;
            int currentPlayerIndex;
            int trickWinnerIndex;
            int totalTricks; 

            currentPlayerIndex = winningBidIndex;
            
            totalTricks = players[currentPlayerIndex].Hand.Count;

            for (int trick = 0; trick < totalTricks; trick++)
            {
                int trickPoints = 0;
                string leadingSuit = null;
                List<Card> currentTrick = new List<Card>(); // empty list to hold tricks

                Console.WriteLine("\n#########################\n");
                Console.WriteLine($"Trick #{trick + 1}:");

                for (int i = 0; i < 4; i++)
                {
                    playerIndex = (currentPlayerIndex + i) % 4; // ensuring player who won the bet goes first
                    currentPlayer = players[playerIndex];
                    bool validInput = false;
                    int cardIndex = -1; // initializing invalid input

                    // adding loop to validate user input
                    while (!validInput)
                    {
                        Player.DisplayHand(currentPlayer); // display current players hand
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
                                if (currentPlayer.Hand[cardIndex].CardSuit != leadingSuit && currentPlayer.Hand.Any(card => card.CardSuit == leadingSuit))
                                {
                                    Console.WriteLine();
                                    Console.WriteLine($"You must play the suit of {leadingSuit} since it's in your deck, try again.");
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
                        leadingSuit = playedCard.CardSuit;
                    }

                    currentTrick.Add(playedCard);
                    Console.WriteLine($"{currentPlayer.Name} played {playedCard}");
                    Console.WriteLine();
                }

                // determine the winner of the trick
                winningPlayerIndex = DetermineTrickWinnerIndex(currentTrick, trumpSuit);
                trickWinnerIndex = (currentPlayerIndex + winningPlayerIndex) % 4;
                Console.WriteLine($"{players[trickWinnerIndex].Name} won the trick with {currentTrick[winningPlayerIndex]}");
                currentPlayerIndex = trickWinnerIndex;

                // adding all points to trickPoints
                trickPoints = currentTrick.Sum(card => card.CardPointValue);
                if (trickWinnerIndex == 0 || trickWinnerIndex == 2)
                {
                    Console.WriteLine($"{players[trickWinnerIndex].Name} collected {trickPoints} points for Team 1");
                    teamOneRoundPoints += trickPoints;
                }
                else
                {
                    Console.WriteLine($"{players[trickWinnerIndex].Name} collected {trickPoints} points for Team 2");
                    teamTwoRoundPoints += trickPoints;
                }
            }
        }

        private int DetermineTrickWinnerIndex(List<Card> trick, string trumpSuit)
        {
            winningPlayerIndex = 0;

            for (int i = 1; i < trick.Count; i++)
            {
                // Check if the current card is a trump card AND the winning card is not a trump card
                if (trick[i].CardSuit == trumpSuit && trick[winningPlayerIndex].CardSuit != trumpSuit)
                {
                    winningPlayerIndex = i;
                }
                // Check if both cards are trump cards or both are not trump cards
                else if (trick[i].CardSuit == trick[winningPlayerIndex].CardSuit)
                {
                    if (trick[i].CardFaceValue > trick[winningPlayerIndex].CardFaceValue)
                    {
                        winningPlayerIndex = i;
                    }
                }
            }

            return winningPlayerIndex;    
        }

        private void UpdateTotalPoints() // tally points and end the round
        {
            Console.WriteLine("\nEnd of round. Scoring:");
            Console.WriteLine($"Team One (Player 1 & Player 3) scored : {teamOneRoundPoints}");
            Console.WriteLine($"Team Two (Player 2 & Player 4) scored : {teamTwoRoundPoints}");

            // Check if the betting team made their bet
            if (winningBidIndex == 0 || winningBidIndex == 2) // team one won the bet
            {
                if (teamTwoTotalPoints >= 100 && (!hasBet[0] || !hasBet[2]))
                {
                    Console.WriteLine($"Team One did not place any bets, their points do not count.");
                    teamTwoRoundPoints = 0;
                }

                if (teamOneRoundPoints >= winningBid)
                {
                    Console.WriteLine($"Team One made their bet of {winningBid} and wins {teamOneRoundPoints} points.");
                    teamOneTotalPoints += teamOneRoundPoints;
                    if (teamTwoTotalPoints < 100)
                    {
                        teamTwoTotalPoints += teamTwoRoundPoints;
                    }
                }
                else
                {
                    Console.WriteLine($"Team One did not make their bet of {winningBid} and loses {winningBid} points.");
                    teamOneRoundPoints = winningBid * -1;
                    teamOneTotalPoints += teamOneRoundPoints;
                    teamTwoTotalPoints += teamTwoRoundPoints;
                 }
            }
            else // team two won the bet
            {
                if (teamOneTotalPoints >= 100 && (!hasBet[1] || !hasBet[3]))
                {
                    Console.WriteLine($"Team Two did not place any bets, their points do not count.");
                    teamTwoRoundPoints = 0;
                }

                if (teamTwoRoundPoints >= winningBid)
                {
                    Console.WriteLine($"Team Two made their bet of {winningBid} and wins {teamTwoRoundPoints} points.");
                    teamTwoTotalPoints += teamTwoRoundPoints;

                    if (teamOneTotalPoints < 100)
                    {
                        teamOneTotalPoints += teamOneRoundPoints;
                    }
                }
                else
                {
                    Console.WriteLine($"Team Two did not make their bet of {winningBid} and loses {winningBid} points.");
                    teamTwoRoundPoints = winningBid * -1;
                    teamTwoTotalPoints += teamTwoRoundPoints;
                    teamOneTotalPoints += teamOneRoundPoints;
                }
            }

            Console.WriteLine();
            Console.WriteLine($"Team One has a total of {teamOneTotalPoints} points");
            Console.WriteLine($"Team Two has a total of {teamTwoTotalPoints} points");
        }

        private void EndGame()
        {
            if (teamOneTotalPoints >= 200 || teamTwoTotalPoints >= 200)
            {
                Console.WriteLine("\n#########################\n");
                Console.WriteLine("Game over!");
                Console.WriteLine($"Team One finished with {teamOneTotalPoints} points");
                Console.WriteLine($"Team Two finished with {teamTwoTotalPoints} points");
                gameEnded = true;
            }
            else
            {
                //RotateDealer();
                Console.WriteLine("\nPress any key to start the next round...");
                Console.ReadKey();
            }
        }
    }
}