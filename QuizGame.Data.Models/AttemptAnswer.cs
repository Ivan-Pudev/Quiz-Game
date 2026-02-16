namespace QuizGame.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;
    public class AttemptAnswer
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(QuizAttempt))]
        public int QuizAttemptId { get; set; }
        public QuizAttempt QuizAttempt { get; set; } = null!;

        public int QuestionId { get; set; }
        public int SelectedAnswerId { get; set; }

        public bool IsCorrect { get; set; }
        public int EarnedPoints { get; set; }
    }
}
