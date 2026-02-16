namespace QuizGame.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using static GCommon.EntityValidationConstants;
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(CategoryNameMaxLength)]
        public string Name { get; set; } = null!;

        [MaxLength(CategoryImageUrlMaxLength)]
        public string? ImageUrl { get; set; }

        public ICollection<Question> Questions { get; set; }
         = new List<Question>();
    }
}
