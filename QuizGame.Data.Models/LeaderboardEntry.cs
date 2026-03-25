using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace QuizGame.Data.Models
{
    public class LeaderboardEntry
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }

        public virtual ApplicationUser User { get; set; } = null!;

        public int Score { get; set; }

        public int Rank { get; set; }

        [Required]
        [ForeignKey(nameof(Leaderboard))]
        public Guid LeaderboardId { get; set; }

        public virtual Leaderboard Leaderboard { get; set; } = null!;
    }
}
