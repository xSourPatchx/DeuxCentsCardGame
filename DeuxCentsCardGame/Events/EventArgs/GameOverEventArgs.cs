namespace DeuxCentsCardGame.Events.EventArgs
{
    public class GameOverEventArgs : System.EventArgs
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