using Microsoft.AspNetCore.Mvc;
using Shortly.Client.Data.ViewModels;
using Shortly.Data;

namespace Shortly.Client.Controllers
{
    public class AuthenticationController : Controller
    {
        private AppDbContext _context;
        public AuthenticationController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Users()
        {
            var users = _context.Users.ToList();
            return View(users);
        }

        public IActionResult Login()
        {
            return View(new LoginVM());
        }

        public IActionResult LoginSubmitted(LoginVM loginVM)
        {
            if(!ModelState.IsValid)
            {
                return View("Login", loginVM);
            }

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Register()
        {
            return View(new RegisterVM());
        }

        public IActionResult RegisterUser(RegisterVM registerVM)
        {
            if(!ModelState.IsValid)
            {
                return View("Register", registerVM);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
