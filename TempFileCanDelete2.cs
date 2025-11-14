// Add this to your existing GameEventHandler.cs

// Update constructor to accept IAIService:
public GameEventHandler(GameEventManager eventManager, IUIGameView ui,
IGameConfig gameConfig, IAIService aiService)
{
_eventManager = eventManager;
_ui = ui ?? throw new ArgumentNullException(nameof(ui));
_gameConfig = gameConfig;
_aiService = aiService ?? throw new ArgumentNullException(nameof(aiService));


SubscribeToEvents();


}

// Modify the OnBetInput event handler:
public void OnBetInput(object? sender, BetInputEventArgs e)
{
// Check if current player is AI
if (e.CurrentPlayer.IsAI())
{
// Get list of already taken bids
var takenBids = GetTakenBids(e);


    // Let AI make decision
    int aiBet = _aiService.MakeAIBettingDecision(
        e.CurrentPlayer, 
        _defaultAIDifficulty,
        e.MinimumBet, 
        e.MaximumBet, 
        e.BetIncrement,
        GetCurrentHighestBid(),
        takenBids
    );
    
    e.Response = aiBet == -1 ? "pass" : aiBet.ToString();
    
    // Show AI decision to player
    _ui.DisplayFormattedMessage("[AI] {0} decision: {1}", 
        e.CurrentPlayer.Name, 
        e.Response);
    Thread.Sleep(1000); // Brief pause for readability
}
else
{
    // Human player input
    string prompt = $"{e.CurrentPlayer.Name}, enter a bet (between {e.MinimumBet}-{e.MaximumBet}, intervals of {e.BetIncrement}) or 'pass': ";
    string betInput = _ui.GetUserInput(prompt).ToLower();
    e.Response = betInput;
}


}

// Modify the OnTrumpSelectionInput event handler:
public void OnTrumpSelectionInput(object? sender, TrumpSelectionInputEventArgs e)
{
if (e.SelectingPlayer.IsAI())
{
// Let AI select trump
CardSuit aiTrumpChoice = _aiService.MakeAITrumpSelection(
e.SelectingPlayer,
_defaultAIDifficulty
);


    e.Response = Deck.CardSuitToString(aiTrumpChoice);
    
    _ui.DisplayFormattedMessage("[AI] {0} selecting trump...", e.SelectingPlayer.Name);
    Thread.Sleep(1000);
}
else
{
    // Human player input
    string[] validSuits = Enum.GetNames<CardSuit>().Select(suit => suit.ToLower()).ToArray();
    string prompt = $"{e.SelectingPlayer.Name}, please choose a trump suit. (enter \"clubs\", \"diamonds\", \"hearts\", \"spades\")";
    e.Response = _ui.GetOptionInput(prompt, validSuits);
}


}

// Modify the OnCardSelectionInput event handler:
public void OnCardSelectionInput(object? sender, CardSelectionInputEventArgs e)
{
if (e.CurrentPlayer.IsAI())
{
// Get cards already played in current trick (passed via event args if available)
var cardsPlayedInTrick = new List<(Card card, Player player)>(); // You may need to track this


    // Let AI choose card
    int aiCardIndex = _aiService.MakeAICardSelection(
        e.CurrentPlayer,
        _defaultAIDifficulty,
        e.LeadingSuit,
        e.TrumpSuit,
        cardsPlayedInTrick
    );
    
    e.Response = aiCardIndex;
    
    _ui.DisplayFormattedMessage("[AI] {0} is choosing a card...", e.CurrentPlayer.Name);
    Thread.Sleep(1000);
}
else
{
    // Human player input
    string leadingSuitString = e.LeadingSuit.HasValue ? Deck.CardSuitToString(e.LeadingSuit.Value) : "none";
    string trumpSuitString = e.TrumpSuit.HasValue ? Deck.CardSuitToString(e.TrumpSuit.Value) : "none";

    string prompt = $"{e.CurrentPlayer.Name}, choose a card to play (enter index 0-{e.Hand.Count - 1}" +
        (e.LeadingSuit.HasValue ? $", leading suit is {leadingSuitString}" : "") +
        $" and trump suit is {trumpSuitString}):";

    e.Response = _ui.GetIntInput(prompt, 0, e.Hand.Count - 1);
}


}

// Helper method to get taken bids
private List<int> GetTakenBids(BetInputEventArgs e)
{
// You’ll need to track this in your BettingManager and pass it through events
// For now, return empty list - implement based on your needs
return new List<int>();
}

// Helper method to get current highest bid
private int GetCurrentHighestBid()
{
// You’ll need to track this - possibly pass through event args
return 0;
}

// Update OnPlayerTurn to show AI players differently:
public void OnPlayerTurn(object? sender, PlayerTurnEventArgs e)
{
if (e.Player.IsHuman())
{
_ui.DisplayHand(e.Player);
}
else
{
_ui.DisplayMessage($”\n[AI] {e.Player.Name}’s turn…”);
}


DisplayTurnInformation(e.Player, e.TrickNumber, e.LeadingSuit, e.TrumpSuit);


}