namespace QuizGame.ViewModels.Admin.User
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using static GCommon.IdentityConstants;
    public class AdminCreateUserViewModel
    {
        [Required]
        [MaxLength(FullNameMaxLength)]
        [MinLength(FullNameMinLength)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = null!;

        [Required]
        [EmailAddress]
        [MaxLength(EmailMaxLength)]
        [MinLength(EmailMinLength)]
        public string Email { get; set; } = null!;

        [Required]
        [Display(Name = "Birth Date")]
        public DateTime BirthDate { get; set; }

        [Required]
        [MaxLength(PasswordMaxLength)]
        [MinLength(PasswordMinLength)]
        public string Password { get; set; } = null!;

        [Required]
        [MaxLength(PasswordMaxLength)]
        [MinLength(PasswordMinLength)]
        [Compare(nameof(Password), ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = null!;

        [Required(ErrorMessage = "Please select at least one role.")]
        [Display(Name = "Assigned Roles")]
        public List<string> SelectedRoles { get; set; } = new List<string>();
    }
}
