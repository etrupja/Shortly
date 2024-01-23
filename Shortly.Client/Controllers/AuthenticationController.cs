using Microsoft.AspNetCore.Mvc;

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
            return View();
        }

        public IActionResult LoginSubmitted(string emailAddress, string password)
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }
    }
}
