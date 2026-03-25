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
        new Question { Id = Guid.Parse("7043874b-ed1a-4ab0-8519-5dc8408abf68"), Content = "Which planet is known as the Red Planet?", QuestionType = QuestionType.MultipleChoice, Complexity = 1, Points = 10 },
        new Question { Id = Guid.Parse("7c6b2449-c14b-4da6-85e5-6e511a16e0ec"), Content = "The Great Wall of China was built in a single century.", QuestionType = QuestionType.TrueFalse, Complexity = 2, Points = 15 },
        new Question { Id = Guid.Parse("9b3b9e33-4e2a-4ea2-9ae0-7c0ab10cb09b"), Content = "What is the chemical symbol for Gold?", QuestionType = QuestionType.ShortText, Complexity = 3, Points = 25 },
        new Question { Id = Guid.Parse("adc049bd-1ef8-423b-9c37-a0e0b2708595"), Content = "Who painted the 'Starry Night'?", QuestionType = QuestionType.MultipleChoice, Complexity = 2, Points = 20 },
        new Question { Id = Guid.Parse("49850a02-893a-45e9-8e5b-2a01a040d60e"), Content = "Sound travels faster in water than in air.", QuestionType = QuestionType.TrueFalse, Complexity = 3, Points = 20 },
        new Question { Id = Guid.Parse("6271f595-33f5-480e-978b-02f9febc50de"), Content = "Which country is home to the Kangaroo?", QuestionType = QuestionType.MultipleChoice, Complexity = 1, Points = 10 },
        new Question { Id = Guid.Parse("5267700d-9487-4ab0-9e24-3962e71df82e"), Content = "What is the square root of 144?", QuestionType = QuestionType.ShortText, Complexity = 2, Points = 15 },
        new Question { Id = Guid.Parse("698f8ac5-a4ae-4031-a7fa-f4bf245f374e"), Content = "In which year did the Titanic sink?", QuestionType = QuestionType.MultipleChoice, Complexity = 3, Points = 30 },
        new Question { Id = Guid.Parse("04df888f-1067-4e40-821b-9892fc603f5b"), Content = "Humans have four lungs.", QuestionType = QuestionType.TrueFalse, Complexity = 1, Points = 10 },
        new Question { Id = Guid.Parse("e861c8e7-8dbd-447b-9943-78c812a14768"), Content = "Which element has the atomic number 1?", QuestionType = QuestionType.ShortText, Complexity = 4, Points = 40 }
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
            new { QuestionsId = Guid.Parse("7043874b-ed1a-4ab0-8519-5dc8408abf68"), CategoriesId = Guid.Parse("b0cc81f8-da63-4ce3-ad27-93298ccf26c1") },
            new { QuestionsId = Guid.Parse("7c6b2449-c14b-4da6-85e5-6e511a16e0ec"), CategoriesId = Guid.Parse("644fb15e-3f0a-4d29-aae9-7deb3f08ee5c") },
            new { QuestionsId = Guid.Parse("7c6b2449-c14b-4da6-85e5-6e511a16e0ec"), CategoriesId = Guid.Parse("31961ab3-d6c8-43f4-8744-d9b21a815ed0") },
            new { QuestionsId = Guid.Parse("9b3b9e33-4e2a-4ea2-9ae0-7c0ab10cb09b"), CategoriesId = Guid.Parse("b0cc81f8-da63-4ce3-ad27-93298ccf26c1") },
            new { QuestionsId = Guid.Parse("49850a02-893a-45e9-8e5b-2a01a040d60e"), CategoriesId = Guid.Parse("b0cc81f8-da63-4ce3-ad27-93298ccf26c1") },
            new { QuestionsId = Guid.Parse("6271f595-33f5-480e-978b-02f9febc50de"), CategoriesId = Guid.Parse("31961ab3-d6c8-43f4-8744-d9b21a815ed0") },
            new { QuestionsId = Guid.Parse("6271f595-33f5-480e-978b-02f9febc50de"), CategoriesId = Guid.Parse("644fb15e-3f0a-4d29-aae9-7deb3f08ee5c") },
            new { QuestionsId = Guid.Parse("5267700d-9487-4ab0-9e24-3962e71df82e"), CategoriesId = Guid.Parse("211a7b3d-1535-4fae-9015-9ce026df66f9") },
            new { QuestionsId = Guid.Parse("698f8ac5-a4ae-4031-a7fa-f4bf245f374e"), CategoriesId = Guid.Parse("31961ab3-d6c8-43f4-8744-d9b21a815ed0") },
            new { QuestionsId = Guid.Parse("e861c8e7-8dbd-447b-9943-78c812a14768"), CategoriesId = Guid.Parse("b0cc81f8-da63-4ce3-ad27-93298ccf26c1") }
                  );
          });
        }
    }
}
