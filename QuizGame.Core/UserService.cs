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
        public async Task<AdminManageUserRolesViewModel?> GetUserByIdAsync(Guid? userId)
        {
            ApplicationUser? user = await _userRepository.FindUserByIdAsync(userId);

            if (user == null)
            {
                throw new InvalidOperationException();
            }

            IEnumerable<string> rolesStr = await _userRepository.GetUserRolesAsync(user!);

            IEnumerable<IdentityRole<Guid>> identityRoles = await _userRepository.GetAllRolesAsync(user!);

            return new AdminManageUserRolesViewModel
            {
                Id = user.Id,
                Email = user!.Email!,
                Roles = rolesStr.ToList(),
                AvailableRoles = identityRoles.ToList()
            };
        }

        public async Task<IEnumerable<AdminUserViewModel>> GetAllUsersAsync(bool getDeletedUsers = false)
        {
            IEnumerable<ApplicationUser> users;
            if (getDeletedUsers)
            {
                users = await _userRepository
                .GetAllUsersAsync(filter: u => u.isDeleted == true);
            }
            else
            {
                users = await _userRepository
                .GetAllUsersAsync(filter: u => u.isDeleted == false);
            }
             

            List<AdminUserViewModel> userViewModels = new List<AdminUserViewModel>();
            foreach (ApplicationUser user in users)
            {
                IEnumerable<string> userRoles = await _userRepository.GetUserRolesAsync(user);

                userViewModels.Add(new AdminUserViewModel
                {
                    Id = user.Id,
                    Email = user.Email!,
                    Roles = userRoles.ToList(),
                    IsDeleted = true
                });
            }
            return userViewModels;
        }

        public async Task CreateUserAsync(AdminCreateUserViewModel viewModel)
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
        }

        public async Task<bool> AssignRoleToUserAsync(Guid? id, string role)
        {
            if (string.IsNullOrWhiteSpace(role))
            {
                throw new InvalidOperationException();
            }

            bool isAssignSuccessful = await _userRepository
                .UpdateUserRoleAsync(id, role);

            if (!isAssignSuccessful)
            {
                throw new InvalidOperationException();
            }

            return isAssignSuccessful;
        }

        public async Task RemoveRoleFromUserAsync(Guid? id, string role)
        {
            if (string.IsNullOrWhiteSpace(role))
            {
                throw new InvalidOperationException();
            }

            bool isRemoveSuccessful = await _userRepository
                .UpdateUserRoleAsync(id, role, removingRole: true);

            if (!isRemoveSuccessful)
            {
                throw new InvalidOperationException();
            }
        }

        public async Task RestoreUserAsync(Guid? id)
        {
            bool isRestoreSuccessful = await _userRepository
                .RestoreUserAsync(id);

            if (!isRestoreSuccessful)
            {
                throw new InvalidOperationException();
            }
        }

        public async Task SoftDeleteUserAsync(Guid? id)
        {
            bool isSoftDeleteSuccessful = await _userRepository
                .SoftDeleteUserAsync(id);

            if (!isSoftDeleteSuccessful)
            {
                throw new InvalidOperationException();
            }
        }

        public async Task HardDeleteUserAsync(Guid? id)
        {
            bool isHardDeleteSuccessful = await _userRepository
                .HardDeleteUserAsync(id);

            if (!isHardDeleteSuccessful)
            {
                throw new InvalidOperationException();
            }
        }
    }
}
