/Core
  /Domain
    /Models
      - Card.cs (pure data model)
      - Deck.cs (collection of cards)
      - Player.cs (player state)
      - GameState.cs
    /Enums
      - CardSuit.cs
      - CardFace.cs
      - Team.cs
    /ValueObjects
      - CardSpecs.cs (validation rules)
      - GameRules.cs
  /Application
    /Services
      - CardValidationService.cs
      - GameRuleService.cs
      - CardComparisonService.cs
    /Managers (your existing managers)
      - BettingManager.cs
      - ScoringManager.cs
      - etc.
  /Infrastructure
    - RandomProvider.cs (renamed to IRandomService)
    - ConsoleWrapper.cs
    - GameConfig.cs

/Game
  /Controllers
    - GameController.cs
  /Events
    - GameEventManager.cs
    - GameEventHandler.cs
    /EventArgs (existing)

/UI
  /Console (current implementation)
    - UIGameView.cs
    - ConsoleWrapper.cs
  /Unity (future Unity implementation)
    - UnityUIGameView.cs
    - UnityInputHandler.cs

/Interfaces
  - (Organized by domain/layer)

/Configuration
  - GameConfig.cs
  - appsettings.json