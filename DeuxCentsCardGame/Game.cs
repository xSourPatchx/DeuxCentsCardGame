namespace DeuxCentsCardGame
{
    public class Game
    {
        public const int ShuffleCount = 3;
        private const int WinningScore = 200;
        private const int MinimumBet = 50;
        private const int MaximumBet = 100;
        private const int BetIncrement = 5;

        private Deck _deck;
        private readonly List<Player> _players;
        private bool _gameEnded;
        private bool[] _hasBet;
        private int _dealerIndex;
        private int _winningBid;
        private int _winningBidIndex;
        private int _winningPlayerIndex;
        private string _trumpSuit;
        private int _teamOneRoundPoints;
        private int _teamTwoRoundPoints;
        private int _teamOneTotalPoints;
        private int _teamTwoTotalPoints;

        public Game()
        {
            _deck = new Deck();
            _players =
            [
                new("Player 1"),
                new("Player 2"),
                new("Player 3"),
                new("Player 4"),
            ];
        }

        public void Start()
        {
            while (!_gameEnded)
            {
                NewGame();
            }
        }

        public void NewGame()
        {
            Console.Clear();
            Console.WriteLine("Starting a new round...");
            ResetRound();
            _deck.ShuffleDeck(ShuffleCount);
            DealCards();
            DisplayAllHands();
            BettingRound();
            SelectTrumpSuit();
            PlayRound();
            UpdateTotalPoints();
            EndGameCheck();
        }

        private void ResetRound()
        {
            _deck = new Deck();
            _gameEnded = false;
            _dealerIndex = 3; // player 4 will start as the dealer
            _teamOneRoundPoints = 0;
            _teamTwoRoundPoints = 0;
            _winningBid = 0;
            _winningBidIndex = 0;
            RotateDealer();
        }

        private void RotateDealer()
        {
            _dealerIndex = (_dealerIndex + 1) % _players.Count;
        }

        private void DealCards()
        {
            foreach (Player player in _players)
            {
                player.Hand.Clear();
            }

            Console.WriteLine("Dealing cards...");
            for (int i = 0; i < _deck.Cards.Count; i++)
            {
                _players[i % _players.Count].AddCard(_deck.Cards[i]);
            }
        }
        private void DisplayAllHands()
        {
            Player.DisplayAllPlayersHandQuadrant(_players[(_dealerIndex) % _players.Count],
                                                 _players[(_dealerIndex + 1) % _players.Count],
                                                 _players[(_dealerIndex + 2) % _players.Count],
                                                 _players[(_dealerIndex + 3) % _players.Count]);
        }

        public void BettingRound()
        {
            Console.WriteLine("Betting round begins!\n");
            List<int> bets = new(new int[_players.Count]);
            bool[] hasPassed = new bool[_players.Count];
            bool bettingRoundEnded = false;
            _hasBet = new bool[_players.Count]; // track if a player has placed a bet for over 100 scoring purposes

            int startingIndex = _dealerIndex % _players.Count;

            while (!bettingRoundEnded)
            {
                bettingRoundEnded = ProcessBettingRound(startingIndex, hasPassed, bets);
            }

            DisplayBettingResults(hasPassed, bets);
            DetermineWinningBid(bets);
        }

        private bool ProcessBettingRound(int startingIndex, bool[] hasPassed, List<int> bets)
        {
            for (int i = 0; i < _players.Count; i++)
                {
                    int playerIndex = (startingIndex + i) % _players.Count;

                    if (hasPassed[playerIndex])
                        continue; // Skip players who have already passed
                    
                    ProcessPlayerBets(playerIndex, hasPassed, bets);

                    // Console.WriteLine($"{_players[playerIndex].Name}, enter a bet (between {MinimumBet}-{MaximumBet}, intervals of {BetIncrement}) or 'pass': ");
                    // string betInput = Console.ReadLine().ToLower();

                    // if (betInput == "pass")
                    //     ProcessPassInput(playerIndex, hasPassed, bets);
                    
                    // else if (int.TryParse(betInput, out int bet) && IsValidBet(bet, bets))
                    // {
                    //     bets[playerIndex] = bet;
                    //     _hasBet[playerIndex] = true;
                    //     Console.WriteLine();

                    //     if (bet == MaximumBet)
                    //         return HandleMaximumBet(playerIndex, hasPassed, bets); // End the betting round immediately
                    // }
                    // else
                    // {
                    //     Console.WriteLine("Invalid bet, please try again");
                    //     playerIndex--;
                    // }

                    if (hasPassed.Count(p => p) >= 3) // End the betting round if 3 players have passed                 
                        return HandleThreePlayerPassed(playerIndex, hasPassed);
                }

                return false;
        }

        private (bool bettingRoundEnded, int bet) ProcessPlayerBets(int playerIndex, bool[] hasPassed, List<int> bets)
        {
            Console.WriteLine($"{_players[playerIndex].Name}, enter a bet (between {MinimumBet}-{MaximumBet}, intervals of {BetIncrement}) or 'pass': ");
            string betInput = Console.ReadLine().ToLower();

            if (betInput == "pass")
                return ProcessPassInput(playerIndex, hasPassed, bets);
            
            if (int.TryParse(betInput, out int bet) && IsValidBet(bet, bets))
            {
                bets[playerIndex] = bet;
                _hasBet[playerIndex] = true;
                Console.WriteLine();
            
                if (bet == MaximumBet)
                    return (HandleMaximumBet(playerIndex, hasPassed, bets), bet); // End the betting round immediately
                else
                    return (false, bet);
            }
            else
            {
                Console.WriteLine("Invalid bet, please try again");
                return (false, -1);
            }
        }

        private (bool bettingRoundEnded, int bet) ProcessPassInput(int playerIndex, bool[] hasPassed, List<int> bets)
        {
            Console.WriteLine($"\n{_players[playerIndex].Name} passed\n");
            hasPassed[playerIndex] = true;
            bets[playerIndex] = -1;
            return (false, -1);
        }

        private static bool IsValidBet(int bet, List<int> bets)
        {
            return bet >= MinimumBet && bet <= MaximumBet && bet % BetIncrement == 0 && !bets.Contains(bet);
        }

        private bool HandleMaximumBet(int playerIndex, bool[] hasPassed, List<int> bets)
        {
            Console.WriteLine($"{_players[playerIndex].Name} bet {MaximumBet}. Betting round ends.\n");
            for (int j = 0; j < _players.Count; j++)
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
                int lastPlayerIndex = (playerIndex + 1) % _players.Count;
                hasBet[lastPlayerIndex] = true;
                _winningBid = MinimumBet;
                _winningBidIndex = lastPlayerIndex;
            }
            return true;
        }

        private void DisplayBettingResults(bool[] hasPassed, List<int> bets)
        {
            Console.WriteLine("\nBetting round complete, here are the results:");
            for (int i = 0; i < _players.Count; i++)
            {
                string result;
                if (hasPassed[i])
                {
                    result = _hasBet[i] ? "Passed after betting" : "Passed";
                }
                else
                {
                    result = $"Bet {bets[i]}";
                }
                Console.WriteLine($"{_players[i].Name} : {result}");
            }
        }

        private void DetermineWinningBid(List<int> bets)
        {
            _winningBid = bets.Max();
            _winningBidIndex = bets.IndexOf(_winningBid);
            Console.WriteLine($"\n{_players[_winningBidIndex].Name} won the bid.");
            Console.WriteLine("\n#########################\n");
            Player.DisplayHand(_players[_winningBidIndex]);
        }

        private void SelectTrumpSuit()
        {
            string[] validSuits = ["clubs", "diamonds", "hearts", "spades"];
            Console.WriteLine($"{_players[_winningBidIndex].Name}, please choose a trump suit. (enter \"clubs\", \"diamonds\", \"hearts\", \"spades\")");
            _trumpSuit = Console.ReadLine().ToLower();

            while (!validSuits.Contains(_trumpSuit))
            {
                Console.WriteLine($"{_trumpSuit} is an invalid input, please try again.");
                _trumpSuit = Console.ReadLine().ToLower();
            }

            Console.WriteLine($"\nTrump suit is {_trumpSuit}.");
        }

        private void PlayRound()
        {
            int currentPlayerIndex;
            int trickWinnerIndex;
            int totalTricks;

            currentPlayerIndex = _winningBidIndex;
            totalTricks = _players[currentPlayerIndex].Hand.Count;

            for (int trick = 0; trick < totalTricks; trick++)
            {
                int trickPoints = 0;
                int cardIndex = -1; // initializing invalid input

                string? leadingSuit = null;
                List<Card> currentTrick = []; // empty list to hold tricks

                Console.WriteLine("\n#########################\n");
                Console.WriteLine($"Trick #{trick + 1}:");

                PlayTrick(currentPlayerIndex, cardIndex, leadingSuit, currentTrick);

                _winningPlayerIndex = DetermineTrickWinnerIndex(currentTrick, _trumpSuit);
                trickWinnerIndex = (currentPlayerIndex + _winningPlayerIndex) % _players.Count;
                Console.WriteLine($"{_players[trickWinnerIndex].Name} won the trick with {currentTrick[_winningPlayerIndex]}");

                currentPlayerIndex = trickWinnerIndex; // set winning player as the current player for the next trick

                trickPoints = currentTrick.Sum(card => card.CardPointValue); // adding all points to trickPoints
                UpdateTrickPoints(trickWinnerIndex, trickPoints);
            }
        }

        private void PlayTrick(int currentPlayerIndex, int cardIndex, string? leadingSuit, List<Card> currentTrick)
        {
            for (int i = 0; i < 4; i++)
                {
                    int playerIndex;
                    Player currentPlayer;
                    playerIndex = (currentPlayerIndex + i) % _players.Count; // ensuring player who won the bet goes first
                    currentPlayer = _players[playerIndex];
                    
                    Card playedCard = ValidateCardInput(currentPlayer, cardIndex, leadingSuit);
                    currentPlayer.RemoveCard(playedCard);

                    if (i == 0)
                    {
                        leadingSuit = playedCard.CardSuit;
                    }

                    currentTrick.Add(playedCard);
                    Console.WriteLine($"{currentPlayer.Name} played {playedCard}\n");
                }
        }

        private Card ValidateCardInput(Player currentPlayer, int cardIndex, string leadingSuit)
        {
            bool validInput = false;

            while (!validInput)
            {
                Player.DisplayHand(currentPlayer); // display current players hand
                Console.WriteLine($"{currentPlayer.Name}, choose a card to play (enter index 0-{currentPlayer.Hand.Count - 1}, leading suit is {leadingSuit} and trump suit is {_trumpSuit}):");
                string Input = Console.ReadLine();

                if (int.TryParse(Input, out cardIndex) && cardIndex < currentPlayer.Hand.Count && cardIndex >= 0)
                {
                    if (currentPlayer.Hand[cardIndex].CardSuit != leadingSuit && currentPlayer.Hand.Any(card => card.CardSuit == leadingSuit))
                    {
                        Console.WriteLine($"\nYou must play the suit of {leadingSuit} since it's in your deck, try again.\n");
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
            _winningPlayerIndex = 0;

            for (int i = 1; i < trick.Count; i++)
            {
                // Check if the current card is a trump card AND the winning card is not a trump card
                if (trick[i].CardSuit == trumpSuit && trick[_winningPlayerIndex].CardSuit != trumpSuit)
                {
                    _winningPlayerIndex = i;
                }
                // Check if both cards are trump cards or both are not trump cards
                else if (trick[i].CardSuit == trick[_winningPlayerIndex].CardSuit)
                {
                    if (trick[i].CardFaceValue > trick[_winningPlayerIndex].CardFaceValue)
                    {
                        _winningPlayerIndex = i;
                    }
                }
            }

            return _winningPlayerIndex;
        }

        private void UpdateTrickPoints(int trickWinnerIndex, int trickPoints)
        {
            if (trickWinnerIndex == 0 || trickWinnerIndex == 2)
            {
                Console.WriteLine($"{_players[trickWinnerIndex].Name} collected {trickPoints} points for Team 1");
                _teamOneRoundPoints += trickPoints;
            }
            else
            {
                Console.WriteLine($"{_players[trickWinnerIndex].Name} collected {trickPoints} points for Team 2");
                _teamTwoRoundPoints += trickPoints;
            }
        }

        private void UpdateTotalPoints() // tally points and end the round
        {
            Console.WriteLine("\nEnd of round. Scoring:");
            Console.WriteLine($"Team One (Player 1 & Player 3) scored : {_teamOneRoundPoints}");
            Console.WriteLine($"Team Two (Player 2 & Player 4) scored : {_teamTwoRoundPoints}");

            if (_winningBidIndex % 2 == 0) // team one won the bet
            {
                UpdateTeamOnePoints();
            }
            else // team two won the bet
            {
                UpdateTeamTwoPoints();
            }

            Console.WriteLine($"\nTeam One has a total of {_teamOneTotalPoints} points");
            Console.WriteLine($"Team Two has a total of {_teamTwoTotalPoints} points");
        }

        private void UpdateTeamOnePoints()
        {
            if (_teamTwoTotalPoints >= 100 && (!_hasBet[0] || !_hasBet[2]))
            {
                Console.WriteLine($"Team One did not place any bets, their points do not count.");
                _teamTwoRoundPoints = 0;
            }

            if (_teamOneRoundPoints >= _winningBid)
            {
                Console.WriteLine($"Team One made their bet of {_winningBid} and wins {_teamOneRoundPoints} points.");
                _teamOneTotalPoints += _teamOneRoundPoints;
                if (_teamTwoTotalPoints < 100)
                {
                    _teamTwoTotalPoints += _teamTwoRoundPoints;
                }
            }
            else
            {
                Console.WriteLine($"Team One did not make their bet of {_winningBid} and loses {_winningBid} points.");
                _teamOneTotalPoints -= _winningBid;
                _teamTwoTotalPoints += _teamTwoRoundPoints;
            }
        }

        private void UpdateTeamTwoPoints()
        {
            if (_teamOneTotalPoints >= 100 && (!_hasBet[1] || !_hasBet[3]))
            {
                Console.WriteLine($"Team Two did not place any bets, their points do not count.");
                _teamTwoRoundPoints = 0;
            }

            if (_teamTwoRoundPoints >= _winningBid)
            {
                Console.WriteLine($"Team Two made their bet of {_winningBid} and wins {_teamTwoRoundPoints} points.");
                _teamTwoTotalPoints += _teamTwoRoundPoints;

                if (_teamOneTotalPoints < 100)
                {
                    _teamOneTotalPoints += _teamOneRoundPoints;
                }
            }
            else
            {
                Console.WriteLine($"Team Two did not make their bet of {_winningBid} and loses {_winningBid} points.");
                _teamTwoTotalPoints -= _winningBid;
                _teamOneTotalPoints += _teamOneRoundPoints;
            }
        }

        private void EndGameCheck()
        {
            if (_teamOneTotalPoints >= WinningScore || _teamTwoTotalPoints >= WinningScore)
            {
                Console.WriteLine("\n#########################\n");
                Console.WriteLine("Game over!");
                Console.WriteLine($"Team One finished with {_teamOneTotalPoints} points");
                Console.WriteLine($"Team Two finished with {_teamTwoTotalPoints} points");
                _gameEnded = true;
            }
            else
            {
                Console.WriteLine("\nPress any key to start the next round...");
                Console.ReadKey();
            }
        }
    }
}