using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shortly.Client.Data.ViewModels;
using Shortly.Client.Helpers.Roles;
using Shortly.Data;
using Shortly.Data.Models;
using Shortly.Data.Services;

namespace Shortly.Client.Controllers
{
    public class AuthenticationController : Controller
    {
        private IUsersService _usersService;
        private SignInManager<AppUser> _signInManager;
        private UserManager<AppUser> _userManager;

        public AuthenticationController(IUsersService usersService,
            SignInManager<AppUser> signInManager,
            UserManager<AppUser> userManager)
        {
            _usersService = usersService;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<IActionResult> Users()
        {
            var users = await _usersService.GetUsersAsync();
            return View(users);
        }

        public async Task<IActionResult> Login()
        {
            return View(new LoginVM());
        }

        public async Task<IActionResult> LoginSubmitted(LoginVM loginVM)
        {
            if(!ModelState.IsValid)
            {
                return View("Login", loginVM);
            }

            var user = await _userManager.FindByEmailAsync(loginVM.EmailAddress);
            if(user != null)
            {
                var userPasswordCheck = await _userManager.CheckPasswordAsync(user, loginVM.Password);
                if (userPasswordCheck)
                {
                    var userLoggedIn = await _signInManager.PasswordSignInAsync(user, loginVM.Password, false, false);

                    if (userLoggedIn.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    } else
                    {
                        ModelState.AddModelError("", "Invalid login attempt. Please, check your username and password");
                        return View("Login", loginVM);
                    }
                }
            }


            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Register()
        {
            return View(new RegisterVM());
        }

        public async Task<IActionResult> RegisterUser(RegisterVM registerVM)
        {
            if(!ModelState.IsValid)
            {
                return View("Register", registerVM);
            }

            //Check if the user exists
            var user = await _userManager.FindByEmailAsync(registerVM.EmailAddress);
            if(user != null)
            {
                ModelState.AddModelError("", "Email address is already in use.");
                return View("Register", registerVM);
            }

            var newUser = new AppUser()
            {
                Email = registerVM.EmailAddress,
                UserName = registerVM.EmailAddress,
                FullName = registerVM.FullName
            };

            var userCreated = await _userManager.CreateAsync(newUser, registerVM.Password);
            if (userCreated.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, Role.User);

                //Login the user
                await _signInManager.PasswordSignInAsync(newUser, registerVM.Password, false, false);
            } else
            {
                foreach (var error in userCreated.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View("Register", registerVM);
            }

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

    }
}
