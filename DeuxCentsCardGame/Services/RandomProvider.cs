using DeuxCentsCardGame.Interfaces;


namespace DeuxCentsCardGame.Services
{
    public class RandomProvider : IRandomProvider
    {
        private readonly Random _random = new Random();

        public int Next(int minValue, int maxValue)
        {
            return _random.Next(minValue, maxValue);
        }
    }
}