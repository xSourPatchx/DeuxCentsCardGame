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

        // Gets a list of all valid cards that can be played from the player's hand.
        // Useful for Unity UI to highlight playable cards.
        public List<Card> GetValidCards(List<Card> hand, CardSuit? leadingSuit)
        {
            if (hand == null || hand.Count == 0)
                return new List<Card>();

            // If no leading suit, all cards are valid
            if (!leadingSuit.HasValue)
                return new List<Card>(hand);

            // Get cards that match the leading suit
            var validCards = hand.Where(card => card.CardSuit == leadingSuit.Value).ToList();

            // If player has cards of the leading suit, they must play one
            if (validCards.Any())
                return validCards;

            // If player has no cards of the leading suit, all cards are valid
            return new List<Card>(hand);
        }

        // Gets the indices of all valid cards in the player's hand.
        // Useful for Unity UI to highlight specific card positions.
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

        // Checks if a specific card at a given index is valid to play.
        // Useful for Unity UI to validate card selection on click.
        public bool IsCardIndexValid(List<Card> hand, int cardIndex, CardSuit? leadingSuit)
        {
            if (hand == null || cardIndex < 0 || cardIndex >= hand.Count)
                return false;

            var selectedCard = hand[cardIndex];
            return IsCardValid(selectedCard, leadingSuit, hand);
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