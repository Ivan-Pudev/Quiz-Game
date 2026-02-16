namespace QuizGame.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;
    using Enums;
    using static GCommon.EntityValidationConstants;

    public class Question
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(QuestionContentMaxLength)]
        public string Content { get; set; } = null!;

        [Required]
        public QuestionType QuestionType { get; set; }

        public int Complexity { get; set; }

        [Required]
        public int Points { get; set; }

        public virtual ICollection<Quiz> Quizzes { get; set; }
        = new List<Quiz>();

        public virtual ICollection<Category> Categories { get; set; }
         = new List<Category>();

        public virtual ICollection<Answer> Answers { get; set; }
         = new List<Answer>();

    }
}
