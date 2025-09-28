using DeuxCentsCardGame.Events;
using DeuxCentsCardGame.Interfaces.Managers;
using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Managers
{
    public class PlayerManager : IPlayerManager
    {
        private readonly GameEventManager _eventManager;
        private readonly List<Player> _players;
        public IReadOnlyList<Player> Players => _players.AsReadOnly();

        public PlayerManager(GameEventManager eventManager)
        {
            _eventManager = eventManager;
            _players = InitializePlayers();
        }

        private List<Player> InitializePlayers()
        {
            return new List<Player>
            {
                new("Player 1"),
                new("Player 2"), 
                new("Player 3"),
                new("Player 4")
            };
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