using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shortly.Client.Data.ViewModels;
using Shortly.Data;
using Shortly.Data.Services;

namespace Shortly.Client.Controllers
{
    public class AuthenticationController : Controller
    {
        private IUsersService _usersService;
        public AuthenticationController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        public IActionResult Users()
        {
            var users = _usersService.GetUsers();
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
