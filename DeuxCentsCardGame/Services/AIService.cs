using DeuxCentsCardGame.AI;
using DeuxCentsCardGame.Interfaces.AI;
using DeuxCentsCardGame.Interfaces.Services;
using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Services
{
    public class AIService : IAIService
    {
        private readonly IRandomService _randomService;
        private readonly ICardUtility _cardUtility;
        private readonly Dictionary<AIDifficulty, IAIPlayer> _aiPlayers;


        public AIService(IRandomService randomService, ICardUtility cardUtility)
        {
            _randomService = randomService ?? throw new ArgumentNullException(nameof(randomService));
            _cardUtility = cardUtility ?? throw new ArgumentNullException(nameof(cardUtility));
            
            // Initialize AI players for each difficulty
            _aiPlayers = new Dictionary<AIDifficulty, IAIPlayer>
            {
                { AIDifficulty.Easy, new EasyAIPlayer(_randomService, _cardUtility) },
                { AIDifficulty.Medium, new MediumAIPlayer(_randomService, _cardUtility) },
                { AIDifficulty.Hard, new HardAIPlayer(_randomService, _cardUtility) }
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