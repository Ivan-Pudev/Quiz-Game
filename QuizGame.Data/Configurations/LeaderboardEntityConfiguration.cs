using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuizGame.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuizGame.Data.Configurations
{
    public class LeaderboardEntityConfiguration : IEntityTypeConfiguration<Leaderboard>
    {
        private readonly ICollection<Leaderboard> _leaderboards = new List<Leaderboard>()
        {
            new Leaderboard()
            {
                Id = 1,
                Title = "Friday Night Rankings",
                Description = "Top scores for friday players",
                QuizId = 1, 
                LastUpdated = DateOnly.FromDateTime(DateTime.UtcNow),
            },
        };

        public void Configure(EntityTypeBuilder<Leaderboard> entity)
        {
            entity.HasData(_leaderboards);
        }
    }
}
