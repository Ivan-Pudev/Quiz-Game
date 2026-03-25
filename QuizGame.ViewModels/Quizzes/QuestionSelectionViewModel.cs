namespace QuizGame.ViewModels.Quizzes
{
    public class QuestionSelectionViewModel
    {
        public Guid QuestionId { get; set; }
        public string Content { get; set; } = null!;
        public bool IsSelected { get; set; }

        public int Points { get; set; }
    }
}
