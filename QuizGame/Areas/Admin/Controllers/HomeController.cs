using Microsoft.AspNetCore.Mvc;

namespace QuizGame.Areas.Admin.Controllers
{
    public class HomeController : BaseAdminController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
