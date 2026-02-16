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
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(User))]
        public string UserId { get; set; } = null!;

        public virtual IdentityUser User { get; set; } = null!;

        public int Score { get; set; }

        public int Rank { get; set; }

        [Required]
        [ForeignKey(nameof(Leaderboard))]
        public int LeaderboardId { get; set; }

        public virtual Leaderboard Leaderboard { get; set; } = null!;
    }
}
