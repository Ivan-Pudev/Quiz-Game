namespace QuizGame.ViewModels.Admin.User
{
    using System;
    using System.Collections.Generic;

    public class AdminUserViewModel
    {
        public Guid Id { get; set; }

        public string Email { get; set; } = null!;

        public bool IsDeleted { get; set; }

        public List<string> Roles { get; set; }
            = new List<string>();
    }
}
