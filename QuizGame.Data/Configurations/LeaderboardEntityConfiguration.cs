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
                Id = Guid.Parse("a68f8eb4-76ef-41d8-beca-10bce9c61403"),
                Title = "Friday Night Rankings",
                Description = "Top scores for friday players",
                QuizId = Guid.Parse("4301f783-5664-41fc-af53-c2de0e1e454a"), 
                LastUpdated = DateOnly.FromDateTime(DateTime.UtcNow),
            },
        };

        public void Configure(EntityTypeBuilder<Leaderboard> entity)
        {
            entity.HasData(_leaderboards);
        }
    }
}
