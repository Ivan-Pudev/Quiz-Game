namespace QuizGame.ViewModels.Admin.Leaderboard
{
    public class GlobalLeaderboardRowVm
    {
        public string UserName { get; set; } = null!;
        public Guid UserId { get; set; }

        public int Rank { get; set; }

        public int TotalScore { get; set; }
        public double AverageScore { get; set; }
        public int BestScore { get; set; }
        public int Attempts { get; set; }

        // UI helpers (THIS is the key improvement)
        public string Initials { get; set; }
        public string RowClass { get; set; }
        public string RankClass { get; set; }
        public string AvatarClass { get; set; }
    }
}
