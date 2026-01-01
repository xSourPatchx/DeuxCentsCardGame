// ===== NEW INTERFACES TO ADD =====

// Interfaces/Gameplay/IHandEvaluator.cs - Already exists but update implementation

// ===== REFACTORED GameEventHandler =====

// Events/GameEventHandler.cs
using DeuxCentsCardGame.AI;
using DeuxCentsCardGame.Interfaces.Events;
using DeuxCentsCardGame.Interfaces.GameConfig;
using DeuxCentsCardGame.Interfaces.Models;
using DeuxCentsCardGame.Interfaces.Services;
using DeuxCentsCardGame.Interfaces.UI;

namespace DeuxCentsCardGame.Events
{
    public class GameEventHandler : IGameEventHandler
    {
        private readonly ICardUtility _cardUtility;
        private readonly IGameConfig _gameConfig;
        private readonly IGameEventManager _eventManager; // Changed from concrete
        private readonly IUIGameView _ui;
        private readonly IAIService _aiService;
        private readonly AIDifficulty _defaultAIDifficulty = AIDifficulty.Medium;

        private List<(Card card, Player player)> _currentTrickCards = [];

        public GameEventHandler(
            IGameEventManager eventManager,
            IUIGameView ui, 
            IGameConfig gameConfig, 
            ICardUtility cardUtility, 
            IAIService aiService)
        {
            _cardUtility = cardUtility ?? throw new ArgumentNullException(nameof(cardUtility));
            _eventManager = eventManager ?? throw new ArgumentNullException(nameof(eventManager));
            _ui = ui ?? throw new ArgumentNullException(nameof(ui));
            _gameConfig = gameConfig ?? throw new ArgumentNullException(nameof(gameConfig));
            _aiService = aiService ?? throw new ArgumentNullException(nameof(aiService));

            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            // Subscribe to events from the event manager
            // Implementation will depend on what events IGameEventManager exposes
        }

        // ... rest of implementation
    }
}