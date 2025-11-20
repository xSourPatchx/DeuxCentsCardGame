using DeuxCentsCardGame.Gameplay;
using DeuxCentsCardGame.Interfaces.Events;
using DeuxCentsCardGame.Interfaces.Managers;
using DeuxCentsCardGame.Models;
using DeuxCentsCardGame.Validators;

namespace DeuxCentsCardGame.Orchestrators
{
    public class TrickOrchestrator
    {
        private readonly IGameEventManager _eventManager;
        private readonly IPlayerManager _playerManager;
        private readonly IPlayerTurnManager _playerTurnManager;
        private readonly IScoringManager _scoringManager;
        private readonly CardComparer _cardComparer;
        private readonly CardPlayValidator _cardPlayValidator;

        public TrickOrchestrator(
            IGameEventManager eventManager,
            IPlayerManager playerManager,
            IPlayerTurnManager playerTurnManager,
            IScoringManager scoringManager,
            CardComparer cardComparer,
            CardPlayValidator cardPlayValidator)
        {
            _eventManager = eventManager ?? throw new ArgumentNullException(nameof(eventManager));
            _playerManager = playerManager ?? throw new ArgumentNullException(nameof(playerManager));
            _playerTurnManager = playerTurnManager ?? throw new ArgumentNullException(nameof(playerTurnManager));
            _scoringManager = scoringManager ?? throw new ArgumentNullException(nameof(scoringManager));
            _cardComparer = cardComparer ?? throw new ArgumentNullException(nameof(cardComparer));
            _cardPlayValidator = cardPlayValidator ?? throw new ArgumentNullException(nameof(cardPlayValidator));
        }

        public void PlayAllTricks(int startingPlayerIndex, CardSuit? trumpSuit)
        {
            var startingPlayer = _playerManager.GetPlayer(startingPlayerIndex);
            int totalTricks = startingPlayer.Hand.Count;

            _playerTurnManager.InitializeTurnSequence(startingPlayerIndex);

            for (int trickNumber = 0; trickNumber < totalTricks; trickNumber++)
            {
                ExecuteSingleTrick(trickNumber, trumpSuit);
            }
        }

        private void ExecuteSingleTrick(int trickNumber, CardSuit? trumpSuit)
        {
            CardSuit? leadingSuit = null;
            List<(Card card, Player player)> currentTrick = [];

            PlayTrickCards(ref leadingSuit, currentTrick, trickNumber, trumpSuit);

            var (trickWinningCard, trickWinner) = DetermineTrickWinner(currentTrick, trumpSuit, leadingSuit);
            int trickPoints = CalculateTrickPoints(currentTrick);
            int trickWinnerIndex = AwardTrickPointsAndNotify(trickNumber, trickWinner, trickWinningCard, currentTrick, trickPoints);

            _playerTurnManager.SetCurrentPlayer(trickWinnerIndex);
        }

        private void PlayTrickCards(ref CardSuit? leadingSuit, List<(Card card, Player player)> currentTrick, 
                                    int trickNumber, CardSuit? trumpSuit)
        {
            var turnOrder = _playerTurnManager.GetTurnOrder();

            foreach (int playerIndex in turnOrder)
            {
                Player currentPlayer = _playerManager.GetPlayer(playerIndex);
                ExecutePlayerTurn(currentPlayer, ref leadingSuit, trickNumber, currentTrick, trumpSuit);
            }
        }

        private void ExecutePlayerTurn(Player currentPlayer, ref CardSuit? leadingSuit, int trickNumber, 
                                    List<(Card card, Player player)> currentTrick, CardSuit? trumpSuit)
        {
            RaisePlayerTurnEvent(currentPlayer, leadingSuit, trumpSuit, trickNumber);
            
            Card playedCard = _cardPlayValidator.GetValidCardFromPlayer(currentPlayer, leadingSuit, trumpSuit);
            
            currentPlayer.RemoveCard(playedCard);
            UpdateLeadingSuit(playedCard, ref leadingSuit, currentTrick);
            currentTrick.Add((playedCard, currentPlayer));
            
            RaiseCardPlayedEvent(currentPlayer, playedCard, trickNumber, leadingSuit, trumpSuit);
        }

        private void RaisePlayerTurnEvent(Player currentPlayer, CardSuit? leadingSuit, CardSuit? trumpSuit, int trickNumber)
        {
            _eventManager.RaisePlayerTurn(currentPlayer, leadingSuit, trumpSuit, trickNumber);
        }

        private void UpdateLeadingSuit(Card playedCard, ref CardSuit? leadingSuit, List<(Card card, Player player)> currentTrick)
        {
            if (currentTrick.Count == 0)
            {
                leadingSuit = playedCard.CardSuit;
            }
        }

        private void RaiseCardPlayedEvent(Player currentPlayer, Card playedCard, int trickNumber, 
                                        CardSuit? leadingSuit, CardSuit? trumpSuit)
        {
            _eventManager.RaiseCardPlayed(currentPlayer, playedCard, trickNumber, leadingSuit, trumpSuit);
        }

        private (Card winningCard, Player winningPlayer) DetermineTrickWinner(
            List<(Card card, Player player)> trick, CardSuit? trumpSuit, CardSuit? leadingSuit)
        {
            var trickWinner = trick[0];
            CardSuit? actualLeadingSuit = leadingSuit ?? trickWinner.card.CardSuit;

            for (int i = 1; i < trick.Count; i++)
            {
                if (_cardComparer.WinsAgainst(trick[i].card, trickWinner.card, trumpSuit, actualLeadingSuit))
                {
                    trickWinner = trick[i];
                }
            }

            return (trickWinner.card, trickWinner.player);
        }

        private int CalculateTrickPoints(List<(Card card, Player player)> trick)
        {
            return trick.Sum(t => t.card.CardPointValue);
        }

        private int AwardTrickPointsAndNotify(int trickNumber, Player trickWinner, Card trickWinningCard, 
                                            List<(Card card, Player player)> currentTrick, int trickPoints)
        {
            int winnerIndex = _playerManager.Players.ToList().IndexOf(trickWinner);
            _scoringManager.AwardTrickPoints(winnerIndex, trickPoints);
            _eventManager.RaiseTrickCompleted(trickNumber, trickWinner, trickWinningCard, currentTrick, trickPoints);
            return winnerIndex;
        }
    }
}