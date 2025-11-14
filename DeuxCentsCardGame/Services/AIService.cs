using DeuxCentsCardGame.AI;
using DeuxCentsCardGame.Interfaces.AI;
using DeuxCentsCardGame.Interfaces.Services;
using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Services
{
    public class AIService : IAIService
    {
        private readonly IRandomService _randomService;
        private readonly Dictionary<AIDifficulty, IAIPlayer> _aiPlayers;


        public AIService(IRandomService randomService)
        {
            _randomService = randomService ?? throw new ArgumentNullException(nameof(randomService));
            
            // Initialize AI players for each difficulty
            _aiPlayers = new Dictionary<AIDifficulty, IAIPlayer>
            {
                { AIDifficulty.Easy, new EasyAIPlayer(_randomService) },
                { AIDifficulty.Medium, new MediumAIPlayer(_randomService) },
                { AIDifficulty.Hard, new HardAIPlayer(_randomService) }
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