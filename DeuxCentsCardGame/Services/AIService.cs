using DeuxCentsCardGame.AI;
using DeuxCentsCardGame.Interfaces.AI;
using DeuxCentsCardGame.Interfaces.Gameplay;
using DeuxCentsCardGame.Interfaces.Services;
using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Services
{
    public class AIService : IAIService
    {
        private readonly IRandomService _randomService;
        private readonly ICardUtility _cardUtility;
        private readonly ICardLogic _cardLogic;
        private readonly IHandEvaluator _handEvaluator;
        private readonly ICardCollectionHelper _cardHelper;
        private readonly ITrickAnalyzer _trickAnalyzer;
        private readonly Dictionary<AIDifficulty, IAIPlayer> _aiPlayers;

        public AIService(
            IRandomService randomService, 
            ICardUtility cardUtility, 
            ICardLogic cardLogic,
            IHandEvaluator handEvaluator,
            ICardCollectionHelper cardHelper,
            ITrickAnalyzer trickAnalyzer)
        {
            _randomService = randomService ?? throw new ArgumentNullException(nameof(randomService));
            _cardUtility = cardUtility ?? throw new ArgumentNullException(nameof(cardUtility));
            _cardLogic = cardLogic ?? throw new ArgumentNullException(nameof(cardLogic));
            _handEvaluator = handEvaluator ?? throw new ArgumentNullException(nameof(handEvaluator));
            _cardHelper = cardHelper ?? throw new ArgumentNullException(nameof(cardHelper));
            _trickAnalyzer = trickAnalyzer ?? throw new ArgumentNullException(nameof(trickAnalyzer));
            
            // Initialize AI players for each difficulty
            _aiPlayers = new Dictionary<AIDifficulty, IAIPlayer>
            {
                { 
                    AIDifficulty.Easy, 
                    new EasyAIPlayer(_randomService, _cardUtility, _handEvaluator, _trickAnalyzer, _cardHelper) 
                },
                { 
                    AIDifficulty.Medium, 
                    new MediumAIPlayer(_randomService, _cardUtility, _handEvaluator, _cardHelper, _trickAnalyzer) 
                },
                { 
                    AIDifficulty.Hard, 
                    new HardAIPlayer(_randomService, _cardUtility, _handEvaluator, _cardHelper, _trickAnalyzer) 
                }
            };
        }

        public IAIPlayer GetAIPlayer(AIDifficulty difficulty)
        {
            return _aiPlayers[difficulty];
        }

        public async Task<int> MakeAIBettingDecision(Player player, AIDifficulty difficulty, int minBet, 
                                        int maxBet, int betIncrement, int currentHighestBid, 
                                        List<int> takenBids)
        {
            var aiPlayer = GetAIPlayer(difficulty);
            int result = aiPlayer.DecideBet(player.Hand, minBet, maxBet, betIncrement, 
                                    currentHighestBid, takenBids);
            await Task.CompletedTask;
            return result;
        }

        public async Task<CardSuit> MakeAITrumpSelection(Player player, AIDifficulty difficulty)
        {
            var aiPlayer = GetAIPlayer(difficulty);
            CardSuit result = aiPlayer.SelectTrumpSuit(player.Hand);
            await Task.CompletedTask;
            return result;
        }

        public async Task<int> MakeAICardSelection(Player player, AIDifficulty difficulty, CardSuit? leadingSuit, 
                                    CardSuit? trumpSuit, List<(Card card, Player player)> cardsPlayedInTrick)
        {
            var aiPlayer = GetAIPlayer(difficulty);
            int result = aiPlayer.ChooseCard(player.Hand, leadingSuit, trumpSuit, cardsPlayedInTrick);
            await Task.CompletedTask;
            return result;
        }
    }
}