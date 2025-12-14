using DeuxCentsCardGame.Interfaces.Gameplay;
using DeuxCentsCardGame.Interfaces.Managers;
using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Gameplay
{
    // Analyzes trick state to determine winning cards, trick values, and strategic information.
    // Can be used by AI players, UI hints, and game analysis features.
    public class TrickAnalyzer
    {
        private readonly ICardLogic _cardLogic;

        public TrickAnalyzer(ICardLogic cardLogic)
        {
            _cardLogic = cardLogic ?? throw new ArgumentNullException(nameof(cardLogic));
        }

        // Gets cards from hand that can legally be played based on leading suit rules.
        public List<Card> GetPlayableCards(List<Card> hand, CardSuit? leadingSuit)
        {
            if (hand == null || hand.Count == 0)
                return new List<Card>();

            // If no leading suit, all cards are playable
            if (!leadingSuit.HasValue)
                return new List<Card>(hand);

            // Must follow suit if possible
            var cardsOfLeadingSuit = hand.Where(c => c.CardSuit == leadingSuit.Value).ToList();
            
            return cardsOfLeadingSuit.Any() ? cardsOfLeadingSuit : new List<Card>(hand);
        }

        // Determines which card is currently winning the trick.
        public Card GetCurrentWinningCard(
            List<(Card card, Player player)> trick, 
            CardSuit? trumpSuit, 
            CardSuit? leadingSuit)
        {
            if (trick == null || trick.Count == 0)
                throw new ArgumentException("Trick cannot be null or empty", nameof(trick));

            var winningCard = trick[0].card;

            for (int i = 1; i < trick.Count; i++)
            {
                if (_cardLogic.WinsAgainst(trick[i].card, winningCard, trumpSuit, leadingSuit))
                {
                    winningCard = trick[i].card;
                }
            }

            return winningCard;
        }

        // Calculates the total point value of all cards in the trick.
        public int CalculateTrickValue(List<(Card card, Player player)> trick)
        {
            if (trick == null || trick.Count == 0)
                return 0;

            return trick.Sum(t => t.card.CardPointValue);
        }

        // Determines if the player's partner is currently winning the trick.
        public bool IsPartnerWinning(
            List<(Card card, Player player)> trick,
            Player currentPlayer,
            List<Player> allPlayers,
            ITeamManager teamManager,
            CardSuit? trumpSuit,
            CardSuit? leadingSuit)
        {
            if (trick == null || trick.Count == 0)
                return false;

            if (teamManager == null)
                throw new ArgumentNullException(nameof(teamManager));

            if (allPlayers == null || allPlayers.Count == 0)
                throw new ArgumentNullException(nameof(allPlayers));

            // Get the current winning card
            var winningCard = GetCurrentWinningCard(trick, trumpSuit, leadingSuit);

            // Find who played the winning card
            var winningPlay = trick.FirstOrDefault(t => t.card == winningCard);
            if (winningPlay.player == null)
                return false;

            // Find indices
            int currentPlayerIndex = allPlayers.IndexOf(currentPlayer);
            int winningPlayerIndex = allPlayers.IndexOf(winningPlay.player);

            if (currentPlayerIndex == -1 || winningPlayerIndex == -1)
                return false;

            // Check if they're on the same team
            var currentPlayerTeam = teamManager.GetPlayerTeam(currentPlayerIndex);
            var winningPlayerTeam = teamManager.GetPlayerTeam(winningPlayerIndex);

            // Partner is winning if they're on same team but not the same player
            return currentPlayerTeam == winningPlayerTeam && currentPlayerIndex != winningPlayerIndex;
        }

        // Gets all cards from hand that would beat the current winning card.
        public List<Card> GetWinningCards(
            List<Card> hand,
            Card currentWinner,
            CardSuit? trumpSuit,
            CardSuit? leadingSuit)
        {
            if (hand == null || hand.Count == 0)
                return new List<Card>();

            if (currentWinner == null)
                throw new ArgumentNullException(nameof(currentWinner));

            return hand
                .Where(card => _cardLogic.WinsAgainst(card, currentWinner, trumpSuit, leadingSuit))
                .OrderBy(card => card.CardFaceValue)
                .ToList();
        }

        // Determines if the trick is worth trying to win based on point value.
        public bool IsTrickValuable(List<(Card card, Player player)> trick, int threshold = 10)
        {
            return CalculateTrickValue(trick) >= threshold;
        }

        // Gets the player who is currently winning the trick.
        public Player GetCurrentWinningPlayer(
            List<(Card card, Player player)> trick,
            CardSuit? trumpSuit,
            CardSuit? leadingSuit)
        {
            if (trick == null || trick.Count == 0)
                throw new ArgumentException("Trick cannot be null or empty", nameof(trick));

            var winningCard = GetCurrentWinningCard(trick, trumpSuit, leadingSuit);
            return trick.First(t => t.card == winningCard).player;
        }

        // Analyzes the complete trick state and returns comprehensive information.
        // Useful for UI displays and AI decision-making.
        public TrickAnalysis AnalyzeTrick(
            List<(Card card, Player player)> trick,
            CardSuit? trumpSuit,
            CardSuit? leadingSuit)
        {
            if (trick == null || trick.Count == 0)
            {
                return new TrickAnalysis
                {
                    IsEmpty = true,
                    TrickValue = 0,
                    WinningCard = null,
                    WinningPlayer = null,
                    CardsPlayed = 0
                };
            }

            var winningCard = GetCurrentWinningCard(trick, trumpSuit, leadingSuit);
            var winningPlayer = trick.First(t => t.card == winningCard).player;

            return new TrickAnalysis
            {
                IsEmpty = false,
                TrickValue = CalculateTrickValue(trick),
                WinningCard = winningCard,
                WinningPlayer = winningPlayer,
                CardsPlayed = trick.Count,
                IsValuable = IsTrickValuable(trick)
            };
        }
    }

    // Comprehensive analysis result for a trick.
    public class TrickAnalysis
    {
        public bool IsEmpty { get; set; }
        public int TrickValue { get; set; }
        public Card? WinningCard { get; set; }
        public Player? WinningPlayer { get; set; }
        public int CardsPlayed { get; set; }
        public bool IsValuable { get; set; }
    }
}