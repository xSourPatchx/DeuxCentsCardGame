using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Events
{
    // Event argument classes
    public class CardPlayedEventArgs : EventArgs
    {
        public Player Player { get; }
        public Card Card { get; }
        public int TrickNumber { get; }
        public CardSuit? LeadingSuit { get; }
        public CardSuit? TrumpSuit { get; }

        public CardPlayedEventArgs(Player player, Card card, int trickNumber, CardSuit? leadingSuit, CardSuit? trumpSuit)
        {
            TrickNumber = trickNumber;
            Player = player;
            Card = card;
            LeadingSuit = leadingSuit;
            TrumpSuit = trumpSuit;
        }
    }

    public class TrickCompletedEventArgs : EventArgs
    {
        public int TrickNumber { get; }
        public Player WinningPlayer { get; }
        public Card WinningCard { get; }
        public List<(Card card, Player player)> PlayedCards { get; }
        public int TrickPoints { get; }

        public TrickCompletedEventArgs(int trickNumber, Player winningPlayer, Card winningCard, List<(Card card, Player player)> playedCards, int trickPoints)
        {
            WinningPlayer = winningPlayer;
            WinningCard = winningCard;
            PlayedCards = new List<(Card card, Player player)>(playedCards);
            TrickPoints = trickPoints;
            TrickNumber = trickNumber;
        }
    }

    public class BettingEventArgs : EventArgs
    {
        public Player Player { get; }
        public int Bet { get; }
        public bool HasPassed { get; }

        public BettingEventArgs(Player player, int bet, bool hasPassed = false)
        {
            Player = player;
            Bet = bet;
            HasPassed = hasPassed;
        }
    }

    public class RoundEventArgs : EventArgs
    {
        public int RoundNumber { get; }
        public Player Dealer { get; }

        public RoundEventArgs(int roundNumber, Player dealer)
        {
            RoundNumber = roundNumber;
            Dealer = dealer;
        }
    }

    public class BettingCompletedEventArgs : EventArgs
    {
        public Player WinningBidder { get; }
        public int WinningBid { get; }
        public Dictionary<Player, int> AllBids { get; }

        public BettingCompletedEventArgs(Player winningBidder, int winningBid, Dictionary<Player, int> allBids)
        {
            WinningBidder = winningBidder;
            WinningBid = winningBid;
            AllBids = new Dictionary<Player, int>(allBids);
        }
    }

    public class TrumpSelectedEventArgs : EventArgs
    {
        public CardSuit TrumpSuit { get; }
        public Player SelectedBy { get; }

        public TrumpSelectedEventArgs(CardSuit trumpSuit, Player selectedBy)
        {
            TrumpSuit = trumpSuit;
            SelectedBy = selectedBy;
        }
    }

    public class CardsDealtEventArgs : EventArgs
    {
        public List<Player> Players { get; }
        public int DealerIndex { get; }

        public CardsDealtEventArgs(List<Player> players, int dealerIndex)
        {
            Players = new List<Player>(players);
            DealerIndex = dealerIndex;
        }
    }

    public class ScoreEventArgs : EventArgs
    {
        public int TeamOneRoundPoints { get; }
        public int TeamTwoRoundPoints { get; }
        public int TeamOneTotalPoints { get; }
        public int TeamTwoTotalPoints { get; }
        public bool IsBidWinnerTeamOne { get; }
        public int WinningBid { get; }

        public ScoreEventArgs(int teamOneRoundPoints, int teamTwoRoundPoints,
            int teamOneTotalPoints, int teamTwoTotalPoints, bool isBidWinnerTeamOne, int winningBid)
        {
            TeamOneRoundPoints = teamOneRoundPoints;
            TeamTwoRoundPoints = teamTwoRoundPoints;
            TeamOneTotalPoints = teamOneTotalPoints;
            TeamTwoTotalPoints = teamTwoTotalPoints;
            IsBidWinnerTeamOne = isBidWinnerTeamOne;
            WinningBid = winningBid;
        }
    }

    public class GameOverEventArgs : EventArgs
    {
        public int TeamOneFinalScore { get; }
        public int TeamTwoFinalScore { get; }
        public bool IsTeamOneWinner { get; }

        public GameOverEventArgs(int teamOneFinalScore, int teamTwoFinalScore)
        {
            TeamOneFinalScore = teamOneFinalScore;
            TeamTwoFinalScore = teamTwoFinalScore;
            IsTeamOneWinner = teamOneFinalScore > teamTwoFinalScore;
        }
    }
}