using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace QuizGame.Data.Models
{
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;
    using static GCommon.EntityValidationConstants;

    public class Leaderboard
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(QuizTitleMaxLength)]
        public string Title { get; set; } = null!;

        [Required]
        [MaxLength(QuizDescriptionMaxLength)]
        public string Description { get; set; } = null!;

        [Required]
        [DataType(DataType.Date)]
        public DateOnly LastUpdated { get; set; }

        [Required]
        [ForeignKey(nameof(Quiz))]
        public int QuizId { get; set; }

        public virtual Quiz Quiz { get; set; } = null!;
        public virtual ICollection<LeaderboardEntry> Entries { get; set; } =
          new List<LeaderboardEntry>();
    }
}
