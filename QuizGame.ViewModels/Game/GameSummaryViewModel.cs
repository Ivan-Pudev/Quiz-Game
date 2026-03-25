namespace QuizGame.ViewModels.Game
{
    using QuizGame.Data.Models;
    using System;
    public class GameSummaryViewModel
    {
        public int QuizId { get; set; }

        public string QuizTitle { get; set; } = null!;

        public int Score { get; set; }

        public int MaxScore { get; set; }

        public int CorrectAnswers { get; set; }

        public int TotalQuestions { get; set; }

        public int LeaderboardId { get; set; }

        public virtual Leaderboard Leaderboard { get; set; } = null!;
    }
}
