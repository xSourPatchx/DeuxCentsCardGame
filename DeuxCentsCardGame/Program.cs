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
using DeuxCentsCardGame.Interfaces.Validators;
using DeuxCentsCardGame.Managers;
using DeuxCentsCardGame.Services;
using DeuxCentsCardGame.UI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DeuxCentsCardGame
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Build configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // Setup dependency injection
            var serviceProvider = ConfigureServices(configuration);

            // Get the game controller and start the game
            var game = serviceProvider.GetRequiredService<IGameController>();
            await game.StartGame();

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
            services.AddSingleton<IRandomService, RandomService>();
            services.AddSingleton<CardCollectionHelper>();
            services.AddSingleton<ICardUtility, CardUtility>();

            // Register gameplay logic components
            services.AddSingleton<CardLogic>();
            services.AddSingleton<ICardLogic>(sp => sp.GetRequiredService<CardLogic>());
            services.AddSingleton<BettingLogic>();
            services.AddSingleton<ScoringLogic>();
            services.AddSingleton<HandEvaluator>();
            services.AddSingleton<TrickAnalyzer>();

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

            services.AddSingleton<ITeamManager, TeamManager>();

            // Register GameValidator - depend on managers
            services.AddSingleton<GameValidator>(sp =>
            {
                var gameConfig = sp.GetRequiredService<IGameConfig>();
                var eventManager = sp.GetRequiredService<IGameEventManager>();
                var cardUtility = sp.GetRequiredService<ICardUtility>();
                var playerManager = sp.GetRequiredService<IPlayerManager>();
                return new GameValidator(
                    gameConfig, 
                    eventManager, 
                    cardUtility,
                    playerManager.Players.ToList());
            });
            services.AddSingleton<IGameValidator>(sp => sp.GetRequiredService<GameValidator>());
        
            // Register DeckManager - depends on IGameValidator
            services.AddSingleton<IDeckManager, DeckManager>();
            services.AddSingleton<IDealingManager, DealingManager>();

            // Register BettingManager - depends on GameValidator
            services.AddSingleton<BettingManager>(sp =>
            {
                var playerManager = sp.GetRequiredService<IPlayerManager>();
                var eventManager = sp.GetRequiredService<IGameEventManager>();
                var gameConfig = sp.GetRequiredService<IGameConfig>();
                var gameValidator = sp.GetRequiredService<GameValidator>();
                var bettingLogic = sp.GetRequiredService<BettingLogic>();
                return new BettingManager(
                    playerManager.Players.ToList(),
                    gameConfig.InitialDealerIndex,
                    eventManager,
                    gameConfig,
                    gameValidator,
                    bettingLogic);
            });
            services.AddSingleton<IBettingManager>(sp => sp.GetRequiredService<BettingManager>());

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

            // Register controllers
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

            // Register game controller
            services.AddSingleton<IGameController, GameController>();

            return services.BuildServiceProvider();
        }
    }
}