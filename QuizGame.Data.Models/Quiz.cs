    namespace QuizGame.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;
    using static GCommon.EntityValidationConstants;
    public class Quiz
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(QuizTitleMaxLength)]
        public string Title { get; set; } = null!;

        [Required]
        [MaxLength(QuizDescriptionMaxLength)]
        public string Description { get; set; } = null!;

        [DataType(DataType.DateTime)]
        [Required]
        public DateTime StartTime { get; set; }

        [ForeignKey(nameof(Leaderboard))]
        public int LeaderboardId { get; set; }
        public virtual Leaderboard? Leaderboard { get; set; }

        public virtual ICollection<Question> Questions { get; set; }
        = new List<Question>();
    }
}
