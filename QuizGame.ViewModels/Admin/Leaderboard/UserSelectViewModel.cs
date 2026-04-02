using QuizGame.Data.Models;

namespace QuizGame.ViewModels.Admin.Leaderboard
{
    public class UserSelectViewModel
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = null!;
    }
}