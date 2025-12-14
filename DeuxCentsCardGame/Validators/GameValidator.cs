using DeuxCentsCardGame.Constants;
using DeuxCentsCardGame.Events.EventArgs;
using DeuxCentsCardGame.Interfaces.Events;
using DeuxCentsCardGame.Interfaces.GameConfig;
using DeuxCentsCardGame.Interfaces.Services;
using DeuxCentsCardGame.Interfaces.Validators;
using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Validators
{
        public class GameValidator : IGameValidator
        {
        private readonly IGameConfig _gameConfig;
        private readonly IGameEventManager _eventManager;
        private readonly ICardUtility _cardUtility;
        private readonly List<Player> _players;

        public GameValidator(
            IGameConfig gameConfig,
            IGameEventManager eventManager,
            ICardUtility cardUtility,
            List<Player> players)
        {
            _gameConfig = gameConfig ?? throw new ArgumentNullException(nameof(gameConfig));
            _eventManager = eventManager ?? throw new ArgumentNullException(nameof(eventManager));
            _cardUtility = cardUtility ?? throw new ArgumentNullException(nameof(cardUtility));
            _players = players ?? throw new ArgumentNullException(nameof(players));
        }

        #region Betting Validation

        public bool IsValidBet(int bet)
        {
            return IsBetInValidRange(bet) &&
                IsBetValidIncrement(bet) &&
                IsBetUnique(bet);
        }

        public bool IsBetInValidRange(int bet)
        {
            return bet >= _gameConfig.MinimumBet && bet <= _gameConfig.MaximumBet;
        }

        public bool IsBetValidIncrement(int bet)
        {
            return bet % _gameConfig.BetIncrement == 0;
        }

        public bool IsBetUnique(int bet)
        {
            return !_players.Any(player => player.CurrentBid == bet);
        }

        public bool IsMaximumBet(int bet)
        {
            return bet == _gameConfig.MaximumBet;
        }

        public bool HasMinimumPlayersPassed()
        {
            int passedPlayersCount = _players.Count(p => p.HasPassed);
            return passedPlayersCount >= _gameConfig.MinimumPlayersToPass;
        }

        public bool HasPlayerPassed(Player player)
        {
            return player.HasPassed;
        }

        public bool IsPassInput(string input)
        {
            return input == "pass";
        }

        #endregion

        #region Card Play Validation

        public async Task<Card> GetValidCardFromPlayer(Player currentPlayer, CardSuit? leadingSuit, CardSuit? trumpSuit)
        {
            while (true)
            {
                int cardIndex = await RequestCardSelection(currentPlayer, leadingSuit, trumpSuit);
                Card selectedCard = currentPlayer.Hand[cardIndex];

                if (IsCardValid(selectedCard, leadingSuit, currentPlayer.Hand))
                {
                    return selectedCard;
                }

                await DisplayInvalidCardMessage(currentPlayer, leadingSuit);
            }
        }

        public List<Card> GetValidCards(List<Card> hand, CardSuit? leadingSuit)
        {
            if (hand == null || hand.Count == 0)
                return new List<Card>();

            if (!leadingSuit.HasValue)
                return new List<Card>(hand);

            var validCards = hand.Where(card => card.CardSuit == leadingSuit.Value).ToList();

            if (validCards.Any())
                return validCards;

            return new List<Card>(hand);
        }

        public List<int> GetValidCardIndices(List<Card> hand, CardSuit? leadingSuit)
        {
            if (hand == null || hand.Count == 0)
                return new List<int>();

            var validCards = GetValidCards(hand, leadingSuit);
            var validIndices = new List<int>();

            foreach (var card in validCards)
            {
                int index = hand.IndexOf(card);
                if (index >= 0)
                    validIndices.Add(index);
            }

            return validIndices;
        }

        public bool IsCardIndexValid(List<Card> hand, int cardIndex, CardSuit? leadingSuit)
        {
            if (hand == null || cardIndex < 0 || cardIndex >= hand.Count)
                return false;

            var selectedCard = hand[cardIndex];
            return IsCardValid(selectedCard, leadingSuit, hand);
        }

        private async Task<int> RequestCardSelection(Player currentPlayer, CardSuit? leadingSuit, CardSuit? trumpSuit)
        {
            return await _eventManager.RaiseCardSelectionInput(currentPlayer, leadingSuit, trumpSuit, currentPlayer.Hand);
        }

        private bool IsCardValid(Card selectedCard, CardSuit? leadingSuit, List<Card> hand)
        {
            return IsPlayableCard(selectedCard, leadingSuit, hand);
        }

        private async Task DisplayInvalidCardMessage(Player currentPlayer, CardSuit? leadingSuit)
        {
            string leadingSuitString = leadingSuit.HasValue 
                ? _cardUtility.CardSuitToString(leadingSuit.Value) 
                : "none";
            
            string message = $"You must play the suit of {leadingSuitString} since it's in your deck, try again.";

            await _eventManager.RaiseInvalidMove(currentPlayer, message, InvalidMoveType.InvalidCard);
        }

        #endregion

        #region Card Validation

        public void ValidateCard(CardSuit suit, CardFace face, int faceValue, int pointValue)
        {
            ValidateSuit(suit);
            ValidateFace(face);
            ValidateFaceValue(faceValue);
            ValidatePointValue(pointValue);
            ValidateFacePointConsistency(face, pointValue);            
        }

        private void ValidateSuit(CardSuit suit)
        {
            if (!Enum.IsDefined(typeof(CardSuit), suit))
                throw new ArgumentException($"Invalid card suit: {suit}", nameof(suit));
        }

        private void ValidateFace(CardFace face)
        {
            if (!Enum.IsDefined(typeof(CardFace), face))
                throw new ArgumentException($"Invalid card face: {face}", nameof(face));
        }

        private void ValidateFaceValue(int faceValue)
        {
            if (faceValue < GameConstants.MINIMUM_CARD_FACE_VALUE || faceValue > GameConstants.MAXIMUM_CARD_FACE_VALUE)
                throw new ArgumentOutOfRangeException(nameof(faceValue),
                $"Invalid card face value, must be between {GameConstants.MINIMUM_CARD_FACE_VALUE}-{GameConstants.MAXIMUM_CARD_FACE_VALUE}. faceValue : {faceValue}");
        }

        private void ValidatePointValue(int pointValue)
        {
            if (Array.IndexOf(_cardUtility.GetCardPointValues(), pointValue) == -1)
                throw new ArgumentOutOfRangeException(nameof(pointValue),
                    $"Invalid card point value, must be {string.Join(", ", _cardUtility.GetCardPointValues())}. pointValue: {pointValue}");
        }

        private void ValidateFacePointConsistency(CardFace face, int pointValue)
        {
            var expectedPointValue = face switch
            {
                CardFace.Five => GameConstants.CARD_POINT_VALUE_FIVE,
                CardFace.Ten or CardFace.Ace => GameConstants.CARD_POINT_VALUE_TEN,
                _ => GameConstants.CARD_POINT_VALUE_ZERO
            };

            if (pointValue != expectedPointValue)
                throw new ArgumentException(
                    $"Invalid point value {pointValue} for card face {face}. Expected {expectedPointValue}");
        }

        public bool IsPlayableCard(Card card, CardSuit? leadingSuit, List<Card> hand)
        {
            if (!leadingSuit.HasValue)
                return true;

            if (card.CardSuit == leadingSuit.Value)
                return true;

            return !hand.Any(card => card.CardSuit == leadingSuit.Value);
        }

        #endregion
    }
}