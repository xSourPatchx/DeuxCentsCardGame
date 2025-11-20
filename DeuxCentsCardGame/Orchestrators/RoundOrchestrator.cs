using DeuxCentsCardGame.Interfaces.Events;
using DeuxCentsCardGame.Interfaces.Managers;
using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Orchestrators
{
    public class RoundOrchestrator
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

        public RoundOrchestrator(
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

        public void InitializeRound(int roundNumber)
        {
            _eventManager.RaiseRoundStarted(roundNumber, _playerManager.GetPlayer(DealerIndex));
            ResetRound();
        }

        public void PrepareRound()
        {
            ShuffleDeck();
            CutDeck();
            DealCards();
        }

        public void ExecuteBettingPhase()
        {
            _bettingManager.UpdateDealerIndex(DealerIndex);
            _bettingManager.ExecuteBettingRound();
        }

        public void SelectTrump()
        {
            TrumpSuit = _trumpSelectionManager.SelectTrumpSuit(_playerManager.GetPlayer(_bettingManager.CurrentWinningBidIndex));
        }

        public void FinalizeRound(int roundNumber)
        {
            ScoreRound();
            RaiseRoundEndedEvent(roundNumber);

            if (!_scoringManager.IsGameOver())
            {
                _eventManager.RaiseNextRoundPrompt();
            }

            RotateDealer();
        }

        public int GetStartingPlayerIndex()
        {
            return _bettingManager.CurrentWinningBidIndex;
        }   

        private void ResetRound()
        {
            _deckManager.ResetDeck();
            TrumpSuit = null;
            _scoringManager.ResetRoundPoints();
            _bettingManager.ResetBettingRound();
            _playerTurnManager.ResetTurnSequence();
        }

        private void ShuffleDeck()
        {   
            _deckManager.ShuffleDeck();
        }

        private void CutDeck()
        {
            int cuttingPlayerIndex = _playerTurnManager.GetPlayerRightOfDealer(DealerIndex);
            Player cuttingPlayer = _playerManager.GetPlayer(cuttingPlayerIndex);

            int deckSize = _deckManager.CurrentDeck.Cards.Count;
            int cutPosition = _eventManager.RaiseDeckCutInput(cuttingPlayer, deckSize);

            _deckManager.CutDeck(cutPosition);
            _eventManager.RaiseDeckCut(cuttingPlayer, cutPosition);
        }

        private void DealCards()
        {
            _dealingManager.DealCards(_deckManager.CurrentDeck, _playerManager.Players.ToList());
            _dealingManager.RaiseCardsDealtEvent(_playerManager.Players.ToList(), DealerIndex);
        }

        private void ScoreRound()
        {  
            _scoringManager.ScoreRound(_bettingManager.CurrentWinningBidIndex, _bettingManager.CurrentWinningBid);
        }

        private void RaiseRoundEndedEvent(int roundNumber)
        {       
            _eventManager.RaiseRoundEnded(
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