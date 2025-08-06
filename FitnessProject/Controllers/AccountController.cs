using FitnessProject.Data;
using FitnessProject.Models;
using FitnessProject.uitls;
using FitnessProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static System.Net.WebRequestMethods;

namespace FitnessProject.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailService _emailService;

        public AccountController(UserManager<ApplicationUser> userManager,
                                 SignInManager<ApplicationUser> signInManager,
                                 RoleManager<IdentityRole> roleManager,
                                 IEmailService emailService
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _emailService = emailService;
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email,
                    OTPCode = "",
                    OTPExpiry = DateTime.UtcNow.AddMinutes(5)
                };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Assign role (default Member)
                    await _userManager.AddToRoleAsync(user, "Member");

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
            }
            return View(model);
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(
                    model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                    return RedirectToAction("Index", "Home");

                ModelState.AddModelError("", "Invalid login attempt");
            }
            return View(model);
        }

        // POST: /Account/Logout
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View("AccessDenied"); // will find Shared/AccessDenied.cshtml
        }



        [HttpGet]
        [AllowAnonymous]
        public IActionResult SendOtp()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> SendOtp(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return View("AccessDenied");
            }

            string otp = new Random().Next(100000, 999999).ToString();
            user.OTPCode = otp;
            user.OTPExpiry = DateTime.UtcNow.AddMinutes(5);

            await _userManager.UpdateAsync(user);
            await _emailService.SendEmailAsync(email, "Your OTP Code", $"Your OTP is: {otp}");

            TempData["Email"] = email; // persist for next steps
            return RedirectToAction("VerifyOtp");
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult VerifyOtp()
        {
            TempData.Keep("Email");
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyOtp(string otp)
        {
            var email = TempData["Email"]?.ToString();
            if (email == null)
                return View("AccessDenied");

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return View("AccessDenied");

            if (user.OTPCode == otp && user.OTPExpiry > DateTime.UtcNow)
            {
                TempData["Email"] = email;

                return RedirectToAction("ResetPassword");
            }

            return View("AccessDenied");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword()
        {
            TempData.Keep("Email");
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(string password)
        {
            var email = TempData["Email"]?.ToString();
            if (email == null)
                return View("AccessDenied");

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return View("AccessDenied");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, password);

            if (result.Succeeded)
            {
                user.OTPCode = "";
                user.OTPExpiry = null;
                await _userManager.UpdateAsync(user);
                return RedirectToAction("Login");
            }

            ModelState.AddModelError("", "Password reset failed.");
            return View();
        }


    }
}
