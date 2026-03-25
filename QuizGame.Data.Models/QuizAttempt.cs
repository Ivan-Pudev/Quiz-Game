namespace QuizGame.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class QuizAttempt
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [ForeignKey(nameof(Quiz))]
        public Guid QuizId { get; set; }
        public Quiz Quiz { get; set; } = null!;

        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }

        public virtual ApplicationUser User { get; set; } = null!;

        public int CurrentQuestionIndex { get; set; }

        public int Score { get; set; }

        public int MaxScore { get; set; }

        public bool IsFinished { get; set; }

        public ICollection<AttemptAnswer> Answers { get; set; } 
            = new List<AttemptAnswer>();
    }
}
