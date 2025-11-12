using DeuxCentsCardGame.Interfaces.Events;
using DeuxCentsCardGame.Interfaces.Managers;
using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Managers
{
    public class PlayerManager : IPlayerManager
    {
        private readonly IGameEventManager _eventManager;
        private readonly List<Player> _players;
        public IReadOnlyList<Player> Players => _players.AsReadOnly();

        public PlayerManager(IGameEventManager eventManager)
        {
            _eventManager = eventManager;
            _players = InitializePlayers();
        }

        /// Initialize players with configurable types
        /// Default type: All human players
        private List<Player> InitializePlayers()
        {
            return new List<Player>
            {
                new("Player 1", PlayerType.Human),
                new("Player 2", PlayerType.Human),
                new("Player 3", PlayerType.Human),
                new("Player 4", PlayerType.Human)
            };
        }

        // Initialize players with specific types
        // Example: InitializePlayersWithTypes(PlayerType.Human, PlayerType.AI, PlayerType.AI, PlayerType.Human)
        public void InitializePlayersWithTypes(params PlayerType[] playerTypes)
        {
            if (playerTypes.Length != 4)
                throw new ArgumentException("Must specify exactly 4 player types");

            _players.Clear();
            for (int i = 0; i < playerTypes.Length; i++)
            {
                string playerName = playerTypes[i] == PlayerType.AI 
                    ? $"CPU {i + 1}" 
                    : $"Player {i + 1}";
                
                _players.Add(new Player(playerName, playerTypes[i]));
            }
        }

        public Player GetPlayer(int index)
        { 
            return _players[index];
        }

        public void ResetAllPlayerBettingStates()
        {
            foreach (var player in _players)
            {
                player.ResetBettingState();
            }
        }

        public void ClearAllPlayerHands()
        { 
            foreach (var player in _players)
            {
                player.Hand.Clear();
            }
        }
    }
}