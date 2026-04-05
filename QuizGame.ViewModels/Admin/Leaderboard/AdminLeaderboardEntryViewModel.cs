namespace QuizGame.ViewModels.Admin.Leaderboard
{
    using QuizGame.ViewModels.Leaderboards;
    using System;


    public class AdminLeaderboardEntryViewModel : LeaderboardRowVm
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid LeaderboardId { get; set; }
        public string LeaderboardTitle { get; set; } = null!;
    }
}
