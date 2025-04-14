namespace DeuxCentsCardGame
{
    class Program
    {
        static void Main(string[] args)
        {
            // create a Game object and call the game method
            Game game = new Game();
            game.Start();
            Console.ReadKey();
        }
    }
}