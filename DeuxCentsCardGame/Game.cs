namespace DeuxCentsCardGame
{
    public class Game
    {
        private Deck deck;
        private readonly List<Player> players;
        private bool gameEnded = false;
        private bool[] hasBet;
        private int dealerIndex = 3; // player 4 will start as the dealer
        private int winningBid;
        private int winningBidIndex;
        private int winningPlayerIndex;
        private string trumpSuit;
        private int teamOneRoundPoints = 0;
        private int teamTwoRoundPoints = 0;
        private int teamOneTotalPoints;
        private int teamTwoTotalPoints;
        public const int shuffleCount = 3;
        private const int WinningScore = 200;
        private const int MinimumBet = 50;
        private const int MaximumBet = 100;
        private const int BetIncrement = 5;

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
                deck.ShuffleDeck(ShuffleCount);
                DealCards();
                DisplayAllHands();
                BettingRound();
                SelectTrumpSuit();
                PlayRound();
                UpdateTotalPoints();
                EndGameCheck();
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
        private void DisplayAllHands()
        {
            Player.DisplayAllPlayersHandQuadrant(players[(dealerIndex) % players.Count],
                                                 players[(dealerIndex + 1) % players.Count],
                                                 players[(dealerIndex + 2) % players.Count],
                                                 players[(dealerIndex + 3) % players.Count]);
        }

        private void DisplayAllHands()
        {
            Player.DisplayAllPlayersHandQuadrant(players[(dealerIndex) % players.Count], 
                                                 players[(dealerIndex + 1) % players.Count], 
                                                 players[(dealerIndex + 2) % players.Count], 
                                                 players[(dealerIndex + 3) % players.Count]);
        }

        public void BettingRound()
        {
            Console.WriteLine("Betting round begins!\n");
            List<int> bets = new List<int>(new int[players.Count]);
            bool[] hasPassed = new bool[players.Count];
            bool bettingRoundEnded = false;
            hasBet = new bool[players.Count]; // track if a player has placed a bet for over 100 scoring purposes

            int startingIndex = dealerIndex % players.Count;

            while (!bettingRoundEnded)
            {
                for (int i = 0; i < players.Count; i++)
                {
                    int playerIndex = (startingIndex + i) % players.Count;

                    if (hasPassed[playerIndex])
                    {
                        continue; // Skip players who have already passed
                    }

                    Console.WriteLine($"{players[playerIndex].Name}, enter a bet (between {MinimumBet}-{MaximumBet}, intervals of {BetIncrement}) or 'pass': ");
                    string betInput = Console.ReadLine().ToLower();

                    if (betInput == "pass")
                    {
                        ProcessPassInput(playerIndex, hasPassed, bets);
                    }
                    else if (int.TryParse(betInput, out int bet) && IsValidBet(bet, bets))
                    {
                        bets[playerIndex] = bet;
                        hasBet[playerIndex] = true;
                        Console.WriteLine();

                        if (bet == MaximumBet)
                        {
                            bettingRoundEnded = HandleMaximumBet(playerIndex, hasPassed, bets); // End the betting round immediately
                            break;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid bet, please try again");
                        playerIndex--;
                    }

                    if (hasPassed.Count(p => p) >= 3) // End the betting round if 3 players have passed
                    {
                        bettingRoundEnded = HandleThreePlayerPassed(playerIndex, hasPassed);
                        break;
                    }
                }
            }

            DisplayBettingResults(hasPassed, bets);
            DetermineWinningBid(bets);
        }

        private void ProcessPassInput(int playerIndex, bool[] hasPassed, List<int> bets)
        {
            Console.WriteLine();
            Console.WriteLine($"{players[playerIndex].Name} passed");
            Console.WriteLine();
            hasPassed[playerIndex] = true;
            bets[playerIndex] = -1;
        }

        private bool IsValidBet(int bet, List<int> bets)
        {
            return bet >= MinimumBet && bet <= MaximumBet && bet % BetIncrement == 0 && !bets.Contains(bet);
        }

        private bool HandleMaximumBet(int playerIndex, bool[] hasPassed, List<int> bets)
        {
            Console.WriteLine($"{players[playerIndex].Name} bet {MaximumBet}. Betting round ends.\n");
            for (int j = 0; j < players.Count; j++)
            {
                if (j != playerIndex && !hasPassed[j])
                {
                    hasPassed[j] = true;
                    bets[j] = -1;
                }
            }
            return true;
        }

        private bool HandleThreePlayerPassed(int playerIndex, bool[] hasBet)
        {
            Console.WriteLine("Three players have passed, betting round ends");
            if (!hasBet.Any(b => b)) // Inserting default bet of 50 to player 4 if all prior players passed
            {
                int lastPlayerIndex = (playerIndex + 1) % players.Count;
                hasBet[lastPlayerIndex] = true;
                winningBid = MinimumBet;
                winningBidIndex = lastPlayerIndex;
            }
            return true;
        }

        private void DisplayBettingResults(bool[] hasPassed, List<int> bets)
        {
            Console.WriteLine();
            Console.WriteLine("Betting round complete, here are the results:");
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
        }

        private void DetermineWinningBid(List<int> bets)
        {
            winningBid = bets.Max();
            winningBidIndex = bets.IndexOf(winningBid);
            Console.WriteLine();
            Console.WriteLine($"{players[winningBidIndex].Name} won the bid.");
            Console.WriteLine("\n#########################\n");
            Player.DisplayHand(players[winningBidIndex]);
        }

        private void SelectTrumpSuit()
        {
            string[] validSuits = { "clubs", "diamonds", "hearts", "spades" };
            Console.WriteLine($"{players[winningBidIndex].Name}, please choose a trump suit. (enter \"clubs\", \"diamonds\", \"hearts\", \"spades\")");
            trumpSuit = Console.ReadLine().ToLower();

            while (!validSuits.Contains(trumpSuit))
            {
                Console.WriteLine($"{trumpSuit} is an invalid input, please try again.");
                trumpSuit = Console.ReadLine().ToLower();
            }

            Console.WriteLine();
            Console.WriteLine($"Trump suit is {trumpSuit}.");
        }

        private void PlayRound()
        {
            int currentPlayerIndex = winningBidIndex;
            
            int totalTricks = players[currentPlayerIndex].Hand.Count;

            for (int trick = 0; trick < totalTricks; trick++)
            {
                PlayTrick(trick, currentPlayerIndex);
            }
        }

        // ############ left off here, bug in determining trick winner ############
        private void PlayTrick(int trick, int currentPlayerIndex)
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

                    Card playedCard = ValidateCardInput(currentPlayer, cardIndex, leadingSuit);
                    currentPlayer.RemoveCard(playedCard);


                    if (i == 0)
                    {
                        leadingSuit = playedCard.CardSuit;
                    }

                    currentTrick.Add(playedCard);
                    Console.WriteLine($"{currentPlayer.Name} played {playedCard}");
                    Console.WriteLine();
                }

                winningPlayerIndex = DetermineTrickWinnerIndex(currentTrick, trumpSuit);
                trickWinnerIndex = (currentPlayerIndex + winningPlayerIndex) % 4;
                Console.WriteLine($"{players[trickWinnerIndex].Name} won the trick with {currentTrick[winningPlayerIndex]}");

                currentPlayerIndex = trickWinnerIndex; // set winning player as the current player for the next trick
 
                trickPoints = currentTrick.Sum(card => card.CardPointValue); // adding all points to trickPoints
                UpdateTrickPoints(trickWinnerIndex, trickPoints);
            }
        }

        private Card ValidateCardInput(Player currentPlayer, int cardIndex, string leadingSuit)
        {
            bool validInput = false;

            while (!validInput)
            {
                Player.DisplayHand(currentPlayer); // display current players hand
                Console.WriteLine($"{currentPlayer.Name}, choose a card to play (enter index 0-{currentPlayer.Hand.Count - 1}, leading suit is {leadingSuit} and trump suit is {trumpSuit}):");
                string Input = Console.ReadLine();

                if (int.TryParse(Input, out cardIndex) && cardIndex < currentPlayer.Hand.Count && cardIndex >= 0)
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
                else
                {
                    Console.WriteLine($"{cardIndex} is an invalid input, please try again.");
                }
            }
            return currentPlayer.Hand[cardIndex];
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

        private void UpdateTrickPoints(int trickWinnerIndex, int trickPoints)
        {
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

        private void UpdateTotalPoints() // tally points and end the round
        {
            Console.WriteLine("\nEnd of round. Scoring:");
            Console.WriteLine($"Team One (Player 1 & Player 3) scored : {teamOneRoundPoints}");
            Console.WriteLine($"Team Two (Player 2 & Player 4) scored : {teamTwoRoundPoints}");

            if (winningBidIndex % 2 == 0) // team one won the bet
            {
                UpdateTeamOnePoints();
            }
            else // team two won the bet
            {
                UpdateTeamTwoPoints();
            }

            Console.WriteLine();
            Console.WriteLine($"Team One has a total of {teamOneTotalPoints} points");
            Console.WriteLine($"Team Two has a total of {teamTwoTotalPoints} points");
        }

        private void UpdateTeamOnePoints()
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
                teamOneTotalPoints -= winningBid;
                teamTwoTotalPoints += teamTwoRoundPoints;
            }
        }

        private void UpdateTeamTwoPoints()
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
                teamTwoTotalPoints -= winningBid;
                teamOneTotalPoints += teamOneRoundPoints;
            }
        }

        private void EndGameCheck()
        {
            if (teamOneTotalPoints >= WinningScore || teamTwoTotalPoints >= WinningScore)
            {
                Console.WriteLine("\n#########################\n");
                Console.WriteLine("Game over!");
                Console.WriteLine($"Team One finished with {teamOneTotalPoints} points");
                Console.WriteLine($"Team Two finished with {teamTwoTotalPoints} points");
                gameEnded = true;
            }
            else
            {
                Console.WriteLine("\nPress any key to start the next round...");
                Console.ReadKey();
            }
        }
    }
}