namespace QuizGame.ViewModels.Quizzes
{
    using QuizGame.Data.Models;
    using System.ComponentModel.DataAnnotations;
    using static GCommon.EntityValidationConstants;
    public class CreateQuizViewModel
    {

        [Required]
        [MinLength(QuizTitleMinLength)]
        [MaxLength(QuizTitleMaxLength)]
        public string Title { get; set; } = null!;

        [Required]
        [MinLength(QuizDescriptionMinLength)]
        [MaxLength(QuizDescriptionMaxLength)]
        public string Description { get; set; } = null!;

        [DataType(DataType.DateTime)]
        [Required]
        public DateTime StartTime { get; set; }

        public List<Question> Questions { get; set; } 
            = new List<Question>();

        public List<Guid> SelectedQuestionIds { get; set; } 
            = new List<Guid>();
    }
}
