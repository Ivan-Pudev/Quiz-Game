namespace QuizGame.Core.Contracts
{
    using QuizGame.Data.Models;
    using QuizGame.ViewModels.Admin.User;
    using System.Collections.Generic;
    public interface IUserService
    {
        Task<AdminManageUserRolesViewModel?> GetUserByIdAsync(Guid? userId);

        Task<IEnumerable<AdminUserViewModel>> GetAllUsersAsync(bool getDeletedUsers = false);

        Task CreateUserAsync(AdminCreateUserViewModel viewModel);

        Task<bool> AssignRoleToUserAsync(Guid? userId, string role);

        Task RemoveRoleFromUserAsync(Guid? userId, string role);

        Task RestoreUserAsync(Guid? userId);

        Task SoftDeleteUserAsync(Guid? userId);

        Task HardDeleteUserAsync(Guid? userId);
    }
}
