using Microsoft.AspNetCore.Mvc;

namespace DynamicQuizGenerator.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult  Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        public IActionResult Profile()
        {
            return View();
        }

        public IActionResult AdminRegister()
        {
            return View();
        }

        public IActionResult AdminLogin()
        {
            return View();
        }

    }
}
