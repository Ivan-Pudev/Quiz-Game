namespace QuizGame.ViewModels.Admin.Leaderboard
{
    public class AdminLeaderboardViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateOnly LastUpdated { get; set; }
        public Guid QuizId { get; set; }
        public string QuizTitle { get; set; } = null!;
        public int EntryCount { get; set; }
    }
}
