using DeuxCentsCardGame.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeuxCentsCardGame.Unity
{
    /// Implement this in Unity with networking solution (Unity Netcode, Mirror, Photon, etc.)
    public interface INetworkSyncService
    {
        // State synchronization
        Task BroadcastGameStateAsync(GameSnapshot snapshot);
        Task RequestGameStateAsync();


        // Player actions
        Task SendPlayerActionAsync(PlayerAction action);
        
        // Events
        event EventHandler<GameSnapshot>? GameStateReceived;
        event EventHandler<PlayerAction>? PlayerActionReceived;
        
        // Connection
        bool IsConnected { get; }
        bool IsHost { get; }
        string PlayerId { get; }
    }
}