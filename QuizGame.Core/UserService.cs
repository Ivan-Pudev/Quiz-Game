namespace QuizGame.Core
{
    using Microsoft.AspNetCore.Identity;
    using QuizGame.Core.Contracts;
    using QuizGame.Data.Models;
    using QuizGame.Data.Repository.Contracts;
    using QuizGame.ViewModels.Admin.User;

    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher<ApplicationUser> _passwordHasher;
        public UserService(IUserRepository userRepository, IPasswordHasher<ApplicationUser> passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }
        public async Task<AdminManageUserRolesViewModel?> GetUserByIdAsync(Guid userId)
        {
            ApplicationUser? user = await _userRepository.FindUserByIdAsync(userId);

            IEnumerable<string> rolesStr = await _userRepository.GetUserRolesAsync(user!);

            IEnumerable<IdentityRole<Guid>> identityRoles = await _userRepository.GetAllRolesAsync(user!);

            return new AdminManageUserRolesViewModel
            {
                Id = userId,
                Email = user!.Email!,
                Roles = rolesStr.ToList(),
                AvailableRoles = identityRoles.ToList()
            };
        }

        public async Task<IEnumerable<AdminUserViewModel>> GetAllUsersAsync(string adminUserId)
        {
            IEnumerable<ApplicationUser> users = await _userRepository
                .GetAllUsersAsync(filter: u => u.Id.ToString() != adminUserId);

            List<AdminUserViewModel> userViewModels = new List<AdminUserViewModel>();
            foreach (ApplicationUser user in users)
            {
                IEnumerable<string> userRoles = await _userRepository.GetUserRolesAsync(user);

                userViewModels.Add(new AdminUserViewModel
                {
                    Id = user.Id,
                    Email = user.Email!,
                    Roles = userRoles.ToList()
                });
            }
            return userViewModels;
        }

        public async Task<bool> CreateUserAsync(AdminCreateUserViewModel viewModel)
        {
            if (viewModel.Password != viewModel.ConfirmPassword)
                throw new InvalidOperationException();

            ApplicationUser newAppUser = new ApplicationUser()
            {
                Id = Guid.NewGuid(),
                BirthDate = viewModel.BirthDate,
                FullName = viewModel.FullName,
                UserName = viewModel.Email,
                Email = viewModel.Email,
                NormalizedEmail = viewModel.Email.ToUpper(),
                NormalizedUserName = viewModel.Email.ToUpper(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                SecurityStamp = Guid.NewGuid().ToString(),

            };

            newAppUser.PasswordHash = _passwordHasher.HashPassword(newAppUser, viewModel.Password);


            bool isAddSuccessful = await _userRepository.AddUserAsync(newAppUser);

            if (!isAddSuccessful)
            {
                throw new InvalidOperationException();
            }

            if (viewModel.SelectedRoles.Any())
            {
                bool isRoleAdded;
                foreach (var role in viewModel.SelectedRoles)
                {
                    isRoleAdded = await AssignRoleToUserAsync(newAppUser.Id, role);

                    if (!isRoleAdded)
                    {
                        throw new InvalidOperationException();
                    }
                }
            }
            return isAddSuccessful;
        }

        public async Task<bool> AssignRoleToUserAsync(Guid userId, string role)
        {
            if (userId == Guid.Empty || string.IsNullOrWhiteSpace(role))
            {
                throw new InvalidOperationException();
            }

            bool result = await _userRepository
                .UpdateUserRoleAsync(userId, role);

            return result;
        }

        public async Task<bool> RemoveRoleFromUserAsync(Guid userId, string role)
        {
            if (userId == Guid.Empty || string.IsNullOrWhiteSpace(role))
            {
                throw new InvalidOperationException();
            }

            bool result = await _userRepository
                .UpdateUserRoleAsync(userId, role, removingRole: true);

            return result;
        }

        public async Task<bool> RestoreUserAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new InvalidOperationException();
            }

            bool result = await _userRepository
                .RestoreUserAsync(userId);

            return result;
        }

        public async Task<bool> SoftDeleteUserAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new InvalidOperationException();
            }

            bool result = await _userRepository
                .SoftDeleteUserAsync(userId);

            return result;
        }

        public async Task<bool> HardDeleteUserAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new InvalidOperationException();
            }

            bool result = await _userRepository
                .HardDeleteUserAsync(userId);

            return result;
        }
    }
}
