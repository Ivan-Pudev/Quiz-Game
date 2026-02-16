namespace QuizGame.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using QuizGame.Data.Models;
    using System;
    using System.Collections.Generic;
    using System.Text;
    public class QuizEntityConfiguration : IEntityTypeConfiguration<Quiz>
    {
        private readonly IEnumerable<Quiz> _quizzes = new List<Quiz>()
    {
        new Quiz
        {
            Id = 1,
            Title = "Friday Night Trivia",
            Description = "A mix of everything!",
            StartTime = DateTime.Parse("2026-02-15"),
        }
    };

        public void Configure(EntityTypeBuilder<Quiz> entity)
        {
            entity.HasData(_quizzes);

            entity.HasMany(q => q.Questions)
                  .WithMany(question => question.Quizzes) 
                  .UsingEntity(j => j.HasData(
                      new { QuizzesId = 1, QuestionsId = 1 }, 
                      new { QuizzesId = 1, QuestionsId = 2 }, 
                      new { QuizzesId = 1, QuestionsId = 3 }  
                  ));
        }
    }
}
