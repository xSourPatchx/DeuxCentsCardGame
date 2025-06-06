namespace DeuxCentsCardGame
{
    public class Game : IGame
    {
        // Game constants
        private const int WinningScore = 200;
        private const int MinimumBet = 50;
        private const int MaximumBet = 100;
        private const int BetIncrement = 5;

        // Game state properties
        private Deck _deck;
        private readonly List<Player> _players;
        private bool[] _playerHasBet;

        private bool _gameEnded;
        private int _currentWinningBid;
        private int _currentWinningBidIndex;
        private CardSuit? _trumpSuit;

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

            _playerHasBet = new bool[4];
            _trumpSuit =  null;
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
            _deck.ShuffleDeck();
            DealCards();
            UIConsoleGameView.DisplayAllHands(_players, _dealerIndex); // display all players hands
            ProcessBettingRound();
            SelectTrumpSuit();
            PlayRound();
            CalculateRoundScores();
            EndGameCheck();
        }

        private void ResetRound()
        {
            _deck = new Deck();
            _gameEnded = false;
            _teamOneRoundPoints = 0;
            _teamTwoRoundPoints = 0;
            _currentWinningBid = 0;
            _currentWinningBidIndex = 0;
            _trumpSuit = null;
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
            for (int cardIndex = 0; cardIndex < _deck.Cards.Count; cardIndex++)
            {
                _players[cardIndex % _players.Count].AddCard(_deck.Cards[cardIndex]);
            }
        }

        public void ProcessBettingRound()
        {
            _ui.DisplayMessage("Betting round begins!\n");
            List<int> bidValue = new(new int[_players.Count]);
            bool[] playerHasPassed = new bool[_players.Count];
            bool bettingRoundEnded = false;
            _playerHasBet = new bool[_players.Count]; // track if a player has placed a bet for over 100 scoring purposes

            int startingIndex = _dealerIndex % _players.Count;

            while (!bettingRoundEnded)
            {
                bettingRoundEnded = ProcessBettingRound(startingIndex, playerHasPassed, bidValue);
            }

            DisplayBettingResults(playerHasPassed, bidValue);
            DetermineWinningBid(bidValue);
        }

        private bool ProcessBettingRound(int startingIndex, bool[] hasPassed, List<int> bets)
        {
            for (int i = 0; i < _players.Count; i++)
                {
                    int playerIndex = (startingIndex + i) % _players.Count;

                    if (hasPassed[playerIndex])
                        continue; // Skip players who have already passed
                    
                    if (ProcessPlayerBets(playerIndex, hasPassed, bets))
                        return true;

                    if (hasPassed.Count(pass => pass) >= 3) // End the betting round if 3 players have passed                 
                        return FinalizeBettingAfterThreePasses(playerIndex, hasPassed);
                }

                return false;
        }

        private bool ProcessPlayerBets(int playerIndex, bool[] hasPassed, List<int> bets)
        {
            while (true)
            {
                string prompt = $"{_players[playerIndex].Name}, enter a bet (between {MinimumBet}-{MaximumBet}, intervals of {BetIncrement}) or 'pass': ";
                string betInput = _ui.GetUserInput(prompt).ToLower();

                if (betInput == "pass")
                {
                    ProcessPassInput(playerIndex, hasPassed, bets);
                    return false;
                }
                
                if (int.TryParse(betInput, out int bet) && IsValidBet(bet, bets))
                {
                    bets[playerIndex] = bet;
                    _playerHasBet[playerIndex] = true;
                    _ui.DisplayMessage("");
                
                    if (bet == MaximumBet)
                        return FinalizeBettingAfterMaximumBet(playerIndex, hasPassed, bets); // End the betting round immediately
                    else
                        return false;
                }
                else
                {
                    _ui.DisplayMessage("Invalid bet, please try again");
                }
            }
        }

        private void ProcessPassInput(int playerIndex, bool[] hasPassed, List<int> bets)
        {
            _ui.DisplayFormattedMessage("\n{0} passed\n", _players[playerIndex].Name);
            hasPassed[playerIndex] = true;
            bets[playerIndex] = -1;
        }

        private static bool IsValidBet(int bet, List<int> bets)
        {
            return bet >= MinimumBet && bet <= MaximumBet && bet % BetIncrement == 0 && !bets.Contains(bet);
        }

        private bool FinalizeBettingAfterMaximumBet(int playerIndex, bool[] hasPassed, List<int> bets)
        {
            _ui.DisplayFormattedMessage("{0} bet {1}. Betting round ends.\n", _players[playerIndex].Name, MaximumBet);
            for (int otherPlayerIndex = 0; otherPlayerIndex < _players.Count; otherPlayerIndex++)
            {
                if (otherPlayerIndex != playerIndex && !hasPassed[otherPlayerIndex])
                {
                    hasPassed[otherPlayerIndex] = true;
                    bets[otherPlayerIndex] = -1;
                }
            }
            return true;
        }

        private bool FinalizeBettingAfterThreePasses(int playerIndex, bool[] hasBet)
        {
            _ui.DisplayMessage("Three players have passed, betting round ends");
            if (!hasBet.Any(bet => bet)) // Inserting default bet of 50 to player 4 if all prior players passed
            {
                int lastPlayerIndex = (playerIndex + 1) % _players.Count;
                hasBet[lastPlayerIndex] = true;
                _currentWinningBid = MinimumBet;
                _currentWinningBidIndex = lastPlayerIndex;
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
                    result = _playerHasBet[i] ? "Passed after betting" : "Passed";
                else
                    result = $"Bet {bets[i]}";

                _ui.DisplayFormattedMessage("{0} : {1}", _players[i].Name, result);
            }
        }

        private void DetermineWinningBid(List<int> bets)
        {
            _currentWinningBid = bets.Max();
            _currentWinningBidIndex = bets.IndexOf(_currentWinningBid);
            _ui.DisplayFormattedMessage("\n{0} won the bid.", _players[_currentWinningBidIndex].Name);
            _ui.DisplayMessage("\n#########################\n");
            UIConsoleGameView.DisplayHand(_players[_currentWinningBidIndex]);
        }

        private void SelectTrumpSuit()
        {
            string[] validSuits = Enum.GetNames<CardSuit>().Select(suit => suit.ToLower()).ToArray();    
            string prompt = $"{_players[_currentWinningBidIndex].Name}, please choose a trump suit. (enter \"clubs\", \"diamonds\", \"hearts\", \"spades\")";
            string trumpSuitString = _ui.GetOptionInput(prompt, validSuits);    
            _trumpSuit = Deck.StringToCardSuit(trumpSuitString);
            _ui.DisplayFormattedMessage("\nTrump suit is {0}.", Deck.CardSuitToString(_trumpSuit.Value));
        }

        private void PlayRound()
        {
            int currentPlayerIndex = _currentWinningBidIndex;

            Card trickWinningCard;
            Player trickWinner;


            for (int trickNumber = 0; trickNumber < _players[currentPlayerIndex].Hand.Count; trickNumber++)
            {
                int trickPoints = 0;
                CardSuit? leadingSuit = null;
                List<(Card card, Player player)> currentTrick = []; // empty list to hold tricks

                _ui.DisplayMessage("\n#########################\n");
                _ui.DisplayFormattedMessage("Trick #{0}:", trickNumber + 1);

                PlayTrick(currentPlayerIndex, leadingSuit, currentTrick);

                (trickWinningCard, trickWinner) = DetermineTrickWinner(currentTrick, _trumpSuit);
                
                _ui.DisplayFormattedMessage("{0} won the trick #{1} with {2}", trickWinner.Name, trickNumber, trickWinningCard);

                currentPlayerIndex = _players.IndexOf(trickWinner); // set winning player as the current player for the next trick
                trickPoints = currentTrick.Sum(trick => trick.card.CardPointValue); // adding all trick points to trickPoints
                AddTrickPointsToTeam(currentPlayerIndex, trickPoints);
            }
        }

        private void PlayTrick(int currentPlayerIndex, CardSuit? leadingSuit, List<(Card card, Player player)> currentTrick)
        {
            for (int trickIndex = 0; trickIndex < _players.Count; trickIndex++)
                {
                    int playerIndex = (currentPlayerIndex + trickIndex) % _players.Count; // ensuring player who won the bet goes first;
                    Player currentPlayer = _players[playerIndex];
                    
                    Card playedCard = GetValidCardFromPlayer(currentPlayer, leadingSuit);
                    currentPlayer.RemoveCard(playedCard);

                    if (trickIndex == 0)
                    {
                        leadingSuit = playedCard.CardSuit;
                    }

                    currentTrick.Add((playedCard, currentPlayer));
                    _ui.DisplayFormattedMessage("{0} played {1}\n", currentPlayer.Name, playedCard);
                }
        }

        private Card GetValidCardFromPlayer(Player currentPlayer, CardSuit? leadingSuit)
        {
            UIConsoleGameView.DisplayHand(currentPlayer);

            string leadingSuitString = leadingSuit.HasValue ? Deck.CardSuitToString(leadingSuit.Value) : "none";
            string trumpSuitString = _trumpSuit.HasValue ? Deck.CardSuitToString(_trumpSuit.Value) : "none";

            string prompt = $"{currentPlayer.Name}, choose a card to play (enter index 0-{currentPlayer.Hand.Count - 1}" +
                (leadingSuit.HasValue ? $", leading suit is {leadingSuitString}" : "") +
                $" and trump suit is {trumpSuitString}):";

            while (true)
            {
                int cardIndex = _ui.GetIntInput(prompt, 0, currentPlayer.Hand.Count - 1);

                Card selectedCard = currentPlayer.Hand[cardIndex];

                if (selectedCard.IsPlayableCard(leadingSuit, currentPlayer.Hand))
                {
                    return selectedCard;
                }
                else
                {
                    _ui.DisplayFormattedMessage("You must play the suit of {0} since it's in your deck, try again.\n", leadingSuitString);
                }
            }
        }

        private (Card winningCard, Player winningPlayer) DetermineTrickWinner(List<(Card card, Player player)> trick, CardSuit? trumpSuit)
        {
            var trickWinner = trick[0];
            CardSuit? leadingSuit = trickWinner.card.CardSuit;

            for (int i = 1; i < trick.Count; i++)
            {        
                if (trick[i].card.WinsAgainst(trickWinner.card, trumpSuit, leadingSuit))
                {
                    trickWinner = trick[i];
                }
            }

            return (trickWinner.card, trickWinner.player);
        }

        private void AddTrickPointsToTeam(int trickWinnerIndex, int trickPoints)
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

        private void CalculateAndUpdateTeamScore(bool isTeamOne)
        {
            int teamRoundPoints;
            int teamTotalPoints;
            bool teamDidNotPlaceBet;
            string teamName;

            if (isTeamOne)
            {
                teamRoundPoints = _teamOneRoundPoints;
                teamTotalPoints = _teamOneTotalPoints;
                teamDidNotPlaceBet = !_playerHasBet[0] && !_playerHasBet[2]; // Check if Players 1 and 3 placed bets
                teamName = "Team One";
            }
            else
            {
                teamRoundPoints = _teamTwoRoundPoints;
                teamTotalPoints = _teamTwoTotalPoints;
                teamDidNotPlaceBet = !_playerHasBet[1] && !_playerHasBet[3]; // Check if Players 2 and 4 placed bets
                teamName = "Team Two";
            }

            bool teamOver100Points = teamTotalPoints >= 100; // check if total point is over 100

            if (teamOver100Points && teamDidNotPlaceBet) // if both condition is true, points are reset
            {
                _ui.DisplayFormattedMessage("{0} did not place any bets, their points do not count.", teamName);
                teamRoundPoints = 0;
            }
            else if (teamRoundPoints >= _currentWinningBid)
            {
                _ui.DisplayFormattedMessage("{0} made their bet of {1} and wins {2} points.", teamName, _currentWinningBid, teamRoundPoints);                
            }
            else    
            {
                _ui.DisplayFormattedMessage("{0} did not make their bet of {1} and loses {1} points.", teamName, _currentWinningBid);   
                teamRoundPoints = -_currentWinningBid;
            }

            // Update the appropriate team's total points
            if (isTeamOne)
            {
                _teamOneRoundPoints = teamRoundPoints;
                _teamOneTotalPoints += teamRoundPoints;
            }
            else
            {
                _teamTwoRoundPoints = teamRoundPoints;
                _teamTwoTotalPoints += teamRoundPoints;
            }
        }

        private void CalculateRoundScores() // tally points and end the round
        {
            _ui.DisplayMessage("\nEnd of round. Scoring:");
            _ui.DisplayFormattedMessage("Team One (Player 1 & Player 3) scored : {0}", _teamOneRoundPoints);
            _ui.DisplayFormattedMessage("Team Two (Player 2 & Player 4) scored : {0}", _teamTwoRoundPoints);

            bool bidWinnerIsTeamOne = IsTeamOne(_currentWinningBidIndex);
            
            CalculateAndUpdateTeamScore(bidWinnerIsTeamOne);
            CalculateAndUpdateTeamScore(!bidWinnerIsTeamOne);

            _ui.DisplayFormattedMessage("\nTeam One has a total of {0} points", _teamOneTotalPoints);
            _ui.DisplayFormattedMessage("Team Two has a total of {0} points", _teamTwoTotalPoints);
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