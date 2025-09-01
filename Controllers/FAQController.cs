using Microsoft.AspNetCore.Mvc;

namespace DynamicQuizGenerator.Controllers
{
    public class FAQController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }
    }
}
