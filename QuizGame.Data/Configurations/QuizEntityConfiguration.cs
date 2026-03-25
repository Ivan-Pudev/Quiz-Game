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
            Id = Guid.Parse("4301f783-5664-41fc-af53-c2de0e1e454a"),
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
                      new { QuizzesId = Guid.Parse("4301f783-5664-41fc-af53-c2de0e1e454a"), QuestionsId = Guid.Parse("7043874b-ed1a-4ab0-8519-5dc8408abf68") }, 
                      new { QuizzesId = Guid.Parse("4301f783-5664-41fc-af53-c2de0e1e454a"), QuestionsId = Guid.Parse("7c6b2449-c14b-4da6-85e5-6e511a16e0ec") }, 
                      new { QuizzesId = Guid.Parse("4301f783-5664-41fc-af53-c2de0e1e454a"), QuestionsId = Guid.Parse("9b3b9e33-4e2a-4ea2-9ae0-7c0ab10cb09b") }  
                  ));
        }
    }
}
