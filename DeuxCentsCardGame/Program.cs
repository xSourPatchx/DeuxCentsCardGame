namespace DeuxCentsCardGame
{
    class Program
    {
        static void Main(string[] args)
        {
            ConsoleGameView ui = new ConsoleGameView();
            Game game = new Game(ui);
            game.Start();
            Console.ReadKey();
        }
    }
}