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
                Id = 1,
                Name = "Math",
                ImageUrl = "https://img.freepik.com/free-photo/blackboard-inscribed-with-scientific-formulas-calculations_1150-19413.jpg?semt=ais_hybrid&w=740&q=80"
            },
            new Category
            {
                Id = 2,
                Name = "Geography",
                ImageUrl = "https://img.freepik.com/free-vector/geography-subject-with-worldmap-books_1308-30998.jpg?semt=ais_hybrid&w=740&q=80"
            },
            new Category
            {
                Id = 3,
                Name = "Science",
                ImageUrl = null
            },

            new Category
            {
                Id = 4,
                Name = "History",
                ImageUrl = "https://dualcreditathome.com/wp-content/uploads/2014/02/history.jpg"
            },

            new Category
            {
                Id = 5,
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
