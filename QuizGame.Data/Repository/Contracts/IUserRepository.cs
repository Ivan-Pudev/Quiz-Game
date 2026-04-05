namespace QuizGame.Data.Repository.Contracts
{
    using Microsoft.AspNetCore.Identity;
    using QuizGame.Data.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    public interface IUserRepository
    {
        Task<ApplicationUser?> FindUserByIdAsync(Guid? userId);

        Task<IEnumerable<ApplicationUser>> GetAllUsersAsync(Expression<Func<ApplicationUser, bool>>? filter = null);

        Task<IEnumerable<string>> GetUserRolesAsync(ApplicationUser applicationUser);

        Task<IEnumerable<IdentityRole<Guid>>> GetAllRolesAsync(ApplicationUser applicationUser);

        Task<bool> AddUserAsync(ApplicationUser newAppUser);

        Task<bool> UpdateUserRoleAsync(Guid? id, string role, bool removingRole = false);

        Task<bool> RestoreUserAsync(Guid? id);

        Task<bool> SoftDeleteUserAsync(Guid? id);

        Task<bool> HardDeleteUserAsync(Guid? id);
    }
}
