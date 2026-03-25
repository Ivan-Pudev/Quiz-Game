namespace QuizGame.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using static GCommon.EntityValidationConstants;

    public class Answer
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(AnswerContentMaxLength)]
        public string Content { get; set; } = null!;

        public bool IsCorrect { get; set; }

        [Required]
        [ForeignKey(nameof(Question))]
        public Guid QuestionId { get; set; }

        [Required]
        public virtual Question Question { get; set; } = null!;
    }
}
