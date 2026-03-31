namespace QuizGame.Core.Contracts
{
    using QuizGame.Data.Models;
    using QuizGame.ViewModels.Admin.User;
    using System.Collections.Generic;
    public interface IUserService
    {
        Task<AdminManageUserRolesViewModel?> GetUserByIdAsync(Guid userId);

        Task<IEnumerable<AdminUserViewModel>> GetAllUsersAsync(string adminUserId);

        Task<bool> CreateUserAsync(AdminCreateUserViewModel viewModel);

        Task<bool> AssignRoleToUserAsync(Guid userId, string role);

        Task<bool> RemoveRoleFromUserAsync(Guid userId, string role);

        Task<bool> RestoreUserAsync(Guid userId);

        Task<bool> SoftDeleteUserAsync(Guid userId);

        Task<bool> HardDeleteUserAsync(Guid userId);
    }
}
