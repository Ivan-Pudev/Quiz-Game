namespace QuizGame.Data.Configurations
{
    using Contracts;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Configuration;
    using QuizGame.Data.Models;


    public class IdentitySeeder : IIdentitySeeder
    {
        // ReSharper disable once InconsistentNaming
        public static string[] ApplicationRoles = new[]
        {
            "Admin",
            "Moderator"
        };

        private readonly RoleManager<IdentityRole<Guid>> roleManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IConfiguration configuration;

        public IdentitySeeder(RoleManager<IdentityRole<Guid>> roleManager, 
            UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.configuration = configuration;
        }

        public async Task SeedRolesAsync()
        {
            foreach (string role in ApplicationRoles)
            {
                bool roleExists = await roleManager.RoleExistsAsync(role);
                if (!roleExists)
                {
                    IdentityRole<Guid> newRole = new IdentityRole<Guid>(role);

                    IdentityResult identityRoleResult =
                        await roleManager.CreateAsync(newRole);
                    if (!identityRoleResult.Succeeded)
                    {
                        throw new InvalidOperationException();
                    }
                }
            }
        }

        public async Task SeedAdminUserAsync()
        {
            string adminEmail = configuration["UserConfig:AdminAcc:Email"] ??
                                throw new InvalidOperationException();
            string adminPassword = configuration["UserConfig:AdminAcc:Password"] ??
                                   throw new InvalidOperationException();
            string adminFullName = configuration["UserConfig:AdminAcc:FullName"] ??
                                   throw new InvalidOperationException();

            ApplicationUser? adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    BirthDate = DateTime.Now.AddYears(-18),
                    FullName = adminFullName
                };

                var result = await userManager
                    .CreateAsync(adminUser, adminPassword);
                if (!result.Succeeded)
                {
                    throw new InvalidOperationException();
                }
            }

            bool isInRole = await userManager
                .IsInRoleAsync(adminUser, ApplicationRoles[0]);
            if (!isInRole)
            {
                IdentityResult result = await userManager
                    .AddToRoleAsync(adminUser, ApplicationRoles[0]);
                if (!result.Succeeded)
                {
                    throw new InvalidOperationException();
                }
            }
        }
    }
}