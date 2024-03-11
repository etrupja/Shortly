using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SendGrid;
using SendGrid.Helpers.Mail;
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
        private IConfiguration _configuration;

        public AuthenticationController(IUsersService usersService,
            SignInManager<AppUser> signInManager,
            UserManager<AppUser> userManager,
            IConfiguration configuration)
        {
            _usersService = usersService;
            _signInManager = signInManager;
            _userManager = userManager;
            _configuration = configuration;
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
                    } else if (userLoggedIn.IsNotAllowed)
                    {
                        return RedirectToAction("EmailConfirmation");
                    } 
                    
                    else
                    {
                        ModelState.AddModelError("", "Invalid login attempt. Please, check your username and password");
                        return View("Login", loginVM);
                    }
                } else
                {
                    await _userManager.AccessFailedAsync(user);

                    if(await _userManager.IsLockedOutAsync(user))
                    {
                        ModelState.AddModelError("", "Your account is locked, please try again in 10 mins");
                        return View("Login", loginVM);
                    }

                    ModelState.AddModelError("", "Invalid login attempt. Please, check your username and password");
                    return View("Login", loginVM);
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
                FullName = registerVM.FullName,
                LockoutEnabled = true
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


        public async Task<IActionResult> EmailConfirmation()
        {
            var confirmEmail = new ConfirmEmailLoginVM();
            return View(confirmEmail);
        }

        public async Task<IActionResult> SendEmailConfirmation(ConfirmEmailLoginVM confirmEmailLoginVM)
        {
            //1. Check if the user exists
            var user = await _userManager.FindByEmailAsync(confirmEmailLoginVM.EmailAddress);

            //2. Create a confirmation link
            if (user != null)
            {
                var userToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                //3. Send the email
                var apiKey = _configuration["SendGrid:ShortlyKey"];
                var sendGridClient = new SendGridClient(apiKey);

                var fromEmailAddress = new EmailAddress(_configuration["SendGrid:FromAddress"], "Shortly Client App");
                var emailSubject = "[Shortly] Verify your account";
                var toEmailAddress = new EmailAddress(confirmEmailLoginVM.EmailAddress);

                var emailContentTxt = "Hello from Shortly App. Please, click this link to verify your account ";
                var emailContentHtml = "Hello from Shortly App. Please, click this link to verify your account ";

                var emailRequest = MailHelper.CreateSingleEmail(fromEmailAddress, toEmailAddress, emailSubject, emailContentTxt, emailContentHtml);
                var emailResponse = sendGridClient.SendEmailAsync(emailRequest);

                TempData["EmailConfirmation"] = "Thank you! Please, check your email to verify your account";

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", $"Email address {confirmEmailLoginVM.EmailAddress} does not exist");
            return View("EmailConfirmation", confirmEmailLoginVM);
        }
    }
}
