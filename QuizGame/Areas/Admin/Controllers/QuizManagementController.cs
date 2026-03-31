using Microsoft.AspNetCore.Mvc;

namespace QuizGame.Areas.Admin.Controllers
{
    public class QuizManagementController : Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction("Index","Quiz");
        }

    }
}
