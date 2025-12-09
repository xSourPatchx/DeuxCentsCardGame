using DeuxCentsCardGame.Interfaces.Events;
using DeuxCentsCardGame.Interfaces.Managers;
using DeuxCentsCardGame.Interfaces.Services;
using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Managers
{
    public class TrumpSelectionManager : ITrumpSelectionManager
    {
        private readonly IGameEventManager _eventManager;
        private readonly ICardUtility _cardUtility;
        public TrumpSelectionManager(IGameEventManager eventManager, ICardUtility cardUtility)
        {
            _eventManager = eventManager;
            _cardUtility = cardUtility;
        }

        public async Task<CardSuit> SelectTrumpSuit(Player winningBidder)
        {
            string trumpSuitInput = await _eventManager.RaiseTrumpSelectionInput(winningBidder);
            CardSuit trumpSuit = _cardUtility.StringToCardSuit(trumpSuitInput);
            await _eventManager.RaiseTrumpSelected(trumpSuit, winningBidder);

            return trumpSuit;
        }   
    }
}