using QuizGame.ViewModels.Leaderboards;

namespace QuizGame.ViewModels.Admin.Leaderboard
{
    public class AddEntryInputModel : LeaderboardRowVm
    {
        public Guid UserId { get; set; }
    }
}