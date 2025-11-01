﻿using DeuxCentsCardGame.Controllers;
using DeuxCentsCardGame.Events;
using DeuxCentsCardGame.Interfaces.Controllers;
using DeuxCentsCardGame.Interfaces.Events;
using DeuxCentsCardGame.Interfaces.GameConfig;
using DeuxCentsCardGame.Interfaces.Managers;
using DeuxCentsCardGame.Interfaces.Services;
using DeuxCentsCardGame.Interfaces.UI;
using DeuxCentsCardGame.Managers;
using DeuxCentsCardGame.Services;
using DeuxCentsCardGame.UI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
            services.AddSingleton<IRandomService, RandomService>();

            // Register UI
            services.AddSingleton<IConsoleWrapper, ConsoleWrapper>();
            services.AddSingleton<IUIGameView, UIGameView>();

            // Register event system
            services.AddSingleton<GameEventManager>();
            services.AddSingleton<IGameEventManager>(sp => sp.GetRequiredService<GameEventManager>());
            services.AddSingleton<IGameEventHandler, GameEventHandler>();

            // Register managers
            services.AddSingleton<IPlayerManager, PlayerManager>();
            services.AddSingleton<IDeckManager, DeckManager>();
            services.AddSingleton<IDealingManager, DealingManager>();
            services.AddSingleton<ITeamManager, TeamManager>();
            services.AddSingleton<IBettingManager>(sp =>
            {
                var playerManager = sp.GetRequiredService<IPlayerManager>();
                var eventManager = sp.GetRequiredService<IGameEventManager>();
                var gameConfig = sp.GetRequiredService<IGameConfig>();
                return new BettingManager(
                    playerManager.Players.ToList(),
                    gameConfig.InitialDealerIndex,
                    eventManager,
                    gameConfig);
            });
            services.AddSingleton<ITrumpSelectionManager, TrumpSelectionManager>();
            services.AddSingleton<IScoringManager>(sp =>
            {
                var eventManager = sp.GetRequiredService<IGameEventManager>();
                var playerManager = sp.GetRequiredService<IPlayerManager>();
                var teamManager = sp.GetRequiredService<ITeamManager>();
                var gameConfig = sp.GetRequiredService<IGameConfig>();
                return new ScoringManager(
                    eventManager,
                    playerManager.Players.ToList(),
                    teamManager,
                    gameConfig);
            });

            // Register controller
            services.AddSingleton<IGameController, GameController>();

            return services.BuildServiceProvider();
        }
    }
}
