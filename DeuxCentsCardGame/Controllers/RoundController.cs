using DeuxCentsCardGame.Interfaces.Controllers;
using DeuxCentsCardGame.Interfaces.Events;
using DeuxCentsCardGame.Interfaces.Managers;
using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Controllers
{
    public class RoundController : IRoundController
    {
        private readonly IGameEventManager _eventManager;
        private readonly IPlayerManager _playerManager;
        private readonly IPlayerTurnManager _playerTurnManager;
        private readonly IDeckManager _deckManager;
        private readonly IDealingManager _dealingManager;
        private readonly IBettingManager _bettingManager;
        private readonly ITrumpSelectionManager _trumpSelectionManager;
        private readonly IScoringManager _scoringManager;

        public int DealerIndex { get; private set; }
        public CardSuit? TrumpSuit { get; private set; }

        public RoundController(
            IGameEventManager eventManager,
            IPlayerManager playerManager,
            IPlayerTurnManager playerTurnManager,
            IDeckManager deckManager,
            IDealingManager dealingManager,
            IBettingManager bettingManager,
            ITrumpSelectionManager trumpSelectionManager,
            IScoringManager scoringManager,
            int initialDealerIndex)
        {
            _eventManager = eventManager ?? throw new ArgumentNullException(nameof(eventManager));
            _playerManager = playerManager ?? throw new ArgumentNullException(nameof(playerManager));
            _playerTurnManager = playerTurnManager ?? throw new ArgumentNullException(nameof(playerTurnManager));
            _deckManager = deckManager ?? throw new ArgumentNullException(nameof(deckManager));
            _dealingManager = dealingManager ?? throw new ArgumentNullException(nameof(dealingManager));
            _bettingManager = bettingManager ?? throw new ArgumentNullException(nameof(bettingManager));
            _trumpSelectionManager = trumpSelectionManager ?? throw new ArgumentNullException(nameof(trumpSelectionManager));
            _scoringManager = scoringManager ?? throw new ArgumentNullException(nameof(scoringManager));
            
            DealerIndex = initialDealerIndex;
        }

        public async Task InitializeRound(int roundNumber)
        {
            await _eventManager.RaiseRoundStarted(roundNumber, _playerManager.GetPlayer(DealerIndex));
            await ResetRound();
        }

        public async Task PrepareRound()
        {
            await ShuffleDeck();
            await CutDeck();
            await DealCards();
        }

        public async Task ExecuteBettingPhase()
        {
            await _bettingManager.UpdateDealerIndex(DealerIndex);
            await _bettingManager.ExecuteBettingRound();
        }

        public async Task SelectTrump()
        {
            TrumpSuit = await _trumpSelectionManager.SelectTrumpSuit(_playerManager.GetPlayer(_bettingManager.CurrentWinningBidIndex));
        }

        public async Task FinalizeRound(int roundNumber)
        {
            ScoreRound();
            await RaiseRoundEndedEvent(roundNumber);

            if (!_scoringManager.IsGameOver())
            {
                await _eventManager.RaiseNextRoundPrompt();
            }

            RotateDealer();
        }

        public int GetStartingPlayerIndex()
        {
            return _bettingManager.CurrentWinningBidIndex;
        }   

        private async Task ResetRound()
        {
            await _deckManager.ResetDeck();
            TrumpSuit = null;
            _scoringManager.ResetRoundPoints();
            await _bettingManager.ResetBettingRound();
            _playerTurnManager.ResetTurnSequence();
        }

        private async Task ShuffleDeck()
        {   
            await _deckManager.ShuffleDeck();
        }

        private async Task CutDeck()
        {
            int cuttingPlayerIndex = _playerTurnManager.GetPlayerRightOfDealer(DealerIndex);
            Player cuttingPlayer = _playerManager.GetPlayer(cuttingPlayerIndex);

            int deckSize = _deckManager.CurrentDeck.Cards.Count;
            int cutPosition = await _eventManager.RaiseDeckCutInput(cuttingPlayer, deckSize);

            await _deckManager.CutDeck(cutPosition);
            await _eventManager.RaiseDeckCut(cuttingPlayer, cutPosition);
        }

        private async Task DealCards()
        {
            await _dealingManager.DealCards(_deckManager.CurrentDeck, _playerManager.Players.ToList());
            await _dealingManager.RaiseCardsDealtEvent(_playerManager.Players.ToList(), DealerIndex);
        }

        private void ScoreRound()
        {  
            _scoringManager.ScoreRound(_bettingManager.CurrentWinningBidIndex, _bettingManager.CurrentWinningBid);
        }

        private async Task RaiseRoundEndedEvent(int roundNumber)
        {       
            await _eventManager.RaiseRoundEnded(
                roundNumber,
                _scoringManager.TeamOneRoundPoints,
                _scoringManager.TeamTwoRoundPoints,
                _playerManager.GetPlayer(_bettingManager.CurrentWinningBidIndex),
                _bettingManager.CurrentWinningBid
            );
        }

        private void RotateDealer()
        {
            DealerIndex = _playerTurnManager.RotateDealer(DealerIndex);
        }
    }
}