    namespace QuizGame.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using static GCommon.EntityValidationConstants;
    public class Quiz
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(QuizTitleMaxLength)]
        public string Title { get; set; } = null!;

        [Required]
        [MaxLength(QuizDescriptionMaxLength)]
        public string Description { get; set; } = null!;

        [DataType(DataType.DateTime)]
        [Required]
        public DateTime StartTime { get; set; }
        public virtual Leaderboard? Leaderboard { get; set; }

        public virtual ICollection<Question> Questions { get; set; }
        = new List<Question>();
    }
}
