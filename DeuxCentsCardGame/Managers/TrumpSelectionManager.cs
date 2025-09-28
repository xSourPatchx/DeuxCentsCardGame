using DeuxCentsCardGame.Events;
using DeuxCentsCardGame.Interfaces.Managers;
using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Managers
{
    public class TrumpSelectionManager : ITrumpSelectionManager
    {
        private readonly GameEventManager _eventManager;
        // private readonly IUIGameView _ui;

        public TrumpSelectionManager(GameEventManager eventManager)
        {
            _eventManager = eventManager;
        }

        public CardSuit SelectTrumpSuit(Player winningBidder)
        {
            string trumpSuitInput = _eventManager.RaiseTrumpSelectionInput(winningBidder);
            CardSuit trumpSuit = Deck.StringToCardSuit(trumpSuitInput);

            _eventManager.RaiseTrumpSelected(trumpSuit, winningBidder);

            return trumpSuit;
        }   
    }
}