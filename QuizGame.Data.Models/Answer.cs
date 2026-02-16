namespace QuizGame.Data.Models
{
    using Microsoft.AspNetCore.Identity;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;
    using static GCommon.EntityValidationConstants;

    public class Answer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(AnswerContentMaxLength)]
        public string Content { get; set; } = null!;

        public bool IsCorrect { get; set; }

        [Required]
        [ForeignKey(nameof(Question))]
        public int QuestionId { get; set; }

        [Required]
        public virtual Question Question { get; set; } = null!;
    }
}
