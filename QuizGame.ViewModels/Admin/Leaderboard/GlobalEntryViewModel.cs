namespace QuizGame.ViewModels.Admin.Leaderboard
{
    public class GlobalEntryViewModel
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = null!;
        public int TotalScore { get; set; }
        public double AverageScore { get; set; }
        public int BestScore { get; set; }
        public int Attempts { get; set; }
    }
}