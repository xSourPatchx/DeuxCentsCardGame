using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Gameplay
{
    public class CardComparer
    {
        public bool WinsAgainst(Card thisCard, Card otherCard, CardSuit? trumpSuit, CardSuit? leadingSuit)
        {
            bool thisCardIsTrump = thisCard.IsTrump(trumpSuit);
            bool otherCardIsTrump = otherCard.IsTrump(trumpSuit);

            // Handle trump card scenarios
            if (thisCardIsTrump || otherCardIsTrump)
                return HandleTrumpComparison(thisCard, otherCard, thisCardIsTrump, otherCardIsTrump);

            // Handle leading suit scenarios
            if (leadingSuit.HasValue)
                return HandleLeadingSuitComparison(thisCard, otherCard, leadingSuit.Value);

            // Handle same suit comparison
            return HandleSameSuitComparison(thisCard, otherCard);
        }

        private bool HandleTrumpComparison(Card thisCard, Card otherCard, bool thisCardIsTrump, bool otherCardIsTrump)
        {
            // Only this card is trump, we win
            if (thisCardIsTrump && !otherCardIsTrump)
                return true;

            // Only other card is trump, we lose
            if (!thisCardIsTrump && otherCardIsTrump)
                return false;

            // Both are trump, higher face value wins
            return thisCard.CardFaceValue > otherCard.CardFaceValue;
        }

        private bool HandleLeadingSuitComparison(Card thisCard, Card otherCard, CardSuit leadingSuit)
        {
            bool thisCardMatchesLeading = thisCard.CardSuit == leadingSuit;
            bool otherCardMatchesLeading = otherCard.CardSuit == leadingSuit;

            // Only this card matches leading suit, we win
            if (thisCardMatchesLeading && !otherCardMatchesLeading)
                return true;

            // Only other card matches leading suit, we lose
            if (!thisCardMatchesLeading && otherCardMatchesLeading)
                return false;

            // Both match or neither matches - compare if same suit
            return HandleSameSuitComparison(thisCard, otherCard);
        }

        private bool HandleSameSuitComparison(Card thisCard, Card otherCard)
        {
            // Only compare face values if cards are same suit
            if (thisCard.IsSameSuit(otherCard))
                return thisCard.CardFaceValue > otherCard.CardFaceValue;

            // Default case
            return false;
        }
    }
}