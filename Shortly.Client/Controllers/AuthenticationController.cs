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
using Twilio;
using Twilio.Rest.Api.V2010.Account;

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
            var loginVM = new LoginVM()
            {
                Schemes = await _signInManager.GetExternalAuthenticationSchemesAsync()
            };

            return View(loginVM);
        }

        public async Task<IActionResult> LoginSubmitted(LoginVM loginVM)
        {
            if (!ModelState.IsValid)
            {
                return View("Login", loginVM);
            }

            var user = await _userManager.FindByEmailAsync(loginVM.EmailAddress);
            if (user != null)
            {
                var userPasswordCheck = await _userManager.CheckPasswordAsync(user, loginVM.Password);
                if (userPasswordCheck)
                {
                    var userLoggedIn = await _signInManager.PasswordSignInAsync(user, loginVM.Password, false, false);

                    if (userLoggedIn.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else if (userLoggedIn.IsNotAllowed)
                    {
                        return RedirectToAction("EmailConfirmation");
                    }
                    else if (userLoggedIn.RequiresTwoFactor)
                    {
                        return RedirectToAction("TwoFactorConfirmation", new { loggedInUserId = user.Id });
                    }

                    else
                    {
                        ModelState.AddModelError("", "Invalid login attempt. Please, check your username and password");
                        return View("Login", loginVM);
                    }
                }
                else
                {
                    await _userManager.AccessFailedAsync(user);

                    if (await _userManager.IsLockedOutAsync(user))
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
            if (!ModelState.IsValid)
            {
                return View("Register", registerVM);
            }

            //Check if the user exists
            var user = await _userManager.FindByEmailAsync(registerVM.EmailAddress);
            if (user != null)
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
            }
            else
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
                var userConfirmationLink = Url.Action("EmailConfirmationVerified", "Authentication", new { userId = user.Id, userConfirmationToken = userToken }, Request.Scheme);

                //3. Send the email
                var apiKey = _configuration["SendGrid:ShortlyKey"];
                var sendGridClient = new SendGridClient(apiKey);

                var fromEmailAddress = new EmailAddress(_configuration["SendGrid:FromAddress"], "Shortly Client App");
                var emailSubject = "[Shortly] Verify your account";
                var toEmailAddress = new EmailAddress(confirmEmailLoginVM.EmailAddress);

                var emailContentTxt = $"Hello from Shortly App. Please, click this link to verify your account: {userConfirmationLink}";
                var emailContentHtml = $"Hello from Shortly App. Please, click this link to verify your account: <a href=\"{userConfirmationLink}\"> Verify your account </a> ";

                var emailRequest = MailHelper.CreateSingleEmail(fromEmailAddress, toEmailAddress, emailSubject, emailContentTxt, emailContentHtml);
                var emailResponse = await sendGridClient.SendEmailAsync(emailRequest);

                TempData["EmailConfirmation"] = "Thank you! Please, check your email to verify your account";

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", $"Email address {confirmEmailLoginVM.EmailAddress} does not exist");
            return View("EmailConfirmation", confirmEmailLoginVM);
        }

        public async Task<IActionResult> EmailConfirmationVerified(string userId, string userConfirmationToken)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var result = await _userManager.ConfirmEmailAsync(user, userConfirmationToken);

            TempData["EmailConfirmationVerified"] = "Thank you! Your account has been confirmed. You can now log in!";
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> TwoFactorConfirmation(string loggedInUserId)
        {
            // 1. Get the user
            var user = await _userManager.FindByIdAsync(loggedInUserId);

            if (user != null)
            {
                var userToken = await _userManager.GenerateTwoFactorTokenAsync(user, "Phone");

                // 2. Send the SMS (set up twilio)
                string twilioPhoneNumber = _configuration["Twilio:PhoneNumber"];
                string twilioSID = _configuration["Twilio:SID"];
                string twilioToken = _configuration["Twilio:Token"];

                TwilioClient.Init(twilioSID, twilioToken);

                var message = MessageResource.Create(
                        body: $"This is your verification code: {userToken}",
                        from: new Twilio.Types.PhoneNumber(twilioPhoneNumber),
                        to: new Twilio.Types.PhoneNumber(user.PhoneNumber)
                    );

                var confirm2FALoginVM = new Confirm2FALoginVM()
                {
                    UserId = loggedInUserId
                };

                return View(confirm2FALoginVM);

            }

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> TwoFactorConfirmationVerified(Confirm2FALoginVM confirm2FALoginVM)
        {
            var user = await _userManager.FindByIdAsync(confirm2FALoginVM.UserId);

            if(user != null)
            {
                var tokenVerification = await _userManager.VerifyTwoFactorTokenAsync(user, "Phone", confirm2FALoginVM.UserConfirmationCode);

                if (tokenVerification)
                {
                    var tokenSignIn = await _signInManager.TwoFactorSignInAsync("Phone", confirm2FALoginVM.UserConfirmationCode, false, false);

                    if (tokenSignIn.Succeeded)
                        return RedirectToAction("Index", "Home");
                }
            }

            ModelState.AddModelError("", "Confirmation code is not correct");
            return View(confirm2FALoginVM);
        }
    }
}
