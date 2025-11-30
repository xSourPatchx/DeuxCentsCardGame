using DeuxCentsCardGame.Controllers;
using DeuxCentsCardGame.Events;
using DeuxCentsCardGame.Gameplay;
using DeuxCentsCardGame.Interfaces.Controllers;
using DeuxCentsCardGame.Interfaces.Events;
using DeuxCentsCardGame.Interfaces.GameConfig;
using DeuxCentsCardGame.Interfaces.Gameplay;
using DeuxCentsCardGame.Interfaces.Managers;
using DeuxCentsCardGame.Interfaces.Services;
using DeuxCentsCardGame.Interfaces.UI;
using DeuxCentsCardGame.Managers;
using DeuxCentsCardGame.Services;
using DeuxCentsCardGame.UI;
using DeuxCentsCardGame.Validators;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DeuxCentsCardGame
{
    class Program
    {
        static void Main(string[] args)
        {
            // Build configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // Setup dependency injection
            var serviceProvider = ConfigureServices(configuration);

            // Players are now automatically configured from appsettings.json
            // Configure player types (example: 2 humans vs 2 AI)
            // var playerManager = serviceProvider.GetRequiredService<IPlayerManager>() as PlayerManager;
                        
            // Uncomment one of these configurations:
            
            // All human players (default)
            // playerManager.InitializePlayersWithTypes(PlayerType.Human, PlayerType.Human, PlayerType.Human, PlayerType.Human);
            
            // 2 humans vs 2 AI
            // playerManager.InitializePlayersWithTypes(PlayerType.Human, PlayerType.AI, PlayerType.Human, PlayerType.AI);
            
            // 1 human vs 3 AI
            // playerManager.InitializePlayersWithTypes(PlayerType.Human, PlayerType.AI, PlayerType.AI, PlayerType.AI);

            // Get the game controller and start the game
            var game = serviceProvider.GetRequiredService<IGameController>();
            game.StartGame();

            Console.ReadKey();
        }

        private static ServiceProvider ConfigureServices(IConfiguration configuration)
        {
            var services = new ServiceCollection();

            // Register configuration
            services.AddSingleton<IConfiguration>(configuration);
            services.AddSingleton<IGameConfig>(sp =>
                GameConfig.GameConfig.CreateFromConfiguration(configuration));

            // Register services
            services.AddSingleton<CardCollectionHelper>();
            services.AddSingleton<ICardUtility, CardUtility>();
            services.AddSingleton<IRandomService, RandomService>();

            // Register gameplay logic components
            services.AddSingleton<IBettingLogic, BettingLogic>();
            services.AddSingleton<ICardLogic, CardLogic>();
            services.AddSingleton<HandEvaluator>();
            services.AddSingleton<IScoringLogic, ScoringLogic>();
            services.AddSingleton<TrickAnalyzer>();

            // Register validators
            services.AddSingleton<BettingValidator>(sp =>
            {
                var gameConfig = sp.GetRequiredService<IGameConfig>();
                var playerManager = sp.GetRequiredService<IPlayerManager>();
                return new BettingValidator(gameConfig, playerManager.Players.ToList());
            });
            services.AddSingleton<CardPlayValidator>();
            services.AddSingleton<CardValidator>();

            // Register AI service
            services.AddSingleton<IAIService, AIService>();

            // Register UI
            services.AddSingleton<IConsoleWrapper, ConsoleWrapper>();
            services.AddSingleton<IUIGameView, UIGameView>();

            // Register event system
            services.AddSingleton<GameEventManager>();
            services.AddSingleton<IGameEventManager>(sp => sp.GetRequiredService<GameEventManager>());
            services.AddSingleton<IGameEventHandler, GameEventHandler>();

            // Register managers
            services.AddSingleton<IPlayerManager, PlayerManager>();
            services.AddSingleton<IPlayerTurnManager>(sp =>
            {
                var gameConfig = sp.GetRequiredService<IGameConfig>();
                return new PlayerTurnManager(gameConfig.TotalPlayers);
            });
            services.AddSingleton<IDeckManager, DeckManager>();
            services.AddSingleton<IDealingManager, DealingManager>();
            services.AddSingleton<ITeamManager, TeamManager>();
            services.AddSingleton<IBettingManager>(sp =>
            {
                var playerManager = sp.GetRequiredService<IPlayerManager>();
                var eventManager = sp.GetRequiredService<IGameEventManager>();
                var gameConfig = sp.GetRequiredService<IGameConfig>();
                var bettingValidator = sp.GetRequiredService<BettingValidator>();
                var bettingLogic = sp.GetRequiredService<BettingLogic>();
                return new BettingManager(
                    playerManager.Players.ToList(),
                    gameConfig.InitialDealerIndex,
                    eventManager,
                    gameConfig,
                    bettingValidator,
                    bettingLogic);
            });
            services.AddSingleton<ITrumpSelectionManager, TrumpSelectionManager>();
            services.AddSingleton<IScoringManager>(sp =>
            {
                var eventManager = sp.GetRequiredService<IGameEventManager>();
                var playerManager = sp.GetRequiredService<IPlayerManager>();
                var teamManager = sp.GetRequiredService<ITeamManager>();
                var scoringLogic = sp.GetRequiredService<ScoringLogic>();
                return new ScoringManager(
                    eventManager,
                    playerManager.Players.ToList(),
                    teamManager,
                    scoringLogic);
            });

            // Register orchestrators
            services.AddSingleton<RoundController>(sp =>
            {
                var gameConfig = sp.GetRequiredService<IGameConfig>();
                return new RoundController(
                    sp.GetRequiredService<IGameEventManager>(),
                    sp.GetRequiredService<IPlayerManager>(),
                    sp.GetRequiredService<IPlayerTurnManager>(),
                    sp.GetRequiredService<IDeckManager>(),
                    sp.GetRequiredService<IDealingManager>(),
                    sp.GetRequiredService<IBettingManager>(),
                    sp.GetRequiredService<ITrumpSelectionManager>(),
                    sp.GetRequiredService<IScoringManager>(),
                    gameConfig.InitialDealerIndex);
            });
            services.AddSingleton<TrickController>();

            // Register controller
            services.AddSingleton<IGameController, GameController>();

            return services.BuildServiceProvider();
        }
    }
}