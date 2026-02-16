namespace QuizGame.ViewModels.Game
{
    using System;
    using System.Collections.Generic;
    public class PlayQuestionViewModel
    {
        public int AttemptId { get; set; }
        public int QuizId { get; set; }

        public int QuestionId { get; set; }
        public string QuestionContent { get; set; } = "";

        public List<AnswerVm> Answers { get; set; } = new();

        public DateTime StartedAtUtc { get; set; }
        public int TimeLimitSeconds { get; set; }
    }

}
