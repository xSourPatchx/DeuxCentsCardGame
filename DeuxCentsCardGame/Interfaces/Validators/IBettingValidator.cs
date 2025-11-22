using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Interfaces.Validators
{
    public interface IBettingValidator
    {
        bool IsValidBet(int bet);
        bool IsBetInValidRange(int bet);
        bool IsBetValidIncrement(int bet);
        bool IsBetUnique(int bet);
        bool IsMaximumBet(int bet);
        bool HasMinimumPlayersPassed();
        bool HasPlayerPassed(Player player);
        bool IsPassInput(string input);
    }
}