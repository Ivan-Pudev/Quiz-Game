namespace QuizGame.ViewModels.Admin.User
{
    using Microsoft.AspNetCore.Identity;
    using System;
    using System.Collections.Generic;
    using System.Text;


    public class AdminManageUserRolesViewModel : AdminUserViewModel
    {
        public IdentityRole<Guid> SelectedRole { get; set; } = null!;

        public List<IdentityRole<Guid>> AvailableRoles {  get; set; } = null!;
    }
}
