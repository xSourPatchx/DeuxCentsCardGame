using DeuxCentsCardGame.Controllers;
using DeuxCentsCardGame.Events;
using DeuxCentsCardGame.Managers;
using DeuxCentsCardGame.UI;
using DeuxCentsCardGame.Services;
using DeuxCentsCardGame.GameConfig;

namespace DeuxCentsCardGame
{
    class Program
    {
        static void Main(string[] args)
        {
            // Build dependency graph //
            // Config
            var gameConfig = GameConfig.CreateDefault();

            // Services
            var randomService = new RandomService();

            // UI
            var console = new ConsoleWrapper();
            var ui = new UIGameView(console);

            // Events
            var eventManager = new GameEventManager();
            var eventHandler = new GameEventHandler(eventManager, ui, gameConfig);

            // Managers
            var playerManager = new PlayerManager(eventManager);
            var deckManager = new DeckManager(eventManager, randomServices);
            var dealingManager = new DealingManager(eventManager);
            var teamManager = new TeamManager(gameConfig);
            var bettingManager = new BettingManager(playerManager.Players.ToList(), 3, eventManager);
            var trumpSelectionManager = new TrumpSelectionManager(eventManager);
            var scoringManager = new ScoringManager(eventManager, playerManager.Players.ToList(), teamManager, gameConfig);

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