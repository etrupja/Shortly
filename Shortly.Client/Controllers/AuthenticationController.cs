using Microsoft.AspNetCore.Mvc;
using Shortly.Client.Data.ViewModels;

namespace Shortly.Client.Controllers
{
    public class AuthenticationController : Controller
    {
        public IActionResult Users()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View(new LoginVM());
        }

        public IActionResult LoginSubmitted(LoginVM loginVM)
        {
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Register()
        {
            return View();
        }
    }
}
