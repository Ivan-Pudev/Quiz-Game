namespace QuizGame.Data
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using QuizGame.Data.Models;
    using System.Reflection.Emit;

    public class QuizGameDbContext(DbContextOptions<QuizGameDbContext> options) 
        : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options)
    {
        public virtual DbSet<Question> Questions { get; set; } = null!;
        public virtual DbSet<Quiz> Quizzes { get; set; } = null!;
        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<Leaderboard> Leaderboards { get; set; } = null!;
        public virtual DbSet<LeaderboardEntry> LeaderboardEntries { get; set; } = null!;
        public virtual DbSet<Answer> Answers { get; set; } = null!;
        public virtual DbSet<QuizAttempt> QuizAttempts { get; set; } = null!;
        public virtual DbSet<AttemptAnswer> AttemptAnswers { get; set; } = null!;
        public virtual DbSet<ApplicationUser> ApplicationUsers { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Quiz>()
            .HasMany(q => q.Questions)
            .WithMany(q => q.Quizzes);

            builder.Entity<Quiz>()
            .HasOne(q => q.Leaderboard)
            .WithOne(l => l.Quiz)
            .HasForeignKey<Leaderboard>(l => l.QuizId);

            builder.Entity<LeaderboardEntry>()
                .HasOne(e => e.Leaderboard)
                .WithMany(l => l.Entries)
                .HasForeignKey(e => e.LeaderboardId);

            builder.Entity<LeaderboardEntry>()
                .HasIndex(e => new { e.LeaderboardId, e.UserId });

            builder.ApplyConfigurationsFromAssembly(typeof(QuizGameDbContext).Assembly);
        }
    }
}
