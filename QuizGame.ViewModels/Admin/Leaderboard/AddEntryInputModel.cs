using System.ComponentModel.DataAnnotations;

namespace QuizGame.ViewModels.Admin.Leaderboard
{
    public class AddEntryInputModel
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Score must be 0 or greater.")]
        public int Score { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Rank must be 1 or greater.")]
        public int Rank { get; set; }
    }
}