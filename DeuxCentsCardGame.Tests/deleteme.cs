// private int DetermineTrickWinnerIndex(List<Card> trick, CardSuit? trumpSuit)
// {
// _winningPlayerIndex = 0;

// bool trumpSuitNotNull = trumpSuit.HasValue;

// for (int i = 1; i < trick.Count; i++)
// {
// if (trumpSuitNotNull)
// {
//     // Check if the current card is a trump card AND the winning card is not a trump card
//     if (trick[i].CardSuit == trumpSuit && trick[_winningPlayerIndex].CardSuit != trumpSuit)
//     {
//         _winningPlayerIndex = i;
//         continue;
//     }
// }

// // Check if both cards are trump cards or both are not trump cards
// if (trick[i].CardSuit == trick[_winningPlayerIndex].CardSuit)
// {
//     if (trick[i].CardFaceValue > trick[_winningPlayerIndex].CardFaceValue)
//         _winningPlayerIndex = i;    
// }
// }

// return _winningPlayerIndex;
// }