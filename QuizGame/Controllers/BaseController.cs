namespace QuizGame.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using System.Security.Claims;
    public class BaseController : Controller
    {
        public string? GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}
