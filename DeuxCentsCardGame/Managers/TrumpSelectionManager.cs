using DeuxCentsCardGame.Events;
using DeuxCentsCardGame.Interfaces.Events;
using DeuxCentsCardGame.Interfaces.Managers;
using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Managers
{
    public class TrumpSelectionManager : ITrumpSelectionManager
    {
        private readonly IGameEventManager _eventManager;
        // private readonly IUIGameView _ui;

        public TrumpSelectionManager(IGameEventManager eventManager)
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