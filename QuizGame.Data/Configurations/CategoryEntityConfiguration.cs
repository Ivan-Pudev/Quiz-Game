namespace QuizGame.Data.Configurations
{
    using System.Collections.Generic;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using QuizGame.Data.Models;
    public class CategoryEntityConfiguration : IEntityTypeConfiguration<Category>
    {
        private readonly IEnumerable<Category> _categories = new List<Category>()
        {
            new Category
            {
                Id = Guid.Parse("211a7b3d-1535-4fae-9015-9ce026df66f9"),
                Name = "Math",
                ImageUrl = "https://img.freepik.com/free-photo/blackboard-inscribed-with-scientific-formulas-calculations_1150-19413.jpg?semt=ais_hybrid&w=740&q=80"
            },
            new Category
            {
                Id = Guid.Parse("644fb15e-3f0a-4d29-aae9-7deb3f08ee5c"),
                Name = "Geography",
                ImageUrl = "https://img.freepik.com/free-vector/geography-subject-with-worldmap-books_1308-30998.jpg?semt=ais_hybrid&w=740&q=80"
            },
            new Category
            {
                Id = Guid.Parse("b0cc81f8-da63-4ce3-ad27-93298ccf26c1"),
                Name = "Science",
                ImageUrl = null
            },

            new Category
            {
                Id = Guid.Parse("31961ab3-d6c8-43f4-8744-d9b21a815ed0"),
                Name = "History",
                ImageUrl = "https://dualcreditathome.com/wp-content/uploads/2014/02/history.jpg"
            },

            new Category
            {
                Id = Guid.Parse("915f826d-fe20-4be5-a8f2-37a65c9a92c4"),
                Name = "Hobbies",
                ImageUrl = null
            }
        };

        public void Configure(EntityTypeBuilder<Category> entity)
        {
            entity.HasData(_categories);
        }
    }
}
