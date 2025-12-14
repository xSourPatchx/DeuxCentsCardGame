using DeuxCentsCardGame.Interfaces.Controllers;
using DeuxCentsCardGame.Interfaces.Events;
using DeuxCentsCardGame.Interfaces.Gameplay;
using DeuxCentsCardGame.Interfaces.Managers;
using DeuxCentsCardGame.Interfaces.Validators;
using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Controllers
{
    public class TrickController : ITrickController
    {
        private readonly IGameEventManager _eventManager;
        private readonly IPlayerManager _playerManager;
        private readonly IPlayerTurnManager _playerTurnManager;
        private readonly IScoringManager _scoringManager;
        private readonly ICardLogic _cardComparer;
        private readonly IGameValidator _gameValidator;

        public TrickController(
            IGameEventManager eventManager,
            IPlayerManager playerManager,
            IPlayerTurnManager playerTurnManager,
            IScoringManager scoringManager,
            ICardLogic cardComparer,
            IGameValidator cardPlayValidator)
        {
            _eventManager = eventManager ?? throw new ArgumentNullException(nameof(eventManager));
            _playerManager = playerManager ?? throw new ArgumentNullException(nameof(playerManager));
            _playerTurnManager = playerTurnManager ?? throw new ArgumentNullException(nameof(playerTurnManager));
            _scoringManager = scoringManager ?? throw new ArgumentNullException(nameof(scoringManager));
            _cardComparer = cardComparer ?? throw new ArgumentNullException(nameof(cardComparer));
            _gameValidator = cardPlayValidator ?? throw new ArgumentNullException(nameof(cardPlayValidator));
        }

        public async Task PlayAllTricks(int startingPlayerIndex, CardSuit? trumpSuit)
        {
            var startingPlayer = _playerManager.GetPlayer(startingPlayerIndex);
            int totalTricks = startingPlayer.Hand.Count;

            _playerTurnManager.InitializeTurnSequence(startingPlayerIndex);

            for (int trickNumber = 0; trickNumber < totalTricks; trickNumber++)
            {
                await ExecuteSingleTrick(trickNumber, trumpSuit);
            }
        }

        public async Task ExecuteSingleTrick(int trickNumber, CardSuit? trumpSuit)
        {
            CardSuit? leadingSuit = null;
            List<(Card card, Player player)> currentTrick = new();

            await PlayTrickCards(leadingSuit, currentTrick, trickNumber, trumpSuit);

            var (trickWinningCard, trickWinner) = DetermineTrickWinner(currentTrick, trumpSuit, leadingSuit);
            int trickPoints = CalculateTrickPoints(currentTrick);
            int trickWinnerIndex = await AwardTrickPointsAndNotify(trickNumber, trickWinner, trickWinningCard, currentTrick, trickPoints);

            _playerTurnManager.SetCurrentPlayer(trickWinnerIndex);
        }

        private async Task PlayTrickCards(CardSuit? leadingSuit, List<(Card card, Player player)> currentTrick, 
                                    int trickNumber, CardSuit? trumpSuit)
        {
            var turnOrder = _playerTurnManager.GetTurnOrder();

            foreach (int playerIndex in turnOrder)
            {
                Player currentPlayer = _playerManager.GetPlayer(playerIndex);
                leadingSuit = await ExecutePlayerTurn(currentPlayer, leadingSuit, trickNumber, currentTrick, trumpSuit);
            }
        }

        private async Task<CardSuit?> ExecutePlayerTurn(Player currentPlayer, CardSuit? leadingSuit, int trickNumber, 
                                    List<(Card card, Player player)> currentTrick, CardSuit? trumpSuit)
        {
            await RaisePlayerTurnEvent(currentPlayer, leadingSuit, trumpSuit, trickNumber);
            
            Card playedCard = await _gameValidator.GetValidCardFromPlayer(currentPlayer, leadingSuit, trumpSuit);
            
            currentPlayer.RemoveCard(playedCard);
            leadingSuit = UpdateLeadingSuit(playedCard, leadingSuit, currentTrick);
            currentTrick.Add((playedCard, currentPlayer));
            
            await RaiseCardPlayedEvent(currentPlayer, playedCard, trickNumber, leadingSuit, trumpSuit);

            return leadingSuit;
        }

        private async Task RaisePlayerTurnEvent(Player currentPlayer, CardSuit? leadingSuit, CardSuit? trumpSuit, int trickNumber)
        {
            await _eventManager.RaisePlayerTurn(currentPlayer, leadingSuit, trumpSuit, trickNumber);
        }

        private CardSuit? UpdateLeadingSuit(Card playedCard, CardSuit? leadingSuit, List<(Card card, Player player)> currentTrick)
        {
            if (currentTrick.Count == 0)
            {
                return playedCard.CardSuit;
            }
            return leadingSuit;
        }

        private async Task RaiseCardPlayedEvent(Player currentPlayer, Card playedCard, int trickNumber, 
                                        CardSuit? leadingSuit, CardSuit? trumpSuit)
        {
            await _eventManager.RaiseCardPlayed(currentPlayer, playedCard, trickNumber, leadingSuit, trumpSuit);
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

        private async Task<int> AwardTrickPointsAndNotify(int trickNumber, Player trickWinner, Card trickWinningCard, 
                                            List<(Card card, Player player)> currentTrick, int trickPoints)
        {
            int winnerIndex = _playerManager.Players.ToList().IndexOf(trickWinner);
            _scoringManager.AwardTrickPoints(winnerIndex, trickPoints);
            await _eventManager.RaiseTrickCompleted(trickNumber, trickWinner, trickWinningCard, currentTrick, trickPoints);
            return winnerIndex;
        }
    }
}