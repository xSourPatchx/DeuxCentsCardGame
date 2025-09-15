using DeuxCentsCardGame.Events;
using DeuxCentsCardGame.Interfaces;
using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Managers
{
    public class TrumpSelectionManager
    {
        private readonly GameEventManager _eventManager;
        // private readonly IUIGameView _ui;

        public TrumpSelectionManager(GameEventManager eventManager, IUIGameView ui)
        {
            _eventManager = eventManager;
            // _ui = ui;
        }

        public CardSuit SelectTrumpSuit(Player winningBidder)
        {
            // string[] validSuits = Enum.GetNames<CardSuit>().Select(suit => suit.ToLower()).ToArray();
            // string prompt = $"{winningBidder.Name}, please choose a trump suit. (enter \"clubs\", \"diamonds\", \"hearts\", \"spades\")";

            string trumpSuitInput = _eventManager.RaiseTrumpSelectionInput(winningBidder);
            CardSuit trumpSuit = Deck.StringToCardSuit(trumpSuitInput);

            _eventManager.RaiseTrumpSelected(trumpSuit, winningBidder);

            return trumpSuit;
        }
        
    }
}