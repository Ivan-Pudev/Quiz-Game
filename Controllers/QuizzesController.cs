using Microsoft.AspNetCore.Mvc;

namespace QuizGame.Controllers
{
    public class QuizzesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
