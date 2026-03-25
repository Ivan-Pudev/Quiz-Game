namespace QuizGame.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    public class AttemptAnswer
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [ForeignKey(nameof(QuizAttempt))]
        public Guid QuizAttemptId { get; set; }

        public virtual QuizAttempt QuizAttempt { get; set; } = null!;

        [ForeignKey(nameof(Question))]
        public Guid QuestionId { get; set; }

        public virtual Question Question { get; set; } = null!;

        [ForeignKey(nameof(Answer))]
        public Guid SelectedAnswerId { get; set; }

        public virtual Answer? Answer { get; set; }

        public bool IsCorrect { get; set; }

        public int EarnedPoints { get; set; }
    }
}
