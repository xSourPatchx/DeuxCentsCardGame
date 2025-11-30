using DeuxCentsCardGame.AI;
using DeuxCentsCardGame.Gameplay;
using DeuxCentsCardGame.Interfaces.AI;
using DeuxCentsCardGame.Interfaces.Services;
using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Services
{
    public class AIService : IAIService
    {
        private readonly IRandomService _randomService;
        private readonly ICardUtility _cardUtility;
        private readonly CardLogic _cardComparer;
        private readonly HandEvaluator _handEvaluator;
        private readonly CardCollectionHelper _cardHelper;
        private readonly TrickAnalyzer _trickAnalyzer;
        private readonly Dictionary<AIDifficulty, IAIPlayer> _aiPlayers;


        public AIService(
            IRandomService randomService, 
            ICardUtility cardUtility, 
            CardLogic cardComparer,
            HandEvaluator handEvaluator,
            CardCollectionHelper cardHelper,
            TrickAnalyzer trickAnalyzer)
        {
            _randomService = randomService ?? throw new ArgumentNullException(nameof(randomService));
            _cardUtility = cardUtility ?? throw new ArgumentNullException(nameof(cardUtility));
            _cardComparer = cardComparer ?? throw new ArgumentNullException(nameof(cardComparer));
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
                    new MediumAIPlayer(_randomService, _cardUtility, _handEvaluator, _cardHelper, _cardComparer, _trickAnalyzer) 
                },
                { 
                    AIDifficulty.Hard, 
                    new HardAIPlayer(_randomService, _cardUtility, _handEvaluator, _cardHelper, _cardComparer, _trickAnalyzer) 
                }
            };
        }

        public IAIPlayer GetAIPlayer(AIDifficulty difficulty)
        {
            return _aiPlayers[difficulty];
        }

        public int MakeAIBettingDecision(Player player, AIDifficulty difficulty, int minBet, 
                                        int maxBet, int betIncrement, int currentHighestBid, 
                                        List<int> takenBids)
        {
            var aiPlayer = GetAIPlayer(difficulty);
            return aiPlayer.DecideBet(player.Hand, minBet, maxBet, betIncrement, 
                                    currentHighestBid, takenBids);
        }

        public CardSuit MakeAITrumpSelection(Player player, AIDifficulty difficulty)
        {
            var aiPlayer = GetAIPlayer(difficulty);
            return aiPlayer.SelectTrumpSuit(player.Hand);
        }

        public int MakeAICardSelection(Player player, AIDifficulty difficulty, CardSuit? leadingSuit, 
                                    CardSuit? trumpSuit, List<(Card card, Player player)> cardsPlayedInTrick)
        {
            var aiPlayer = GetAIPlayer(difficulty);
            return aiPlayer.ChooseCard(player.Hand, leadingSuit, trumpSuit, cardsPlayedInTrick);
        }
    }
}