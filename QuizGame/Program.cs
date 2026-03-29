using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuizGame.Data;
namespace QuizGame
{
    using CinemaApp.Web.Infrastructure.Extensions;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using QuizGame.Core;
    using QuizGame.Core.Contracts;
    using QuizGame.Data;
    using QuizGame.Data.Configurations;
    using QuizGame.Data.Configurations.Contracts;
    using QuizGame.Data.Models;
    using QuizGame.Data.Repository;
    using QuizGame.Data.Repository.Contracts;
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<QuizGameDbContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
            {
                ConfigureIdentity(builder.Configuration, options);
            }).AddRoles<IdentityRole<Guid>>()
              .AddEntityFrameworkStores<QuizGameDbContext>();

            builder.Services.AddRazorPages();
            builder.Services.AddControllersWithViews();

            builder.Services.AddScoped<IQuizRepository, QuizRepository>();
            builder.Services.AddScoped<ILeaderboardRepository, LeaderboardRepository>();
            builder.Services.AddScoped<IGameRepository, GameRepository>();

            builder.Services.AddScoped<IQuizService, QuizService>();
            builder.Services.AddScoped<ILeaderboardService, LeaderboardService>();
            builder.Services.AddScoped<IGameService, GameService>();

            builder.Services.AddScoped<IIdentitySeeder, IdentitySeeder>();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
               
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseRolesSeeder();
            app.UseAdminUserSeeder();

            app.MapStaticAssets();
            app.MapRazorPages();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();
            app.MapRazorPages()
               .WithStaticAssets();

            app.Run();
        }

        private static void ConfigureIdentity(ConfigurationManager configuration,
            IdentityOptions options)
        {
            options.SignIn.RequireConfirmedAccount = configuration
                 .GetValue<bool>("Identity:SignIn:RequireConfirmedAccount");
            options.SignIn.RequireConfirmedEmail = configuration
                 .GetValue<bool>("Identity:SignIn:RequireConfirmedEmail");
            options.SignIn.RequireConfirmedPhoneNumber = configuration
                 .GetValue<bool>("Identity:SignIn:RequireConfirmedPhoneNumber");

            options.Password.RequireDigit = configuration
                 .GetValue<bool>("Identity:Password:RequireDigit");
            options.Password.RequiredLength = configuration
                 .GetValue<int>("Identity:Password:RequiredLength");
            options.Password.RequiredUniqueChars = configuration
                 .GetValue<int>("Identity:Password:RequiredUniqueChars");
            options.Password.RequireLowercase = configuration
                 .GetValue<bool>("Identity:Password:RequireLowercase");
            options.Password.RequireNonAlphanumeric = configuration
                 .GetValue<bool>("Identity:Password:RequireNonAlphanumeric");
            options.Password.RequireUppercase = configuration
                 .GetValue<bool>("Identity:Password:RequireUppercase");
        }
    }
}
