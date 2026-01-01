using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Interfaces.Gameplay
{
    // Evaluates card hands for strength, composition, and strategic value.
    public interface IHandEvaluator
    {
        int CalculateHandStrength(List<Card> hand);
        int CalculateAdvancedHandStrength(List<Card> hand);
        Dictionary<CardSuit, int> GetSuitCounts(List<Card> hand);
        CardSuit GetStrongestSuit(List<Card> hand);
        // Dictionary<CardSuit, SuitStrength> GetSuitStrengths(List<Card> hand);
        int CountHighCards(List<Card> hand, int threshold);
        bool HasVoid(List<Card> hand, CardSuit suit);
        bool HasSingleton(List<Card> hand, CardSuit suit);
        List<CardSuit> GetVoids(List<Card> hand);
        int EvaluateTrumpPotential(List<Card> hand, CardSuit suit);
        CardSuit RecommendTrumpSuit(List<Card> hand);
    }

    // Represents the evaluated strength of a suit in a hand.
    // public enum SuitStrength
    // {
    //     None = 0,
    //     VeryWeak = 1,
    //     Weak = 2,
    //     Moderate = 3,
    //     Strong = 4,
    //     Dominant = 5
    // }
}