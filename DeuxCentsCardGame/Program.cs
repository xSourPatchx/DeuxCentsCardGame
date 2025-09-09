using DeuxCentsCardGame.UI;
using DeuxCentsCardGame.Controllers;

namespace DeuxCentsCardGame
{
    class Program
    {
        static void Main(string[] args)
        {
            UIGameView ui = new UIGameView();
            GameController game = new GameController(ui);
            game.StartGame();
            Console.ReadKey();
        }
    }
}