namespace DeuxCentsCardGame
{
    public class Game : IGame
    {
        // Game constants
        private const int TEAM_ONE_PLAYER_1 = 0;
        private const int TEAM_ONE_PLAYER_2 = 2;
        private const int TEAM_TWO_PLAYER_1 = 1;
        private const int TEAM_TWO_PLAYER_2 = 3;
        private const int PLAYERS_PER_TEAM = 2;
        private const int TOTAL_PLAYERS = 4;
        private const int WinningScore = 200;
        
        // Game state properties
        private Deck _deck;
        private readonly List<Player> _players;
        private BettingState _bettingState;
        private bool _isGameEnded;
        private CardSuit? _trumpSuit;
        public int DealerIndex = TEAM_TWO_PLAYER_2; // dealer starts at player 4 (index 3)

        // scoring properties
        private int _teamOneRoundPoints;
        private int _teamTwoRoundPoints;
        private int _teamOneTotalPoints;
        private int _teamTwoTotalPoints;

        // UI reference
        private readonly IUIGameView _ui;

        public Game(IUIGameView ui)
        {
            _ui = ui;
            _deck = new Deck();
            _players =
            [
                new("Player 1"),
                new("Player 2"),
                new("Player 3"),
                new("Player 4"),
            ];

            // _bettingState = new BettingState(_players, _ui, DealerIndex);
            _trumpSuit = null;
        }

        public void StartGame()
        {
            while (!_isGameEnded)
            {
                NewRound();
            }
        }

        public void NewRound()
        {
            _ui.ClearScreen();
            _ui.DisplayMessage("Starting a new round...");
            ResetRound();
            _deck.ShuffleDeck();
            DealCards();
            UIGameView.DisplayAllHands(_players, DealerIndex); // display all players hands
            ExecuteBettingRound();
            SelectTrumpSuit();
            PlayRound();
            ScoreRound();
            EndGameCheck();
            RotateDealer();
        }

        private void ResetRound()
        {
            _deck = new Deck();
            // _isGameEnded = false;
            _teamOneRoundPoints = 0;
            _teamTwoRoundPoints = 0;
            _trumpSuit = null;
            _bettingState = new BettingState(_players, _ui, DealerIndex);
            // RotateDealer();
        }

        private void RotateDealer()
        {
            DealerIndex = (DealerIndex + 1) % _players.Count;
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

        public void ExecuteBettingRound()
        {
            _bettingState.ResetBettingRound();
            _bettingState.ExecuteBettingRound();
        }

        private void SelectTrumpSuit()
        {
            string[] validSuits = Enum.GetNames<CardSuit>().Select(suit => suit.ToLower()).ToArray();    
            string prompt = $"{_players[_bettingState.CurrentWinningBidIndex].Name}, please choose a trump suit. (enter \"clubs\", \"diamonds\", \"hearts\", \"spades\")";
            string trumpSuitString = _ui.GetOptionInput(prompt, validSuits);    
            _trumpSuit = Deck.StringToCardSuit(trumpSuitString);
            _ui.DisplayFormattedMessage("\nTrump suit is {0}.", Deck.CardSuitToString(_trumpSuit.Value));
        }

        private void PlayRound()
        {
            int currentPlayerIndex = _bettingState.CurrentWinningBidIndex;
            PlayAllTricks(currentPlayerIndex);
        }

        private void PlayAllTricks(int startingPlayerIndex)
        {
            int totalTricks = _players[startingPlayerIndex].Hand.Count;
            int currentPlayerIndex = startingPlayerIndex;

            Card trickWinningCard;
            Player trickWinner;

            for (int trickNumber = 0; trickNumber < totalTricks; trickNumber++)
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
                AwardTrickPoints(currentPlayerIndex, trickPoints);
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
            UIGameView.DisplayHand(currentPlayer);

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

        private void AwardTrickPoints(int trickWinnerIndex, int trickPoints)
        {
            // bool isTeamOne = trickWinnerIndex % 2 == 0;
            string teamName = IsPlayerOnTeamOne(trickWinnerIndex) ? "Team One" : "Team Two";
     
            _ui.DisplayFormattedMessage("{0} collected {1} points for {2}", _players[trickWinnerIndex].Name, trickPoints, teamName);
            if (IsPlayerOnTeamOne(trickWinnerIndex))
            {
                _teamOneRoundPoints += trickPoints;
            }
            else
            {
                _teamTwoRoundPoints += trickPoints;
            }
        }

        private bool IsPlayerOnTeamOne(int playerIndex)
        {
            return playerIndex % 2 == 0;
        }

        private void ScoreRound() // tally points and end the round
        {
            _ui.DisplayMessage("\nEnd of round. Scoring:");
            _ui.DisplayFormattedMessage("Team One (Player 1 & Player 3) scored : {0}", _teamOneRoundPoints);
            _ui.DisplayFormattedMessage("Team Two (Player 2 & Player 4) scored : {0}", _teamTwoRoundPoints);

            bool bidWinnerIsTeamOne = IsPlayerOnTeamOne(_bettingState.CurrentWinningBidIndex);
            
            ScoreBidWinningTeam(bidWinnerIsTeamOne);

            ScoreBidLosingTeam(!bidWinnerIsTeamOne);

            _ui.DisplayFormattedMessage("\nTeam One has a total of {0} points", _teamOneTotalPoints);
            _ui.DisplayFormattedMessage("Team Two has a total of {0} points", _teamTwoTotalPoints);
        }

        private void ScoreBidWinningTeam(bool isTeamOne)
        {
            int teamRoundPoints;
            int teamTotalPoints;
            bool teamCannotScore;
            string teamName;

            if (isTeamOne)
            {
                teamRoundPoints = _teamOneRoundPoints;
                teamTotalPoints = _teamOneTotalPoints;
                teamCannotScore = teamTotalPoints >= 100 && !_bettingState.PlayerHasBet[TEAM_ONE_PLAYER_1] && !_bettingState.PlayerHasBet[TEAM_ONE_PLAYER_2];
                teamName = "Team One";
            }
            else
            {
                teamRoundPoints = _teamTwoRoundPoints;
                teamTotalPoints = _teamTwoTotalPoints;
                teamCannotScore = teamTotalPoints >= 100 && !_bettingState.PlayerHasBet[TEAM_TWO_PLAYER_1] && !_bettingState.PlayerHasBet[TEAM_TWO_PLAYER_2];
                teamName = "Team Two";
            }

            int awardedPoints;

            if (teamCannotScore)
            {
                _ui.DisplayFormattedMessage("{0} did not place any bets and has over 100 points, so they score 0 points this round.", teamName);
                awardedPoints = 0;
            }
            else if (teamRoundPoints >= _bettingState.CurrentWinningBid)
            {
                _ui.DisplayFormattedMessage("{0} made their bet of {1} and wins {2} points.", teamName, _bettingState.CurrentWinningBid, teamRoundPoints);
                awardedPoints = teamRoundPoints;
            }
            else
            {
                _ui.DisplayFormattedMessage("{0} did not make their bet of {1} and loses {1} points.", teamName, _bettingState.CurrentWinningBid);
                awardedPoints = -_bettingState.CurrentWinningBid;
            }

            if (isTeamOne)
            {
                _teamOneTotalPoints += awardedPoints;
            }
            else
            {
                _teamTwoTotalPoints += awardedPoints;
            }     
        }

        private void ScoreBidLosingTeam(bool isTeamOne)
        {
            int teamRoundPoints;
            int teamTotalPoints;
            bool teamCannotScore;
            string teamName;

            if (isTeamOne)
            {
                teamRoundPoints = _teamOneRoundPoints;
                teamTotalPoints = _teamOneTotalPoints;
                teamCannotScore = teamTotalPoints >= 100 && !_bettingState.PlayerHasBet[TEAM_ONE_PLAYER_1] && !_bettingState.PlayerHasBet[TEAM_ONE_PLAYER_2];
                teamName = "Team One";
            }
            else
            {
                teamRoundPoints = _teamTwoRoundPoints;
                teamTotalPoints = _teamTwoTotalPoints;
                teamCannotScore = teamTotalPoints >= 100 && !_bettingState.PlayerHasBet[TEAM_TWO_PLAYER_1] && !_bettingState.PlayerHasBet[TEAM_TWO_PLAYER_2];
                teamName = "Team Two";
            }

            int awardedPoints;

            if (teamCannotScore)
            {
                _ui.DisplayFormattedMessage("{0} did not place any bets and has over 100 points, so they score 0 points this round.", teamName);
                awardedPoints = 0;
            }
            else if (teamRoundPoints >= _bettingState.CurrentWinningBid)
            {
                _ui.DisplayFormattedMessage("{0} made their bet of {1} and wins {2} points.", teamName, _bettingState.CurrentWinningBid, teamRoundPoints);
                awardedPoints = teamRoundPoints;
            }
            else
            {
                _ui.DisplayFormattedMessage("{0} (non-betting team) scores {1} points.", teamName, teamRoundPoints);
                awardedPoints = teamRoundPoints;
            }


            if (isTeamOne)
            {
                _teamOneTotalPoints += awardedPoints;
            }
            else
            {
                _teamTwoTotalPoints += awardedPoints;
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
                _isGameEnded = true;
            }
            else
            {
                _ui.WaitForUser("\nPress any key to start the next round...");
            }
        }
    }
}