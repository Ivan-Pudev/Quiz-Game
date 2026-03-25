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
        new Answer { Id = Guid.Parse("96e0ef87-3297-49ac-975e-77dc99fd09fe"), Content = "Mars", IsCorrect = true, QuestionId = Guid.Parse("7043874b-ed1a-4ab0-8519-5dc8408abf68") },
        new Answer { Id = Guid.Parse("a367a8af-d6b8-4034-b64e-25b32bb9263e"), Content = "Venus", IsCorrect = false, QuestionId = Guid.Parse("7043874b-ed1a-4ab0-8519-5dc8408abf68") },
        new Answer { Id = Guid.Parse("65ea829c-9836-411d-8c36-8592770bd3a8"), Content = "Jupiter", IsCorrect = false, QuestionId = Guid.Parse("7043874b-ed1a-4ab0-8519-5dc8408abf68") },

        // Q2: The Great Wall of China was built in a single century.
        new Answer { Id = Guid.Parse("b0c63cdc-c8b0-480f-aee0-2fbf3c8ec052"), Content = "True", IsCorrect = false, QuestionId = Guid.Parse("7c6b2449-c14b-4da6-85e5-6e511a16e0ec") },
        new Answer { Id = Guid.Parse("ec7da6e0-c8ad-40dd-88b3-7f1a6176f9de"), Content = "False", IsCorrect = true, QuestionId = Guid.Parse("7c6b2449-c14b-4da6-85e5-6e511a16e0ec") },

        // Q3: What is the chemical symbol for Gold?
        new Answer { Id = Guid.Parse("c6e4797b-245a-43a5-b1cb-e7786affc96c"), Content = "Au", IsCorrect = true, QuestionId = Guid.Parse("9b3b9e33-4e2a-4ea2-9ae0-7c0ab10cb09b") },
        new Answer { Id = Guid.Parse("4df293fa-7c21-43b4-af7e-224c627adcf3"), Content = "Ag", IsCorrect = false, QuestionId = Guid.Parse("9b3b9e33-4e2a-4ea2-9ae0-7c0ab10cb09b") },
        new Answer { Id = Guid.Parse("4643ff98-ea40-4f28-aa75-1a4628263c72"), Content = "Gd", IsCorrect = false, QuestionId = Guid.Parse("9b3b9e33-4e2a-4ea2-9ae0-7c0ab10cb09b") },

        // Q4: Who painted the "Starry Night"?
        new Answer { Id = Guid.Parse("862cccfe-37fa-4219-bbe2-020497a550e9"), Content = "Vincent van Gogh", IsCorrect = true, QuestionId = Guid.Parse("adc049bd-1ef8-423b-9c37-a0e0b2708595") },
        new Answer { Id = Guid.Parse("80ada585-44cf-44c8-8265-288dd2789c0d"), Content = "Pablo Picasso", IsCorrect = false, QuestionId = Guid.Parse("adc049bd-1ef8-423b-9c37-a0e0b2708595") },
        new Answer { Id = Guid.Parse("78ea56a6-5157-491e-87b9-93c92e4cc54a"), Content = "Claude Monet", IsCorrect = false, QuestionId = Guid.Parse("adc049bd-1ef8-423b-9c37-a0e0b2708595") },

        // Q5: Sound travels faster in water than in air.
        new Answer { Id = Guid.Parse("79a88322-5898-4b26-8cd7-35d41fac5dcd"), Content = "True", IsCorrect = true, QuestionId = Guid.Parse("49850a02-893a-45e9-8e5b-2a01a040d60e") },
        new Answer { Id = Guid.Parse("3dfbcde1-75c8-495a-9c33-f8551c040928"), Content = "False", IsCorrect = false, QuestionId = Guid.Parse("49850a02-893a-45e9-8e5b-2a01a040d60e") },

        // Q6: Which country is home to the Kangaroo?
        new Answer { Id = Guid.Parse("6df6355c-2db9-4d95-8928-2ff9d084576a"), Content = "Australia", IsCorrect = true, QuestionId = Guid.Parse("6271f595-33f5-480e-978b-02f9febc50de")},
        new Answer { Id = Guid.Parse("54b83914-8e31-4ce0-abd2-5eca6289248f"), Content = "South Africa", IsCorrect = false, QuestionId = Guid.Parse("6271f595-33f5-480e-978b-02f9febc50de") },
        new Answer { Id = Guid.Parse("d1fc7d89-7ba0-4132-8890-877d30b1b12b"), Content = "Brazil", IsCorrect = false, QuestionId = Guid.Parse("6271f595-33f5-480e-978b-02f9febc50de") },

        // Q7: What is the square root of 144?
        new Answer { Id = Guid.Parse("241b93eb-9da0-4c28-b71c-be72e60d81e5"), Content = "12", IsCorrect = true, QuestionId = Guid.Parse("5267700d-9487-4ab0-9e24-3962e71df82e") },
        new Answer { Id = Guid.Parse("4d0eb419-e569-484a-aaa6-ee3cc330b47d"), Content = "14", IsCorrect = false, QuestionId = Guid.Parse("5267700d-9487-4ab0-9e24-3962e71df82e") },
        new Answer { Id = Guid.Parse("6c05587a-61a7-4d0c-ba67-9a14c32f1c7a"), Content = "16", IsCorrect = false, QuestionId = Guid.Parse("5267700d-9487-4ab0-9e24-3962e71df82e") },

        // Q8: In which year did the Titanic sink?
        new Answer { Id = Guid.Parse("61a0689b-7621-4862-8c67-7dde69f2d2c3"), Content = "1912", IsCorrect = true, QuestionId = Guid.Parse("698f8ac5-a4ae-4031-a7fa-f4bf245f374e") },
        new Answer { Id = Guid.Parse("2f1c439a-eb0e-40b3-8106-25f79c81b9d1"), Content = "1905", IsCorrect = false, QuestionId = Guid.Parse("698f8ac5-a4ae-4031-a7fa-f4bf245f374e") },
        new Answer { Id = Guid.Parse("fd1cd964-91e3-496f-8c34-6311644fc383"), Content = "1920", IsCorrect = false, QuestionId = Guid.Parse("698f8ac5-a4ae-4031-a7fa-f4bf245f374e") },

        // Q9: Humans have four lungs.
        new Answer { Id = Guid.Parse("616afaf1-57b7-45c5-aeb1-6268efbc2337"), Content = "True", IsCorrect = false, QuestionId = Guid.Parse("04df888f-1067-4e40-821b-9892fc603f5b") },
        new Answer { Id = Guid.Parse("3922d6ba-c178-44d6-b080-109bccd7af25"), Content = "False", IsCorrect = true, QuestionId = Guid.Parse("04df888f-1067-4e40-821b-9892fc603f5b") },

        // Q10: Which element has the atomic number 1?
        new Answer { Id = Guid.Parse("653274d3-62e7-4fe3-9eb3-c5fc0f54e5fa"), Content = "Hydrogen", IsCorrect = true, QuestionId = Guid.Parse("e861c8e7-8dbd-447b-9943-78c812a14768") },
        new Answer { Id = Guid.Parse("fcee2ffa-8b19-4650-9652-5e09aeb770ee"), Content = "Helium", IsCorrect = false, QuestionId = Guid.Parse("e861c8e7-8dbd-447b-9943-78c812a14768") },
        new Answer { Id = Guid.Parse("5804614e-88a6-48a5-a9a6-faf1c76ddc77"), Content = "Oxygen", IsCorrect = false, QuestionId = Guid.Parse("e861c8e7-8dbd-447b-9943-78c812a14768") }
        };

        public void Configure(EntityTypeBuilder<Answer> entity)
        {
            entity.HasData(_answers);
        }
    }
}
