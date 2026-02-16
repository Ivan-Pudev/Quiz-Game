namespace QuizGame.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;

    public class QuizAttempt
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Quiz))]
        public int QuizId { get; set; }
        public Quiz Quiz { get; set; } = null!;

        public string UserId { get; set; } = null!;

        public int CurrentQuestionIndex { get; set; }

        public int Score { get; set; }
        public int MaxScore { get; set; }

        public bool IsFinished { get; set; }

        public ICollection<AttemptAnswer> Answers { get; set; } 
            = new List<AttemptAnswer>();
    }
}
