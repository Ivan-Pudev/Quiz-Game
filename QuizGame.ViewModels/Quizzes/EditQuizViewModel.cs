namespace QuizGame.ViewModels.Quizzes
{
    using System.ComponentModel.DataAnnotations;
    using static GCommon.EntityValidationConstants;

    public class EditQuizViewModel
    {
        public Guid Id { get; set; }

        [Required]
        [MinLength(QuizTitleMinLength)]
        [MaxLength(QuizTitleMaxLength)]
        public string Title { get; set; } = null!;

        [Required]
        [MinLength(QuizDescriptionMinLength)]
        [MaxLength(QuizDescriptionMaxLength)]
        public string Description { get; set; } = null!;

        public int TotalPoints
            => SelectedQuestions.Sum(q => q.Points);

        [DataType(DataType.DateTime)]
        [Required]
        public DateTime StartTime { get; set; }

        public virtual List<QuestionSelectionViewModel> SelectedQuestions { get; set; }
        = new List<QuestionSelectionViewModel>();

        public virtual List<Guid> SelectedQuestionsIds 
         => SelectedQuestions.Select(q=>q.QuestionId).ToList();
    }
}
