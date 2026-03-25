namespace QuizGame.ViewModels.Quizzes
{
    using QuizGame.Data.Models;
    using System;
    using System.Collections.Generic;
    public class DetailsQuizViewModel
    {
        
        public Guid Id { get; set; }

        public string Title { get; set; } = null!;

        public string Description { get; set; } = null!;

        public DateTime StartTime { get; set; }

        public int TotalPoints
            => Questions.Sum(q => q.Points);

        public Guid LeaderboardId { get; set; }

        public virtual Leaderboard Leaderboard { get; set; } = null!;

        public virtual ICollection<Question> Questions { get; set; }
        = new List<Question>();

        public virtual List<ICollection<Answer>> Answers { get; set; }
       = new List<ICollection<Answer>>();

        public virtual List<ICollection<Category>> Categories { get; set; }
       = new List<ICollection<Category>>();
    }
}
