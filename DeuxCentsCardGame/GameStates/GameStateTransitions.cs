namespace DeuxCentsCardGame.GameStates
{
    public class GameStateTransitions
    {
        private readonly Dictionary<GameState, List<GameState>> _validTransitions;

        public GameStateTransitions()
        {
            _validTransitions = InitializeValidTransitions();
        }

        // Initializes the valid state transitions for the game.
        // Each state can only transition to specific next states.
        private Dictionary<GameState, List<GameState>> InitializeValidTransitions()
        {
            return new Dictionary<GameState, List<GameState>>
            {
                // Initialization can only go to RoundStart
                [GameState.Initialization] = new List<GameState>
                {
                    GameState.RoundStart
                },

                // RoundStart can only go to DeckPreparation
                [GameState.RoundStart] = new List<GameState>
                {
                    GameState.DeckPreparation
                },

                // DeckPreparation can only go to Betting
                [GameState.DeckPreparation] = new List<GameState>
                {
                    GameState.Betting
                },

                // Betting can only go to TrumpSelection
                [GameState.Betting] = new List<GameState>
                {
                    GameState.TrumpSelection
                },

                // TrumpSelection can only go to Playing
                [GameState.TrumpSelection] = new List<GameState>
                {
                    GameState.Playing
                },

                // Playing can only go to RoundEnd
                [GameState.Playing] = new List<GameState>
                {
                    GameState.RoundEnd
                },

                // RoundEnd can go to either RoundStart (next round) or GameOver
                [GameState.RoundEnd] = new List<GameState>
                {
                    GameState.RoundStart,
                    GameState.GameOver
                },

                // GameOver is a terminal state - no valid transitions
                [GameState.GameOver] = new List<GameState>()
            };
        }

        // Checks if a transition from one state to another is valid.
        public bool IsValidTransition(GameState from, GameState to)
        {
            if (!_validTransitions.ContainsKey(from))
            {
                return false;
            }

            return _validTransitions[from].Contains(to);
        }

        // Gets a list of valid next states from the current state.
        public List<GameState> GetValidNextStates(GameState current)
        {
            if (!_validTransitions.ContainsKey(current))
            {
                return new List<GameState>();
            }

            return new List<GameState>(_validTransitions[current]);
        }

        // Validates a state transition and throws an exception if invalid.
        // Use this method when you want to enforce strict validation.
        public void ValidateTransition(GameState from, GameState to)
        {
            if (!IsValidTransition(from, to))
            {
                var validStates = GetValidNextStates(from);
                string validStatesString = validStates.Count > 0 
                    ? string.Join(", ", validStates) 
                    : "none (terminal state)";

                throw new InvalidOperationException(
                    $"Invalid state transition from {from} to {to}. " +
                    $"Valid transitions from {from}: {validStatesString}");
            }
        }

        // Checks if a state is a terminal state (no valid outgoing transitions).
        public bool IsTerminalState(GameState state)
        {
            return _validTransitions.ContainsKey(state) && 
                _validTransitions[state].Count == 0;
        }

        // Gets all possible states in the game state machine.
        public List<GameState> GetAllStates()
        {
            return _validTransitions.Keys.ToList();
        }

        // Checks if a state has multiple possible next states (branching point).
        public bool IsBranchingState(GameState state)
        {
            return _validTransitions.ContainsKey(state) && 
                _validTransitions[state].Count > 1;
        }
    }
}