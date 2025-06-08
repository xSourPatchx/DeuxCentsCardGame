namespace DeuxCentsCardGame
{
    class Program
    {
        static void Main(string[] args)
        {
            UIConsoleGameView ui = new UIConsoleGameView();
            Game game = new Game(ui);
            game.StartGame();
            Console.ReadKey();
        }
    }
}