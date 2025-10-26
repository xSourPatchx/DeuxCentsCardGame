using DeuxCentsCardGame.Models;
using DeuxCentsCardGame.Enums;

namespace DeuxCentsCardGame.Events.EventArgs
{
    public class InvalidMoveEventArgs : System.EventArgs
    {
        public Player Player { get; }
        public string Message { get; }
        public InvalidMoveType MoveType { get; }

        public InvalidMoveEventArgs(Player player, string message, InvalidMoveType moveType)
        {
            Player = player;
            Message = message;
            MoveType = moveType;
        }
    }
}