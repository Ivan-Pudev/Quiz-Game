using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuizGame.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuizGame.Data.Configurations
{
    public class LeaderboardEntryEntityConfiguration : IEntityTypeConfiguration<LeaderboardEntry>
    {
        private readonly ICollection<LeaderboardEntry> _leaderboardEntries = new List<LeaderboardEntry>()
        {
            new LeaderboardEntry
                {
                    Id = 1,
                    UserId = "1",
                    LeaderboardId = 1,
                    Score = 100,
                },
        };

        public void Configure(EntityTypeBuilder<LeaderboardEntry> entity)
        {
            //entity.HasData(_leaderboardEntries);
        }
    }
}
