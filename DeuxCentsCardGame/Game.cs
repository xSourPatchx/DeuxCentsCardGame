namespace DeuxCentsCardGame
{
    public class Game : IGame
    {
        // Game constants
        public const int ShuffleCount = 3;
        private const int WinningScore = 200;
        private const int MinimumBet = 50;
        private const int MaximumBet = 100;
        private const int BetIncrement = 5;

        // Game state properties

        private Deck _deck;
        private readonly List<Player> _players;

        private bool _gameEnded;
        private bool[] _hasBet;
        private int _winningBid;
        private int _winningBidIndex;
        private int _winningPlayerIndex;
        private string _trumpSuit;

        // scoring properties
        private int _teamOneRoundPoints;
        private int _teamTwoRoundPoints;
        private int _teamOneTotalPoints;
        private int _teamTwoTotalPoints;
        public int _dealerIndex = 3; // dealer starts at player 4 (index 3)

        // UI reference
        private readonly IUIConsoleGameView _ui;

        public Game(IUIConsoleGameView userInterfaceConsoleGameView)
        {
            _ui = userInterfaceConsoleGameView;
            _deck = new Deck();
            _players =
            [
                new("Player 1"),
                new("Player 2"),
                new("Player 3"),
                new("Player 4"),
            ];

            _hasBet = new bool[4];
            _trumpSuit = string.Empty;
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
            _ui.ClearScreen();
            _ui.DisplayMessage("Starting a new round...");
            ResetRound();
            _deck.ShuffleDeck(ShuffleCount);
            DealCards();
            UIConsoleGameView.DisplayAllHands(_players, _dealerIndex); // display all players hands
            BettingRound();
            SelectTrumpSuit();
            PlayRound();
            UpdateScore();
            EndGameCheck();
        }

        private void ResetRound()
        {
            _deck = new Deck();
            _gameEnded = false;
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

            _ui.DisplayMessage("Dealing cards...");
            for (int i = 0; i < _deck.Cards.Count; i++)
            {
                _players[i % _players.Count].AddCard(_deck.Cards[i]);
            }
        }

        public void BettingRound()
        {
            _ui.DisplayMessage("Betting round begins!\n");
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

                    if (hasPassed.Count(p => p) >= 3) // End the betting round if 3 players have passed                 
                        return HandleThreePlayerPassed(playerIndex, hasPassed);
                }

                return false;
        }

        private (bool bettingRoundEnded, int bet) ProcessPlayerBets(int playerIndex, bool[] hasPassed, List<int> bets)
        {
            string prompt = $"{_players[playerIndex].Name}, enter a bet (between {MinimumBet}-{MaximumBet}, intervals of {BetIncrement}) or 'pass': ";
            string betInput = _ui.GetUserInput(prompt).ToLower();

            if (betInput == "pass")
                return ProcessPassInput(playerIndex, hasPassed, bets);
            
            if (int.TryParse(betInput, out int bet) && IsValidBet(bet, bets))
            {
                bets[playerIndex] = bet;
                _hasBet[playerIndex] = true;
                _ui.DisplayMessage("");
            
                if (bet == MaximumBet)
                    return (HandleMaximumBet(playerIndex, hasPassed, bets), bet); // End the betting round immediately
                else
                    return (false, bet);
            }
            else
            {
                _ui.DisplayMessage("Invalid bet, please try again");
                return (false, -1);
            }
        }

        private (bool bettingRoundEnded, int bet) ProcessPassInput(int playerIndex, bool[] hasPassed, List<int> bets)
        {
            _ui.DisplayFormattedMessage("\n{0} passed\n", _players[playerIndex].Name);
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
            _ui.DisplayFormattedMessage("{0} bet {1}. Betting round ends.\n", _players[playerIndex].Name, MaximumBet);
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
            _ui.DisplayMessage("Three players have passed, betting round ends");
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
            _ui.DisplayMessage("\nBetting round complete, here are the results:");
            for (int i = 0; i < _players.Count; i++)
            {
                string result;
                if (hasPassed[i])
                    result = _hasBet[i] ? "Passed after betting" : "Passed";
                else
                    result = $"Bet {bets[i]}";

                _ui.DisplayFormattedMessage("{0} : {1}", _players[i].Name, result);
            }
        }

        private void DetermineWinningBid(List<int> bets)
        {
            _winningBid = bets.Max();
            _winningBidIndex = bets.IndexOf(_winningBid);
            _ui.DisplayFormattedMessage("\n{0} won the bid.", _players[_winningBidIndex].Name);
            _ui.DisplayMessage("\n#########################\n");
            UIConsoleGameView.DisplayHand(_players[_winningBidIndex]);
        }

        private void SelectTrumpSuit()
        {
            string[] validSuits = Deck.GetCardSuits();
            string prompt = $"{_players[_winningBidIndex].Name}, please choose a trump suit. (enter \"clubs\", \"diamonds\", \"hearts\", \"spades\")";
            _trumpSuit = _ui.GetOptionInput(prompt, validSuits);

            _ui.DisplayFormattedMessage("\nTrump suit is {0}.", _trumpSuit);
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
                string? leadingSuit = null;
                List<Card> currentTrick = []; // empty list to hold tricks

                _ui.DisplayMessage("\n#########################\n");
                _ui.DisplayFormattedMessage("Trick #{0}:", trick + 1);

                PlayTrick(currentPlayerIndex, leadingSuit, currentTrick);

                _winningPlayerIndex = DetermineTrickWinnerIndex(currentTrick, _trumpSuit);
                trickWinnerIndex = (currentPlayerIndex + _winningPlayerIndex) % _players.Count;
                _ui.DisplayFormattedMessage("{0} won the trick with {1}", _players[trickWinnerIndex].Name, currentTrick[_winningPlayerIndex]);
                currentPlayerIndex = trickWinnerIndex; // set winning player as the current player for the next trick

                trickPoints = currentTrick.Sum(card => card.CardPointValue); // adding all trick points to trickPoints
                UpdateTrickPoints(trickWinnerIndex, trickPoints);
            }
        }

        private void PlayTrick(int currentPlayerIndex, string? leadingSuit, List<Card> currentTrick)
        {
            for (int i = 0; i < _players.Count; i++)
                {
                    int playerIndex;
                    Player currentPlayer;
                    playerIndex = (currentPlayerIndex + i) % _players.Count; // ensuring player who won the bet goes first
                    currentPlayer = _players[playerIndex];
                    
                    Card playedCard = ValidateCardInput(currentPlayer, leadingSuit);
                    currentPlayer.RemoveCard(playedCard);

                    if (i == 0)
                        leadingSuit = playedCard.CardSuit;

                    currentTrick.Add(playedCard);
                    _ui.DisplayFormattedMessage("{0} played {1}\n", currentPlayer.Name, playedCard);
                }
        }

        private Card ValidateCardInput(Player currentPlayer, string? leadingSuit)
        {
            UIConsoleGameView.DisplayHand(currentPlayer);

            string prompt = $"{currentPlayer.Name}, choose a card to play (enter index 0-{currentPlayer.Hand.Count - 1}" +
                (leadingSuit != null ? $", leading suit is {leadingSuit}" : "") +
                $" and trump suit is {_trumpSuit}):";

            while (true)
            {
                int cardIndex = _ui.GetIntInput(prompt, 0, currentPlayer.Hand.Count - 1);

                // Validate card follows suit if possible
                if (leadingSuit != null && 
                    currentPlayer.Hand[cardIndex].CardSuit != leadingSuit && 
                    currentPlayer.Hand.Any(card => card.CardSuit == leadingSuit))
                {
                    _ui.DisplayFormattedMessage("You must play the suit of {0} since it's in your deck, try again.\n", leadingSuit);
                    continue;
                }

                return currentPlayer.Hand[cardIndex];
            }
        }

        private int DetermineTrickWinnerIndex(List<Card> trick, string trumpSuit)
        {
            _winningPlayerIndex = 0;

            for (int i = 1; i < trick.Count; i++)
            {
                // Check if the current card is a trump card AND the winning card is not a trump card
                if (trick[i].CardSuit == trumpSuit && trick[_winningPlayerIndex].CardSuit != trumpSuit)
                    _winningPlayerIndex = i;
                // Check if both cards are trump cards or both are not trump cards
                else if (trick[i].CardSuit == trick[_winningPlayerIndex].CardSuit)
                {
                    if (trick[i].CardFaceValue > trick[_winningPlayerIndex].CardFaceValue)
                        _winningPlayerIndex = i;    
                }
            }

            return _winningPlayerIndex;
        }

        private void UpdateTrickPoints(int trickWinnerIndex, int trickPoints)
        {
            bool isTeamOne = trickWinnerIndex % 2 == 0;
            string teamName = isTeamOne ? "Team One" : "Team Two";

            _ui.DisplayFormattedMessage("{0} collected {1} points for {2}", _players[trickWinnerIndex].Name, trickPoints, teamName);
            if (isTeamOne)
                _teamOneRoundPoints += trickPoints;
            else
                _teamTwoRoundPoints += trickPoints; 
        }

        private bool IsTeamOne(int playerIndex)
        {
            return playerIndex % 2 == 0;
        }

        private void UpdateScore() // tally points and end the round
        {
            _ui.DisplayMessage("\nEnd of round. Scoring:");
            _ui.DisplayFormattedMessage("Team One (Player 1 & Player 3) scored : {0}", _teamOneRoundPoints);
            _ui.DisplayFormattedMessage("Team Two (Player 2 & Player 4) scored : {0}", _teamTwoRoundPoints);

            if(IsTeamOne(_winningBidIndex))
                UpdateTeamOnePoints();
            else // team two won the bet
                UpdateTeamTwoPoints();
            
            _ui.DisplayFormattedMessage("\nTeam One has a total of {0} points", _teamOneTotalPoints);
            _ui.DisplayFormattedMessage("Team Two has a total of {0} points", _teamTwoTotalPoints);
        }

        private void UpdateTeamOnePoints()
        {
            bool teamTwoOver100Points = _teamTwoTotalPoints >= 100;
            bool teamOneDidNotPlaceBet = !_hasBet[0] || !_hasBet[2];
            
            if (teamTwoOver100Points && teamOneDidNotPlaceBet)
            {
                _ui.DisplayMessage("Team One did not place any bets, their points do not count.");
                _teamTwoRoundPoints = 0;
            }

            if (_teamOneRoundPoints >= _winningBid)
            {
                _ui.DisplayFormattedMessage("Team One made their bet of {0} and wins {1} points.", _winningBid, _teamOneRoundPoints);
                _teamOneTotalPoints += _teamOneRoundPoints;
                if (!teamTwoOver100Points)
                    _teamTwoTotalPoints += _teamTwoRoundPoints;   
            }
            else
            {
                _ui.DisplayFormattedMessage("Team One did not make their bet of {0} and loses {0} points.", _winningBid);
                _teamOneTotalPoints -= _winningBid;
                _teamTwoTotalPoints += _teamTwoRoundPoints;
            }
        }

        private void UpdateTeamTwoPoints()
        {
            bool teamOneOver100Points = _teamOneTotalPoints >= 100;
            bool teamTwoDidNotPlaceBet = !(_hasBet[1] || _hasBet[3]);

            if (teamOneOver100Points && teamTwoDidNotPlaceBet)
            {
                _ui.DisplayMessage("Team Two did not place any bets, their points do not count.");
                _teamTwoRoundPoints = 0;
            }

            if (_teamTwoRoundPoints >= _winningBid)
            {
                _ui.DisplayFormattedMessage("Team Two made their bet of {0} and wins {1} points.", _winningBid, _teamTwoRoundPoints);                
                _teamTwoTotalPoints += _teamTwoRoundPoints;

                if (!teamOneOver100Points)
                    _teamOneTotalPoints += _teamOneRoundPoints;   
            }
            else
            {
                _ui.DisplayFormattedMessage("Team Two did not make their bet of {0} and loses {0} points.", _winningBid);                
                _teamTwoTotalPoints -= _winningBid;
                _teamOneTotalPoints += _teamOneRoundPoints;
            }
        }

        private void EndGameCheck()
        {
            if (_teamOneTotalPoints >= WinningScore || _teamTwoTotalPoints >= WinningScore)
            {
                _ui.DisplayMessage("\n#########################\n");
                _ui.DisplayMessage("Game over!");
                _ui.DisplayFormattedMessage("Team One finished with {0} points", _teamOneTotalPoints);
                _ui.DisplayFormattedMessage("Team Two finished with {0} points", _teamTwoTotalPoints);
                _gameEnded = true;
            }
            else
            {
                _ui.WaitForUser("\nPress any key to start the next round...");
            }
        }
    }
}