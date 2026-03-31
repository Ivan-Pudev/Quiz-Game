namespace QuizGame.Data.Repository
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using QuizGame.Data.Models;
    using QuizGame.Data.Repository.Contracts;
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;


    public class UserRepository : BaseRepository, IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;

        public UserRepository(QuizGameDbContext dbContext,
            UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<Guid>> roleManager)
            : base(dbContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task<ApplicationUser?> FindUserByIdAsync(Guid userId)
        {
            return await _userManager.FindByIdAsync(userId.ToString());
        }


        public async Task<IEnumerable<ApplicationUser>> GetAllUsersAsync(Expression<Func<ApplicationUser, bool>>? filter = null)
        {
            IQueryable<ApplicationUser> applicationUsers = DbContext!
                .Users
                .AsNoTracking();

            if (filter != null)
            {
                applicationUsers = applicationUsers
                    .Where(filter);
            }

            IEnumerable<ApplicationUser> appUsers = await applicationUsers
                .OrderBy(u => u.Email)
                .ToArrayAsync();

            return appUsers;
        }
        public async Task<IEnumerable<IdentityRole<Guid>>> GetAllRolesAsync(ApplicationUser applicationUser)
        {
            return await _roleManager
                .Roles
                .ToListAsync();
        }


        public async Task<IEnumerable<string>> GetUserRolesAsync(ApplicationUser appUser)
        {
            IEnumerable<string> userRoles = await _userManager.GetRolesAsync(appUser);

            return userRoles;
        }

        public async Task<bool> AddUserAsync(ApplicationUser newAppUser)
        {
            await DbContext.AddAsync(newAppUser);
            int resultsCount = await SaveChangesAsync();

            return resultsCount > 0;
        }

        public async Task<bool> UpdateUserRoleAsync(Guid userId, string role, bool removingRole = false)
        {
            ApplicationUser? appUser = await _userManager
                .FindByIdAsync(userId.ToString());
            if (appUser == null)
            {
                return false;
            }

            bool roleExists = await _roleManager.RoleExistsAsync(role);
            if (!roleExists)
            {
                return false;
            }

            bool alreadyInRole = await _userManager.IsInRoleAsync(appUser, role);
            if (!removingRole && alreadyInRole)
            {
                return false;
            }

            if (removingRole && !alreadyInRole)
            {
                return false;
            }

            IdentityResult roleOperationResult;
            if (removingRole)
            {
                roleOperationResult = await _userManager
                    .RemoveFromRoleAsync(appUser, role);
            }
            else
            {
                roleOperationResult = await _userManager
                    .AddToRoleAsync(appUser, role);
            }

            if (roleOperationResult != IdentityResult.Success)
            {
                return false;
            }

            return true;
        }
        public async Task<bool> RestoreUserAsync(Guid userId)
        {
            ApplicationUser? appUser = await _userManager
                .FindByIdAsync(userId.ToString());
            if (appUser == null)
            {
                return false;
            }

            appUser.isDeleted = false;
            int resultsCount = await SaveChangesAsync();

            return resultsCount > 0;
        }
        public async Task<bool> SoftDeleteUserAsync(Guid userId)
        {
            ApplicationUser? appUser = await _userManager
                .FindByIdAsync(userId.ToString());
            if (appUser == null)
            {
                return false;
            }

            appUser.isDeleted = true;
            int resultsCount = await SaveChangesAsync();

            return resultsCount > 0;
        }

        public async Task<bool> HardDeleteUserAsync(Guid userId)
        {
            ApplicationUser? appUser = await _userManager
                .FindByIdAsync(userId.ToString());
            if (appUser == null)
            {
                return false;
            }

            IdentityResult deleteResult = await _userManager
                .DeleteAsync(appUser);
            if (deleteResult != IdentityResult.Success)
            {
                return false;
            }

            return true;
        }
    }
}
