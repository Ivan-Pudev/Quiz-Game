using Microsoft.AspNetCore.Identity;

namespace QuizGame.Data.Models
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FullName { get; set; } = null!;
        public DateTime BirthDate { get; set; }

        public virtual ICollection<QuizAttempt> QuizAttempts { get; set; }
        = new List<QuizAttempt>();

        public virtual ICollection<LeaderboardEntry> LeaderboardEntries { get; set; }
        = new List<LeaderboardEntry>();
    }
}
