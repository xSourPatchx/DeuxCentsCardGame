using DeuxCentsCardGame.Models;
using DeuxCentsCardGame.Interfaces;
using DeuxCentsCardGame.UI;
using DeuxCentsCardGame.Events;

namespace DeuxCentsCardGame.Core
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

        // dealer starts at player 4 (index 3)
        public int DealerIndex = TEAM_TWO_PLAYER_2;

        // scoring properties
        private int _teamOneRoundPoints;
        private int _teamTwoRoundPoints;
        private int _teamOneTotalPoints;
        private int _teamTwoTotalPoints;
        private int _currentRoundNumber = 1;

        // UI reference
        private readonly IUIGameView _ui;

        // Event reference
        private readonly GameEventManager _eventManager;
        private readonly GameEventHandler _eventHandler;

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

            _trumpSuit = null;
            _eventManager = new GameEventManager();
            _eventHandler = new GameEventHandler(_eventManager, _ui);

            _bettingState = new BettingState(_players, DealerIndex, _eventManager);
        }

        public void StartGame()
        {
            while (!_isGameEnded)
            {
                NewRound();
            }

            _eventHandler.UnsubscribeFromEvents();
        }

        public void NewRound()
        {
            _eventManager.RaiseRoundStarted(_currentRoundNumber, _players[DealerIndex]);
            ResetRound();
            _deck.ShuffleDeck();
            DealCards();
            _eventManager.RaiseCardsDealt(_players, DealerIndex);
            ExecuteBettingRound();
            SelectTrumpSuit();
            PlayRound();
            ScoreRound();
            EndGameCheck();
            RotateDealer();
            _currentRoundNumber++;
        }

        private void ResetRound()
        {
            _deck = new Deck();
            _teamOneRoundPoints = 0;
            _teamTwoRoundPoints = 0;
            _trumpSuit = null;
            _bettingState = new BettingState(_players, /*_ui, */DealerIndex, _eventManager);
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

            _eventManager.RaiseTrumpSelected(_trumpSuit.Value, _players[_bettingState.CurrentWinningBidIndex]);
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

                // empty list to hold tricks
                List<(Card card, Player player)> currentTrick = [];

                PlayTrick(currentPlayerIndex, leadingSuit, currentTrick, trickNumber);

                (trickWinningCard, trickWinner) = DetermineTrickWinner(currentTrick, _trumpSuit);

                // set winning player as the current player for the next trick
                currentPlayerIndex = _players.IndexOf(trickWinner);
                
                // adding all trick points to trickPoints
                trickPoints = currentTrick.Sum(trick => trick.card.CardPointValue);

                _eventManager.RaiseTrickCompleted(trickNumber, trickWinner, trickWinningCard, currentTrick, trickPoints);

                AwardTrickPoints(currentPlayerIndex, trickPoints);
            }
        }

        private void PlayTrick(int currentPlayerIndex, CardSuit? leadingSuit, List<(Card card, Player player)> currentTrick, int trickNumber)
        {
            for (int trickIndex = 0; trickIndex < _players.Count; trickIndex++)
                {
                    // ensuring player who won the bet goes first;
                    int playerIndex = (currentPlayerIndex + trickIndex) % _players.Count;
                    Player currentPlayer = _players[playerIndex];

                    // Raise event for player's turn and display their hand
                    _eventManager.RaisePlayerTurn(currentPlayer, leadingSuit, _trumpSuit, trickNumber);

                    Card playedCard = GetValidCardFromPlayer(currentPlayer, leadingSuit);

                    currentPlayer.RemoveCard(playedCard);

                    if (trickIndex == 0)
                    {
                        leadingSuit = playedCard.CardSuit;
                    }

                    currentTrick.Add((playedCard, currentPlayer));

                    _eventManager.RaiseCardPlayed(currentPlayer, playedCard, trickNumber, leadingSuit, _trumpSuit);
                }
        }

        private Card GetValidCardFromPlayer(Player currentPlayer, CardSuit? leadingSuit)
        {
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

        // tally points and end the round 
        private void ScoreRound()
        {
            bool bidWinnerIsTeamOne = IsPlayerOnTeamOne(_bettingState.CurrentWinningBidIndex);

            ScoreTeam(bidWinnerIsTeamOne, true);   // Bid winning team
            ScoreTeam(!bidWinnerIsTeamOne, false); // Bid losing team

            _eventManager.RaiseScoreUpdated(_teamOneRoundPoints, _teamTwoRoundPoints, 
                                            _teamOneTotalPoints, _teamTwoTotalPoints, 
                                            bidWinnerIsTeamOne, _bettingState.CurrentWinningBid);
        }

        private void ScoreTeam(bool isTeamOne, bool isBidWinner)
        {
            int teamRoundPoints = isTeamOne ? _teamOneRoundPoints : _teamTwoRoundPoints;
            int teamTotalPoints = isTeamOne ? _teamOneTotalPoints : _teamTwoTotalPoints;
            
            var (playerOneIndex, playerTwoIndex) = GetPlayerIndices(isTeamOne);

            bool teamCannotScore = teamTotalPoints >= 100 &&
                                    !_players[playerOneIndex].HasBet &&
                                    !_players[playerTwoIndex].HasBet;
            
            int awardedPoints = CalculateAwardedPoints(teamRoundPoints, teamCannotScore, isBidWinner);

            if (isTeamOne)
            {
                _teamOneTotalPoints += awardedPoints;
            }
            else
            {
                _teamTwoTotalPoints += awardedPoints;
            }     
        }

        private (int playerOne, int playerTwo) GetPlayerIndices(bool isTeamOne)
        {
            return isTeamOne ? (TEAM_ONE_PLAYER_1, TEAM_ONE_PLAYER_2) : (TEAM_TWO_PLAYER_1, TEAM_TWO_PLAYER_2);
        }

        private int CalculateAwardedPoints(int teamRoundPoints, bool teamCannotScore, bool isBidWinner)
        {
            if (teamCannotScore)
            {
                return 0;
            }

            if (isBidWinner)
            {
                return teamRoundPoints >= _bettingState.CurrentWinningBid
                    ? teamRoundPoints 
                    : -_bettingState.CurrentWinningBid;
            }
            else
            {
                return teamRoundPoints;
            }
        }

        private void EndGameCheck()
        {
            if (_teamOneTotalPoints >= WinningScore || _teamTwoTotalPoints >= WinningScore)
            {
                _eventManager.RaiseGameOver(_teamOneTotalPoints, _teamTwoTotalPoints);
                _isGameEnded = true;
            }
            else
            {
                _ui.WaitForUser("\nPress any key to start the next round...");
                _eventManager.RaiseGameOver(_teamOneTotalPoints, _teamTwoTotalPoints);
            }
        }
    }
}