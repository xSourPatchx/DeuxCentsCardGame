// public void ExecuteBettingRound()
// {
//     _bettingState.ResetBettingRound();
    
//     // We'll need to modify BettingState to raise events - for now, execute normally
//     _bettingState.ExecuteBettingRound();
    
//     // After betting is complete, create the bid dictionary and raise event
//     var allBids = new Dictionary<Player, int>();
//     for (int i = 0; i < _players.Count; i++)
//     {
//         allBids[_players[i]] = _bettingState.PlayerBids[i];
//     }
    
//     Player winningBidder = _players[_bettingState.CurrentWinningBidIndex];
//     _eventManager.RaiseBettingCompleted(winningBidder, _bettingState.CurrentWinningBid, allBids);
// }