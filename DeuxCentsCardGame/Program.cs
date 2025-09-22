using DeuxCentsCardGame.Controllers;
using DeuxCentsCardGame.Events;
using DeuxCentsCardGame.Managers;
using DeuxCentsCardGame.UI;

namespace DeuxCentsCardGame
{
    class Program
    {
        static void Main(string[] args)
        {
            // Build dependency graph //
            // UI
            var console = new ConsoleWrapper();
            var ui = new UIGameView(console);

            // Events
            var eventManager = new GameEventManager();
            var eventHandler = new GameEventHandler(eventManager, ui);

            // Managers
            var playerManager = new PlayerManager(eventManager);
            var deckManager = new DeckManager(eventManager);
            var dealingManager = new DealingManager(eventManager);
            var bettingManager = new BettingManager(playerManager.Players.ToList(), 3, eventManager);
            var trumpSelectionManager = new TrumpSelectionManager(eventManager);
            var scoringManager = new ScoringManager(eventManager, playerManager.Players.ToList());

            // Controller
            var game = new GameController
            (
                playerManager,
                deckManager,
                dealingManager,
                bettingManager,
                trumpSelectionManager,
                scoringManager,
                eventManager,
                eventHandler
            );

            game.StartGame();
            Console.ReadKey();
        }
    }
}