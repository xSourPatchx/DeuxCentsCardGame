using DeuxCentsCardGame.Events.EventArgs;
using DeuxCentsCardGame.Interfaces.Events;
using DeuxCentsCardGame.Interfaces.Services;
using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Validators
{
    public class CardPlayValidator
    {
        private readonly IGameEventManager _eventManager;
        private readonly ICardUtility _cardUtility;
        private readonly CardValidator _cardValidator;

        public CardPlayValidator(
            IGameEventManager eventManager,
            ICardUtility cardUtility,
            CardValidator cardValidator)
        {
            _eventManager = eventManager ?? throw new ArgumentNullException(nameof(eventManager));
            _cardUtility = cardUtility ?? throw new ArgumentNullException(nameof(cardUtility));
            _cardValidator = cardValidator ?? throw new ArgumentNullException(nameof(cardValidator));
        }

        public Card GetValidCardFromPlayer(Player currentPlayer, CardSuit? leadingSuit, CardSuit? trumpSuit)
        {
            while (true)
            {
                int cardIndex = RequestCardSelection(currentPlayer, leadingSuit, trumpSuit);
                Card selectedCard = currentPlayer.Hand[cardIndex];

                if (IsCardValid(selectedCard, leadingSuit, currentPlayer.Hand))
                {
                    return selectedCard;
                }

                DisplayInvalidCardMessage(currentPlayer, leadingSuit);
            }
        }

        private int RequestCardSelection(Player currentPlayer, CardSuit? leadingSuit, CardSuit? trumpSuit)
        {
            return _eventManager.RaiseCardSelectionInput(currentPlayer, leadingSuit, trumpSuit, currentPlayer.Hand);
        }

        private bool IsCardValid(Card selectedCard, CardSuit? leadingSuit, List<Card> hand)
        {
            return _cardValidator.IsPlayableCard(selectedCard, leadingSuit, hand);
        }

        private void DisplayInvalidCardMessage(Player currentPlayer, CardSuit? leadingSuit)
        {
            string leadingSuitString = leadingSuit.HasValue 
                ? _cardUtility.CardSuitToString(leadingSuit.Value) 
                : "none";
            
            string message = $"You must play the suit of {leadingSuitString} since it's in your deck, try again.";

            _eventManager.RaiseInvalidMove(currentPlayer, message, InvalidMoveType.InvalidCard);
        }
    }
}