namespace QuizGame.Data.Configurations.Contracts
{
    public interface IIdentitySeeder
    {
        Task SeedRolesAsync();

        Task SeedAdminUserAsync();
    }
}