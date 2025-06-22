namespace DeuxCentsCardGame
{
    class Program
    {
        static void Main(string[] args)
        {
            UIGameView ui = new UIGameView();
            Game game = new Game(ui);
            game.StartGame();
            Console.ReadKey();
        }
    }
}