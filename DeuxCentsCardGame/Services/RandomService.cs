using DeuxCentsCardGame.Interfaces.Services;


namespace DeuxCentsCardGame.Services
{
    public class RandomService : IRandomService
    {
        private readonly Random _random = new Random();

        public int Next(int minValue, int maxValue)
        {
            return _random.Next(minValue, maxValue);
        }
    }
}