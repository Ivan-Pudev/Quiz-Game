namespace QuizGame.ViewModels.Game
{
    using System;
    using System.Collections.Generic;
    public class PlayQuestionViewModel
    {
        public Guid AttemptId { get; set; }
        public Guid QuizId { get; set; }

        public Guid QuestionId { get; set; }
        public string QuestionContent { get; set; } = null!;

        public List<AnswerVm> Answers { get; set; } = null!;

        public DateTime StartedAtUtc { get; set; }
        public int TimeLimitSeconds { get; set; }
    }

}
