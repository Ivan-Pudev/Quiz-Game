namespace QuizGame.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using QuizGame.Data.Models;
    using System.Collections.Generic;

    public class AnswerEntityConfiguration : IEntityTypeConfiguration<Answer>
    {
        private readonly ICollection<Answer> _answers = new List<Answer>()
        {
            // Q1: Which planet is known as the Red Planet?
        new Answer { Id = 1, Content = "Mars", IsCorrect = true, QuestionId = 1 },
        new Answer { Id = 2, Content = "Venus", IsCorrect = false, QuestionId = 1 },
        new Answer { Id = 3, Content = "Jupiter", IsCorrect = false, QuestionId = 1 },

        // Q2: The Great Wall of China was built in a single century.
        new Answer { Id = 4, Content = "True", IsCorrect = false, QuestionId = 2 },
        new Answer { Id = 5, Content = "False", IsCorrect = true, QuestionId = 2 },

        // Q3: What is the chemical symbol for Gold?
        new Answer { Id = 6, Content = "Au", IsCorrect = true, QuestionId = 3 },
        new Answer { Id = 7, Content = "Ag", IsCorrect = false, QuestionId = 3 },
        new Answer { Id = 8, Content = "Gd", IsCorrect = false, QuestionId = 3 },

        // Q4: Who painted the "Starry Night"?
        new Answer { Id = 9, Content = "Vincent van Gogh", IsCorrect = true, QuestionId = 4 },
        new Answer { Id = 10, Content = "Pablo Picasso", IsCorrect = false, QuestionId = 4 },
        new Answer { Id = 11, Content = "Claude Monet", IsCorrect = false, QuestionId = 4 },

        // Q5: Sound travels faster in water than in air.
        new Answer { Id = 12, Content = "True", IsCorrect = true, QuestionId = 5 },
        new Answer { Id = 13, Content = "False", IsCorrect = false, QuestionId = 5 },

        // Q6: Which country is home to the Kangaroo?
        new Answer { Id = 14, Content = "Australia", IsCorrect = true, QuestionId = 6 },
        new Answer { Id = 15, Content = "South Africa", IsCorrect = false, QuestionId = 6 },
        new Answer { Id = 16, Content = "Brazil", IsCorrect = false, QuestionId = 6 },

        // Q7: What is the square root of 144?
        new Answer { Id = 17, Content = "12", IsCorrect = true, QuestionId = 7 },
        new Answer { Id = 18, Content = "14", IsCorrect = false, QuestionId = 7 },
        new Answer { Id = 19, Content = "16", IsCorrect = false, QuestionId = 7 },

        // Q8: In which year did the Titanic sink?
        new Answer { Id = 20, Content = "1912", IsCorrect = true, QuestionId = 8 },
        new Answer { Id = 21, Content = "1905", IsCorrect = false, QuestionId = 8 },
        new Answer { Id = 22, Content = "1920", IsCorrect = false, QuestionId = 8 },

        // Q9: Humans have four lungs.
        new Answer { Id = 23, Content = "True", IsCorrect = false, QuestionId = 9 },
        new Answer { Id = 24, Content = "False", IsCorrect = true, QuestionId = 9 },

        // Q10: Which element has the atomic number 1?
        new Answer { Id = 25, Content = "Hydrogen", IsCorrect = true, QuestionId = 10 },
        new Answer { Id = 26, Content = "Helium", IsCorrect = false, QuestionId = 10 },
        new Answer { Id = 27, Content = "Oxygen", IsCorrect = false, QuestionId = 10 }
        };

        public void Configure(EntityTypeBuilder<Answer> entity)
        {
            entity.HasData(_answers);
        }
    }
}
