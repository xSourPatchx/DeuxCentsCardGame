// // Modified Beats method to be more clear about its purpose
// public bool Beats(Card otherCard, CardSuit? trumpSuit, CardSuit? leadingSuit)
// {
//     // Current card is trump and other isn't - we win
//     if (IsTrump(trumpSuit) && !otherCard.IsTrump(trumpSuit))
//         return true;

//     // Other card is trump and we aren't - they win
//     if (!IsTrump(trumpSuit)) && otherCard.IsTrump(trumpSuit))
//         return false;

//     // Both are trump - higher value wins
//     if (IsTrump(trumpSuit)) && otherCard.IsTrump(trumpSuit))
//         return CardFaceValue > otherCard.CardFaceValue;

//     // Neither is trump - check leading suit
//     if (leadingSuit.HasValue)
//     {
//         // Current card matches leading suit, other doesn't - we win
//         if (CardSuit == leadingSuit.Value && otherCard.CardSuit != leadingSuit.Value)
//             return true;

//         // Other card matches leading suit, we don't - they win
//         if (CardSuit != leadingSuit.Value && otherCard.CardSuit == leadingSuit.Value)
//             return false;

//         // Neither matches leading suit - higher value wins if same suit
//         if (IsSameSuit(otherCard))
//             return CardFaceValue > otherCard.CardFaceValue;
//     }

//     // Default case (shouldn't normally reach here)
//     return false;
// }