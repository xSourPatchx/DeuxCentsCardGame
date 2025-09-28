namespace DeuxCentsCardGame.Interfaces.Services
{
    public interface IRandomService
    {
        int Next(int minValue, int maxValue);
    }
}