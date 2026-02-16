namespace QuizGame.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using QuizGame.Data.Models;
    using QuizGame.Data.Models.Enums;
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class QuestionEntityConfiguration : IEntityTypeConfiguration<Question>
    {
        private readonly IEnumerable<Question> _questions = new List<Question>()
    {
        new Question { Id = 1, Content = "Which planet is known as the Red Planet?", QuestionType = QuestionType.MultipleChoice, Complexity = 1, Points = 10 },
        new Question { Id = 2, Content = "The Great Wall of China was built in a single century.", QuestionType = QuestionType.TrueFalse, Complexity = 2, Points = 15 },
        new Question { Id = 3, Content = "What is the chemical symbol for Gold?", QuestionType = QuestionType.ShortText, Complexity = 3, Points = 25 },
        new Question { Id = 4, Content = "Who painted the 'Starry Night'?", QuestionType = QuestionType.MultipleChoice, Complexity = 2, Points = 20 },
        new Question { Id = 5, Content = "Sound travels faster in water than in air.", QuestionType = QuestionType.TrueFalse, Complexity = 3, Points = 20 },
        new Question { Id = 6, Content = "Which country is home to the Kangaroo?", QuestionType = QuestionType.MultipleChoice, Complexity = 1, Points = 10 },
        new Question { Id = 7, Content = "What is the square root of 144?", QuestionType = QuestionType.ShortText, Complexity = 2, Points = 15 },
        new Question { Id = 8, Content = "In which year did the Titanic sink?", QuestionType = QuestionType.MultipleChoice, Complexity = 3, Points = 30 },
        new Question { Id = 9, Content = "Humans have four lungs.", QuestionType = QuestionType.TrueFalse, Complexity = 1, Points = 10 },
        new Question { Id = 10, Content = "Which element has the atomic number 1?", QuestionType = QuestionType.ShortText, Complexity = 4, Points = 40 }
    };

        public void Configure(EntityTypeBuilder<Question> entity)
        {
            entity.HasData(_questions);

            entity
    .HasMany(q => q.Categories)
    .WithMany(c => c.Questions)
    .UsingEntity(j =>
    {
        j.ToTable("CategoriesQuestions");
        j.HasData(
            new { QuestionsId = 1, CategoriesId = 2 },
            new { QuestionsId = 1, CategoriesId = 3 },
            new { QuestionsId = 2, CategoriesId = 2 },
            new { QuestionsId = 2, CategoriesId = 4 },
            new { QuestionsId = 3, CategoriesId = 3 },
            new { QuestionsId = 5, CategoriesId = 3 },
            new { QuestionsId = 6, CategoriesId = 4 },
            new { QuestionsId = 6, CategoriesId = 2 },
            new { QuestionsId = 7, CategoriesId = 1 },
            new { QuestionsId = 8, CategoriesId = 4 },
            new { QuestionsId = 10, CategoriesId = 3 }
                  );
          });
        }
    }
}
